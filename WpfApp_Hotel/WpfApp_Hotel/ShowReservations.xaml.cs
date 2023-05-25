using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WpfApp_Hotel
{
    /// <summary>
    /// Interaction logic for ShowReservations.xaml
    /// </summary>
    public partial class ShowReservations : Window
    {
        private List<string> reservationsData = new List<string>();
        private string connectionString = "data source=localhost;initial catalog=hotel;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
        private string[] userQuery;
        private DataTable dataTableDeafault;
        /// <summary>
        /// Wyświetla wszystkie rezerwacje
        /// </summary>
        public ShowReservations()
        {
            InitializeComponent();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Zapytanie SQL do pobrania najważniejszych informacji o rezerwacji
                    string query = $"SELECT g.LastName, g.FirstName, r.CheckInDate, r.CheckOutDate, r.ReservationID FROM Guests g JOIN Reservations r ON g.GuestId = r.GuestID";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    dataTableDeafault = new DataTable();
                    dataTableDeafault.Load(reader);
                    dataGrid.ItemsSource = dataTableDeafault.DefaultView;

                    reader.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas pobierania rezerwacji: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
        /// <summary>
        /// Wyświetla szczegółowe informacje o rezeracji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reservationsListBox_Click(object sender, MouseButtonEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string resID = "";
                    DataRowView rowView = dataGrid.SelectedItem as DataRowView;
                    if (rowView != null)
                    {
                        string columnName = "ReservationID";
                        resID = rowView[columnName].ToString();

                        // Zapytanie SQL do pobrania wszystkich szczegółowych informacji o rezerwacji
                        string query = $"SELECT r.*, g.FirstName AS gFirstName, g.LastName AS gLastName, e.FirstName AS eFirstName, e.LastName AS eLastName FROM Reservations r JOIN Guests g ON r.GuestID = g.GuestID JOIN Employees e ON r.EmployeeID = e.EmployeeID WHERE r.ReservationID = {resID}";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@SortOption", resID);
                        SqlDataReader reader = command.ExecuteReader();

                        // Wypisanie wyników 
                        while (reader.Read())
                        {
                            string gLastName = reader["gLastName"].ToString();
                            string gFirstName = reader["gFirstName"].ToString();
                            DateTime checkInDate = (DateTime)reader["CheckInDate"];
                            DateTime checkOutDate = (DateTime)reader["CheckOutDate"];
                            string room = reader["RoomNumber"].ToString();
                            string status = reader["Status"].ToString();
                            string eLastName = reader["eLastName"].ToString();
                            string eFirstName = reader["eFirstName"].ToString();
                            string ifSettled = reader["IfSettled"].ToString();
                            string GuestID = reader["GuestID"].ToString();

                            // Zmiana formatu daty
                            string formattedCheckInDate = checkInDate.ToString("yyyy.MM.dd");
                            string formattedCheckOutDate = checkOutDate.ToString("yyyy.MM.dd");

                            MessageBox.Show($"Guest: {gFirstName} {gLastName} | Reservation from: {formattedCheckInDate} to: {formattedCheckOutDate} | Room nr: {room} | Status: {status} | Settled: {ifSettled} | Reservations accepted by: {eFirstName} {eLastName}");
                        }
                        reader.Close();
                    }
                    else
                        resID = "";
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas pobierania szczegółowych informacji o rezerwacji: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (textBox.Text == "Search for guest name...")
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
                textBox.Text = "Search for guest name...";
            else
            {
                userQuery = textBox.Text.Split(' ');
                if (userQuery.Length > 2)
                {
                    MessageBox.Show("no matching results found");
                    textBox.Text = "Search for guest name...";
                    //Przywroć pierwotny wygląd tabeli
                    dataGrid.ItemsSource = dataTableDeafault.DefaultView;
                } 
            }
                
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Zapytanie SQL do pobrania najważniejszych informacji o rezerwacji
                    string query = $"SELECT g.LastName, g.FirstName, r.CheckInDate, r.CheckOutDate, r.ReservationID FROM Guests g JOIN Reservations r ON g.GuestId = r.GuestID WHERE g.LastName LIKE '{userQuery[0]}%' OR g.FirstName LIKE '{userQuery[0]}%'";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataGrid.ItemsSource = dataTable.DefaultView;

                    if (dataGrid.Items.Count < 1)
                    {
                        MessageBox.Show("no matching results found");
                        dataGrid.ItemsSource = dataTableDeafault.DefaultView;
                        textBoxForSearch.Text = "Search for guest name...";
                    }
                        

                    reader.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas pobierania rezerwacji: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            
        }
    }
}
