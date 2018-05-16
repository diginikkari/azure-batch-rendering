﻿
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BatchLabs.Max2016.Plugin.Common
{
    public class FileActions
    {
        public static Dictionary<string, List<string>> GetFileDictionaryWithLocations(string directoryPath, bool excludeHidden = true)
        {
            if (string.IsNullOrEmpty(directoryPath))
            {
                return null;
            }

            var dictionary = new Dictionary<string, List<string>>();
            var dirInfo = new DirectoryInfo(directoryPath);
            var files = dirInfo.GetAllDirectoryFiles(excludeHidden);

            foreach (var fileInfo in files)
            {
                if (!dictionary.ContainsKey(fileInfo.Name))
                {
                    dictionary.Add(fileInfo.Name, new List<string> { fileInfo.FullName });
                }
                else
                {
                    dictionary[fileInfo.Name].Add(fileInfo.FullName);
                }
            }

            return dictionary;
        }

        public static IEnumerable<FileInfo> GetFileInfos(IEnumerable<string> filePaths)
        {
            return filePaths
                .Select(GetFileInfo)
                .ToList();
        }

        public static FileInfo GetFileInfo(string filePath)
        {
            return new FileInfo(filePath);
        }
    }
}

