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
using MySql.Data.MySqlClient;

namespace K1J
{
    public partial class Homepage : Window
    {
        private string connStr = "server=ND-COMPSCI;" +
            "user=TL_S2101550;" +
            "database=TL_S2101550_k1j;" +
            "port=3306;" +
            "password=Notre260605";

        private List<string> shoppingCartItems = new List<string>();

        public Homepage(string username)
        {

            InitializeComponent();
            PopulateComboBoxes();
            getUserID();
            session.username = username;
        }

        private void PopulateComboBoxes()
        {
            // Populate Category ComboBox
            List<string> categories = GetCategories();
            cmb_Category.ItemsSource = categories;
        }

        private List<string> GetCategories()
        {
            List<string> categories = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT CategoryName FROM Category";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string category = reader["CategoryName"].ToString();
                        categories.Add(category);
                    }
                }
            }
            return categories;
        }

        private void cmb_Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string selectedCategory = cmb_Category.SelectedItem as string;
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                List<string> products = GetProducts(selectedCategory);
                cmb_Product.ItemsSource = products;
            }
        }

        private List<string> GetProducts(string category)
        {
            List<string> products = new List<string>();
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT ProductName FROM Product WHERE CategoryID = (SELECT CategoryID FROM Category WHERE CategoryName = @Category)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Category", category);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string product = reader["ProductName"].ToString();
                        products.Add(product);
                    }
                }
            }
            return products;
        }
        private void getUserID()
        {
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    string run = "SELECT CustomerID FROM customer WHERE Username = @userName";
                    using (MySqlCommand cmd = new MySqlCommand(run, conn))
                    {
                        cmd.Parameters.AddWithValue("@userName", session.username);
                        using (MySqlDataReader rdr = cmd.ExecuteReader())
                        {
                            if (rdr.Read())
                            {
                                string userID = rdr["CustomerID"].ToString();
                                session.userID = userID;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error with getting user ID: " + ex.Message);
            }
        }

        private void btn_AddToOrder_Click(object sender, RoutedEventArgs e)
        {

            if (cmb_Product.SelectedItem != null)
            {
                string selectedProduct = cmb_Product.SelectedItem.ToString();
                shoppingCartItems.Add(selectedProduct);
                UpdateTotalPrice();
                UpdateShoppingCartListBox();
            }
            else
            {
                MessageBox.Show("Please select a product before adding to the order.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btn_UpdateBox_Click(object sender, RoutedEventArgs e)
        {
            PopulateComboBoxes();
        }
        private void btn_PlaceOrder_Click(object sender, RoutedEventArgs e)
        {
            if (shoppingCartItems.Count > 0)
            {
                InsertOrderIntoDatabase();
                shoppingCartItems.Clear();
                UpdateTotalPrice();
                UpdateShoppingCartListBox();

                MessageBox.Show("Order placed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please add items to the shopping cart before placing an order.", "Empty Cart", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateShoppingCartListBox()
        {

            LB_ShoppingCart.ItemsSource = null; 
            LB_ShoppingCart.ItemsSource = shoppingCartItems;
            lbl_ShoppingCart.Content = $"Shopping Cart ({shoppingCartItems.Count} items)";
        }

        private void UpdateTotalPrice()
        {
            decimal totalPrice = 0;

            foreach (string product in shoppingCartItems)
            {
                decimal productPrice = GetProductPrice(product);
                totalPrice += productPrice;
            }
            Lbl_PriceTotal.Content = $"Total: £{totalPrice}";
        }

        private decimal GetProductPrice(string productName)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT Price FROM Product WHERE ProductName = @ProductName";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductName", productName);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToDecimal(result);
                }
            }

            return 0;
        }
        private void btn_getPrice_Click(object sender, RoutedEventArgs e)
        {
            // Check if a product is selected
            if (cmb_Product.SelectedItem != null)
            {
                string selectedProduct = cmb_Product.SelectedItem.ToString();
                

                // Get and display the price of the selected product
                decimal productPrice = GetProductPriceE(selectedProduct);
                Lbl_PriceEnq.Content = $"Price: £{productPrice}";
                Lbl_SelectedItemPE.Content = $"Selected Item: {selectedProduct}";
            }
            else
            {
                MessageBox.Show("Please select a product for price enquiry.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private decimal GetProductPriceE(string productName)
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT Price FROM Product WHERE ProductName = @ProductName";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductName", productName);

                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToDecimal(result);
                }
            }

            return 0;
        }


        private void InsertOrderIntoDatabase()
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string UserID = session.userID;

                string insertOrderQuery = "INSERT INTO ordertable (CustomerID, OrderDate) VALUES (@CustomerID, NOW())";
                MySqlCommand insertOrderCmd = new MySqlCommand(insertOrderQuery, conn);
                insertOrderCmd.Parameters.AddWithValue("@CustomerID", UserID); 

                insertOrderCmd.ExecuteNonQuery();


                long orderId = insertOrderCmd.LastInsertedId;


                foreach (string product in shoppingCartItems)
                {
                    InsertOrderItem(conn, orderId, product);
                }
            }
        }

        private void InsertOrderItem(MySqlConnection conn, long orderId, string productName)
        {
            string getProductIDQuery = "SELECT ProductID FROM Product WHERE ProductName = @ProductName";
            MySqlCommand getProductIDCmd = new MySqlCommand(getProductIDQuery, conn);
            getProductIDCmd.Parameters.AddWithValue("@ProductName", productName);

            object result = getProductIDCmd.ExecuteScalar();
            if (result != null)
            {
                long productId = Convert.ToInt64(result);
                string insertOrderItemQuery = "INSERT INTO OrderItems (OrderID, ProductID) VALUES (@OrderID, @ProductID)";
                MySqlCommand insertOrderItemCmd = new MySqlCommand(insertOrderItemQuery, conn);
                insertOrderItemCmd.Parameters.AddWithValue("@OrderID", orderId);
                insertOrderItemCmd.Parameters.AddWithValue("@ProductID", productId);

                insertOrderItemCmd.ExecuteNonQuery();
            }
        }


    }
}





