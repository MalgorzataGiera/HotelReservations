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
        private string name;
        private string lastName;

        private int room;
        private string phone;

        private string mail;
        private string employee;
        private string employeeName = "";
        private string employeeLastName = "";
        public NewReservation()
        {
            InitializeComponent();
            _checkIn.Loaded += DatePicker1_Loaded;
            _checkOut.Loaded += DatePicker2_Loaded;
        }

        private void DatePicker1_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;
            datePicker.SelectedDate = DateTime.Today.AddDays(1);
        }
        private void DatePicker2_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;
            datePicker.SelectedDate = DateTime.Today.AddDays(2);
        }
        private void AddReservation(object sender, RoutedEventArgs e)
        {
            checkIn = _checkIn.SelectedDate.Value.ToString("yyyy.MM.dd");

            checkOut = _checkOut.SelectedDate.Value.ToString("yyyy.MM.dd");

            bool canAdd = true;

            SqlCommand command;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    // Zapytanie SQL do pobrania id gościa
                    if (!String.IsNullOrWhiteSpace(name) && !String.IsNullOrWhiteSpace(lastName))
                    {
                        string queryGuestID = $"SELECT GuestID FROM Guests WHERE FirstName = '{name}' AND LastName = '{lastName}'";
                        command = new SqlCommand(queryGuestID, connection);
                        SqlDataReader reader = command.ExecuteReader();

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
                        command = new SqlCommand(queryEmployeeID, connection);
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            employeeID = Convert.ToInt32(reader["EmployeeID"]);
                        }
                        reader.Close();
                    }
                    else
                        canAdd = false;

                    if (!String.IsNullOrWhiteSpace(phone) && canAdd == true)
                    {
                        // Zapytanie SQL do dodania rezerwacji
                        string queryAddReservation = $"INSERT INTO Reservations (RoomNumber, GuestID, CheckInDate, CheckOutDate, EmployeeID) VALUES ({room}, {guestID}, '{checkIn}', '{checkOut}', {employeeID})";
                        command = new SqlCommand(queryAddReservation, connection);

                        command.ExecuteNonQuery();
                        int rowsAffected = command.ExecuteNonQuery();

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
            }
        }

        private void _name_LostFocus(object sender, RoutedEventArgs e)
        {
            _name.Foreground = Brushes.Red;
            if (_name.Text == "")
                _name.Text = "mandatory field";
            else
                name= _name.Text;
            
        }

        private void _name_GotFocus(object sender, RoutedEventArgs e)
        {
            _name.Foreground = Brushes.Red;
            if (_name.Text == "mandatory field")
                _name.Text = string.Empty;
            
        }

        private void _lastName_LostFocus(object sender, RoutedEventArgs e)
        {
            _lastName.Foreground = Brushes.Red;
            if (_lastName.Text == "")
                _lastName.Text = "mandatory field";
            else
                lastName = _lastName.Text;
            
        }

        private void _lastName_GotFocus(object sender, RoutedEventArgs e)
        {
            _lastName.Foreground = Brushes.Red;
            if (_lastName.Text == "mandatory field")
                _lastName.Text = string.Empty;
        }

        private void _room_LostFocus(object sender, RoutedEventArgs e)
        {
            _room.Foreground = Brushes.Red;
            if (_room.Text == "")
                _room.Text = "mandatory field";
            else
                room = int.Parse(_room.Text);
            
        }

        private void _room_GotFocus(object sender, RoutedEventArgs e)
        {
            _room.Foreground = Brushes.Red;
            if (_room.Text == "mandatory field")
                _room.Text = string.Empty;
            
        }

        private void _phone_LostFocus(object sender, RoutedEventArgs e)
        {
            _phone.Foreground = Brushes.Red;
            if (_phone.Text == "")
                _phone.Text = "mandatory field";
            else
                phone = _phone.Text;
            
        }

        private void _phone_GotFocus(object sender, RoutedEventArgs e)
        {
            _phone.Foreground = Brushes.Red;
            if (_phone.Text == "mandatory field")
                _phone.Text = string.Empty;
            
        }

        private void _mail_LostFocus(object sender, RoutedEventArgs e)
        {
            _mail.Foreground = Brushes.Red;
            if (_mail.Text != "")
                mail = _employee.Text;
            
        }

        private void _mail_GotFocus(object sender, RoutedEventArgs e)
        {
            _mail.Foreground = Brushes.Red;
            if (_mail.Text == "mandatory field")
                _mail.Text = string.Empty;
            
        }

        private void _employee_LostFocus(object sender, RoutedEventArgs e)
        {
            _employee.Foreground = Brushes.Red;
            if (_employee.Text == "")
                _employee.Text = "mandatory field";
            else
            {
                employee = _employee.Text;
                var temp = employee.Split(' ');
                    if(temp.Length == 2)
                {
                    employeeName = temp[0];
                    employeeLastName = temp[1];
                }
            }
            
        }

        private void _employee_GotFocus(object sender, RoutedEventArgs e)
        {
            _employee.Foreground = Brushes.Red;
            if (_employee.Text == "mandatory field")
                _employee.Text = string.Empty;
            
        }
    }
}
