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
        private NewReservation reservationsWindow = new NewReservation();
        private ShowReservations showReservationsWindow = new ShowReservations();
        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

            Closing += MainWindow_Closing;

        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (reservationsWindow != null && reservationsWindow.IsVisible)
                reservationsWindow.Close();

            if (showReservationsWindow != null && showReservationsWindow.IsVisible)
                showReservationsWindow.Close();

            Application.Current.Shutdown();
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
            if (reservationsWindow != null && !reservationsWindow.IsVisible)
                reservationsWindow = new NewReservation();

            reservationsWindow.Show();
        }
        
        private void ShowReservs(object sender, RoutedEventArgs e)
        {
            if (showReservationsWindow != null && !showReservationsWindow.IsVisible)
                showReservationsWindow = new ShowReservations();

            showReservationsWindow.Show();
        }        
    }
}
