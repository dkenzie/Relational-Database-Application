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
    /// Interaction logic for MainWindow2.xaml
    /// </summary>
    public partial class MainWindow2 : Window
    {
        private List<Project> projects = new List<Project> ();
        public Project activeProject = null;
        public User activeUser = null;

        public MainWindow2( User activeUser )
        {
            this.activeUser = activeUser;
            int activeUserId = activeUser.getUserId ();
            InitializeComponent ();
            string username = DatabaseHandler.getInstance ().getUsername ( activeUserId );
            CurrentUserLabel.Content = username;

            ReloadProjectsComboBox ();
        }

        private void CloseButton_Click( object sender, RoutedEventArgs e )
        {
            this.Close ();
        }

        private void LogoutButton_Click( object sender, RoutedEventArgs e )
        {
            var result = MessageBox.Show ("This will bring you back to the login window. \nProceed?", "Attention", MessageBoxButton.YesNo);
            if ( result == MessageBoxResult.Yes )
            {
                MainWindow loginWindow = new MainWindow ();
                loginWindow.Show ();
                this.Close ();
            }
            else if ( result == MessageBoxResult.No )
            {
                // Do nothing
            }
        }

        internal void AddNewList( ProjectList newProjectList )
        {
            activeProject.AddProjectList ( newProjectList );
        }

        internal void AddProject( Project newProject )
        {
            projects.Add ( newProject );
            DatabaseHandler.getInstance ().addNewProject ( newProject );
            DatabaseHandler.getInstance ().addUserToProject ( activeUser, newProject );

            ActiveUsersProjectsComboBox.Items.Add ( new ComboBoxItem () { Content = newProject.getName (), Tag = newProject.getProjectId () } );
            ActiveUsersProjectsComboBox.Items.Remove ( AddNewProjectComboBoxItem );
            ActiveUsersProjectsComboBox.Items.Add ( AddNewProjectComboBoxItem );

            ActiveUsersProjectsComboBox.SelectedIndex = ActiveUsersProjectsComboBox.Items.Count - 2;
        }

        internal void ReloadProjectsComboBox()
        {
            
            ActiveUsersProjectsComboBox.Items.Clear ();
            projects = DatabaseHandler.getInstance ().getUsersProjects ( activeUser.id );
            foreach ( Project project in projects )
            {
                ActiveUsersProjectsComboBox.Items.Add ( new ComboBoxItem () { Content = project.getName (), Tag = project.getProjectId () } );
            }
            ActiveUsersProjectsComboBox.Items.Add ( AddNewProjectComboBoxItem );
        }

        private void ProjectListComboBox_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            ComboBoxItem selectedItem = ActiveUsersProjectsComboBox.SelectedItem as ComboBoxItem;
            if ( selectedItem != null )
            {
                if ( selectedItem == AddNewProjectComboBoxItem )
                {
                    CreateNewProject ();
                }
                else
                {
                    projects = DatabaseHandler.getInstance ().getUsersProjects ( activeUser.id );
                    int projectId = Convert.ToInt32 ( ( selectedItem as ComboBoxItem ).Tag );
                    RefreshProjectUI ( projectId );
                }

            }
        }

        private void CreateNewProject()
        {
            NewProjectWindow newProjectWindow = new NewProjectWindow ( this );
            this.IsEnabled = false;
            newProjectWindow.Show ();
        }

        public void RefreshProjectUI( int projectId )
        {
            MainStackPanel.Children.Clear ();
            Button newListButton = new Button ();
            newListButton.Content = "+";
            newListButton.FontSize = 24;
            newListButton.Height = 36;
            newListButton.Width = 36;
            newListButton.VerticalAlignment = VerticalAlignment.Top;
            newListButton.Margin = new Thickness ( 16, 10, 0, 0 );
            newListButton.Click += AddNewListButton_Click;
            MainStackPanel.Children.Add ( newListButton );

            activeProject = getProjectWithId ( projectId );
            foreach ( ProjectList projectList in activeProject.getProjectLists () )
            {
                GenerateProjectListUI ( projectList );
            }

        }

        private Project getProjectWithId( int projectId )
        {
            foreach ( Project project in projects )
            {
                if ( project.getProjectId() == projectId )
                {
                    return project;
                }
            }
            return null;
        }

        private void ShowProjectMembersButton_Click( object sender, RoutedEventArgs e )
        {
            // Opens a new window which shows all of the members on the current project
            if ( ActiveUsersProjectsComboBox.SelectedIndex == -1 )
            {
                return;
            }
            ProjectMembersWindow projectMembersWindow = new ProjectMembersWindow ( this, activeProject );
            this.IsEnabled = false;
            projectMembersWindow.Show ();
        }

        public void GenerateProjectListUI( ProjectList newProjectList )
        {
            string listName = newProjectList.getName ();
            string listPriority = newProjectList.getListPriority ();

            MainStackPanel.Children.Add ( new Separator () { Width = 20, Visibility = Visibility.Hidden } );

            StackPanel newStackPanel = new StackPanel ();
            newStackPanel.Width = 120;
            newStackPanel.Margin = new Thickness ( 0, 10, 0, 10 );
            newStackPanel.Background = new SolidColorBrush ( Colors.LightGray );

            Button deleteListButton = new Button ()
            {
                Content = "-",
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Right,
                Tag = newProjectList,
                Height = 12,
                Margin = new Thickness(0,6,6,0),
            };
            deleteListButton.Click += DeleteListButton_Click;
            newStackPanel.Children.Add ( deleteListButton );

            Label listHeaderLabel = new Label ();
            listHeaderLabel.Content = listName;
            newStackPanel.Children.Add ( listHeaderLabel );

            ComboBox priorityComboBox = new ComboBox ();
            ComboBoxItem lowPriority = new ComboBoxItem ();
            lowPriority.Content = "Low";
            ComboBoxItem mediumPriority = new ComboBoxItem ();
            mediumPriority.Content = "Medium";
            ComboBoxItem highPriority = new ComboBoxItem ();
            highPriority.Content = "High";
            priorityComboBox.Items.Add ( lowPriority );
            priorityComboBox.Items.Add ( mediumPriority );
            priorityComboBox.Items.Add ( highPriority );
            switch ( listPriority )
            {
                case "Low":
                    priorityComboBox.SelectedIndex = 0;
                    break;
                case "Medium":
                    priorityComboBox.SelectedIndex = 1;
                    break;
                case "High":
                    priorityComboBox.SelectedIndex = 2;
                    break;
            }
            newStackPanel.Children.Add ( priorityComboBox );
            
            newStackPanel.Children.Add ( new Separator () { Visibility = Visibility.Hidden } );

            Button addTaskButton = new Button ();
            addTaskButton.Click += AddTaskButton_Click;
            addTaskButton.Content = "+";
            addTaskButton.Width = 20;
            addTaskButton.Height = 20;
            addTaskButton.HorizontalAlignment = HorizontalAlignment.Left;
            addTaskButton.Margin = new Thickness ( 10, 0, 0, 0 );
            addTaskButton.Tag = newStackPanel;
            newStackPanel.Tag = newProjectList.Id;
            newStackPanel.Children.Add ( addTaskButton );

            MainStackPanel.Children.Add ( newStackPanel );

            Button addNewButton = null;
            foreach ( var child in MainStackPanel.Children )
            {
                if ( child is Button )
                {
                    addNewButton = child as Button;
                }
            }

            // Generate tasks UI
            List<Task> tasks = DatabaseHandler.getInstance ().getTasksOnList ( newProjectList );
            foreach ( Task task in tasks )
            {
                AddNewTaskToList ( task, newStackPanel );
            }

            MainStackPanel.Children.Remove ( addNewButton );
            MainStackPanel.Children.Add ( addNewButton );
        }

        private void DeleteListButton_Click( object sender, RoutedEventArgs e )
        {
            ProjectList list = ( sender as Button ).Tag as ProjectList;
            DatabaseHandler.getInstance ().removeList ( list );
            projects = DatabaseHandler.getInstance ().getUsersProjects ( activeUser.id );
            activeProject = DatabaseHandler.getInstance ().getProjectFromId ( activeProject.getProjectId () );
            RefreshProjectUI ( activeProject.getProjectId () );
        }

        private void AddTaskButton_Click( object sender, RoutedEventArgs e )
        {
            Button addTaskButton = sender as Button;
            StackPanel parentStackPanel = addTaskButton.Tag as StackPanel;
            int listId = Convert.ToInt32 ( ( parentStackPanel.Tag ) );
            int projectId = getActiveProjectId ();
            NewTaskWindow newTaskWindow = new NewTaskWindow ( parentStackPanel, this, listId, projectId );
            this.IsEnabled = false;
            newTaskWindow.Show ();
        }

        private int getActiveProjectId()
        {
            return Convert.ToInt32 ( ( ActiveUsersProjectsComboBox.SelectedItem as ComboBoxItem ).Tag );
        }

        internal void AddNewTaskToList( Task newTask, StackPanel parentStackPanel )
        {
            StackPanel newTaskStackPanel = new StackPanel ();

            Label taskNameLabel = new Label ()
            {
                Content = newTask.GetName (),
                FontWeight = FontWeights.Bold,
                Tag = newTask,
            };
            newTaskStackPanel.Children.Add ( taskNameLabel );
            taskNameLabel.MouseEnter += TaskNameLabel_MouseEnter;
            taskNameLabel.MouseLeave += TaskNameLabel_MouseLeave;
            taskNameLabel.MouseLeftButtonUp += TaskNameLabel_MouseLeftButtonUp;

            Label taskDescriptionLabel = new Label ();
            taskDescriptionLabel.Content = newTask.GetDescription ();
            newTaskStackPanel.Children.Add ( taskDescriptionLabel );
            if ( newTask.GetDescription ().ToString() == "" )
            {
                taskDescriptionLabel.Visibility = Visibility.Collapsed;
            }

            parentStackPanel.Children.Add ( new Separator () );
            parentStackPanel.Children.Add ( new Separator () { Visibility = Visibility.Hidden } );
            parentStackPanel.Children.Add ( newTaskStackPanel );
            parentStackPanel.Children.Add ( new Separator () { Visibility = Visibility.Hidden } );
            parentStackPanel.Children.Add ( new Separator () );

            Button addNewTaskButton = null;
            foreach ( UIElement child in parentStackPanel.Children )
            {
                if ( child is Button )
                {
                    addNewTaskButton = child as Button;
                }
            }
            parentStackPanel.Children.Remove ( addNewTaskButton );
            parentStackPanel.Children.Add ( addNewTaskButton );
        }

        private void TaskNameLabel_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            Task task = ( sender as Label ).Tag as Task;
            task = DatabaseHandler.getInstance ().getTaskFromId ( task.Id );
            ViewTaskWindow taskWindow = new ViewTaskWindow ( task, this, activeUser );
            this.IsEnabled = false;
            taskWindow.Show ();
        }

        private void TaskNameLabel_MouseLeave( object sender, MouseEventArgs e )
        {
            Label taskNameLabel = sender as Label;
            taskNameLabel.Background = new SolidColorBrush ( Colors.LightGray );
            taskNameLabel.Foreground = new SolidColorBrush ( Colors.Black );

        }

        private void TaskNameLabel_MouseEnter( object sender, MouseEventArgs e )
        {
            Label taskNameLabel = sender as Label;
            taskNameLabel.Background = new SolidColorBrush ( Colors.DarkGray );
            taskNameLabel.Foreground = new SolidColorBrush ( Colors.White );
        }

        private void DragRectangle_MouseDown( object sender, MouseButtonEventArgs e )
        {
            this.DragMove ();
        }

        private void AddNewListButton_Click( object sender, RoutedEventArgs e )
        {
            if ( ActiveUsersProjectsComboBox.SelectedIndex == -1 )
            {
                MessageBox.Show ( "Please select a project first.", "Error" );
                return;
            }

            NewListWindow newListWindow = new NewListWindow ( this, getActiveProjectId () );
            this.IsEnabled = false;
            newListWindow.Show ();
        }

        private void CurrentUserLabel_MouseEnter( object sender, MouseEventArgs e )
        {
            CurrentUserLabel.Background = new SolidColorBrush ( Colors.DarkGray );
            CurrentUserLabel.Foreground = new SolidColorBrush ( Colors.White );
            // CurrentUserLabel.FontWeight = FontWeights.Bold;
        }

        private void CurrentUserLabel_MouseLeave( object sender, MouseEventArgs e )
        {
            CurrentUserLabel.Background = new SolidColorBrush ( Colors.Transparent );
            CurrentUserLabel.Foreground = new SolidColorBrush ( Colors.Black );
            CurrentUserLabel.FontWeight = FontWeights.Normal;

        }

        private void CurrentUserLabel_PreviewMouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
            UserInformationWindow userInfoWindow = new UserInformationWindow ( activeUser, this );
            this.IsEnabled = false;
            userInfoWindow.Show ();
        }

        private void ProjectsButton_Click( object sender, RoutedEventArgs e )
        {
            if ( activeUser.isManager == false )
            {
                MessageBox.Show ( "You require manager priviliges in order to perform this action.", "Error" );
                return;
            }

            AddUserToProjectWindow addUserToProjectWindow = new AddUserToProjectWindow ( this );
            this.IsEnabled = false;
            addUserToProjectWindow.Show ();
        }

        private void QueriesButton_Click( object sender, RoutedEventArgs e )
        {
            AdvancedQueriesWindow advancedQueriesWindow = new AdvancedQueriesWindow ( this );
            this.IsEnabled = false;
            advancedQueriesWindow.Show ();
        }
    }
}
