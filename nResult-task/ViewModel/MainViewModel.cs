using nResult_task.Base;
using nResult_task.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using CsvHelper;

namespace nResult_task.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        private IList<Customer> _bindedCustomersList;
        public IList<Customer> BindedCustomersList
        {
            get { return _bindedCustomersList; }
            set
            {
                if (value != _bindedCustomersList)
                {
                    _bindedCustomersList = value;
                    NotifyPropertyChanged("BindedCustomersList");
                }
            }
        }

        public List<string> _customersHeader = getCustomersHeader();

        private static List<string> getCustomersHeader()
        {
            List<string> Headers = new List<string>();
            Headers.Add("Gender");
            Headers.Add("Title");
            Headers.Add("Occupation");
            Headers.Add("Company");
            Headers.Add("GivenName");
            Headers.Add("MiddleInitial");
            Headers.Add("Surname");
            Headers.Add("BloodType");
            Headers.Add("mailAddress");
            return Headers;
        }

        public List<string> CustomersHeader
        {
            get { return _customersHeader; }
            set
            {
                _customersHeader = value;
                NotifyPropertyChanged("CustomersHeader");
            }
        }

        private IEnumerable<Customer> _customersOperations;

        public IEnumerable<Customer> CustomersOperations
        {
            get { return _customersOperations; }
            set
            {
                _customersOperations = value;
                NotifyPropertyChanged("CustomersOperations");
            }
        }

        private ObservableCollection<Customer> _customers;
        public ObservableCollection<Customer> Customers
        {
            get { return _customers; }
            set
            {
                if (value != _customers)
                {
                    _customers = value;
                    NotifyPropertyChanged("Customers");
                }
            }
        }

        private Customer _selectedCustomer;

        public Customer SelectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                NotifyPropertyChanged("SelectedCustomer");
            }
        }


        private string _dataGridVisibility = "Hidden";
        public string DataGridVisibility
        {
            get
            {
                return _dataGridVisibility;
            }

            set
            {
                _dataGridVisibility = value;
                NotifyPropertyChanged("DataGridVisibility");


            }
        }



        #region navigation

        private string _pageIndex;

        public string PageIndex
        {
            get { return _pageIndex; }
            set
            {
                _pageIndex = value;
                NotifyPropertyChanged("PageIndex");

            }
        }

        private int _currentPageIndex;

        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                _currentPageIndex = value;
                UpdatePageIndex(value);
                NotifyPropertyChanged("CurrentPageIndex");
            }
        }

        private int _pagesCount;

        public int PagesCount
        {
            get { return _pagesCount; }
            set
            {
                _pagesCount = value;
                UpdatePagesCount(value);
                NotifyPropertyChanged("PagesCount");
            }
        }

        private int _pageSize = 15;

        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                NotifyPropertyChanged("PageSize");
            }
        }

        private bool _prevEnabled = false;
        public bool PrevEnabled
        {
            get
            {
                return _prevEnabled;
            }

            set
            {
                _prevEnabled = value;
                NotifyPropertyChanged("PrevEnabled");
            }
        }

        private bool _nextEnabled = true;
        public bool NextEnabled
        {
            get
            {
                return _nextEnabled;
            }

            set
            {
                _nextEnabled = value;
                NotifyPropertyChanged("NextEnabled");
            }
        }

        private bool _firstEnabled = false;
        public bool FirstEnabled
        {
            get
            {
                return _firstEnabled;
            }

            set
            {
                _firstEnabled = value;
                NotifyPropertyChanged("FirstEnabled");
            }
        }

        private bool _lastEnabled = true;
        public bool LastEnabled
        {
            get
            {
                return _lastEnabled;
            }

            set
            {
                _lastEnabled = value;
                NotifyPropertyChanged("LastEnabled");
            }
        }

        #endregion


        #region Commands 
        public ICommand FirstPageCommand { get; set; }
        public ICommand LastPageCommand { get; set; }

        public ICommand PreviousePageCommand { get; set; }
        public ICommand NextPageCommand { get; set; }

        public ICommand OpenFileCommand { get; set; }
        public ICommand ExportCustomersCommand { get; set; }
        public ICommand FilterCommand { get; set; }
        public ICommand SortCommand { get; set; }

        #endregion



        public MainViewModel()
        {
            // regarding can execute i made it always can executed and handled the conditions on the properties
            // there is another aproach which will implement the canexecute for every command both of them is ok for me
            // but i choosed the first approach just because i think its easier 

            OpenFileCommand = new RelayCommand(GetCustomersData, (param)=> true);
            FirstPageCommand = new RelayCommand(LoadFirstPage, (param)=> true);
            LastPageCommand = new RelayCommand(LoadLastPage, (param)=> true);
            PreviousePageCommand = new RelayCommand(LoadPreviousePage, (param)=> true);
            NextPageCommand = new RelayCommand(LoadNextPage, (param)=> true);
            ExportCustomersCommand = new RelayCommand(ExportCustomers, (param)=> true);
            FilterCommand = new RelayCommand(FilterCustomers, (param)=> true);
            SortCommand = new RelayCommand(SortCustomers, (param)=> true);
        }


        private void ExportCustomers(object obj)
        {
            var filename = GetExportedFileName();

            WriteCsv(CustomersOperations, filename);
        }

        private string GetExportedFileName()
        {
            Microsoft.Win32.SaveFileDialog ExportDlg = new Microsoft.Win32.SaveFileDialog();
            ExportDlg.DefaultExt = ".csv"; // Default file extension
            ExportDlg.Filter = "Excel Files| *.xlsx;*.xls;*.csv;"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = ExportDlg.ShowDialog();
            string filename = string.Empty;
            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                filename = ExportDlg.FileName;
            }
            return filename;
        }

        public void WriteCsv<T>(IEnumerable<T> items, string path)
        {
            Type itemType = typeof(T);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                foreach (var item in items)
                {
                    writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));
                }
            }
        }


        private void LoadLastPage(object obj)
        {
            
            if (CurrentPageIndex != PagesCount)
            {
                CurrentPageIndex = PagesCount;
                BindedCustomersList = GetPage(CustomersOperations, CurrentPageIndex, PageSize);
                NextEnabled = false;
                LastEnabled = false;
                FirstEnabled = true;
                PrevEnabled = true;
            }
        }

        private void LoadNextPage(object obj)
        {
            CurrentPageIndex++;
            BindedCustomersList = GetPage(CustomersOperations, CurrentPageIndex, PageSize);
            PrevEnabled = true;
            FirstEnabled = true;
            if (CurrentPageIndex == PagesCount)
            {
                NextEnabled = false;
                LastEnabled = false;
            }

        }

        private void LoadPreviousePage(object obj)
        {
            CurrentPageIndex--;
            BindedCustomersList = GetPage(CustomersOperations, CurrentPageIndex, PageSize);
            NextEnabled = true;
            LastEnabled = true;
            if (CurrentPageIndex == 0)
            {
                PrevEnabled = false;
                FirstEnabled = false;
            }
        }

        private void LoadFirstPage(object obj)
        {
            if (CurrentPageIndex != 0)
            {
                CurrentPageIndex = 0;
                BindedCustomersList = GetPage(CustomersOperations, CurrentPageIndex, PageSize);
                NextEnabled = true;
                LastEnabled = true;
                PrevEnabled = false;
                FirstEnabled = false;
            }
            
        }

        private void UpdatePageIndex(int index)
        {
            var cureentIndex = index;
            var cureentCount = PagesCount;
            PageIndex = (cureentIndex + 1)+ " of " + (cureentCount + 1);
        }


        private void UpdatePagesCount(int pagesCount)
        {
            var cureentCount = pagesCount;
            var currentPageIndex = CurrentPageIndex;
            PageIndex = (currentPageIndex + 1) + " of " + (cureentCount + 1);
        }

        private string GetCustomersFilePath()
        {
            OpenFileDialog ChooseFileDlg = new OpenFileDialog();
            ChooseFileDlg.Filter = "Excel Files| *.xlsx;*.xls;*.csv;";
            if (ChooseFileDlg.ShowDialog() == DialogResult.Cancel)
                return String.Empty;
            return ChooseFileDlg.FileName;
        }

        private void GetCustomersData(object obj)
        {
            string fileName = GetCustomersFilePath();

            if (!string.IsNullOrEmpty(fileName))
            {
                // Create a list buffer
                var myList = new List<Customer>();
                using (var streamReader = new StreamReader(fileName))
                {
                    string headerLine = streamReader.ReadLine();
                    // browse the csv file line by line until the end of the file
                    while (!streamReader.EndOfStream)
                    {
                        // for each line, split it with the split caractere (that may no be ',')
                        var readLine = streamReader.ReadLine();
                        if (readLine != null)
                        {
                            var splitLine = readLine.Split(',');

                            // map the splitted line with an entity
                            var myNewCustomer = new Customer()
                            {
                                Gender = splitLine[0].Trim(),
                                Title = splitLine[1].Trim(),
                                Occupation = splitLine[2].Trim(),
                                Company = splitLine[3].Trim(),
                                GivenName = splitLine[4].Trim(),
                                MiddleInitial = splitLine[5].Trim(),
                                Surname = splitLine[6].Trim(),
                                BloodType = splitLine[7].Trim(),
                                EmailAddress = splitLine[8].Trim(),
                            };

                            // add the entity  in the list
                            myList.Add(myNewCustomer);
                        }
                    }
                    streamReader.Close();
                }
                // Convert the list into an observable collection
                Customers = new ObservableCollection<Customer>(myList);
                PagesCount = Customers.Count / PageSize;
                CurrentPageIndex = 0;
                CustomersOperations = Customers;
                BindedCustomersList = GetPage(CustomersOperations, CurrentPageIndex, PageSize);
                DataGridVisibility = "Visible";
            }
            
        }

        private void FilterCustomers(object obj)
        {
            string filterParam = obj.ToString();
            //CustomersOperations.Filter = customer =>
            //{
            //    Customer c = customer as Customer;
            //    return c.Gender.ToString().Contains(obj.ToString());
            //};
            CustomersOperations = Customers.Where(c => c.Gender.Contains(filterParam));
            BindedCustomersList = GetPage(CustomersOperations, 0,15);

        }

        private void SortCustomers(object obj)
        {
            string colName = obj.ToString();
            switch (colName)
            {
                case "Gender":
                    CustomersOperations = from i in Customers orderby i.Gender select i;
                    break;
                case "Title":
                    CustomersOperations = from i in Customers orderby i.Title select i;
                    break;
                case "Occupation":
                    CustomersOperations = from i in Customers orderby i.Occupation select i;
                    break;
                case "Company":
                    CustomersOperations = from i in Customers orderby i.Company select i;
                    break;
                case "GivenName":
                    CustomersOperations = from i in Customers orderby i.GivenName select i;
                    break;
                case "MiddleInitial":
                    CustomersOperations = from i in Customers orderby i.MiddleInitial select i;
                    break;
                case "Surname":
                    CustomersOperations = from i in Customers orderby i.Surname select i;
                    break;
                case "BloodType":
                    CustomersOperations = from i in Customers orderby i.BloodType select i;
                    break;
                case "EmailAddress":
                    CustomersOperations = from i in Customers orderby i.EmailAddress select i;
                    break;
                 default:
                    break;

            }
            BindedCustomersList = GetPage(CustomersOperations, 0, 15);
        }


        IList<Customer> GetPage(IEnumerable<Customer> list, int page, int pageSize)
        {
            PagesCount = list.Count()/pageSize;
            return list.Skip(page * pageSize).Take(pageSize).ToList();
        }

    }
}
