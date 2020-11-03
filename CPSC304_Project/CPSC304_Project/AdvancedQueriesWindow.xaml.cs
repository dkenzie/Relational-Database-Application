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
    /// Interaction logic for AdvancedQueriesWindow.xaml
    /// </summary>
    public partial class AdvancedQueriesWindow : Window
    {
        Window callerWindow;

        public AdvancedQueriesWindow( Window caller )
        {
            callerWindow = caller;
            InitializeComponent ();
            MostTasksLabel.Content = Convert.ToString ( DatabaseHandler.getInstance ().getMaxNumTasksAssignedToSingleUser () );
            MaxLabel.Content = Convert.ToString ( DatabaseHandler.getInstance ().getMaxAverageTasksPerListOutOfAllProjects () );
            MinLabel.Content = Convert.ToString ( DatabaseHandler.getInstance ().getMinAverageTasksPerListOutOfAllProjects () );
        }

        private void CloseButton_Click( object sender, RoutedEventArgs e )
        {
            this.Close ();
            callerWindow.IsEnabled = true;
        }
    }
}
