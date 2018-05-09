using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KWDM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(tbLogin.Text) || pbPasswordBox.Password == "")
            {
                MessageBox.Show("Podaj login i hasło");
            }
            else
            {
                SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\v11.0;AttachDbFilename=C:\Users\Kinga\Desktop\KWDM\KWDM\KWDM\KWDM.mdf;Integrated Security=True;Connect Timeout=30");
                try
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    { sqlCon.Open(); }
                    string query = "SELECT COUNT(1) FROM tbLogins WHERE Login = @UserName AND Password=@UserPassword";
                    SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                    sqlCmd.CommandType = CommandType.Text;
                    sqlCmd.Parameters.AddWithValue("@UserName", tbLogin.Text);
                    sqlCmd.Parameters.AddWithValue("@UserPassword", pbPasswordBox.Password);
                    int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                    if (count == 1)
                    {
                        SegmentationWindow segmentation = new SegmentationWindow();
                        segmentation.Show();
                        this.Close();
                    }
                    else
                        MessageBox.Show("Nieprawidłowy login lub hasło");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    sqlCon.Close();

                }
                
                
            }
        }
    }
}
