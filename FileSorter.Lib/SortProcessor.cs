using FileSorter.Lib.Abstractions;
using FileSorter.Lib.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FileSorter.Lib
{
    public class SortProcessor
    {
        int initialFilesCount, duplicates, ambiguous;
        private List<FileInfo> _files;
        private readonly Sorter _sorter;
        private readonly DirectoryInfo _targetFolderInfo;

        public SortProcessor(Sorter sorter, DirectoryInfo targetFolderInfo)
        {
            LogHelper.GetLogger().Info("SortProcessor initialized");
            _sorter = sorter;
            _targetFolderInfo = targetFolderInfo;
        }

        public bool Organize()
        {
            if (_targetFolderInfo == null || !_targetFolderInfo.Exists)
                throw new ArgumentException("Folder does not exists.");

            try
            {
                LogHelper.GetLogger().Info(">> Sorting started..");

                _files = _targetFolderInfo.GetFiles("*", SearchOption.AllDirectories).ToList();
                initialFilesCount = _files.Count;
                HandleDuplicates();

                var sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                Parallel.ForEach(_files, (file) =>
                {
                    _sorter.Sort(file, _targetFolderInfo.FullName);
                });

                sw.Stop();
                LogHelper.GetLogger().Info(">> Sorting finished - Time elapsed: " + sw.Elapsed);
            }
            catch (Exception e)
            {
                LogHelper.GetLogger().Error(e.Message, e);
            }

            PerformCleanUp();
            return ValidateFileCount();
        }

        private void HandleDuplicates()
        {
            for (int i = 0; i < _files.Count; i++)
            {
                var file1 = _files[i];
                byte[] file1hash = ComputeFileHash(file1);

                for (int j = 0; j < _files.Count; j++)
                {
                    var file2 = _files[j];
                    if (j != i)
                    {
                        if (file1.Length == file2.Length && IsDuplicate(file1hash, file2))
                        {
                            LogHelper.GetLogger().Info(string.Format("DUPLICATE FILES File1: {0} File2: {1} ", file1.Name, file2.Name));
                            _files.Remove(file2);
                            File.Delete(file2.FullName);
                            duplicates++;
                        }
                        else if (file1.Name.Equals(file2.Name))
                        {
                            LogHelper.GetLogger().Info(string.Format("AMBIGUOUS FILE NAMES File1: {0} File2: {1} ", file1.Name, file2.Name));
                            string fullNameWithSalt = GetNameWithSalt(file2, i);
                            file2.MoveTo(fullNameWithSalt); //TODO: handle path to long exception
                            ambiguous++; //TODO: record 
                        }
                    }
                }
            }
        }

        private byte[] ComputeFileHash(FileInfo file)
        {
            using (var fileReader = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            using (var md5 = new MD5CryptoServiceProvider())
            {
                return md5.ComputeHash(fileReader);
            }
        }

        private bool IsDuplicate(byte[] file1hash, FileInfo file2)
        {
            byte[] f2Hash = ComputeFileHash(file2);

            for (int i = 0; i < file1hash.Length; i++)
            {
                if (file1hash[i] != f2Hash[i])
                    return false;
            }

            return file1hash.Length == f2Hash.Length;
        }

        private string GetNameWithSalt(FileInfo fileInfo, int i)
        {
            string name = fileInfo.Name.Substring(0, (fileInfo.Name.Length - fileInfo.Extension.Length));
            return string.Format("{0}\\{1}-RENAME-{2}{3}", fileInfo.Directory, name, i, fileInfo.Extension);
        }

        private void PerformCleanUp()
        { 
            var folders = Directory.GetDirectories(_targetFolderInfo.FullName, "*", SearchOption.AllDirectories)
                                            .OrderByDescending(x => x.Count(c => c == '\\')).ToList();

            folders.ForEach(folder =>
            {
                if (!Directory.GetFiles(folder, "*", SearchOption.AllDirectories).Any())
                    Directory.Delete(folder, true);
            });
        }

        private bool ValidateFileCount()
        {
            int newCount = _targetFolderInfo.GetFiles("*", SearchOption.AllDirectories).Count(); 
            return (_files.Count() == newCount) && ((initialFilesCount-duplicates) == newCount);
        }
    }
}
