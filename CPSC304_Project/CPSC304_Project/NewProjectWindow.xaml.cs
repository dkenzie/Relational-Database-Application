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
    /// Interaction logic for NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow : Window
    {
        private MainWindow2 callerWindow;

        public NewProjectWindow( MainWindow2 caller ) 
        {
            callerWindow = caller;
            InitializeComponent ();
        }

        private void CancelButton_Click( object sender, RoutedEventArgs e )
        {
            this.Close ();
            callerWindow.IsEnabled = true;
        }

        private void CreateButton_Click( object sender, RoutedEventArgs e )
        {
            string projectName = ProjectNameTextBox.Text;
            int projectId = DatabaseHandler.generateNextProjectId ();
            Project newProject = new Project ( projectName, new List<ProjectList> (), new List<User> (), projectId );
            callerWindow.AddProject ( newProject );

            this.Close ();
            callerWindow.IsEnabled = true;

        }
    }
}
