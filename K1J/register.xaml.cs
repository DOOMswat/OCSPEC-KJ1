using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
    /// Interaction logic for register.xaml
    /// </summary>
    public partial class register : Window
    {
        public register()
        {
            InitializeComponent();
        }

        private void registerButt_Click(object sender, RoutedEventArgs e)
        {
            string connStr = "server=ND-COMPSCI;" + "user=TL_S2101550;" + "database=TL_S2101550_k1j;" + "port=3306;" + "password=Notre260605";
            MySqlConnection conn;
            string username = reg_username.Text;
            string FirstName = reg_FirstName.Text;
            string LastName = reg_LastName.Text;
            string Email = reg_Email.Text;
            string password = reg_password.Password;
            string conPass = reg_conPassword.Password;


            if (conPass == password)
            {
                try
                {
                    using (conn = new MySqlConnection(connStr))
                    {
                        conn.Open();
                        string checkQuery = "SELECT COUNT(*) FROM customer WHERE Username = @Username";//Shows count of how many user have the same username. If count is bigger than 0, return.
                        using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                        {
                            checkCmd.Parameters.AddWithValue("@Username", reg_username.Text); //ANTI-SQLINJECTION STUFF
                            int userCount = Convert.ToInt32(checkCmd.ExecuteScalar());
                            if (userCount > 0)
                            {
                                MessageBox.Show("Username already exists. Choose a different username.");
                                return;
                            }
                            else
                            {
                                string insertUserQuery = "INSERT INTO customer (Username, FirstName, LastName, Email, Password) VALUES (@Username, @FirstName, @LastName, @Email, SHA(@password))";
                                using (MySqlCommand insertCustomerCmd = new MySqlCommand(insertUserQuery, conn)) //ANTI-SQLINJECTION STUFF
                                {
                                    insertCustomerCmd.Parameters.AddWithValue("@Username", username);
                                    insertCustomerCmd.Parameters.AddWithValue("@FirstName", FirstName);
                                    insertCustomerCmd.Parameters.AddWithValue("@LastName", LastName);
                                    insertCustomerCmd.Parameters.AddWithValue("@Email", Email);
                                    insertCustomerCmd.Parameters.AddWithValue("@password", password);
                                    insertCustomerCmd.ExecuteNonQuery();
                                }
                                MessageBox.Show("User successfully created");
                                Homepage Homepage = new Homepage(username);
                                Homepage.Show();
                                this.Close();
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error..." + ex);
                }
            }
            else
            {
                MessageBox.Show("Password does not match.");
            }
            
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}
