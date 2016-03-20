using FileSorter.Lib.Entities;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace FileSorter.Lib.Helpers
{
    internal static class ConfigurationHelper
    {
        public static ICollection<FolderData> GetDestinations()
        {
            var xConfig = XDocument.Load(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath);
            var folderNames = new List<FolderData>();
            var extensionsNode = xConfig.XPathSelectElements("//Extensions");

            if (extensionsNode == null || extensionsNode.Elements() == null)
                throw new ConfigurationErrorsException("Missing Extensions in configuration.");

            foreach (var item in extensionsNode.Elements())
            {
                folderNames.Add(new FolderData
                    {
                        FolderName = item.Name.LocalName,
                        Extensions = item.Elements().Select(x => x.Value.Substring(1)).ToList()
                    });
            }

            return folderNames;
        }
    }
}
