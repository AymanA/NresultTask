using nResult_task.Base;
using nResult_task.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CsvHelper;

namespace nResult_task.ViewModel
{
    class MainViewModel : ViewModelBase
    {
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

        private ICommand openFileCommand;
        public ICommand OpenFileCommand
        {
            get
            {
                return openFileCommand;
            }
            set
            {
                openFileCommand = value;
            }
        }

        public MainViewModel()
        {
           //Customers = GetCustomers();
            OpenFileCommand = new RelayCommand(OpenFile, (param)=> true);
        }

        private void OpenFile(object obj)
        {

            // Create a list buffer
            var myList = new List<Customer>();
            //using (var streamReader = new StreamReader("E:\\nresult-task\\CodeInterview/Interview Name List.csv"))
            using (var streamReader = new StreamReader("E:\\nresult-task\\CodeInterview/test.csv"))
            {
                // browse the csv file line by line until the end of the file
                while (!streamReader.EndOfStream)
                {
                    // for each line, split it with the split caractere (that may no be ';')
                    var splitLine = streamReader.ReadLine().Split(',');

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
                streamReader.Close();
            }

            // Convert the list into an observable collection
            Customers = new ObservableCollection<Customer>(myList);
            DataGridVisibility = "Visible";
        }

      
        //private ObservableCollection<Customer> GetCustomers()
        //{
        //    //string[] lines = File.ReadAllLines(System.IO.Path.ChangeExtension(fileName, ".csv"));

        //    //// lines.Select allows me to project each line as a Person. 
        //    //// This will give me an IEnumerable<Person> back.
        //    //return lines.Select(line =>
        //    //{
        //    //    string[] data = line.Split(';');
        //    //    // We return a person with the data in order.
        //    //    return new Customer(data[0], data[1], Convert.ToInt32(data[2]), data[3]);
        //    //});
        //}
    }
}
