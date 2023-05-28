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
using System.Windows.Shapes;

namespace WpfApp_Hotel
{
    /// <summary>
    /// Interaction logic for GuestsBrowse.xaml
    /// </summary>
    public partial class GuestsBrowse : Window
    {
        private string connectionString = "data source=localhost;initial catalog=hotel2;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
        private string[] userQuery;
        private DataTable dataTableDeafault;
        private string firstName;
        private string lastName;
        public GuestsBrowse()
        {
            InitializeComponent();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT LastName, FirstName, PESEL, DocumentType, DocumentNumber, City, PhoneNumber FROM Guests";
                    SqlCommand commandGuestID = new SqlCommand(query, connection);
                    SqlDataReader reader = commandGuestID.ExecuteReader();

                    dataTableDeafault = new DataTable();
                    dataTableDeafault.Load(reader);
                    dataGrid.ItemsSource = dataTableDeafault.DefaultView;

                    reader.Close();
                
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas wyszukiwania gościa: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Reaguje na kliknięcie text boxa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Background = Brushes.WhiteSmoke;
            textBox.Foreground = Brushes.Black;
            textBox.BorderThickness = new Thickness(0);
            if (textBox.Text == "Search...")
                textBox.Text = string.Empty;

        }

        /// <summary>
        /// Reaguje na opuszczenie text boxa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF394DA0"));
            textBox.Foreground = Brushes.WhiteSmoke;
            textBox.BorderThickness = new Thickness(3);
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {

                userQuery = null;
                textBox.Text = "Search...";
            }

            else
            {
                userQuery = textBox.Text.Split(' ');
                if (userQuery.Length > 2)
                {
                    MessageBoxResult result = MessageBox.Show("no matching results found");

                    // Dadaj opcje dodania nowego gościa
                    if(result == MessageBoxResult.OK)

                    textBox.Text = "Search...";
                }
            }

        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string query;
                    connection.Open();
                    // Zapytanie SQL do pobrania najważniejszych informacji o rezerwacji
                    if (userQuery == null || userQuery.Length == 0)
                        query = $"SELECT LastName, FirstName, PESEL, DocumentType, DocumentNumber, City, PhoneNumber FROM Guests";

                    else
                    {
                        query = $"SELECT LastName, FirstName, PESEL, DocumentType, DocumentNumber, City, PhoneNumber FROM Guests WHERE LastName LIKE '{userQuery[0]}%' OR FirstName LIKE '{userQuery[0]}%'";
                    }

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataGrid.ItemsSource = dataTable.DefaultView;

                    if (dataGrid.Items.Count < 1)
                    {
                        MessageBox.Show("Do you want to add new guest?", "No matching results found", MessageBoxButton.OK);
                    }

                    reader.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas wyszukiwania gościa: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
        }
        

        private void GuestChosen_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string documentNr = "";
                    DataRowView rowView = dataGrid.SelectedItem as DataRowView;
                    if (rowView != null)
                    {
                        string columnName = "DocumentNumber";
                        documentNr = rowView[columnName].ToString();

                        // Zapytanie SQL do pobrania id goscia
                        string query = $"SELECT GuestID, FirstName, LastName FROM Guests WHERE DocumentNumber = '{documentNr}'";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@SortOption", documentNr);
                        SqlDataReader reader = command.ExecuteReader();


                        // Wypisanie wyników 
                        while (reader.Read())
                        {
                            lastName = reader["LastName"].ToString();
                            firstName = reader["FirstName"].ToString();

                        }
                        reader.Close();

                        //Przekazanie danych do NewReservation
                        NewReservation newReservation = new NewReservation();
                        newReservation.SetGuestData(firstName, lastName);
                        newReservation.Show();

                        

                        newReservation.guestName = firstName;
                        newReservation.guestLastName = lastName;

                    }
                    else
                        documentNr = "";
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas wybierania gościa: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


    }
}
