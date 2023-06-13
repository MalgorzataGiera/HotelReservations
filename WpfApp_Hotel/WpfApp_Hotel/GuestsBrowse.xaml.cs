using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WpfApp_Hotel
{
    /// <summary>
    /// Interaction logic for GuestsBrowse.xaml
    /// </summary>
    public partial class GuestsBrowse : Window
    {
        private string[] userQuery;

        /// <summary>
        /// Przechuowuje imię wybranego gościa potrzebne do dodania rezerwacji
        /// </summary>
        public string GuestName { get; private set; }

        /// <summary>
        /// Przechuowuje nazwisko wybranego gościa potrzebne do dodania rezerwacji
        /// </summary>
        public string GuestLastName { get; private set; }

        /// <summary>
        /// Przechowuje numer telefonu gościa potrzebny do dodanaia rezerwacji
        /// </summary>
        public string Phone { get; private set; }

        /// <summary>
        /// Przechowuje e-mail gościa potrzebny do dodanaia rezerwacji
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Tworzy nowe okno pozwalające przeglądać listę gości oraz wybierać gościa z listy
        /// </summary>
        public GuestsBrowse()
        {
            InitializeComponent();

            using (var context = new hotel2Entities())
            {
                try
                {
                    var results = context.Guests
                        .Select(g => new
                        {
                            g.LastName, g.FirstName,
                            g.PESEL, 
                            g.DocumentType,
                            g.DocumentNumber,
                            g.City,
                            g.PhoneNumber, g.Email
                        })
                        .ToList();
                    dataGrid.ItemsSource= results;
                }
                catch (DbException ex)
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

        /// <summary>
        /// Obsługuje kliknięcie przycisku lupy (wyszukiwania)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            
            if (userQuery != null && userQuery.Length > 0 && !String.IsNullOrWhiteSpace(userQuery[0]))
            {
                try
                {
                    string query1 = userQuery[0];
                    using (var context = new hotel2Entities())
                    {
                        var results = context.Guests
                            .Where(g => g.LastName.StartsWith(query1)
                            || g.FirstName.StartsWith(query1))
                            .Select(g => new
                            {
                                g.LastName,
                                g.FirstName,
                                g.PESEL,
                                g.DocumentType,
                                g.DocumentNumber,
                                g.City,
                                g.PhoneNumber,
                                g.Email
                            })
                            .ToList();
                        dataGrid.ItemsSource = results;
                        if (results == null)
                        {
                            MessageBoxResult result = MessageBox.Show("Do you want to add new guest?", "No matching results found", MessageBoxButton.OK);

                            if (result == MessageBoxResult.OK)
                            {
                                NewGuest newGuest = new NewGuest();
                                newGuest.Show();
                            }
                        }
                    }

                }
                catch (DbException ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas wyszukiwania gościa: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku "Confirm". Przekazuje imię i nazwisko do okna dodawania rezerwacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                var selectedRow = dataGrid.SelectedItem as dynamic;
                GuestName = selectedRow.FirstName.ToString();
                GuestLastName = selectedRow.LastName.ToString();
                Phone = selectedRow.PhoneNumber.ToString();
                Email = selectedRow.Email.ToString();

                DialogResult = true;
            }
            else
                MessageBox.Show("Please select guest.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
