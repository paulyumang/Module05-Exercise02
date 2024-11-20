using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Graphics;
using Module07DataAccess.Model;
using Module07DataAccess.Services;

namespace Module07DataAccess.ViewModel
{
    public class EmployeeViewModel : INotifyPropertyChanged
    {
        private readonly EmployeeService _employeeService;
        public ObservableCollection<Employee> EmployeeList { get; set; }


        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                if (_selectedEmployee != null)
                {
                    NewEmployeeName = _selectedEmployee.Name;
                    NewEmployeeAddress = _selectedEmployee.Address;
                    NewEmployeeemail = _selectedEmployee.email ;
                    NewEmployeeContactNo = _selectedEmployee.ContactNo;
                    IsEmployeeSelected = true;
                }
                else
                {
                    IsEmployeeSelected = false;
                }
                OnPropertyChanged();
            }
        }

        private bool _IsEmployeeSelected;
        public bool IsEmployeeSelected
        {
            get => _IsEmployeeSelected;
            set
            {
                _IsEmployeeSelected = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        private Color _statusMessageColor;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public Color StatusMessageColor
        {
            get => _statusMessageColor;
            set
            {
                _statusMessageColor = value;
                OnPropertyChanged();
            }
        }

        //New Personal entry for name, Address, email, ContactNo
        private string _newEmployeeName;
        public string NewEmployeeName
        {
            get => _newEmployeeName;
            set
            {
                _newEmployeeName = value;
                OnPropertyChanged();
            }
        }

        private string _newEmployeeAddress;
        public string NewEmployeeAddress
        {
            get => _newEmployeeAddress;
            set
            {
                _newEmployeeAddress = value;
                OnPropertyChanged();
            }
        }

        private string _newEmployeeemail;
        public string NewEmployeeemail
        {
            get => _newEmployeeemail;
            set
            {
                _newEmployeeemail = value;
                OnPropertyChanged();
            }
        }

        private string _newEmployeeContactNo;
        public string NewEmployeeContactNo
        {
            get => _newEmployeeContactNo;
            set
            {
                _newEmployeeContactNo = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand AddEmployeeCommand { get; }
        public ICommand SelectedEmployeeCommand { get; }
        public ICommand DeleteEmployeeCommand { get; }

        public EmployeeViewModel()
        {
            _employeeService = new EmployeeService();
            EmployeeList = new ObservableCollection<Employee>();
            LoadDataCommand = new Command(async () => await LoadDataAsync());
            StatusMessageColor = Colors.Red;  // Default to red for errors or initial state
            AddEmployeeCommand = new Command(async () => await AddEmployee());
            SelectedEmployeeCommand = new Command<Employee>(person => SelectedEmployee = person);
            DeleteEmployeeCommand = new Command(async () =>
                                    await DeleteEmployee(),
                                    () => SelectedEmployee != null);

            LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            if (IsBusy) return;
            IsBusy = true;
            StatusMessage = "Loading employee data...";
            try
            {
                // Fetch data from the EmployeeService
                var employees = await _employeeService.GetAllEmployeesAsync();

                // Clear existing data
                EmployeeList.Clear();

                // Add fetched employees to the ObservableCollection
                foreach (var employee in employees)
                {
                    EmployeeList.Add(employee);
                }

                // Set success status message and color
                StatusMessage = "Data loaded successfully!";
                StatusMessageColor = Colors.Green; // Green for success
            }
            catch (Exception ex)
            {
                // Set error status message and color
                StatusMessage = $"Failed to load data: {ex.Message}";
                StatusMessageColor = Colors.Red; // Red for failure
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddEmployee()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(NewEmployeeName) || string.IsNullOrWhiteSpace(NewEmployeeAddress) || string.IsNullOrWhiteSpace(NewEmployeeemail) || string.IsNullOrWhiteSpace(NewEmployeeContactNo))
            {
                StatusMessage = "Please fill in all fields before adding";
                return;
            }
            IsBusy = true;
            StatusMessage = "Adding new person...";

            try
            {
                var newPerson = new Employee
                {
                    Name = NewEmployeeName,
                    Address = NewEmployeeAddress,
                    email = NewEmployeeemail,
                    ContactNo = NewEmployeeContactNo
                };
                var isSuccess = await _employeeService.AddEmployeeAsync(newPerson);
                if (isSuccess)
                {
                    NewEmployeeName = string.Empty;
                    NewEmployeeAddress = string.Empty;
                    NewEmployeeemail = string.Empty;
                    NewEmployeeContactNo = string.Empty;
                    StatusMessage = "New person added successfully!";
                }
                else
                {
                    StatusMessage = "Failed to add the new person!";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed adding person: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
                await LoadDataAsync();
            }
        }

        private async Task DeleteEmployee()
        {
            if (SelectedEmployee == null) return;

            var answer = await Application.Current.MainPage.DisplayAlert(
                "Confirm Delete",
                $"Are you sure you want to delete {SelectedEmployee.Name}?",
                "Yes",
                "No");

            if (!answer) return;

            IsBusy = true;
            StatusMessage = "Deleting Employee...";
            try
            {
                var success = await _employeeService.DeleteEmployeeAsync(SelectedEmployee.EmployeeID);

                if (success)
                {
                    // Remove the deleted employee from the list and clear the selected employee
                    EmployeeList.Remove(SelectedEmployee);
                    SelectedEmployee = null;

                    // Clear entry fields
                    NewEmployeeName = string.Empty;
                    NewEmployeeAddress = string.Empty;
                    NewEmployeeemail = string.Empty;
                    NewEmployeeContactNo = string.Empty;

                    StatusMessage = "Employee deleted successfully";
                    StatusMessageColor = Colors.Green;
                }
                else
                {
                    StatusMessage = "Failed to delete employee.";
                    StatusMessageColor = Colors.Red;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting employee: {ex.Message}";
                StatusMessageColor = Colors.Red;
            }
            finally
            {
                IsBusy = false;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
