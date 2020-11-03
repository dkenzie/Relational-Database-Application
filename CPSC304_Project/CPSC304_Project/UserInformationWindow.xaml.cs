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
    /// Interaction logic for UserInformationWindow.xaml
    /// </summary>
    public partial class UserInformationWindow : Window
    {
        private User activeUser;
        private Window callerWindow;

        public UserInformationWindow( User user, Window caller )
        {
            InitializeComponent ();
            activeUser = user;
            callerWindow = caller;
            UsernameTextBlock.Text = activeUser.username;
            GenerateUserTasksUI ();
        }

        private void GenerateUserTasksUI()
        {
            MainStackPanel.Children.Clear ();
            // Get all tasks assigned to the active user and display them
            List<Task> usersTasks = DatabaseHandler.getInstance ().getAllTasksForUser ( activeUser );
            foreach ( Task task in usersTasks )
            {
                Label taskNameLabel = new Label ()
                {
                    Content = task.GetName(),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Tag = task,
                };

                MainStackPanel.Children.Add ( taskNameLabel );
                MainStackPanel.Children.Add ( new Separator () );

                taskNameLabel.MouseEnter += TaskNameLabel_MouseEnter;
                taskNameLabel.MouseLeave += TaskNameLabel_MouseLeave;
                taskNameLabel.MouseLeftButtonUp += TaskNameLabel_MouseLeftButtonUp;
            }
        }

        private void TaskNameLabel_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            Label taskNameLabel = sender as Label;
            Task task = taskNameLabel.Tag as Task;
            ViewTaskWindow viewTaskWindow = new ViewTaskWindow ( task, this, activeUser );
            this.IsEnabled = false;
            viewTaskWindow.Show ();
        }

        private void TaskNameLabel_MouseEnter( object sender, MouseEventArgs e )
        {
            Label taskNameLabel = sender as Label;
            taskNameLabel.Background = new SolidColorBrush ( Colors.DarkGray );
        }

        private void TaskNameLabel_MouseLeave( object sender, MouseEventArgs e )
        {
            Label taskNameLabel = sender as Label;
            taskNameLabel.Background = new SolidColorBrush ( Colors.White );


        }

        private void CloseButton_Click( object sender, RoutedEventArgs e )
        {
            callerWindow.IsEnabled = true;
            this.Close ();
        }

        private void TaskDateRangeComboBox_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            if ( TaskDateRangeComboBox.SelectedIndex == 0 )
            {
                if ( DaysTextLabel != null )
                {
                    DaysTextLabel.Visibility = Visibility.Hidden;
                    GenerateUserTasksUI ();
                }
                return;
            }
            else
            {
                DaysTextLabel.Visibility = Visibility.Visible;
                int dateRange = Convert.ToInt32 ( ( TaskDateRangeComboBox.SelectedItem as ComboBoxItem ).Content as string );
                GenerateUserTasksUIWithinDateRange ( dateRange );
            }

        }

        private void GenerateUserTasksUIWithinDateRange( int dateRange )
        {
            MainStackPanel.Children.Clear ();
            // Get all tasks withing dateRange assigned to the active user and display them
            List<Task> usersTasks = DatabaseHandler.getInstance ().getAllTasksForUser ( activeUser );
            foreach ( Task task in usersTasks )
            {
                if ( ( task.GetDueDate () - DateTime.Now ) > new TimeSpan ( dateRange, 0, 0, 0 ) ) 
                {
                    continue;
                }

                Label taskNameLabel = new Label ()
                {
                    Content = task.GetName (),
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    Tag = task,
                };

                MainStackPanel.Children.Add ( taskNameLabel );
                MainStackPanel.Children.Add ( new Separator () );

                taskNameLabel.MouseEnter += TaskNameLabel_MouseEnter;
                taskNameLabel.MouseLeave += TaskNameLabel_MouseLeave;
                taskNameLabel.MouseLeftButtonUp += TaskNameLabel_MouseLeftButtonUp;
            }
        }
    }
}
