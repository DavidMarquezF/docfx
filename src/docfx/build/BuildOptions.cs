// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;

namespace Microsoft.Docs.Build;

/// <summary>
/// Represents build options needed before loading config (excluding preload config)
/// </summary>
internal class BuildOptions
{
    public PathString DocsetPath { get; }

    public PathString? FallbackDocsetPath { get; }

    public PathString OutputPath { get; }

    public Repository? Repository { get; }

    /// <summary>
    /// Gets the lower-case culture name computed from <see cref="CommandLineOptions.Locale" or <see cref="Config.DefaultLocale"/>/>
    /// </summary>
    public string Locale { get; }

    /// <summary>
    /// Sets the animal profile
    /// </summary>
    public string Profile { get; }

    public CultureInfo Culture { get; }

    public bool IsLocalizedBuild => FallbackDocsetPath != null;

    public BuildOptions(
        string docsetPath, string? fallbackDocsetPath, string? outputPath, Repository? repository, PreloadConfig config, Package package, string locale, string profile)
    {
        Repository = repository;
        DocsetPath = package.GetFullFilePath(new PathString(docsetPath));
        if (fallbackDocsetPath != null)
        {
            FallbackDocsetPath = package.GetFullFilePath(new PathString(fallbackDocsetPath));
        }
        OutputPath = package.GetFullFilePath(new PathString(outputPath ?? Path.Combine(docsetPath, config.OutputPath)));
        Locale = (locale ?? LocalizationUtility.GetLocale(repository) ?? config.DefaultLocale).ToLowerInvariant();
        Profile = profile;
        Culture = LocalizationUtility.CreateCultureInfo(Locale);
    }
}
