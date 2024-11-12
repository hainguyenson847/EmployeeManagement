
using Repositories;
using System.Windows;
using System.Windows.Controls;
using WPFApp.Models;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
namespace WPFApp
{
    public partial class MainWindow : Window
    {
        private readonly int employeeID;
        public BusinessObjects.LeaveRequest LoggedInEmployee { get; set; }
        public MainWindow(int employeeId)
        {
            InitializeComponent();
            employeeID = employeeId;
            LoadLeavesRequestByID();
        }

        private void Button_Click(object sender, RoutedEventArgs e)

        {
            AttendanceForm attendanceForm = new AttendanceForm(employeeID);
            attendanceForm.Show();
            this.Close();
        }
        public void LoadLeavesRequestByID()
        {
            LeaveRequestRepository leaveRepository = new LeaveRequestRepository();

            // Sử dụng trực tiếp employeeID để lấy danh sách yêu cầu nghỉ phép
            var leaveRequests = leaveRepository.GetLeaveRequestsByEmployeeID(employeeID);

            // Gán dữ liệu cho ViewLeaveRequest
            ViewLeaveRequest.ItemsSource = leaveRequests;

        }

        private void ViewLeaveRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewLeaveRequest.SelectedItem is BusinessObjects.LeaveRequest selectedLeave)
            {
                LeaveRequestRepository leaveRepository = new LeaveRequestRepository();
                int leaveRequest = selectedLeave.LeaveRequestId;
                BusinessObjects.LeaveRequest leaveRequestDetail = leaveRepository.getLeaveRequest(leaveRequest);
                if (leaveRequestDetail != null)
                {
                    contentTypeEdit.Text = leaveRequestDetail.LeaveType;
                    startDateEdit.SelectedDate = leaveRequestDetail.StartDate.ToDateTime(TimeOnly.MinValue);
                    endDateEdit.SelectedDate = leaveRequestDetail.EndDate.ToDateTime(TimeOnly.MinValue);

                }
            }
        }
        public bool EditLeaveRequest(int leaveRequestId, string newLeaveType, DateOnly newStartDate, DateOnly newEndDate)
        {
            using FuhrmContext _context = new FuhrmContext();
            var leaveRequest = _context.LeaveRequests.FirstOrDefault(lr => lr.LeaveRequestId == leaveRequestId);

            if (leaveRequest == null)
            {
                MessageBox.Show("Đơn nghỉ phép không tồn tại.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (leaveRequest.Status != "Pending")
            {
                MessageBox.Show("Chỉ có thể chỉnh sửa đơn nghỉ phép với trạng thái đang chờ xử lý.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Check for overlapping leave requests
            bool hasOverlap = _context.LeaveRequests.Any(lr =>
                lr.EmployeeId == leaveRequest.EmployeeId &&
                lr.LeaveRequestId != leaveRequestId &&
                lr.Status == "Pending" &&
                (
                    (newStartDate <= lr.EndDate && newStartDate >= lr.StartDate) ||
                    (newEndDate <= lr.EndDate && newEndDate >= lr.StartDate) ||
                    (newStartDate <= lr.StartDate && newEndDate >= lr.EndDate)
                )
            );

            if (hasOverlap)
            {
                MessageBox.Show("Không thể chỉnh sửa vì đã có đơn nghỉ phép khác trong khoảng thời gian này.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Update the properties
            leaveRequest.LeaveType = newLeaveType;
            leaveRequest.StartDate = newStartDate;
            leaveRequest.EndDate = newEndDate;
            _context.SaveChanges();
            return true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ViewLeaveRequest.SelectedItem is BusinessObjects.LeaveRequest selectedLeave && selectedLeave.Status == "Pending")
            {
                // Validate input
                if (startDateEdit.SelectedDate == null || endDateEdit.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày bắt đầu và ngày kết thúc hợp lệ.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (startDateEdit.SelectedDate > endDateEdit.SelectedDate)
                {
                    MessageBox.Show("Ngày bắt đầu không được lớn hơn ngày kết thúc.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                selectedLeave.LeaveType = contentTypeEdit.Text;
                selectedLeave.StartDate = DateOnly.FromDateTime(startDateEdit.SelectedDate.Value);
                selectedLeave.EndDate = DateOnly.FromDateTime(endDateEdit.SelectedDate.Value);
                bool result = EditLeaveRequest(selectedLeave.LeaveRequestId, selectedLeave.LeaveType, selectedLeave.StartDate, selectedLeave.EndDate);

                if (result)
                {
                    MessageBox.Show("Đơn nghỉ phép đã được cập nhật.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadLeavesRequestByID();
                    ClearEditform();
                }
            }
            else
            {
                MessageBox.Show("Chỉ có thể chỉnh sửa đơn nghỉ phép với trạng thái đang chờ xử lý.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                LoadLeavesRequestByID();
                ClearEditform();
            }
        }
        private void ClearEditform()
        {
            contentTypeEdit.Text = string.Empty;
            startDateEdit.SelectedDate = null;
            endDateEdit.SelectedDate = null;
        }
        private void DeleteLeaveRequest_Click(object sender, EventArgs e)
        {
            Button deleteButton = sender as Button;
            LeaveRequestRepository leaveRepository = new LeaveRequestRepository();
            if (deleteButton != null)
            {
                BusinessObjects.LeaveRequest selectedRequest = (BusinessObjects.LeaveRequest)deleteButton.DataContext;
                MessageBoxResult result = MessageBox.Show($"Are you sure you want to delete {selectedRequest.LeaveRequestId}?", "Confirm Delete", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        leaveRepository.RemoveLeaveRequest(selectedRequest);
                        LoadLeavesRequestByID();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ClearEditform();
        }
    }
}