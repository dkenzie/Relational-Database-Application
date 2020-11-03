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
    /// Interaction logic for SelectUserWindow.xaml
    /// </summary>
    public partial class SelectUserWindow : Window
    {
        private Window callerWindow;
        private List<User> allUsers;
        private Project selectedProject;
        private Task selectedTask;

        public SelectUserWindow( Window caller, Project selectedProject )
        {
            InitializeComponent ();
            callerWindow = caller;
            allUsers = DatabaseHandler.getInstance ().getAllUsers ();
            this.selectedProject = selectedProject;
            this.selectedTask = null;
            ;
            foreach ( User user in allUsers )
            {
                Button selectUserButton = new Button ()
                {
                    Content = user.username,
                    FontSize = 16,
                    Tag = user
                };
                selectUserButton.Click += SelectUserButton_Click;
                MainStackPanel.Children.Add ( selectUserButton );
            }
        }

        public SelectUserWindow( Window caller, Task selectedTask )
        {
            InitializeComponent ();
            callerWindow = caller;
            allUsers = DatabaseHandler.getInstance ().getAllUsers ();
            this.selectedProject = null;
            this.selectedTask = selectedTask;
            foreach ( User user in allUsers )
            {
                Button selectUserButton = new Button ()
                {
                    Content = user.username,
                    FontSize = 16,
                    Tag = user
                };
                selectUserButton.Click += SelectUserButton_Click;
                MainStackPanel.Children.Add ( selectUserButton );
            }
        }

        private void SelectUserButton_Click( object sender, RoutedEventArgs e )
        {
            if ( callerWindow is AddUserToProjectWindow ) 
            {

                User selectedUser = ( sender as Button ).Tag as User;
                try
                {
                    DatabaseHandler.getInstance ().addUserToProject ( selectedUser, selectedProject );
                }
                catch ( Exception ex )
                {
                    MessageBox.Show ( "Failed to add user to project./n" + ex.Message, "Error" );
                    DatabaseHandler.getInstance ().closeConnection ();
                    return;
                }
                MessageBox.Show ( String.Format ( "User: {0} successfully added to Project: {1}", selectedUser.username, selectedProject.getName () ), "Success" );
            }
            else if ( callerWindow is ViewTaskWindow )
            {
                User selectedUser = ( sender as Button ).Tag as User;
                DatabaseHandler.getInstance ().assignUserToTask ( selectedUser, selectedTask );
                callerWindow.IsEnabled = true;
                (callerWindow as ViewTaskWindow).RefreshUI ();

                this.Close ();
            }
        }

        private void CloseButton_Click( object sender, RoutedEventArgs e )
        {
            this.Close ();
            callerWindow.IsEnabled = true;
        }
    }
}
