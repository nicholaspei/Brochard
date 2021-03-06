﻿using Microsoft.Framework.ConfigurationModel;
using System;
using System.Collections.Generic;
using System.IO;

namespace OrchardVNext.Environment.Configuration.Sources
{
    public class DefaultFileConfigurationSource : BaseConfigurationSource, ICommitableConfigurationSource {
        public const char Separator = ':';
        public const string EmptyValue = "null";
        public const char ThemesSeparator = ';';

        public DefaultFileConfigurationSource(string path) {
            if (string.IsNullOrEmpty(path)) {
                throw new ArgumentException("Invalid Filepath", "path");
            }

            Path = path;
        }

        public string Path { get; private set; }

        public override void Load() {
            using (var stream = new FileStream(Path, FileMode.Open, FileAccess.Read)) {
                Load(stream);
            }
        }

        public void Commit() {
            throw new NotImplementedException();
        }

        internal void Load(Stream stream) {
            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (var reader = new StreamReader(stream)) {
                while (reader.Peek() != -1) {
                    var rawLine = reader.ReadLine();
                    var line = rawLine.Trim();

                    // Ignore blank lines
                    if (string.IsNullOrWhiteSpace(line)) {
                        continue;
                    }
                    // Ignore comments
                    if (line[0] == ';' || line[0] == '#' || line[0] == '/') {
                        continue;
                    }

                    var separatorIndex = line.IndexOf(Separator);
                    if (separatorIndex == -1) {
                        continue;
                    }
                    string key = line.Substring(0, separatorIndex).Trim();
                    string value = line.Substring(separatorIndex + 1).Trim();

                    if (value.Equals(EmptyValue, StringComparison.OrdinalIgnoreCase)) {
                        continue;
                    }

                    data[key] = value;
                }
            }

            ReplaceData(data);
        }
    }
}