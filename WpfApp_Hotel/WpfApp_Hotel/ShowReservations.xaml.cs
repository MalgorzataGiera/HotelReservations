using System;
using System.Collections.Generic;
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
        private string option;
        private List<string> reservationsData = new List<string>();
        private string connectionString = "data source=localhost;initial catalog=hotel;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";

        public ShowReservations()
        {
            InitializeComponent();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Zapytanie SQL do pobrania wszystkich rezerwacji
                    string query = $"SELECT g.LastName, g.FirstName, r.CheckInDate, r.CheckOutDate, r.ReservationID FROM Guests g JOIN Reservations r ON g.GuestId = r.GuestID";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    // Przypisanie wyników do kontrolki ListBox
                    while (reader.Read())
                    {
                        string LastName = reader["LastName"].ToString();
                        string FirstName = reader["FirstName"].ToString();
                        DateTime checkInDate = (DateTime)reader["CheckInDate"];
                        DateTime checkOutDate = (DateTime)reader["CheckOutDate"];
                        string resID = reader["ReservationID"].ToString();

                        // Zmiana formatu daty
                        string formattedCheckInDate = checkInDate.ToString("yyyy.MM.dd");
                        string formattedCheckOutDate = checkOutDate.ToString("yyyy.MM.dd");


                        reservationsListBox.Items.Add(resID + " " + LastName + " " + FirstName + " " + formattedCheckInDate + " - " + formattedCheckOutDate);
                    }
                    reader.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas pobierania rezerwacji: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
        }
        public void UpdateReservationsList()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Zapytanie SQL do pobrania wszystkich rezerwacji
                    string query = $"SELECT g.LastName, g.FirstName, r.CheckInDate, r.CheckOutDate, r.ReservationID FROM Guests g JOIN Reservations r ON g.GuestId = r.GuestID ORDER BY {option}";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SortOption", option); // Przekazanie wartości wybranej opcji jako parametru

                    SqlDataReader reader = command.ExecuteReader();
                    // Wyczyść zawartość kontrolki ListBox
                    reservationsListBox.Items.Clear();

                    // Przypisanie wyników do kontrolki ListBox
                    while (reader.Read())
                    {
                        string resID = reader["ReservationID"].ToString();
                        string LastName = reader["LastName"].ToString();
                        string FirstName = reader["FirstName"].ToString();
                        //string resID = reader["ReservationID"].ToString();
                        DateTime checkInDate = (DateTime)reader["CheckInDate"];
                        DateTime checkOutDate = (DateTime)reader["CheckOutDate"];

                        // Zmiana formatu daty
                        string formattedCheckInDate = checkInDate.ToString("yyyy.MM.dd");
                        string formattedCheckOutDate = checkOutDate.ToString("yyyy.MM.dd");
                        
                        
                        reservationsListBox.Items.Add(resID + " " + LastName + " " + FirstName + " " + formattedCheckInDate + " - " + formattedCheckOutDate);
                    }
                    reader.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas pobierania rezerwacji: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            option = sortOptions.SelectedIndex.ToString();
            switch (option)
            {
                case "0": option = "ReservationID ASC"; break;
                case "1": option = "ReservationID DESC"; break;
                case "2": option = "LastName"; break;
                case "3": option = "LastName DESC"; break;
                case "4": option = "CheckInDate ASC"; break;
                case "5": option = "CheckInDate DESC"; break;
                case "6": option = "CheckOutDate ASC"; break;
                case "7": option = "CheckOutDate DESC"; break;
                default: option = "ReservationID ASC"; break;
            }
            UpdateReservationsList();
        }
        //private void reservationsListBox_Click(object sender, MouseButtonEventArgs e)
        //{
        //    if (reservationsListBox.SelectedItem != null)
        //    {
        //        // Pobierz wybrany element ListBox
        //        string selectedReservation = reservationsListBox.SelectedItem.ToString();

        //        // Wykonaj określone działania dla wybranego elementu
        //        // np. wyświetl szczegóły rezerwacji w innym kontrolce
        //        MessageBox.Show("info");
        //    }
        //}

        private void reservationsListBox_Click(object sender, MouseButtonEventArgs e)
        {
            string oneID ="";
            if (reservationsListBox.SelectedItem != null)
            {
                //Pobierz wybrany element ListBox
                string selectedReservation = reservationsListBox.SelectedItem.ToString();

                //Podziel wybrany element na poszczególne części
                string[] parts = selectedReservation.Split(' ');

                if (parts.Length >= 2)
                    oneID = parts[0];
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Zapytanie SQL do pobrania wszystkich informacji o rezerwacji
                    string query = $"SELECT r.*, g.FirstName AS gFirstName, g.LastName AS gLastName, e.FirstName AS eFirstName, e.LastName AS eLastName FROM Reservations r JOIN Guests g ON r.GuestID = g.GuestID JOIN Employees e ON r.EmployeeID = e.EmployeeID WHERE r.ReservationID = {oneID}";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@SortOption", oneID);
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
                catch (SqlException ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas pobierania szczegółowych informacji o rezerwacji: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
