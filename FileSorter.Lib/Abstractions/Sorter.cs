using System.IO;

namespace FileSorter.Lib.Abstractions
{
    public abstract class Sorter
    {
        protected void HandleFile(FileInfo file, string folderName, string targetFolder)
        {
            string destination = Path.Combine(targetFolder, folderName);
            Directory.CreateDirectory(destination);
            string filePath = Path.Combine(destination, file.Name);
            File.Move(file.FullName, filePath);
        }

        public abstract string ResolveDestination(FileInfo file);
        public abstract void Sort(FileInfo _file, string targetFolder);
    }
}
