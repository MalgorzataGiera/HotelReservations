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
using System.Data.Common;

namespace WpfApp_Hotel
{
    /// <summary>
    /// Interaction logic for RoomsBrowse.xaml
    /// </summary>
    public partial class RoomsBrowse : Window
    {
        /// <summary>
        /// Przechowuje numer wybranego pokoju potrzebny do dodania rezerwacji
        /// </summary>
        public string Room { get; private set; }

        /// <summary>
        /// Otwiera nowe okno wyświetlające wszystkie pokoje hotelowe
        /// </summary>
        public RoomsBrowse(DateTime checkIn, DateTime checkOut)
        {
            InitializeComponent();

            using (var context = new hotel2Entities())
            {
                var reservedRooms = context.Reservations
                    .Where(r => (r.CheckInDate >= checkIn && r.CheckInDate <= checkOut) ||
                                        (r.CheckOutDate >= checkIn && r.CheckOutDate <= checkOut))
                    .Select(r => r.RoomNumber)
                    .ToList();

                var availableRooms = context.Rooms
                    .Where(room => !reservedRooms.Contains(room.RoomNumber))
                    .ToList();

                dataGrid.ItemsSource = availableRooms;
            }
        }

        /// <summary>
        /// Obsługuje zdarzenie kliknięcia przycisku "Confirm". Przekazuje imię i nazwisko do okna dodawania rezerwacji
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = dataGrid.SelectedItem as dynamic;
            if (selectedRow != null)
            {
                Room = selectedRow.RoomNumber.ToString();
                DialogResult = true;
            }
            else
                MessageBox.Show("Please select a room.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
