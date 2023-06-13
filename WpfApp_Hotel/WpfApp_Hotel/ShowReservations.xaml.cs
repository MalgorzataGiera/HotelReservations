using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WpfApp_Hotel
{
    /// <summary>
    /// Interaction logic for ShowReservations.xaml
    /// </summary>
    public partial class ShowReservations : Window
    {
        private List<string> reservationsData = new List<string>();
        private string[] userQuery;
        private DateTime dateStart;
        private DateTime dateEnd;
        private int reservationID;

        private string statusCheckBox;
        private string settledCheckBox;

        private DateTime lastDayOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1);


        /// <summary>
        /// Wyświetla wszystkie rezerwacje
        /// </summary>
        public ShowReservations()
        {
            InitializeComponent();
            datePicker1.Loaded += DatePicker1_Loaded;
            datePicker2.Loaded += DatePicker2_Loaded;
            dateStart = DateTime.Today;
            dateEnd = lastDayOfMonth;

            using (var context = new hotel2Entities())
            {
                try
                {
                    var results = context.Guests
                    .Join(context.Reservations,
                        g => g.GuestID,
                        r => r.GuestID,
                        (g, r) => new
                        {
                            g.LastName,
                            g.FirstName,
                            r.CheckInDate,
                            r.CheckOutDate,
                            r.ReservationID
                        })
                    .Where(r => (r.CheckInDate >= dateStart && r.CheckInDate <= dateEnd) ||
                                (r.CheckOutDate >= dateStart && r.CheckOutDate <= dateEnd))
                    .ToList();

                    dataGrid.ItemsSource = results;
                    dataGrid.MouseDoubleClick += dataGrid_MouseDoubleClick;
                }
                catch (DbException ex)
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
        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var selectedRow = dataGrid.SelectedItem as dynamic;
            if (selectedRow != null)
            {
                reservationID = selectedRow.ReservationID;
                StringBuilder popupMessageBuilder = new StringBuilder();

                using (var context = new hotel2Entities())
                {
                    try
                    {

                        var result = context.Reservations
                            .Join(context.Guests, r => r.GuestID, g => g.GuestID,
                            (r, g) => new { Reservation = r, Guest = g })
                            .Join(context.Employees, rg => rg.Reservation.EmployeeID, emp => emp.EmployeeID,
                            (rg, emp) => new { rg.Reservation, rg.Guest, Employee = emp })
                            .Where(rg => rg.Reservation.ReservationID == reservationID)
                            .Select(rg => new
                            {
                                Reservation = rg.Reservation,
                                GuestFirstName = rg.Guest.FirstName,
                                GuestLastName = rg.Guest.LastName,
                                EmployeeFirstName = rg.Employee.FirstName,
                                EmployeeLastName = rg.Employee.LastName
                            })
                        .FirstOrDefault();

                        popupMessageBuilder.AppendLine($"Guest: {result.GuestFirstName} {result.GuestLastName}");
                        popupMessageBuilder.AppendLine($"Reservation from: {result.Reservation.CheckInDate.ToString("yy:MM:dd")} to: {result.Reservation.CheckOutDate.ToString("yy:MM:dd")}");
                        popupMessageBuilder.AppendLine($"Room nr: {result.Reservation.RoomNumber}");
                        popupMessageBuilder.AppendLine($"Reservations accepted by: {result.EmployeeFirstName} {result.EmployeeLastName}");

                        if (result.Reservation.Status == "potwierdzona")
                            popupCheckBoxStatus.IsChecked = true;
                        else
                            popupCheckBoxStatus.IsChecked = false;

                        if (result.Reservation.IfSettled == "tak")
                            popupCheckBoxSettled.IsChecked = true;
                        else
                            popupCheckBoxSettled.IsChecked = false;

                        popupTextBlock.Text = popupMessageBuilder.ToString().TrimEnd();
                        reservationPopup.IsOpen = true;
                    }
                    catch (DbException ex)
                    {
                        MessageBox.Show("Wystąpił błąd podczas pobierania rezerwacji: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

        }

        /// <summary>
        /// Reaguje na kliknięcie przycisku "Close" dla popup
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {
            if (popupCheckBoxStatus.IsChecked == true)
                statusCheckBox = "potwierdzona";
            else
                statusCheckBox = "niepotwierdzona";

            if (popupCheckBoxSettled.IsChecked == true)
                settledCheckBox = "tak";
            else
                settledCheckBox = "nie";

            using (var context = new hotel2Entities())
            {
                try
                {
                    var reservationToUpdate = context.Reservations
                        .Where(r => r.ReservationID == reservationID)
                        .FirstOrDefault();
                        if (reservationToUpdate.Status != statusCheckBox)
                            reservationToUpdate.Status = statusCheckBox;
                        if (reservationToUpdate.IfSettled != settledCheckBox)
                            reservationToUpdate.IfSettled = settledCheckBox;

                        context.SaveChanges();
                    
                    
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
                    // Dodatkowe działania po obsłudze błędów walidacji
                }
                catch (DbException ex)
                {
                    MessageBox.Show("Wystąpił błąd: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            reservationPopup.IsOpen = false;
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
            {
                textBox.Text = string.Empty;
            }
                
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
                textBox.Text = "Search for guest name...";
            }
                
            else
            {
                userQuery = textBox.Text.Split(' ');
                if (userQuery.Length > 2)
                {
                    MessageBox.Show("no matching results found");
                    textBox.Text = "Search for guest name...";
                }
            }

        }
        /// <summary>
        /// Metoda pomocnicza dla wyszukiwania oraz wybierania przez użytkownika daty
        /// </summary>

        private void UserQueries()
        {
            try
            {
                using (var context = new hotel2Entities())
                {
                    if (userQuery == null || userQuery.Length == 0)
                    {
                        var results = context.Guests
                                    .Join(context.Reservations, g => g.GuestID, r => r.GuestID,
                                        (g, r) => new
                                        {
                                            g.LastName,
                                            g.FirstName,
                                            r.CheckInDate,
                                            r.CheckOutDate,
                                            r.ReservationID
                                        })
                                     .Where(r => (r.CheckInDate >= dateStart && r.CheckInDate <= dateEnd) ||
                                        (r.CheckOutDate >= dateStart && r.CheckOutDate <= dateEnd))
                                     .ToList();

                        dataGrid.ItemsSource = results;
                    }
                    else
                    {
                        string query1 = userQuery[0];
                        var results = context.Guests
                                    .Where(g => (g.LastName.StartsWith(query1) || g.FirstName.StartsWith(query1))
                                                && g.Reservations.Any(r => (r.CheckInDate >= dateStart && r.CheckInDate <= dateEnd) || (r.CheckOutDate >= dateStart && r.CheckOutDate <= dateEnd)))
                                    .SelectMany(g => g.Reservations
                                        .Where(r => r.CheckInDate >= dateStart)
                                        .Select(r => new
                                        {
                                            g.LastName,
                                            g.FirstName,
                                            r.CheckInDate,
                                            r.CheckOutDate,
                                            r.ReservationID
                                        }))
                                    .ToList();
                        if (results.Any())
                            dataGrid.ItemsSource = results;
                        else
                        {
                            dataGrid.ItemsSource = null;
                            MessageBox.Show("no matching results found");
                        }
                            
                    }
                }
            }
            catch (DbException ex)
            {
                MessageBox.Show("Wystąpił błąd podczas pobierania rezerwacji: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } 

        /// <summary>
        /// Reaguje na kliknięcie przycisku lupy (wyszukiwania)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            dateStart = datePicker1.SelectedDate.Value;
            dateEnd = datePicker2.SelectedDate.Value;

            UserQueries();
        }

        /// <summary>
        /// Domyślnie ustawia datę na bieżącą dla pierwszego datePickera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatePicker1_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;
            datePicker.SelectedDate = DateTime.Today;
        }

        /// <summary>
        /// Domyślnie ustawia datę na ostatni dzień bieżącego miesiąca dla drugiego datePickera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatePicker2_Loaded(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;
            datePicker.SelectedDate = lastDayOfMonth;
        }

        /// <summary>
        /// Reaguje na wybranie daty początkowej
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatePicker_StartDate_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;
            if(datePicker != null)
            {
                dateStart = datePicker.SelectedDate.Value;
                UserQueries();
            }
                    
        }

        /// <summary>
        /// Reaguje na wybranie daty końcowej
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DatePicker_EndDate_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;
            if (datePicker != null)
            {
                dateEnd = datePicker.SelectedDate.Value;
                UserQueries();
            }
        }

    }
}
