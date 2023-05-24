using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp_Hotel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
            

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Pobierz aktualną datę i godzinę
            DateTime currentDateTime = DateTime.Now;

            // Aktualizuj zawartość etykiety
            date.Content = currentDateTime.ToString();
        }

        private void NewReserv(object sender, RoutedEventArgs e)
        {
           
            NewReservation reservationsWindow = new NewReservation();
            reservationsWindow.Show();
        }

        private void ShowReservs(object sender, RoutedEventArgs e)
        {
            ShowReservations reservationsWindow = new ShowReservations();
            reservationsWindow.Show();
        }
    }
}
