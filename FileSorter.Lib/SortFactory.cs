using FileSorter.Lib.Abstractions;
using FileSorter.Lib.Helpers;
using FileSorter.Lib.Modules;

namespace FileSorter.Lib
{
    public class SortFactory
    {
        public static Sorter Resolve(string sortType)
        {
            switch (sortType)
            {
                case (Constants.DATE_SORT):
                    return new DateSorter();
                default:
                    return new TypeSorter();
            }
        }
    }
}
