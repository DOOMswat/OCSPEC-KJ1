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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
namespace K1J
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string username;
        private System.Windows.Threading.DispatcherTimer timer;

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Text = string.Empty;
                textBox.GotFocus -= TextBox_GotFocus;
            }
            else if (sender is PasswordBox passwordBox)
            {
                passwordBox.Password = string.Empty;
                passwordBox.GotFocus -= TextBox_GotFocus;
            }
        }
        public MainWindow(string username)
        {
            InitializeComponent();
            UpdateUserJoined();
            this.username = username;
            userChat.Content = "Welcome, " + username + "!";//make work later

            // Set up a timer for realtime updates
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTickyTocky);
            timer.Interval = new TimeSpan(0, 0, 5); // Update every 5 seconds cuz not work without tick for some reason (almost realtime)
            timer.Start();
            UpdateChat();
        }

        private void timerTickyTocky(object sender, EventArgs e)
        {
            UpdateChat();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string connStr = "server=ND-COMPSCI;" + "user=TL_S2101550;" + "database=TL_S2101550_k1j;" + "port=3306;" + "password=Notre260605";
            MySqlConnection conn;
            string messageText = MessageBox.Text;

            using (conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "INSERT INTO InstantMessages (SenderUserID, MessageText) VALUES ((SELECT UserID FROM Users WHERE Username = @Username), @Message)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Message", messageText);//antisql injection

                cmd.ExecuteNonQuery();
            }

            MessageBox.Text = "";
        }
        private void UpdateChat()
        {
            string connStr = "server=ND-COMPSCI;" + "user=TL_S2101550;" + "database=TL_S2101550_k1j;" + "port=3306;" + "password=Notre260605";
            MySqlConnection conn;
            using (conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT Username, MessageText FROM InstantMessages INNER JOIN Users ON InstantMessages.SenderUserID = Users.UserID ORDER BY Timestamp DESC LIMIT 10";//long ahh query
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    StringBuilder chatBuilder = new StringBuilder();

                    while (reader.Read())
                    {
                        string sender = reader["Username"].ToString();
                        string message = reader["MessageText"].ToString();
                        chatBuilder.AppendLine($"{sender}: {message}");
                    }

                    userChat.Content = chatBuilder.ToString();
                }
            }
        }

        private void UpdateUserJoined()
        {

            string connStr = "server=ND-COMPSCI;" + "user=TL_S2101550;" + "database=TL_S2101550_k1j;" + "port=3306;" + "password=Notre260605";
            MySqlConnection conn;
            try
            {


                using (conn = new MySqlConnection(connStr))
                {

                    conn.Open();
                    string query = $"SELECT Username FROM users Where Username = ('{username}')";
                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        StringBuilder chatBuilder = new StringBuilder();

                        while (reader.Read())
                        {
                            string userperson = reader["Username"].ToString();
                            chatBuilder.AppendLine($"{userperson}, has joined the chat!");
                        }

                        userChat.Content = chatBuilder.ToString();



                    }
                }

            }
            catch (Exception ex) 
            {
                MessageBox.AppendText(ex.Message);

            
            }

        }











    }
}






