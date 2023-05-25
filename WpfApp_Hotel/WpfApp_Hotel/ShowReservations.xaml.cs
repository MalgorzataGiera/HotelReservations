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

        public ShowReservations()
        {
            InitializeComponent();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT g.LastName, g.FirstName, r.CheckInDate, r.CheckOutDate, r.ReservationID FROM Guests g JOIN Reservations r ON g.GuestId = r.GuestID";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataGrid.ItemsSource = dataTable.DefaultView;

                    reader.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas pobierania rezerwacji: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }


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

                        MessageBox.Show(resID);



                        // Zapytanie SQL do pobrania wszystkich informacji o rezerwacji
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
        

    }
}
