﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.DocAsCode.Build.Engine.Incrementals
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Microsoft.DocAsCode.Common;

    public class BuildInfo
    {
        public const string FileName = "build.info";

        /// <summary>
        /// The build start time for this build.
        /// </summary>
        public DateTime BuildStartTime { get; set; }
        /// <summary>
        /// The version of docfx.
        /// </summary>
        public string DocfxVersion { get; set; }
        /// <summary>
        /// The hash info for all plugins.
        /// </summary>
        public string PluginHash { get; set; }
        /// <summary>
        /// The hash info for templates.
        /// </summary>
        public string TemplateHash { get; set; }
        /// <summary>
        /// The file info for each version.
        /// </summary>
        public List<BuildVersionInfo> Versions { get; } = new List<BuildVersionInfo>();

        public static BuildInfo Load(string jsonFilePath)
        {
            var buildInfo = JsonUtility.Deserialize<BuildInfo>(jsonFilePath);
            var baseDir = Path.GetDirectoryName(jsonFilePath);
            foreach (var version in buildInfo.Versions)
            {
                version.Load(baseDir);
            }
            return buildInfo;
        }

        public static void Save(string jsonFilePath, BuildInfo buildInfo)
        {
            var baseDir = Path.GetDirectoryName(jsonFilePath);
            foreach (var version in buildInfo.Versions)
            {
                version.Save(baseDir);
            }
            JsonUtility.Serialize(jsonFilePath, buildInfo);
        }
    }
}
