using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SetTableProject
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string connectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\igrok\\source\\repos\\SetTableProject\\SetTableProject\\Database1.mdf;Integrated Security=True";
        public SqlConnection sqlConnection;
        public DBManager dbManager;
        public MainWindow()
        {
            InitializeComponent();
            dbManager = new DBManager(this);
            GenerateDBConnectionString();
            Process();
        }

        public void GenerateDBConnectionString()
        {
            var appDir = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var relativePath = "Database1.mdf"; // ваш относительный путь
            var fullPath = System.IO.Path.Combine(appDir, relativePath);
            connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename=" + fullPath + "; Integrated Security = True";
        }

        async public void Process()
        {

        }

        public void ConnectionToDB()
        {
            //sqlConnection = new SqlConnection(connectionString);
            //sqlConnection.Open();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(customersTab.IsSelected)
            {
                dbManager.LoadDataCustomers();
            }
            else if(itemsTab.IsSelected)
            {
                dbManager.LoadDataItems();
            }
            else if (ordersTab.IsSelected)
            {
                dbManager.LoadDataOrders();
            }
            else if (registersTab.IsSelected)
            {
                dbManager.LoadDataRegisters();
            }
        }

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO customer (Name, LastName, Email,EmType) VALUES (@Name, @LastName, @Email, @EmType)";
                    using (var cmd = new SqlCommand(insertQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", $"{CustomerNameTextBox.Text}");
                        cmd.Parameters.AddWithValue("@LastName", $"{CustomerLastNameTextBox.Text}");
                        cmd.Parameters.AddWithValue("@Email", $"{CustomerEmailTextBox.Text}");
                        cmd.Parameters.AddWithValue("@EmType", $"{CustomerStatusTextBox.Text}");
                        cmd.ExecuteNonQuery();
                    }
                    dbManager.LoadDataCustomers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при занесении данных: " + ex.Message);
            }
        }

        private void RemoveCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(CustomerIdTextBox.Text);
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string deleteQuery = "DELETE FROM customer WHERE Id = @CustomerID";
                    using (var cmd = new SqlCommand(deleteQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", id); // Замените на ID клиента, которого нужно удалить
                        cmd.ExecuteNonQuery();
                    }
                }
                dbManager.LoadDataCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении данных: " + ex.Message);
            }
            
        }

        private void ItemAddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO Item (Name, Price) VALUES (@Name, @Price)";
                    using (var cmd = new SqlCommand(insertQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", $"{ItemNameTextBox.Text}");
                        cmd.Parameters.AddWithValue("@Price", $"{ItemPriceTextBox.Text}");
                        cmd.ExecuteNonQuery();
                    }
                    dbManager.LoadDataItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при занесении данных: " + ex.Message);
            }
        }

        private void ItemEditButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string updateQuery = "UPDATE customer SET Name = @Name, Price = @Price WHERE Id = @CustomerID";
                    using (var cmd = new SqlCommand(updateQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", ItemNameTextBox.Text);
                        cmd.Parameters.AddWithValue("@Price", ItemPriceTextBox.Text);
                        cmd.Parameters.AddWithValue("@CustomerID", ItemIdTextBox.Text); // ID клиента, которого нужно изменить
                        cmd.ExecuteNonQuery();
                    }
                    dbManager.LoadDataItems();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при изменении данных: " + ex.Message);
            }
        }

        private void ItemDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(ItemIdTextBox.Text);
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string deleteQuery = "DELETE FROM Item WHERE Id = @CustomerID";
                    using (var cmd = new SqlCommand(deleteQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", id); // Замените на ID клиента, которого нужно удалить
                        cmd.ExecuteNonQuery();
                    }
                }
                dbManager.LoadDataCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении данных: " + ex.Message);
            }
        }
    }

    public partial class DBManager
    {
        public MainWindow mainWindow;

        public DBManager(MainWindow window)
        {
            mainWindow = window;
        }

        public void LoadDataCustomers()
        {
            try
            {
                var customers = new List<Customer>();
                using (var connection = new SqlConnection(mainWindow.connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM customer"; // Замените на ваш запрос
                    using (var cmd = new SqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new Customer
                            {
                                Id = (int)reader["Id"],
                                Name = (string)reader["Name"],
                                LastName = (string)reader["LastName"],
                                Email = (string)reader["Email"],
                                EmType = (string)reader["EmType"]
                            });
                        }
                    }
                }
                mainWindow.cutomersDT.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        public void LoadDataItems()
        {
            try
            {
                var items = new List<Item>();
                using (var connection = new SqlConnection(mainWindow.connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Item"; // Замените на ваш запрос
                    using (var cmd = new SqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new Item
                            {
                                Id = (int)reader["Id"],
                                Name = (string)reader["Name"],
                                Price = (string)reader["Price"]
                            });
                        }
                    }
                }
                mainWindow.ItemsDT.ItemsSource = items;
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        public void LoadDataOrders()
        {
            try
            {
                var orders = new List<Order>();
                using (var connection = new SqlConnection(mainWindow.connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM orders"; // Замените на ваш запрос
                    using (var cmd = new SqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new Order
                            {
                                Id = (int)reader["Id"],
                                EmId = (string)reader["EmId"],
                                Date = (string)reader["Date"],
                                OrderItems = (string)reader["OrderItems"]
                            });
                        }
                    }
                }
                mainWindow.OrdersDT.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        public void LoadDataRegisters()
        {
            try
            {
                var registerRequest = new List<RegisterRequest>();
                using (var connection = new SqlConnection(mainWindow.connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM RegisterRequests"; // Замените на ваш запрос
                    using (var cmd = new SqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            registerRequest.Add(new RegisterRequest
                            {
                                Id = (int)reader["Id"],
                                Name = (string)reader["Name"],
                                LastName = (string)reader["LastName"],
                                Email = (string)reader["Email"],
                                EmType = (string)reader["EmType"]
                            });
                        }
                    }
                }
                mainWindow.registersDT.ItemsSource = registerRequest;
            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string EmType { get; set; }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public string EmId { get; set; }
        public string Date { get; set; }
        public string OrderItems { get; set; }
    }

    public class RegisterRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string EmType { get; set; }
    }
}
