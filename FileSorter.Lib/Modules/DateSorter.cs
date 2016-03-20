using System;
using System.IO;
using System.Windows.Media.Imaging;
using FileSorter.Lib.Helpers;
using FileSorter.Lib.Abstractions;

namespace FileSorter.Lib.Modules
{
    public class DateSorter : Sorter
    {
        public override void Sort(FileInfo file, string targetFolder)
        {
            string folderName = ResolveDestination(file);
            HandleFile(file, folderName, targetFolder);
        }

        public override string ResolveDestination(FileInfo file)
        {
            string date = String.Empty;
            try
            {
                using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                {
                    var source = BitmapFrame.Create(fileStream);
                    var metaData = (BitmapMetadata)source.Metadata;
                    date = metaData.DateTaken;
                }
            }
            catch (NotSupportedException e)
            {
                LogHelper.GetLogger().Info(e.Message);
            }
            DateTime dateTaken;
            bool hasDateTaken = DateTime.TryParse(date, out dateTaken);
            return hasDateTaken ? dateTaken.ToString("MMMM yyyy") : file.CreationTimeUtc.ToString("MMMM yyyy");
        }

    }
}
