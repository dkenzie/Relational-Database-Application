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
    /// Interaction logic for ViewTaskWindow.xaml
    /// </summary>
    public partial class ViewTaskWindow : Window
    {
        private Task activeTask;
        private Window callerWindow;
        private User activeUser;

        public ViewTaskWindow( Task task, Window caller, User user )
        {
            this.callerWindow = caller;
            this.activeTask = task;
            this.activeUser = user;
            InitializeComponent ();

            TaskNameLabel.Content = activeTask.GetName ();
            TaskDescriptionTextBox.Text = activeTask.GetDescription () as string;
            AssignedToLabel.Content = DatabaseHandler.getInstance ().getUserFromId ( activeTask.AssignedToUserId ).username;
            DueDateLabel.Content = task.GetDueDate ().ToString ( "yyyy-MM-dd" );
        }

        private void CloseButton_Click( object sender, RoutedEventArgs e )
        {
            this.Close ();
            callerWindow.IsEnabled = true;
        }

        private void ChangeAssignedUserButton_Click( object sender, RoutedEventArgs e )
        {
            if ( activeUser.isManager )
            {
                SelectUserWindow selectUserWindow = new SelectUserWindow ( this, activeTask );
                this.IsEnabled = false;
                selectUserWindow.Show ();
            }
            else
            {
                MessageBox.Show ( "You require manager priviliges in order to perform this action.", "Error" );
            }
        }

        internal void RefreshUI()
        {
            activeTask = DatabaseHandler.getInstance ().getTaskFromId ( activeTask.Id );
            TaskNameLabel.Content = activeTask.GetName ();
            TaskDescriptionTextBox.Text = activeTask.GetDescription () as string;
            AssignedToLabel.Content = DatabaseHandler.getInstance ().getUserFromId ( activeTask.AssignedToUserId ).username;
        }

        private void DeleteTaskButton_Click( object sender, RoutedEventArgs e )
        {
            if ( activeUser.isManager )
            {
                var result = MessageBox.Show ( "Are you sure you want to delete the selected task?", "Attention", MessageBoxButton.YesNo );
                switch ( result )
                {
                    case MessageBoxResult.Yes:
                        // Delete the task
                        DatabaseHandler.getInstance ().removeTask ( activeTask );
                        this.Close ();
                        ( callerWindow as MainWindow2 ).RefreshProjectUI ( ( callerWindow as MainWindow2 ).activeProject.getProjectId () );
                        callerWindow.IsEnabled = true;
                        break;
                    case MessageBoxResult.No:
                        // Do nothing
                        break;
                }
            }
            else
            {
                MessageBox.Show ( "You require manager priviliges in order to perform this action.", "Error" );
            }
        }
    }
}
