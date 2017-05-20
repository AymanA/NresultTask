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

namespace nResult_task
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void datagrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            if (e.Column.Header.Equals("Gender") || e.Column.Header.Equals("Title") ||
                e.Column.Header.Equals("Occupation") || e.Column.Header.Equals("Company") || 
                e.Column.Header.Equals("GivenName") || e.Column.Header.Equals("MiddleInitial") ||
                e.Column.Header.Equals("Surname") || e.Column.Header.Equals("BloodType") ||
                e.Column.Header.Equals("EmailAddress"))
            {
                e.Handled = true;
            }
        }
    }
}
