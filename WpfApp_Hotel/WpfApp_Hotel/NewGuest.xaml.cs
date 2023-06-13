using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
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
using static System.Net.Mime.MediaTypeNames;

namespace WpfApp_Hotel
{
    /// <summary>
    /// Interaction logic for NewGuest.xaml
    /// </summary>
    public partial class NewGuest : Window
    {
        private bool canConvert = false;

        private string name;
        private string lastName;
        private string docType;
        private string docNr;
        private string pesel;
        private string country;
        private string postCode;
        private string city;
        private string street;
        private string house;
        private string apartment;
        private string phone;
        private string mail;

        public NewGuest()
        {
            InitializeComponent();

            List<string> docTypes = new List<string>();
            docTypes.Add("dowód osobisty");
            docTypes.Add("paszport");
            _docType.ItemsSource = docTypes;


        }

        /// <summary>
        /// Reaguje na kliknięcie textboxa _name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _name_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            name = string.Empty;
        }

        /// <summary>
        /// Reaguje na opuszczenie textboxa _name
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _name_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!String.IsNullOrEmpty(textBox.Text))
            {
                var temp = textBox.Text.ToCharArray();
                foreach (char c in temp)
                {
                    if (Char.IsLetter(c))
                    {
                        canConvert = true;
                        name += c;
                    }
                    else
                    {
                        canConvert = false;
                        break;
                    }

                }

                if (canConvert)
                {
                    name = char.ToUpper(name[0]) + name.Substring(1).ToLower();
                    textBox.Foreground = Brushes.Black;
                    textBox.Text = name;
                    
                }
                else
                {
                    name = string.Empty;
                    textBox.Foreground = Brushes.Red;
                    MessageBox.Show("Please enter the data in the correct format, e.g. John", "Wrong format", MessageBoxButton.OK);
                }
            }

            else
            {
                MessageBox.Show("Enter all necessary data");
            }

        }

        /// <summary>
        /// Reaguje na kliknięcie textboxa _lastName
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _lastName_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            lastName = "";
        }

        /// <summary>
        /// Reaguje na opuszczenie textboxa _lastName
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _lastName_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!String.IsNullOrEmpty(textBox.Text))
            {
                var temp = textBox.Text.ToCharArray();
                foreach (char c in temp)
                {
                    if (Char.IsLetter(c))
                    {
                        canConvert = true;
                        lastName += c;
                    }
                    else
                    {
                        canConvert = false;
                        break;
                    }
                }

                if (canConvert)
                {
                    lastName = char.ToUpper(lastName[0]) + lastName.Substring(1).ToLower();
                    textBox.Foreground = Brushes.Black;
                    textBox.Text = lastName;
                }
                else
                {
                    lastName = string.Empty;
                    textBox.Foreground = Brushes.Red;
                    MessageBox.Show("Please enter the data in the correct format, e.g. Smith", "Wrong format", MessageBoxButton.OK);
                }
            }

        }

        /// <summary>
        /// Pozwala na wybranie rodzaju dokumentu, którym identyfikuje się gość
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _docType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_docType.SelectedItem != null)
                docType = _docType.SelectedItem.ToString();
        }

        /// <summary>
        /// Reaguje na kliknięcie textboxa _docNr
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _docNr_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            docNr = "";
        }

        /// <summary>
        /// Reaguje na opuszczenie textboxa _docNr
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _docNr_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!String.IsNullOrEmpty(textBox.Text))
            {
                var temp = textBox.Text.ToCharArray();
                foreach (char c in temp)
                {
                    if (Char.IsLetter(c))
                    {
                        canConvert = true;
                        docNr += char.ToUpper(c);
                    }
                    else if (Char.IsNumber(c))
                    {
                        canConvert = true;
                        docNr += c;
                    }
                    else
                    {
                        canConvert = false;
                        break;
                    }
                }

                if (canConvert)
                {
                    textBox.Foreground = Brushes.Black;
                    textBox.Text = docNr;
                }
                else
                {
                    docNr = string.Empty;
                    textBox.Foreground = Brushes.Red;
                    MessageBox.Show("Please enter the data in the correct format, e.g. ABC123456", "Wrong format", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Reaguje na kliknięcie textboxa _pesel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pesel_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            pesel = "";
        }

        /// <summary>
        /// Reaguje na opuszczenie textboxa _pesel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _pesel_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!String.IsNullOrEmpty(textBox.Text))
            {
                var temp = textBox.Text.ToCharArray();
                foreach (char c in temp)
                {
                    if (Char.IsNumber(c))
                    {
                        canConvert = true;
                        pesel += c;
                    }
                    else
                    {
                        canConvert = false;
                        break;
                    }
                }
                if (canConvert && temp.Length == 11)
                {
                    textBox.Foreground = Brushes.Black;
                    textBox.Text = pesel;
                }
                else
                {
                    pesel = string.Empty;
                    textBox.Foreground = Brushes.Red;
                    MessageBox.Show("Only numbers allowed. Please enter the data in the correct format, e.g. 12345678909", "Wrong format", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Reaguje na kliknięcie textboxa _country
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _country_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            country = "";
        }

        /// <summary>
        /// Reaguje na opuszczenie textboxa _country
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _country_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!String.IsNullOrEmpty(textBox.Text))
            {
                var temp = textBox.Text.Trim().ToCharArray();
                foreach (char c in temp)
                {
                    if (Char.IsLetter(c) || c == ' ')
                    {
                        canConvert = true;
                        country += c;
                    }
                    else
                    {
                        canConvert = false;
                        break;
                    }
                }

                if (canConvert)
                {
                    country = char.ToUpper(country[0]) + country.Substring(1).ToLower();
                    textBox.Foreground = Brushes.Black;
                    textBox.Text = country;
                }
                else
                {
                    country = string.Empty;
                    textBox.Foreground = Brushes.Red;
                    MessageBox.Show("Please enter the data in the correct format, e.g. Poland", "Wrong format", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Reaguje na kliknięcie textobxa _postCode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _postCode_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            postCode = "";
        }

        /// <summary>
        /// Reaguje na opuszczenie textobxa _postCode
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _postCode_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!String.IsNullOrEmpty(textBox.Text))
            {
                var temp = textBox.Text.Trim().ToCharArray();
                foreach (char c in temp)
                {
                    if (Char.IsNumber(c) || c == '-')
                    {
                        canConvert = true;
                        postCode += c;
                    }
                    else
                    {
                        canConvert = false;
                        break;
                    }
                }
                if (canConvert) //dlugosc 5 znakow??
                {
                    textBox.Foreground = Brushes.Black;
                    textBox.Text = postCode;
                }
                else
                {
                    postCode = string.Empty;
                    textBox.Foreground = Brushes.Red;
                    MessageBox.Show("Please enter the data in the correct format, e.g. 11-222", "Wrong format", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Reaguje na kliknięcie textobxa _city
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _city_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            city = "";
        }
        /// <summary>
        /// Reaguje na opuszczenie textobxa _city
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _city_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!String.IsNullOrEmpty(textBox.Text))
            {
                var temp = textBox.Text.Trim().ToCharArray();
                foreach (char c in temp)
                {
                    if (Char.IsLetter(c) || c == ' ')
                    {
                        canConvert = true;
                        city += c;
                    }
                    else
                    {
                        canConvert = false;
                        break;
                    }
                }

                if (canConvert)
                {
                    city = char.ToUpper(city[0]) + city.Substring(1).ToLower();
                    textBox.Foreground = Brushes.Black;
                    textBox.Text = city;
                }
                else
                {
                    city = string.Empty;
                    textBox.Foreground = Brushes.Red;
                    MessageBox.Show("Please enter the data in the correct format, e.g. Sample City", "Wrong format", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Reaguje na kliknięcie textobxa _street
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _street_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            street = "";
        }

        /// <summary>
        /// Reaguje na opuszczenie textobxa _street
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _street_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!String.IsNullOrEmpty(textBox.Text))
            {
                var temp = textBox.Text.Trim().ToCharArray();
                foreach (char c in temp)
                {
                    if (Char.IsLetter(c) || c == ' ' || c == '.' || c == '-')
                    {
                        canConvert = true;
                        street += c;
                    }
                    else
                    {
                        canConvert = false;
                        break;
                    }
                }

                if (canConvert)
                {
                    street = char.ToUpper(street[0]) + street.Substring(1).ToLower();
                    textBox.Foreground = Brushes.Black;
                    textBox.Text = street;
                }
                else
                {
                    street = string.Empty;
                    textBox.Foreground = Brushes.Red;
                    MessageBox.Show("Please enter the data in the correct format, e.g. Sample street", "Wrong format", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Reaguje na kliknięcie textobxa _house
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _house_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            house = "";
        }

        /// <summary>
        /// Reaguje na opuszczenie textobxa _house
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _house_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!String.IsNullOrEmpty(textBox.Text))
            {
                var temp = textBox.Text.Trim().ToCharArray();
                foreach (char c in temp)
                {
                    if (Char.IsLetter(c) || Char.IsNumber(c))
                    {
                        canConvert = true;
                        house += c;
                    }
                    else
                    {
                        canConvert = false;
                        break;
                    }
                }

                if (canConvert)
                {
                    textBox.Foreground = Brushes.Black;
                    textBox.Text = house;
                }
                else
                {
                    house = string.Empty;
                    textBox.Foreground = Brushes.Red;
                    MessageBox.Show("Please enter the data in the correct format, e.g. 12a", "Wrong format", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Reaguje na kliknięcie textobxa _apartment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _apartment_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            apartment = "";
        }

        /// <summary>
        /// Reaguje na opuszczenie textobxa _apartment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _apartment_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!String.IsNullOrEmpty(textBox.Text))
            {
                var temp = textBox.Text.Trim().ToCharArray();
                foreach (char c in temp)
                {
                    if (Char.IsNumber(c))
                    {
                        canConvert = true;
                        apartment += c;
                    }
                    else
                    {
                        canConvert = false;
                        break;
                    }
                }

                if (canConvert)
                {
                    textBox.Foreground = Brushes.Black;
                    textBox.Text = apartment;
                }
                else
                {
                    apartment = string.Empty;
                    textBox.Foreground = Brushes.Red;
                    MessageBox.Show("Only numbers allowed. Please enter the data in the correct format, e.g. 123", "Wrong format", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Reaguje na kliknięcie textobxa _phone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _phone_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            phone = "";
        }

        /// <summary>
        /// Reaguje na opuszczenie textobxa _phone
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _phone_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!String.IsNullOrEmpty(textBox.Text))
            {
                var temp = textBox.Text.Trim().ToCharArray();
                if (temp[0] == '+')
                    phone += temp[0].ToString();
                if (Char.IsNumber(temp[1]))
                    phone += temp[1].ToString();
                if (Char.IsNumber(temp[2]))
                    phone += temp[2].ToString();
                if (temp[3] == ' ')
                    phone += temp[3].ToString();
                for (int i = 4; i < temp.Length; i++)
                {
                    if (Char.IsDigit(temp[i]))
                    {
                        canConvert = true;
                        phone += temp[i];
                    }
                    else
                    {
                        canConvert = false;
                        break;
                    }
                }
                if (canConvert && phone.Length == 13)
                {
                    textBox.Foreground = Brushes.Black;
                    textBox.Text = phone;
                }
                else
                {
                    phone = string.Empty;
                    textBox.Foreground = Brushes.Red;
                    MessageBox.Show("Please enter the data in the correct format, e.g. +11 222333444", "Wrong format", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Reaguje na kliknięcie textobxa _mail
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _mail_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            mail = "";
        }

        /// <summary>
        /// Reaguje na opuszczenie textobxa _mail
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _mail_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!String.IsNullOrEmpty(textBox.Text))
            {
                var temp = textBox.Text.Trim().ToCharArray();
                foreach (char c in temp)
                {
                    if (Char.IsLetter(c) || Char.IsNumber(c) || c == '.' || c == '_' || c == '@')
                    {
                        canConvert = true;
                        mail += Char.ToLower(c);
                    }
                    else
                    {
                        canConvert = false;
                        break;
                    }
                }
                if (canConvert)
                {
                    textBox.Foreground = Brushes.Black;
                    textBox.Text = mail;
                }
                else
                {
                    mail = string.Empty;
                    textBox.Foreground = Brushes.Red;
                    MessageBox.Show("Please enter the data in the correct format, e.g. sample_mail@gmail.com", "Wrong format", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku "Add". Dodaje nowego gościa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddNewGuest(object sender, RoutedEventArgs e)
        {
            using (var context = new hotel2Entities())
            {
                try
                {
                    if (String.IsNullOrEmpty(apartment)) apartment = null;
                    if (String.IsNullOrEmpty(pesel)) pesel = null;
                    if (String.IsNullOrEmpty(mail)) mail = null;

                    if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(lastName) && !String.IsNullOrEmpty(docType) && !String.IsNullOrEmpty(docNr) && !String.IsNullOrEmpty(country) && !String.IsNullOrEmpty(postCode) && !String.IsNullOrEmpty(city) && !String.IsNullOrEmpty(street) && !String.IsNullOrEmpty(house) && !String.IsNullOrEmpty(phone))
                    {
                        Guests newGuest = new Guests
                        {
                            FirstName = name,
                            LastName = lastName,
                            DocumentType = docType,
                            DocumentNumber = docNr,
                            Country = country,
                            City = city,
                            PostalCode = postCode,
                            Street = street,
                            HouseNumber = house,
                            ApartmentNumber = apartment,
                            PhoneNumber = phone,
                            PESEL = pesel,
                            Email = mail
                        };
                        context.Guests.Add(newGuest);
                        context.SaveChanges();

                        MessageBox.Show("Guest added successfully");
                    }
                    else
                        MessageBox.Show("Enter all necessery data");
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            MessageBox.Show($"Błąd walidacji: {validationError.PropertyName} - {validationError.ErrorMessage}");
                        }
                    }
                }

                name = string.Empty;
                lastName = string.Empty;
                docType = string.Empty;
                docNr = string.Empty;
                pesel = string.Empty;
                country = string.Empty;
                postCode = string.Empty;
                city = string.Empty;
                street = string.Empty;
                house = string.Empty;
                apartment = string.Empty;
                phone = string.Empty;
                mail = string.Empty;

                _name.Text = string.Empty;
                _lastName.Text = string.Empty;
                _docType.Text = string.Empty;
                _docNr.Text = string.Empty;
                _pesel.Text = string.Empty;
                _country.Text = string.Empty;
                _postCode.Text = string.Empty;
                _city.Text = string.Empty;
                _street.Text = string.Empty;
                _house.Text = string.Empty;
                _apartment.Text = string.Empty;
                _phone.Text = string.Empty;
                _mail.Text = string.Empty;

            }
        }
    }
}
