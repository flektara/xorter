using FileSorter.Lib;
using FileSorter.Lib.Helpers;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace FileSorter.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //edit1
        private string sortType = string.Empty;
        private string _targetFolder;
        private SortProcessor processor;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public MainWindow()
        {
            LogHelper.GetLogger().Info("Application started.");
            InitializeComponent();
            SortBtn.IsEnabled = false;
            TypeSortCheckBox.IsChecked = true;
            StatusLabel.Content = "Waiting";
        }

        private void Browse(object sender, RoutedEventArgs e)
        {
            LogHelper.GetLogger().Info("Browse folder.");
            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            //TODO: bind from textbox
            _targetFolder = dialog.SelectedPath;
            Targetpath.Text = _targetFolder;
            StatusLabel.Content = "Ready";
            SortBtn.IsEnabled = true;
        }

        private void Sort(object sender, RoutedEventArgs e)
        {
            if (_targetFolder == null)
            {
                MessageBox.Show("Select a directory to organize.");
                return;
            }

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;

            processor = new SortProcessor(SortFactory.Resolve(sortType), new DirectoryInfo(_targetFolder));
            bool success = processor.Organize();

            SortBtn.IsEnabled = false;
            _targetFolder = string.Empty;

            if (success)
            {
                StatusLabel.Content = "Folder organized successfully";
                MessageBox.Show("Files sorted successfully");
            }
            else
            {
                StatusLabel.Content = "Error";
                MessageBox.Show("Something went wrong!");
            }

            Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;

            //TODO: implement rollback
        }

        #region <<< Checkbox handlers >>>

        private void DateSortChecked(object sender, RoutedEventArgs e)
        {
            TypeSortCheckBox.IsChecked = false;
            sortType = Constants.DATE_SORT;
        }

        private void TypeSortChecked(object sender, RoutedEventArgs e)
        {
            DateSortCheckBox.IsChecked = false;
            sortType = Constants.TYPE_SORT;
        }

        #endregion
    }
}
