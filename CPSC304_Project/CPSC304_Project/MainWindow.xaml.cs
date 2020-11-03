using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CPSC304_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string DATABASE_NAME = "CPSC304DATABASE";

        public MainWindow()
        {
            // When the app is opened, initialize the database
            DatabaseHandler.setDatabase ( DATABASE_NAME );
            DatabaseHandler.getInstance ();
            DatabaseHandler.initDatabase ();


            InitializeComponent ();
        }

        private void Rect_MouseDown( object sender, MouseButtonEventArgs e )
        {
            this.DragMove ();
        }

        private void ProcessLoginAttemptButton_Click( object sender, RoutedEventArgs e )
        {
            // User input values
            string username = UsernameTextBox.Text;
            string password = PasswordTextbox.Text;

            List<User> allUsers = DatabaseHandler.getInstance().getAllUsers ();

            // Validate login attempt
            foreach ( User user in allUsers )
            {
                if ( user.username == username && user.password == password )
                {
                    MainWindow2 loggedInWindow = new MainWindow2 ( user );
                    loggedInWindow.Show ();
                    this.Close ();
                    return;
                }
            }

        }

        private void CreateNewUserButton_Click( object sender, RoutedEventArgs e )
        {
            NewUserWindow newUserWindow = new NewUserWindow ();
            newUserWindow.Show ();
            this.Close ();

        }

        private void CloseButton_Click( object sender, RoutedEventArgs e )
        {
            this.Close ();
        }
    }
}
