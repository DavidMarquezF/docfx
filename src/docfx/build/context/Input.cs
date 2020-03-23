// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Microsoft.Docs.Build
{
    /// <summary>
    /// Application level input abstraction
    /// </summary>
    internal class Input
    {
        private readonly string _docsetPath;
        private readonly RepositoryProvider _repositoryProvider;
        private readonly ConcurrentDictionary<FilePath, (List<Error>, JToken)> _jsonTokenCache = new ConcurrentDictionary<FilePath, (List<Error>, JToken)>();
        private readonly ConcurrentDictionary<FilePath, (List<Error>, JToken)> _yamlTokenCache = new ConcurrentDictionary<FilePath, (List<Error>, JToken)>();
        private readonly ConcurrentDictionary<FilePath, byte[]?> _gitBlobCache = new ConcurrentDictionary<FilePath, byte[]?>();

        public Input(string docsetPath, RepositoryProvider repositoryProvider)
        {
            _repositoryProvider = repositoryProvider;
            _docsetPath = Path.GetFullPath(docsetPath);
        }

        /// <summary>
        /// Check if the specified file path exist.
        /// </summary>
        public bool Exists(FilePath file)
        {
            var (basePath, path, commit) = ResolveFilePath(file);

            if (basePath is null)
            {
                return false;
            }

            if (commit is null)
            {
                return File.Exists(PathUtility.Normalize(Path.Combine(basePath, path)));
            }

            var (repo, pathToRepo) = _repositoryProvider.GetRepository(file);
            if (repo is null || pathToRepo is null)
            {
                return false;
            }

            return _gitBlobCache.GetOrAdd(file, _ => GitUtility.ReadBytes(repo.Path, pathToRepo, commit)) != null;
        }

        /// <summary>
        /// Try get the absolute path of the specified file if it exists physically on disk.
        /// Some file path like content from a bare git repo does not exist physically
        /// on disk but we can still read its content.
        /// </summary>
        public bool TryGetPhysicalPath(FilePath file, [NotNullWhen(true)] out string? physicalPath)
        {
            var (basePath, path, commit) = ResolveFilePath(file);

            if (basePath != null && commit is null)
            {
                var fullPath = Path.Combine(basePath, path);
                if (File.Exists(fullPath))
                {
                    physicalPath = fullPath;
                    return true;
                }
            }

            physicalPath = null;
            return false;
        }

        /// <summary>
        /// Reads the specified file as a string.
        /// </summary>
        public string ReadString(FilePath file)
        {
            using var reader = ReadText(file);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Reads the specified file as JSON.
        /// </summary>
        public (List<Error> errors, JToken token) ReadJson(FilePath file)
        {
            return _jsonTokenCache.GetOrAdd(file, path =>
            {
                using var reader = ReadText(path);
                return JsonUtility.Parse(reader, path);
            });
        }

        /// <summary>
        /// Reads the specified file as YAML.
        /// </summary>
        public (List<Error> errors, JToken token) ReadYaml(FilePath file)
        {
            return _yamlTokenCache.GetOrAdd(file, path =>
            {
                using var reader = ReadText(path);
                return YamlUtility.Parse(reader, path);
            });
        }

        /// <summary>
        /// Open the specified file and read it as text.
        /// </summary>
        public TextReader ReadText(FilePath file)
        {
            return new StreamReader(ReadStream(file));
        }

        public Stream ReadStream(FilePath file)
        {
            var (basePath, path, commit) = ResolveFilePath(file);

            if (basePath is null)
            {
                throw new NotSupportedException($"{nameof(ReadStream)}: {file}");
            }

            if (commit is null)
            {
                return File.OpenRead(PathUtility.NormalizeFile(Path.Combine(basePath, path)));
            }

            var bytes = _gitBlobCache.GetOrAdd(file, _ => GitUtility.ReadBytes(basePath, path, commit))
                ?? throw new InvalidOperationException($"Error reading '{file}'");

            return new MemoryStream(bytes, writable: false);
        }

        /// <summary>
        /// List all the file path.
        /// </summary>
        public FilePath[] ListFilesRecursive(FileOrigin origin, PathString? dependencyName = null)
        {
            switch (origin)
            {
                case FileOrigin.Default:
                    return ListFilesRecursive(_docsetPath);

                case FileOrigin.Fallback:
                    var (fallbackEntry, _) = _repositoryProvider.GetRepositoryWithDocsetEntry(origin);

                    return ListFilesRecursive(fallbackEntry);

                case FileOrigin.Dependency:
                    var (dependencyEntry, _) = _repositoryProvider.GetRepositoryWithDocsetEntry(origin, dependencyName);

                    return ListFilesRecursive(dependencyEntry);

                default:
                    throw new NotSupportedException($"{nameof(ListFilesRecursive)}: {origin}");
            }

            FilePath[] ListFilesRecursive(string entry)
            {
                if (!Directory.Exists(entry))
                {
                    return Array.Empty<FilePath>();
                }

                return Directory
                    .GetFiles(entry, "*", SearchOption.AllDirectories)
                    .Select(path => CreateFilePath(Path.GetRelativePath(entry, path)))
                    .ToArray();
            }

            FilePath CreateFilePath(string path)
            {
                return dependencyName is null ? new FilePath(path, origin) : new FilePath(path, dependencyName.Value);
            }
        }

        private (string basePath, string path, string? commit) ResolveFilePath(FilePath file)
        {
            switch (file.Origin)
            {
                case FileOrigin.Default:
                    return (_docsetPath, file.Path, file.Commit);

                case FileOrigin.Dependency:
                    var (dependencyEntry, dependencyRepository) = _repositoryProvider.GetRepositoryWithDocsetEntry(file.Origin, file.DependencyName);
                    return (dependencyEntry, file.GetPathToOrigin(), file.Commit ?? dependencyRepository?.Commit);

                case FileOrigin.Fallback:
                case FileOrigin.Template:
                    var (docsetPath, _) = _repositoryProvider.GetRepositoryWithDocsetEntry(file.Origin);
                    return (docsetPath, file.Path, file.Commit);

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}