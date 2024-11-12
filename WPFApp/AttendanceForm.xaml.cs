using DataAccessObjects;
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
using BusinessObjects;
using Repositories;
using Microsoft.Identity.Client;
namespace WPFApp
{
    /// <summary>
    /// Interaction logic for AttendanceForm.xaml
    /// </summary>
    public partial class AttendanceForm : Window
    {
        private readonly int _employeeID;
        public AttendanceForm(int employeeID)
        {
            InitializeComponent();
            _employeeID = employeeID;
        }

        private void btnSubmitLeaveRequest_Click(object sender, RoutedEventArgs e)
        {
            if (StartDate.SelectedDate == null || EndDate.SelectedDate == null)
            {
                MessageBox.Show("Vui lòng chọn ngày bắt đầu và ngày kết thúc hợp lệ.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (StartDate.SelectedDate > EndDate.SelectedDate)
            {
                MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var leaveRequestRepo = new LeaveRequestRepository();

            if (_employeeID != 0)
            {
                DateOnly startDate = DateOnly.FromDateTime(StartDate.SelectedDate.Value);
                DateOnly endDate = DateOnly.FromDateTime(EndDate.SelectedDate.Value);

                bool hasOverlap = leaveRequestRepo.GetLeaveRequestsByEmployeeID(_employeeID).Any(lr =>
                    lr.Status == "Pending" &&
                    (
                        (startDate <= lr.EndDate && startDate >= lr.StartDate) ||
                        (endDate <= lr.EndDate && endDate >= lr.StartDate) ||
                        (startDate <= lr.StartDate && endDate >= lr.EndDate)
                    )
                );

                if (hasOverlap)
                {
                    MessageBox.Show("Không thể tạo yêu cầu nghỉ phép vì đã có đơn nghỉ phép khác trong khoảng thời gian này.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                var leaveRequest = new LeaveRequest
                {
                    EmployeeId = _employeeID,
                    LeaveType = LeaveType.Text,
                    StartDate = startDate,
                    EndDate = endDate,
                    Status = "Pending"
                };

                leaveRequestRepo.AddLeaveRequest(leaveRequest);

                MessageBox.Show("Yêu cầu nghỉ phép đã được gửi thành công!");
                MainWindow mainWindow = new MainWindow(_employeeID);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Không tìm thấy thông tin nhân viên cho tài khoản hiện tại.");
            }
        }


    }

}
