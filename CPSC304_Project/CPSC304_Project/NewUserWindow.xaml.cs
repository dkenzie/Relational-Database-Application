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
using System.Windows.Shapes;

namespace CPSC304_Project
{
    /// <summary>
    /// Interaction logic for NewUserWindow.xaml
    /// </summary>
    public partial class NewUserWindow : Window
    {
        public NewUserWindow()
        {
            InitializeComponent();
        }

        private void CreateNewUserButton_Click( object sender, RoutedEventArgs e )
        {
            string username  = UsernameTextBox.Text;
            string password  = PasswordTextbox.Text;
            bool   isManager = IsManagerCheckBox.IsChecked == true;

            // Add new user to db then login as new user
            User newUser = DatabaseHandler.getInstance ().addNewUser ( username, password, isManager );
            MainWindow2 loggedInWindow = new MainWindow2 ( newUser );
            loggedInWindow.Show ();
            this.Close ();
        }

        private void Rect_MouseDown( object sender, MouseButtonEventArgs e )
        {
            this.DragMove ();
        }

        private void BackButton_Click( object sender, RoutedEventArgs e )
        {
            MainWindow loginWindow = new MainWindow ();
            loginWindow.Show ();
            this.Close ();
        }
    }
}
