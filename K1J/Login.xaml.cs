using MySql.Data.MySqlClient;
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

namespace K1J
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {

        public Login()
        {
            InitializeComponent();
        }

        private void txt_login_Click(object sender, RoutedEventArgs e)
        {
            string connStr = "server=ND-COMPSCI;" + 
                "user=TL_S2101550;" + 
                "database=TL_S2101550_k1j;" + 
                "port=3306;" + 
                "password=Notre260605";//Connection string
            MySqlConnection conn;

            string username = txt_username.Text;
            string password = txt_password.Password;

            using (conn = new MySqlConnection(connStr))
            {
                if (username == "JJ")
                {
                    conn.Open();
                    string query = "SELECT * FROM customer WHERE Username = @Username AND Password = SHA(@Password)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);//anti_Sql stuff

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            AdminPanel adminPanel = new AdminPanel(username);
                            adminPanel.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password");
                        }
                    }
                }
                else
                {

                    conn.Open();
                    string query = "SELECT * FROM customer WHERE Username = @Username AND Password = SHA(@Password)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);//anti_Sql stuff

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            Homepage Homepage = new Homepage(username);
                            Homepage.Show();
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password");
                        }
                    }
                }

            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            register register  = new register();
            register.Show();
            this.Close ();

        }




    }



}
