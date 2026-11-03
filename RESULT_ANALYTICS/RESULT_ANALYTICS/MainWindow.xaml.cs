using System.Windows.Media.Animation;
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
using RESULT_ANALYTICS.ViewModels;
using RESULT_ANALYTICS.Helpers;

namespace RESULT_ANALYTICS
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel viewModel;

        // Ham khoi tao
        public MainWindow()
        {
            InitializeComponent();

            viewModel = new MainViewModel();

            DataContext = viewModel;
        }

        // Nhan thong tin dang nhap tu LoginWindow
        public void HienThiTaiKhoan(string tenDangNhap, string vaiTro)
        {
            viewModel.GanTaiKhoan(tenDangNhap, vaiTro);
        }

        // Xu ly nut 3 gach (Hamburger) thu gon / mo rong thanh ben
        private void ToggleSidebar_Click(object sender, RoutedEventArgs e)
        {
            bool thuGonMoi = !viewModel.ThanhBenThuGon;
            viewModel.ThanhBenThuGon = thuGonMoi;

            double targetWidth = thuGonMoi ? 60.0 : 230.0;
            DoubleAnimation anim = new DoubleAnimation
            {
                To = targetWidth,
                Duration = TimeSpan.FromMilliseconds(180),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            anim.Completed += (s, ev) =>
            {
                SidebarBorder.BeginAnimation(FrameworkElement.WidthProperty, null);
                SidebarBorder.Width = targetWidth;
            };
            SidebarBorder.BeginAnimation(FrameworkElement.WidthProperty, anim);
        }

        private void DangXuat_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult xacNhan = MessageBox.Show(
                "Bạn có chắc chắn muốn đăng xuất?",
                "Xác nhận đăng xuất",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (xacNhan != MessageBoxResult.Yes)
                return;

            try
            {
                // 1. Ket thuc phien dang nhap hien tai
                PhienDangNhap.KetThuc();

                // 2. Mo lai man hinh dang nhap
                Views.LoginWindow loginWindow = new Views.LoginWindow();
                Application.Current.MainWindow = loginWindow;
                loginWindow.Show();

                // 3. Dong cua so chinh hien tai
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(
                    "Không thể đăng xuất: " + ex.Message,
                    "Lỗi đăng xuất",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

    }
}
