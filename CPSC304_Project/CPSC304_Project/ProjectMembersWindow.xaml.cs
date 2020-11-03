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
    /// Interaction logic for ProjectMembersWindow.xaml
    /// </summary>
    public partial class ProjectMembersWindow : Window
    {
        private MainWindow2 callerWindow;
        private Project activeProject;

        public ProjectMembersWindow( MainWindow2 caller, Project project )
        {
            InitializeComponent ();

            callerWindow = caller;
            activeProject = project;

            // Generate buttons for all users on the active project
            List<User> usersOnProject = DatabaseHandler.getInstance ().getUsersOnProject ( project.getProjectId () );
            foreach ( User user in usersOnProject )
            {
                Button userButton = new Button ()
                {
                    Background = new SolidColorBrush ( Colors.Transparent ),
                    FontSize = 16,
                    Height = 58,
                    Content = user.username
                };
                userButton.Click += UserButton_Click;
                userButton.Tag = user;
                MainStackPanel.Children.Add ( userButton );
            }
            ProjectNameLabel.Content = activeProject.getName ();

        }

        private void UserButton_Click( object sender, RoutedEventArgs e )
        {
            // Opens a UserInformationWindow
            Button userButton = sender as Button;
            User selectedUser = userButton.Tag as User;
            UserInformationWindow userInfoWindow = new UserInformationWindow ( selectedUser, this );
            this.IsEnabled = false;
            userInfoWindow.Show ();
        }

        private void CloseButton_Click( object sender, RoutedEventArgs e )
        {
            callerWindow.IsEnabled = true;
            this.Close ();
        }
    }
}
