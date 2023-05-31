using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WpfApp_Hotel
{
    /// <summary>
    /// Interaction logic for NewReservation.xaml
    /// </summary>
    public partial class NewReservation : Window
    {
        private string connectionString = "data source=localhost;initial catalog=hotel2;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";
        
        private int employeeID;
        private int guestID;
        private string checkIn = "";
        private string checkOut = "";

        private string guest;
        public string guestName;
        public string guestLastName;

        public int room;
        private string phone;

        private string mail;
        private string employee;
        private string employeeName = "";
        private string employeeLastName = "";

        /// <summary>
        /// Tworzy okno umożliwiające dodawanie nowych rezerwacji
        /// </summary>
        public NewReservation()
        {
            InitializeComponent();
            _checkIn.Loaded += DatePicker1_Loaded;
            _checkOut.Loaded += DatePicker2_Loaded;
            using (SqlConnection connection= new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT FirstName, LastName FROM Employees";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader= command.ExecuteReader();

                    List<string> employees = new List<string>();
                    while(reader.Read())
                    {
                        employees.Add(reader["FirstName"].ToString() + " "  + reader["LastName"].ToString());
                    }

                    _employee.ItemsSource= employees;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Nie udało się połączyć z bazą. " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
        }

        /// <summary>
        /// Domyślnie ustawia datę rozpoczęcia rezerwacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatePicker1_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;
            datePicker.SelectedDate = DateTime.Today.AddDays(1);
        }

        /// <summary>
        /// Domyślnie ustawia datę zakończenia rezerwacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatePicker2_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;
            datePicker.SelectedDate = DateTime.Today.AddDays(2);
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku "Add". Dodaje nową rezerwację
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddReservation(object sender, RoutedEventArgs e)
        {
            checkIn = _checkIn.SelectedDate.Value.ToString("yyyy.MM.dd");

            checkOut = _checkOut.SelectedDate.Value.ToString("yyyy.MM.dd");

            bool canAdd = true;

            SqlCommand commandGuestID;
            SqlCommand commandEmployeeID;
            SqlCommand commandInsert;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    // Zapytanie SQL do pobrania id gościa
                    if (!String.IsNullOrWhiteSpace(guestName) && !String.IsNullOrWhiteSpace(guestLastName))
                    {
                        string queryGuestID = $"SELECT GuestID FROM Guests WHERE FirstName = '{guestName}' AND LastName = '{guestLastName}'";
                        commandGuestID = new SqlCommand(queryGuestID, connection);
                        SqlDataReader reader = commandGuestID.ExecuteReader();

                        while (reader.Read())
                        {
                            guestID = Convert.ToInt32(reader["GuestID"]);
                        }
                        reader.Close();
                    }
                    else
                        canAdd = false;


                    if (!String.IsNullOrWhiteSpace(employeeName) && !String.IsNullOrWhiteSpace(employeeLastName))
                    {
                        // Zapytanie SQL do pobrania id pracownika
                        string queryEmployeeID = $"SELECT EmployeeID FROM Employees WHERE FirstName = '{employeeName}' AND LastName = '{employeeLastName}'";
                        commandEmployeeID = new SqlCommand(queryEmployeeID, connection);
                        SqlDataReader reader = commandEmployeeID.ExecuteReader();

                        while (reader.Read())
                        {
                            employeeID = Convert.ToInt32(reader["EmployeeID"]);
                        }
                        reader.Close();
                    }
                    else
                        canAdd = false;

                    if (!String.IsNullOrWhiteSpace(phone) && canAdd == true && room != 0)
                    {
                       
                        // Zapytanie SQL do dodania rezerwacji
                        string queryAddReservation = $"INSERT INTO Reservations (RoomNumber, GuestID, CheckInDate, CheckOutDate, EmployeeID) VALUES ({room}, {guestID}, '{checkIn}', '{checkOut}', {employeeID})";
                        
                        commandInsert = new SqlCommand(queryAddReservation, connection);

                        //command.ExecuteNonQuery();
                        int rowsAffected = commandInsert.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("dodano rezerwacje");
                        }
                    }
                    else
                        MessageBox.Show("Enter all necessery data");

                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas dodawania rezerwacji: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                _guest.Text = string.Empty;
                _room.Text = string.Empty;
                _phone.Text = string.Empty;
                _mail.Text = string.Empty;
                _employee.Text = string.Empty;
                _checkIn.SelectedDate = DateTime.Today.AddDays(1);
                _checkOut.SelectedDate = DateTime.Today.AddDays(2);

                guestName = string.Empty;
                guestLastName = string.Empty;
                employeeName = string.Empty;
                employeeLastName = string.Empty;
                room = 0;
                phone = string.Empty;
                mail= string.Empty;
            }
        }

        /// <summary>
        /// Reaguje na opuszczenie text boxa _guest
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _guest_LostFocus(object sender, RoutedEventArgs e)
        {
            _guest.Foreground = Brushes.Red;
            if (String.IsNullOrWhiteSpace(_guest.Text))
            {
                _guest.Text = "mandatory field";
                _guest.Text = string.Empty;
                guest = string.Empty;
            }

            else
            {
                guest = _guest.Text;
                var temp = guest.Split(' ');
                if (temp.Length == 2)
                {
                    guestName = temp[0];
                    guestLastName = temp[1];
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = $"SELECT * FROM Guests WHERE FirstName = '{guestName}' AND LastName = '{guestLastName}'";

                        SqlCommand command1 = new SqlCommand(query, connection);
                        int rowsAffected1 = command1.ExecuteNonQuery();

                        if (rowsAffected1 <= 0)
                        {
                            MessageBoxResult result = MessageBox.Show("Do you want to add a new guest?", "Guest does not exist", MessageBoxButton.OK);

                            if (result == MessageBoxResult.OK)
                            {
                                NewGuest newGuest = new NewGuest();
                                newGuest.Closed += NewGuest_Closed;
                                newGuest.Show();
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        MessageBox.Show("Wystąpił błąd podczas dodawania rezerwacji: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void NewGuest_Closed(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT PhoneNumber, Email, FirstName, LastName FROM Guests WHERE FirstName = '{guestName}' AND LastName = '{guestLastName}'";

                    SqlCommand command1 = new SqlCommand(query, connection);
                    SqlDataReader reader = command1.ExecuteReader();

                    while(reader.Read())
                    {
                        _phone.Text = reader["PhoneNumber"].ToString();
                        phone = reader["PhoneNumber"].ToString();

                        _mail.Text = reader["Email"].ToString();
                        mail = reader["Email"].ToString();

                        _guest.Text = reader["FirstName"].ToString() + " " + reader["LastName"].ToString();
                        guestName = reader["FirstName"].ToString();
                        guestLastName = reader["LastName"].ToString();

                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas dodawania rezerwacji: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Reaguje na kliknięcie text boxa _guest
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _guest_GotFocus(object sender, RoutedEventArgs e)
        {
            _guest.Foreground = Brushes.Red;
            if (_guest.Text == "mandatory field")
                _guest.Text = string.Empty;

        }

        /// <summary>
        /// Reaguje na opuszczenie text boxa _room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _room_LostFocus(object sender, RoutedEventArgs e)
        {
            _room.Foreground = Brushes.Red;
            if (_room.Text == "")
            {
                _room.Text = string.Empty;
                _room.Text = "mandatory field";
                room = 0;
            }
                
            else
                room = int.Parse(_room.Text);
            
        }

        /// <summary>
        /// Reaguje na kliknięcie text boxa _room
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _room_GotFocus(object sender, RoutedEventArgs e)
        {
            _room.Foreground = Brushes.Red;
            if (_room.Text == "mandatory field")
                _room.Text = string.Empty;
            
        }

        /// <summary>
        /// Reaguje na opuszczenie text boxa _phone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _phone_LostFocus(object sender, RoutedEventArgs e)
        {
            _phone.Foreground = Brushes.Red;
            if (_phone.Text == "")
            {
                _phone.Text = string.Empty;
                _phone.Text = "mandatory field";
                phone = string.Empty;
            }
                
            else
                phone = _phone.Text;
            
        }

        /// <summary>
        /// Reaguje na kliknięcie text boxa _phone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _phone_GotFocus(object sender, RoutedEventArgs e)
        {
            _phone.Foreground = Brushes.Red;
            if (_phone.Text == "mandatory field")
                _phone.Text = string.Empty;
            
        }

        /// <summary>
        /// Reaguje na opuszczenie text boxa _mail
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _mail_LostFocus(object sender, RoutedEventArgs e)
        {
            _mail.Foreground = Brushes.Red;
            if (_mail.Text != "")
                mail = _employee.Text;
            else mail= string.Empty;
            
        }

        /// <summary>
        /// Reaguje na kliknięcie text boxa _mail
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _mail_GotFocus(object sender, RoutedEventArgs e)
        {
            _mail.Foreground = Brushes.Red;
            if (_mail.Text == "mandatory field")
                _mail.Text = string.Empty;
            
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku "..." przy polu guest. Otwiera okno dialogowe z listą gości i przypisuje do tekst boxa _guest wybrane imię i nazwisko
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GuestSelectingDialog(object sender, RoutedEventArgs e)
        {
            GuestsBrowse guestsBrowse = new GuestsBrowse();
            if (guestsBrowse.ShowDialog() == true)
            {
                _guest.Text = guestsBrowse.GuestName + " " + guestsBrowse.GuestLastName;
                _phone.Text = guestsBrowse.Phone;
                _mail.Text = guestsBrowse.Email;

                guestName = guestsBrowse.GuestName;
                guestLastName = guestsBrowse.GuestLastName;
                phone = guestsBrowse.Phone;
                mail = guestsBrowse.Email;
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku "..." przy polu room. Otwiera okno dialogowe z listą pokoi i przypisuje do tekst boxa _room numer wybranego pokoju.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void BrowseRooms(object sender, RoutedEventArgs e)
        {
            RoomsBrowse roomsBrowse = new RoomsBrowse();
            if (roomsBrowse.ShowDialog() == true)
            {
                _room.Text = roomsBrowse.Room;
                room = int.Parse(roomsBrowse.Room);
            }
        }

        /// <summary>
        /// Pozwala na wybranie pracownika, który przyjmuje rezerwację
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _employee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_employee.SelectedItem!= null)
            {
                var temp = _employee.SelectedItem.ToString().Split(' ');
                employeeName = temp[0];
                employeeLastName = temp[1];
            }
        }
    }
}
