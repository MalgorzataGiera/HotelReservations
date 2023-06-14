using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
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
        private int employeeID;
        private int guestID;
        private DateTime checkIn;
        private DateTime checkOut;

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
            try
            {
                using (var context = new hotel2Entities())
                {
                    var employees = context.Employees
                        .Select(e => e.FirstName + " " + e.LastName)
                        .ToList();

                    _employee.ItemsSource = employees;
                }

            }
            catch (DbException ex)
            {
                MessageBox.Show("Nie udało się połączyć z bazą. " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
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
            checkIn = DateTime.Today.AddDays(1);
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
            checkIn = _checkIn.SelectedDate.Value;
            checkOut = _checkOut.SelectedDate.Value;

            bool canAdd = true;
            try
            {
                using (var context = new hotel2Entities())
                {
                    // Zapytanie SQL do pobrania id gościa
                    if (!String.IsNullOrWhiteSpace(guestName) && !String.IsNullOrWhiteSpace(guestLastName))
                    {

                        var result = context.Guests
                        .Where(g => g.FirstName == guestName && g.LastName == guestLastName)
                        .Select(g => g.GuestID)
                        .FirstOrDefault();
                        guestID = result;


                    }
                    else
                        canAdd = false;

                    if (!String.IsNullOrWhiteSpace(employeeName) && !String.IsNullOrWhiteSpace(employeeLastName))
                    {
                        // Zapytanie SQL do pobrania id pracownika
                        var result = context.Employees
                            .Where(emp => emp.FirstName == employeeName && emp.LastName == employeeLastName)
                            .Select(emp => emp.EmployeeID)
                            .FirstOrDefault();
                        employeeID= result;

                    }
                    else
                        canAdd = false;


                    if (!String.IsNullOrWhiteSpace(phone) && canAdd == true && room != 0)
                    {

                        // Zapytanie SQL do dodania rezerwacji
                        Reservations newReservation = new Reservations
                        {
                            RoomNumber = (byte)room,
                            GuestID = guestID,
                            CheckInDate = checkIn,
                            CheckOutDate = checkOut,
                            EmployeeID = (byte)employeeID,
                            Status = "niepotwierdzona",
                            IfSettled = "nie"
                        };

                        context.Reservations.Add(newReservation);
                        context.SaveChanges();
                        MessageBox.Show("dodano rezerwacje");
                    }
                    else
                        MessageBox.Show("Enter all necessery data");
                }
            }


            catch (DbEntityValidationException ex)
            {
                // Obsługa błędów walidacji
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        MessageBox.Show($"Błąd walidacji: {validationError.PropertyName} - {validationError.ErrorMessage}");
                    }
                }
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
                mail = string.Empty;
            
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

                using (var context = new hotel2Entities())
                {
                    try
                    {
                        var gResult = context.Guests
                            .Where(g => g.FirstName== guestName && g.LastName == guestLastName)
                            .FirstOrDefault();

                        if( gResult == null)
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
                    catch (DbException ex)
                    {
                        MessageBox.Show("Wystąpił błąd podczas dodawania rezerwacji: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void NewGuest_Closed(object sender, EventArgs e)
        {
            using (var context = new hotel2Entities())
            {
                try
                {
                    var guestData = context.Guests
                        .Where(g => g.FirstName == guestName && g.LastName == guestLastName)
                        .Select(g => new
                        {
                            phone = g.PhoneNumber,
                            _phoneNumber = g.PhoneNumber,
                            mail = g.Email,
                            _mail= g.Email,
                            guestName = g.FirstName,                            
                            guestLastName = g.LastName,
                            _guest = g.FirstName + " " + g.LastName                            
                        });
                }
                catch (DbException ex)
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
            checkIn = _checkIn.SelectedDate.Value;
            checkOut = _checkOut.SelectedDate.Value;
            RoomsBrowse roomsBrowse = new RoomsBrowse(checkIn, checkOut);
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
