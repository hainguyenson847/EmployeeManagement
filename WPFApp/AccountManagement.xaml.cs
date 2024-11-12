using BusinessObjects;
using Repositories;
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

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for AccountManagement.xaml
    /// </summary>
    public partial class AccountManagement : Window
    {
        AccountRepository accountRepository;
        EmployeeRepository employeeRepository;
        SalaryRepository salaryRepository;

        public AccountManagement()
        {
            InitializeComponent();
            accountRepository = new AccountRepository();
            employeeRepository = new EmployeeRepository();
            LoadAccounts();
            LoadRoles();
        }

        private void LoadRoles()
        {
            try
            {
                var roles = accountRepository.GetRoles();
                RoleCombobox.ItemsSource = roles;
                RoleCombobox.DisplayMemberPath = "RoleName";
                RoleCombobox.SelectedValuePath = "RoleId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading roles: {ex.Message}");
            }
            finally
            {
                ResetForm();
            }
        }

        public void LoadAccounts()
        {
            try
            {
                var accounts = accountRepository.GetAccounts();
                AccountDataGrid.ItemsSource = accounts;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading roles: {ex.Message}");
            }
            finally
            {
                ResetForm();
            }
        }

        private void AccountDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(AccountDataGrid.SelectedItem is Account selectedAccount)
            {
                Account a = accountRepository.GetAccountById(selectedAccount.AccountId);
                DisplayAccountDetails(a);
            }
        }

        private void DisplayAccountDetails(Account account)
        {
            AccountIdTextBox.Text = account.AccountId.ToString();
            UsernameTextBox.Text = account.Username;
            PasswordTextBox.Text = account.Password;
            RoleCombobox.SelectedValue = account.Role.RoleId;
        }

        private void ResetForm()
        {
            AccountIdTextBox.Text = "";
            UsernameTextBox.Text = "";
            PasswordTextBox.Text = "";
            RoleCombobox.SelectedValue = 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == "" || PasswordTextBox.Text == "" || RoleCombobox.SelectedValue == null)
            {
                MessageBox.Show("Please fill in all fields");
            }
            else
            {
                
                if (accountRepository.GetAccountByUserNameEx(UsernameTextBox.Text) != null)
                {
                    MessageBox.Show("Username already exists");
                    return;
                }
                else
                {
                    Account account = new Account
                    {
                        Username = UsernameTextBox.Text,
                        Password = PasswordTextBox.Text,
                        RoleId = (int)RoleCombobox.SelectedValue
                    };
                    accountRepository.AddAccount(account);
                    if (account.RoleId == 2)
                    {

                        Employee employee = new Employee
                        {
                            FullName = "Default Name", 
                            DateOfBirth = new DateTime(2000, 1, 1), 
                            Gender = "Not Specified", 
                            Address = "Default Address", 
                            PhoneNumber = "000-000-0000", 
                            DepartmentId = 1, 
                            PositionId = 1, 
                            Salary = 0, 
                            StartDate = DateTime.Now,
                            AccountId = account.AccountId 
                        };

                        employeeRepository.AddEmployee(employee);


                    }
                    LoadAccounts();
                }
            }
        }


        private void EditAccount_Click(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == "" || PasswordTextBox.Text == "" || RoleCombobox.SelectedValue == null)
            {
                MessageBox.Show("Please fill in all fields");
            }
            else
            {
                var u = accountRepository.GetAccountById(Int32.Parse(AccountIdTextBox.Text));
                var u2 = accountRepository.GetAccountByUserNameEx(UsernameTextBox.Text);
                if (u2 != null)
                {
                    MessageBox.Show("Username already exists");
                    return;
                }
                else
                {
                    Account account = new Account
                    {
                        AccountId = int.Parse(AccountIdTextBox.Text),
                        Username = UsernameTextBox.Text,
                        Password = PasswordTextBox.Text,
                        RoleId = (int)RoleCombobox.SelectedValue
                    };
                    accountRepository.UpdateAccount(account);
                    LoadAccounts();
                }

            }
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            if (AccountIdTextBox.Text == "")
            {
                MessageBox.Show("Cannot delete without account id");
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Are you sure to delete?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    int accountId = int.Parse(AccountIdTextBox.Text);
                    var employee = employeeRepository.GetEmployeeById(accountId);
                    if (employee != null)
                    {
                        var salary = salaryRepository.GetSalaryByEmployeeId(employee.EmployeeId);
                        if (salary != null)
                        {
                            salaryRepository.DeleteSalary(salary);
                        }
                        employeeRepository.DeleteEmployee(employee.EmployeeId);
                    }
                    accountRepository.DeleteAccount(accountId);
                    LoadAccounts();
                }
            }
        }

        private void NavigateButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                Window currentWindow = Window.GetWindow(this);

                switch (button.Content.ToString())
                {
                    // Uncomment and implement the HomeView navigation if needed
                    case "Trang Chủ":
                        var homeView = new AdminDashboard();
                        homeView.Show();
                        currentWindow.Close();
                        break;
                    case "Tài khoản":
                        var account = new AccountManagement();
                        account.Show();
                        currentWindow.Close();
                        break;
                    case "Nhân viên":
                        var employeeView = new EmployeeWindow();
                        employeeView.Show();
                        currentWindow.Close();
                        break;
                    case "Bộ phận":
                        var departmentView = new DepartmentManagement();
                        departmentView.Show();
                        currentWindow.Close();
                        break;
                    case "Vị trí":
                        var position = new PositionManagement();
                        position.Show();
                        currentWindow.Close();
                        break;
                    case "Chấm công":
                        var attendanceView = new AttendanceView();
                        attendanceView.Show();
                        currentWindow.Close();
                        break;
                    case "Bảng lương":
                        var salaryView = new SalaryView();
                        salaryView.Show();
                        currentWindow.Close();
                        break;
                    case "Nghỉ phép":
                        var leaveView = new LeaveRequestView();
                        leaveView.Show();
                        currentWindow.Close();
                        break;
                    default:
                        break;
                }
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear the session
            SessionManager.CurrentAccount = null;

            // Redirect to login screen
            LoginScreen loginScreen = new LoginScreen();
            loginScreen.Show();

            // Close the current window
            Window.GetWindow(this).Close();
        }

    }
}
