﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Docs.Build
{
    internal static class Build
    {
        public static async Task Run(string docsetPath, CommandLineOptions options, ErrorLog errorLog)
        {
            var repository = Repository.Create(docsetPath);
            Telemetry.SetRepository(repository?.Remote, repository?.Branch);

            var locale = LocalizationUtility.GetLocale(repository?.Remote, repository?.Branch, options);
            using (var restoreGitMap = GetRestoreGitMap(docsetPath, locale, options))
            {
                var (fallbackRepo, fallbackRestoreGitMap) = GetFallbackRepository(docsetPath, repository, restoreGitMap);
                await Run(docsetPath, repository, locale, options, errorLog, fallbackRestoreGitMap ?? restoreGitMap, fallbackRepo);
            }
        }

        private static async Task Run(
            string docsetPath,
            Repository repository,
            string locale,
            CommandLineOptions options,
            ErrorLog errorLog,
            RestoreGitMap restoreMap,
            Repository fallbackRepo = null)
        {
            var (configErrors, config) = GetBuildConfig(docsetPath, options, locale, fallbackRepo);
            errorLog.Configure(config);

            // just return if config loading has errors
            if (errorLog.Write(configErrors))
                return;

            var docset = GetBuildDocset(new Docset(errorLog, docsetPath, locale, config, options, restoreMap, repository, fallbackRepo));
            var outputPath = Path.Combine(docsetPath, config.Output.Path);

            using (var context = new Context(outputPath, errorLog, docset, BuildFile))
            {
                context.BuildQueue.Enqueue(context.BuildScope.Files);

                using (Progress.Start("Building files"))
                {
                    await context.BuildQueue.Drain(Progress.Update);
                }

                context.BookmarkValidator.Validate();

                var (publishModel, fileManifests) = context.PublishModelBuilder.Build(context, docset.Legacy);
                var dependencyMap = context.DependencyMapBuilder.Build();
                var xrefMapModel = context.XrefMap.ToXrefMapModel(context);

                context.Output.WriteJson(xrefMapModel, "xrefmap.json");
                context.Output.WriteJson(publishModel, ".publish.json");
                context.Output.WriteJson(dependencyMap.ToDependencyMapModel(), ".dependencymap.json");

                if (options.Legacy)
                {
                    if (config.Output.Json)
                    {
                        // TODO: decouple files and dependencies from legacy.
                        Legacy.ConvertToLegacyModel(docset, context, fileManifests, dependencyMap);
                    }
                    else
                    {
                        context.TemplateEngine.CopyTo(outputPath);
                    }
                }

                context.GitHubUserCache.Save();
                context.MicrosoftGraphCache.Save();
                context.ContributionProvider.Save();
                context.GitCommitProvider.Save();
            }
        }

        private static async Task BuildFile(Context context, Document file)
        {
            if (!ShouldBuildFile())
            {
                return;
            }

            try
            {
                var errors = Enumerable.Empty<Error>();
                switch (file.ContentType)
                {
                    case ContentType.Resource:
                        errors = BuildResource.Build(context, file);
                        break;
                    case ContentType.Page:
                        errors = await BuildPage.Build(context, file);
                        break;
                    case ContentType.TableOfContents:
                        // TODO: improve error message for toc monikers overlap
                        errors = BuildTableOfContents.Build(context, file);
                        break;
                    case ContentType.Redirection:
                        errors = BuildRedirection.Build(context, file);
                        break;
                }

                var hasErrors = context.ErrorLog.Write(file, errors);
                if (hasErrors)
                {
                    context.PublishModelBuilder.MarkError(file);
                }

                Telemetry.TrackBuildItemCount(file.ContentType);
            }
            catch (Exception ex) when (DocfxException.IsDocfxException(ex, out var dex))
            {
                context.ErrorLog.Write(file, dex.Error, isException: true);
                context.PublishModelBuilder.MarkError(file);
            }
            catch
            {
                Console.WriteLine($"Build {file.FilePath} failed");
                throw;
            }

            bool ShouldBuildFile()
            {
                if (file.ContentType == ContentType.TableOfContents)
                {
                    if (!context.TocMap.Contains(file))
                    {
                        return false;
                    }

                    // if A toc includes B toc and only B toc is localized, then A need to be included and built
                    return !file.Docset.IsFallback() || (context.TocMap.TryGetTocReferences(file, out var tocReferences) && tocReferences.Any(toc => !toc.Docset.IsFallback()));
                }

                return !file.Docset.IsFallback();
            }
        }

        private static RestoreGitMap GetRestoreGitMap(
            string docsetPath,
            string locale,
            CommandLineOptions commandLineOptions)
        {
            Debug.Assert(!string.IsNullOrEmpty(docsetPath));

            var (_, config) = ConfigLoader.TryLoad(docsetPath, commandLineOptions);

            var dependencyLock = DependencyLock.Load(docsetPath, string.IsNullOrEmpty(config.DependencyLock) ? new SourceInfo<string>(AppData.GetDependencyLockFile(docsetPath, locale)) : config.DependencyLock) ?? new DependencyLockModel();
            return RestoreGitMap.Create(dependencyLock);
        }

        private static (Repository fallbackRepo, RestoreGitMap fallbackRestoreGitMap) GetFallbackRepository(
            string docsetPath,
            Repository repository,
            RestoreGitMap restoreGitMap)
        {
            Debug.Assert(restoreGitMap != null);
            Debug.Assert(!string.IsNullOrEmpty(docsetPath));

            if (LocalizationUtility.TryGetFallbackRepository(repository, out var fallbackRemote, out string fallbackBranch, out _))
            {
                if (restoreGitMap.DependencyLock.GetGitLock(fallbackRemote, fallbackBranch) == null && restoreGitMap.DependencyLock.GetGitLock(fallbackRemote, "master") != null)
                {
                    // fallback to master branch
                    fallbackBranch = "master";
                }

                var (fallbackRepoPath, fallbackRestoreMap) = restoreGitMap.GetGitRestorePath(fallbackRemote, fallbackBranch, docsetPath);
                var fallbackRepository = Repository.Create(fallbackRepoPath, fallbackBranch, fallbackRemote);

                if (!ConfigLoader.TryGetConfigPath(docsetPath, out _))
                {
                    // build from loc repo directly with overwrite config
                    // which means it's using source repo's dependency loc;
                    return (fallbackRepository, fallbackRestoreMap);
                }

                return (fallbackRepository, default);
            }

            return default;
        }

        private static (List<Error> errors, Config config) GetBuildConfig(
            string docset,
            CommandLineOptions options,
            string locale,
            Repository fallbackRepo = null)
        {
            if (ConfigLoader.TryGetConfigPath(docset, out _) || fallbackRepo is null)
            {
                return ConfigLoader.Load(docset, options, locale);
            }

            return ConfigLoader.Load(fallbackRepo.Path, options, locale);
        }

        private static Docset GetBuildDocset(Docset sourceDocset)
        {
            Debug.Assert(sourceDocset != null);

            return sourceDocset.LocalizationDocset ?? sourceDocset;
        }
    }
}