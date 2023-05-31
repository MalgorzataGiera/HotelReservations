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
using System.Windows.Interop;
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

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT LastName, FirstName, PESEL, DocumentType, DocumentNumber, City, PhoneNumber, Email FROM Guests";
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

        /// <summary>
        /// Obsługuje kliknięcie przycisku lupy (wyszukiwania)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        query = $"SELECT LastName, FirstName, PESEL, DocumentType, DocumentNumber, City, PhoneNumber, Email FROM Guests";

                    else
                    {
                        query = $"SELECT LastName, FirstName, PESEL, DocumentType, DocumentNumber, City, PhoneNumber, Email FROM Guests WHERE LastName LIKE '{userQuery[0]}%' OR FirstName LIKE '{userQuery[0]}%'";
                    }

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    DataTable dataTable = new DataTable();
                    dataTable.Load(reader);
                    dataGrid.ItemsSource = dataTable.DefaultView;

                    if (dataGrid.Items.Count < 1)
                    {
                        MessageBoxResult result = MessageBox.Show("Do you want to add new guest?", "No matching results found", MessageBoxButton.OK);

                        if (result == MessageBoxResult.OK)
                        {
                            NewGuest newGuest = new NewGuest();
                            newGuest.Show();
                        }
                    }

                    reader.Close();
                }
                catch (SqlException ex)
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
                DataRowView rowView = dataGrid.SelectedItem as DataRowView;
                GuestName = rowView["FirstName"].ToString();
                GuestLastName = rowView["LastName"].ToString();
                Phone = rowView["PhoneNumber"].ToString();
                Email = rowView["Email"].ToString();

                DialogResult = true;
            }
            else
                MessageBox.Show("Please select guest.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
