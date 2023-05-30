using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
    /// Interaction logic for RoomsBrowse.xaml
    /// </summary>
    public partial class RoomsBrowse : Window
    {
        private string connectionString = "data source=localhost;initial catalog=hotel2;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework";

        /// <summary>
        /// Przechowuje numer wybranego pokoju potrzebny do dodania rezerwacji
        /// </summary>
        public string Room { get; private set; }

        /// <summary>
        /// Otwiera nowe okno wyświetlające wszystkie pokoje hotelowe
        /// </summary>
        public RoomsBrowse()
        {
            InitializeComponent();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = $"SELECT * FROM Rooms";

                    SqlCommand commandRoomNr = new SqlCommand(query, connection);
                    SqlDataReader reader = commandRoomNr.ExecuteReader();

                    DataTable dataTableDeafault = new DataTable();
                    dataTableDeafault.Load(reader);
                    dataGrid.ItemsSource = dataTableDeafault.DefaultView;
                    reader.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas wyszukiwania pokoju: " + ex.Message, "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
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
                Room = rowView["RoomNumber"].ToString();
                DialogResult = true;
            }
            else
                MessageBox.Show("Please select a room.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
