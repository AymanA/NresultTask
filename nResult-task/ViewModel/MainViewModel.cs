using nResult_task.Base;
using nResult_task.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CsvHelper;

namespace nResult_task.ViewModel
{
    class MainViewModel : ViewModelBase
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

        #endregion



        public MainViewModel()
        {
           //Customers = GetCustomers();
            OpenFileCommand = new RelayCommand(GetCustomersData, (param)=> true);
            FirstPageCommand = new RelayCommand(LoadFirstPage, (param)=> true);
            LastPageCommand = new RelayCommand(LoadLastPage, (param)=> true);
            PreviousePageCommand = new RelayCommand(LoadPreviousePage, (param)=> true);
            NextPageCommand = new RelayCommand(LoadNextPage, (param)=> true);
            ExportCustomersCommand = new RelayCommand(ExportCustomers, (param)=> true);
        }

        private void ExportCustomers(object obj)
        {
            var filename = GetExportedFileName();

            WriteCsv(BindedCustomersList, filename);
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

        public void WriteCsv<T>(IList<T> items, string path)
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
                BindedCustomersList = GetPage(Customers, CurrentPageIndex, PageSize);
                NextEnabled = false;
                LastEnabled = false;
                FirstEnabled = true;
                PrevEnabled = true;
            }
        }

        private void LoadNextPage(object obj)
        {
            CurrentPageIndex++;
            BindedCustomersList = GetPage(Customers, CurrentPageIndex, PageSize);
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
            BindedCustomersList = GetPage(Customers, CurrentPageIndex, PageSize);
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
                BindedCustomersList = GetPage(Customers, CurrentPageIndex, PageSize);
                NextEnabled = true;
                LastEnabled = true;
                PrevEnabled = false;
                FirstEnabled = false;
            }
            
        }

        private void UpdatePageIndex(int index)
        {
            var cureentIndex = index++;
            PageIndex = cureentIndex + " of " + PagesCount;
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
                BindedCustomersList = GetPage(Customers, CurrentPageIndex, PageSize);
                DataGridVisibility = "Visible";
            }
            
        }


        IList<Customer> GetPage(ObservableCollection<Customer> list, int page, int pageSize)
        {
            return list.Skip(page * pageSize).Take(pageSize).ToList();
        }

    }
}
