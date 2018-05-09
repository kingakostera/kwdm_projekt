using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;
using System.Data.SqlClient;


namespace KWDM
{
    public partial class SegmentationWindow : Window
    {
        SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Kinga\Desktop\KWDM\KWDM\KWDM\KWDM.mdf;Integrated Security=True;Connect Timeout=30");
        List<Patient> data = new List<Patient>();
        MLApp.MLApp matlab = new MLApp.MLApp();
        string debug_path = AppDomain.CurrentDomain.BaseDirectory;
        string matlabFunctionPath;
        string selectedItemPath;

        public SegmentationWindow()
        {
            InitializeComponent();
        }
        #region Constructor


        #endregion

        #region On Loaded

        /// <summary>
        /// When the application first opens
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var tree = new TreeView();
            tree.Name = "Baza";

            List<string> patients = DataBase_DownloadData("PatientID");
            foreach (var patient in patients)
            {
                var item = new TreeViewItem()
                {
                    Tag = patient,
                    Header = patient
                };
                item.Items.Add(null);
                item.Expanded += Folder_Expanded;
                FolderView.Items.Add(item);
            }
        }

        #endregion

        #region Folder Expanded

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            #region Initial Checks

            var item = (TreeViewItem)sender;

            if (item.Items.Count != 1 || item.Items[0] != null)
                return;

            item.Items.Clear();

            var fullPath = "";
            if (item.Tag.ToString().Length > 7)
            {
                fullPath = (string)item.Tag;
            }
            else
                fullPath = debug_path + "Baza\\" + (string)item.Tag;


            #endregion

            #region Get Folders

            var directories = new List<string>();

            try
            {
                var dirs = Directory.GetDirectories(fullPath);

                if (dirs.Length > 0)
                    directories.AddRange(dirs);
            }
            catch { }

            directories.ForEach(directoryPath =>
            {
                var subItem = new TreeViewItem()
                {
                    Header = GetFileFolderName(directoryPath),

                    Tag = directoryPath
                };

                subItem.Items.Add(null);

                subItem.Expanded += Folder_Expanded;

                item.Items.Add(subItem);
            });

            #endregion

            #region Get Files

            var files = new List<string>();

            try
            {
                var fs = Directory.GetFiles(fullPath);

                if (fs.Length > 0)
                    files.AddRange(fs);
            }
            catch { }

            files.ForEach(filePath =>
            {
                var subItem = new TreeViewItem()
                {
                    Header = GetFileFolderName(filePath),

                    Tag = filePath
                };

                item.Items.Add(subItem);
            });

            #endregion
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Find the file or folder name from a full path
        /// </summary>
        /// <param name="path">The full path</param>
        /// <returns></returns>
        public static string GetFileFolderName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            var normalizedPath = path.Replace('/', '\\');

            var lastIndex = normalizedPath.LastIndexOf('\\');

            if (lastIndex <= 0)
                return path;

            return path.Substring(lastIndex + 1);
        }

        #endregion

        public void Baza()
        {
            try
            {
                StreamReader sr = new StreamReader("seriesInstanceUids1521138224642.csv");

                string line;

                string pierwsza = sr.ReadLine();
                while ((line = sr.ReadLine()) != null)
                {
                    string[] linia = line.Split(',');
                    Patient patient = new Patient();
                    patient.Collection = linia[0];
                    patient.PatientID = linia[1];
                    patient.StudyDate = linia[2];
                    patient.StudyDescription = linia[3];
                    patient.Modality = linia[4];
                    patient.SeriesDescription = linia[5];
                    patient.Manufacturer = linia[6];
                    patient.ManufacturerModel = linia[7];
                    patient.SoftwareVersion = linia[8];
                    patient.SeriesUID = linia[9];

                    data.Add(patient);
                }
                sr.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Błąd bazy danych");
            }
        }

        private void TreeViewItem_OnItemSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem item = (TreeViewItem)FolderView.SelectedItem;
                ItemsControl parent = GetSelectedTreeViewItemParent(item);
                string selectedItem = item.Tag.ToString();
                if (selectedItem[selectedItem.Length - 1] == 'm')
                {
                    matlabFunctionPath = parent.Tag.ToString();
                    //matlabFunctionPath = item.Tag.ToString();
                }
                else
                    matlabFunctionPath = parent.Tag.ToString();
                //matlabFunctionPath = item.Tag.ToString();
                selectedItemPath = item.Tag.ToString();
            }
            catch (Exception)
            { }
        }
        public ItemsControl GetSelectedTreeViewItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as ItemsControl;
        }

        private List<string> DataBase_DownloadData(string column)
        {
            List<string> list = new List<string>();
            if (sqlCon.State == ConnectionState.Closed)
                sqlCon.Open();
            SqlCommand sqlCmd = new SqlCommand("SELECT DISTINCT " + column + " FROM tbStudies", sqlCon);
            try
            {
                SqlDataReader dataReader = sqlCmd.ExecuteReader();

                while (dataReader.Read())
                {
                    string listElement = dataReader[column].ToString();
                    list.Add(listElement);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlCon.Close();
            }
            return list;
        }
        //private List<string> DataBase_DownloadData(string selectedColumn, string conditionColumn, string condition)
        //{
        //    List<string> list = new List<string>();
        //    if (sqlCon.State == ConnectionState.Closed)
        //        sqlCon.Open();
        //    SqlCommand sqlCmd = new SqlCommand("SELECT DISTINCT " + selectedColumn + " FROM tbStudies WHERE " + conditionColumn + " like '" + condition + "'", sqlCon);
        //    try
        //    {
        //        SqlDataReader dataReader = sqlCmd.ExecuteReader();

        //        while (dataReader.Read())
        //        {
        //            string listElement = dataReader[selectedColumn].ToString();
        //            list.Add(listElement);
        //        }
        //    }

        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    finally
        //    {
        //        sqlCon.Close();
        //    }
        //    return list;
        // }
        private void automaticalSegmentation_Click(object sender, RoutedEventArgs e)
        {
            metaData();
            object result = null;
            matlab.Feval("automaticalSegmentation", 0, out result, matlabFunctionPath);
            object[] res = result as object[];
            Display("wyniki_segmentacji_automatycznej");
        }
        private void ClearFromTree()
        {
            contour.Source = null;

            Remove();
        }
        public void Display(string segmentacja)
        {
            try
            {
                // element zaznaczony
                TreeViewItem treeViewItem = (TreeViewItem)FolderView.SelectedItem;
                // folder wyżej od zaznaczonego
                ItemsControl item = GetSelectedTreeViewItemParent(treeViewItem);
                string c = item.Tag.ToString();

                TreeViewItem wynikSeg;

                // elementy w folderze
                ItemCollection itemCollection = item.Items;

                var folders = new List<string>();

                // FOLDERY występujące w tej samej lokalizacji co element zaznaczony
                var folderss = Directory.GetDirectories(c);

                // ustalenie folderu z wynikami jako parent
                string d;
                if (segmentacja == "wyniki_segmentacji_automatycznej")
                {
                    d = folderss[0];
                    wynikSeg = (TreeViewItem)itemCollection[0];
                }
                else
                {
                    d = folderss[1];
                    wynikSeg = (TreeViewItem)itemCollection[1];
                }

                // pobranie obrazow z folderu z wynikami
                var files = new List<string>();
                var fs = Directory.GetFiles(d);

                if (fs.Length > 0)
                    files.AddRange(fs);
                int ind = fs.Length - 1;

                // wyswietlenie wyniku
                contour.Source = new BitmapImage(new Uri(fs[ind]));


                var subItem = new TreeViewItem()
                {
                    Header = GetFileFolderName(fs[ind]),

                    Tag = fs[ind]
                };
                wynikSeg.Items.Add(subItem);

            }
            catch { }
        }
        public void Remove()
        {
            TreeViewItem treeViewItem = (TreeViewItem)FolderView.SelectedItem;
            ItemsControl item = GetSelectedTreeViewItemParent(treeViewItem);
            string c = item.Tag.ToString();
            ItemCollection itemCollection = item.Items;
            TreeViewItem wynikSeg = (TreeViewItem)itemCollection[0];
            string d = wynikSeg.Tag.ToString();
            ItemCollection pngImages = wynikSeg.Items;

            for (int i = pngImages.Count - 1; i > 0; i--)
            {
                TreeViewItem vItem = (TreeViewItem)pngImages[i];
                string a = vItem.Tag.ToString();
                string b = vItem.Tag.ToString();
                pngImages.RemoveAt(i);

            }
            if (itemCollection == null)
            {
                return;
            }
        }
        private void manualSegmentation_Click(object sender, RoutedEventArgs e)
        {
            metaData();
            object result = null;
            matlab.Feval("manualSegmentation", 0, out result, matlabFunctionPath);
            object[] res = result as object[];
            Display("wyniki_segmentacji_manualnej");
        }
        private void displayContour(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewItem treeViewItem = (TreeViewItem)FolderView.SelectedItem;
                string selectedItem = treeViewItem.Tag.ToString();
                if (selectedItem[selectedItem.Length - 1] == 'g')
                {
                    string temp = matlabFunctionPath;
                    matlabFunctionPath = Directory.GetParent(matlabFunctionPath).ToString();
                    metaData();
                    matlabFunctionPath = temp;
                    contour.Source = new BitmapImage(new Uri(selectedItemPath));
                }
                else
                {
                    //contour.Source = new BitmapImage(new Uri(selectedItem));
                    MessageBox.Show("Jeszcze nie zaimplementowano funkcji wyświetlania plików DICOM.");
                }
            }
            catch { }

        }
        private void metaData()
        {
            matlab.Execute(@"cd " + debug_path);
            object metadata_result = null;
            matlab.Feval("metadata", 12, out metadata_result, matlabFunctionPath);
            object[] metadata_results = metadata_result as object[];

            patientNameValue.Content = metadata_results[0];
            patientIDValue.Content = metadata_results[1];
            patientSexValue.Content = metadata_results[2];
            patientAgeValue.Content = metadata_results[3];
            patientWeightValue.Content = metadata_results[4];
            studyDateValue.Content = metadata_results[5];
            studyTimeValue.Content = metadata_results[6];
            studyModalityValue.Content = metadata_results[7];
            studyDescriptionValue.Content = metadata_results[8];

            seriesDateValue.Content = metadata_results[9];
            seriesTimeValue.Content = metadata_results[10];
            seriesDescriptionValue.Content = metadata_results[11];
        }

    }
}
