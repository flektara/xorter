using FileSorter.Lib.Abstractions;
using FileSorter.Lib.Helpers;
using System.IO;
using System.Linq;

namespace FileSorter.Lib.Modules
{
    public sealed class TypeSorter : Sorter
    {
        public override void Sort(FileInfo file, string targetFolder)
        {
            var folderName = ResolveDestination(file);
            HandleFile(file, folderName, targetFolder);
        }

        public override string ResolveDestination(FileInfo file)
        {
            string extension = file.Extension;
            if (!string.IsNullOrEmpty(extension))
            {
                extension = extension.Substring(1);
                var destination = ConfigurationHelper.GetDestinations().Where(f => f.Extensions.Contains(extension));

                if (destination.Any())
                    return destination.First().FolderName;
            }
            LogHelper.GetLogger().Warn(string.Format("Unknown extension occured: [.{0}] -File moved to {1}", extension, Constants.UNKNOW_TYPES_FOLDER_NAME));
            return Constants.UNKNOW_TYPES_FOLDER_NAME;
        }
    }
}
