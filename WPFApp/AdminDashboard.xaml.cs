using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using OxyPlot.Axes;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WPFApp.Models;

namespace WPFApp
{
    public partial class AdminDashboard : Window
    {
        private readonly FuhrmContext _context = new FuhrmContext();

        public AdminDashboard()
        {
            InitializeComponent();
            LoadDashboardData();
        }

        private void LoadDashboardData()
        {
            LoadDepartmentBarChart();
            LoadAttendanceBarChart(); // Updated method name
        }

        private void LoadDepartmentBarChart()
        {
            var plotModel = new PlotModel { Title = "Số Nhân Viên Theo Phòng Ban" };

            var barSeries = new BarSeries
            {
                Title = "Nhân Viên",
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0}",
                IsStacked = false // Ensures bars are independent, not stacked
            };

            // Querying department data
            var employeesByDepartment = _context.Employees
                .GroupBy(e => e.Department.DepartmentName)
                .Select(g => new { DepartmentName = g.Key, Count = g.Count() })
                .ToList();

            // Adding data points
            foreach (var dept in employeesByDepartment)
            {
                barSeries.Items.Add(new BarItem { Value = dept.Count });
            }

            // Adjusting the CategoryAxis for vertical alignment
            plotModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Left, // Places department names on the left for a vertical layout
                ItemsSource = employeesByDepartment.Select(d => d.DepartmentName).ToList(),
                Title = "Phòng Ban"
            });

            // Adding a LinearAxis for the number of employees
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom, // Values appear on the bottom, in horizontal orientation
                Title = "Số Nhân Viên",
                MinimumPadding = 0.1,
                MaximumPadding = 0.1
            });

            plotModel.Series.Add(barSeries);
            DepartmentBarChart.Model = plotModel;
        }

        private void LoadAttendanceBarChart()
        {
            var plotModel = new PlotModel { Title = "Thống Kê Điểm Danh Hàng Tháng" };

            var barSeries = new BarSeries
            {
                Title = "Điểm Danh",
                LabelPlacement = LabelPlacement.Outside,
                LabelFormatString = "{0}",
                IsStacked = false // Ensures bars are independent, not stacked
            };

            var attendanceData = _context.Attendances
                .GroupBy(a => a.Date.Month)
                .Select(g => new { Month = g.Key, Count = g.Count(a => a.Status == "Present") })
                .OrderBy(a => a.Month)
                .ToList();

            // Adding data points
            foreach (var data in attendanceData)
            {
                barSeries.Items.Add(new BarItem { Value = data.Count });
            }

            // Adjusting the CategoryAxis for vertical alignment
            plotModel.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Left, // Places month names on the left for a vertical layout
                ItemsSource = attendanceData.Select(a => $"Tháng {a.Month}").ToList(),
                Title = "Tháng"
            });

            // Adding a LinearAxis for the number of attendances
            plotModel.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom, // Values appear on the bottom, in horizontal orientation
                Title = "Số Lần Điểm Danh",
                MinimumPadding = 0.1,
                MaximumPadding = 0.1
            });

            plotModel.Series.Add(barSeries);
            AttendanceBarChart.Model = plotModel;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.CurrentAccount = null;
            var loginScreen = new LoginScreen();
            loginScreen.Show();
            Window.GetWindow(this).Close();
        }
    }
}
