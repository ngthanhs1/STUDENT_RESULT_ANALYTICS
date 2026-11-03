using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RESULT_ANALYTICS.Helpers;
using RESULT_ANALYTICS.Models;
using RESULT_ANALYTICS.ViewModels;

namespace RESULT_ANALYTICS.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel viewModel;

        public LoginWindow()
        {
            InitializeComponent();

            viewModel = new LoginViewModel();

            DataContext = viewModel;

            viewModel.DangNhapThanhCong += XuLyDangNhapThanhCong;

            Loaded += (s, e) => txtTenDangNhap.Focus();
        }

        private void BtnDangNhap_Click(object sender, RoutedEventArgs e)
        {
            string matKhau = txtMatKhau.Password;

            viewModel.DangNhapCommand.Execute(matKhau);

            if (viewModel.CoLoi)
            {
                txtMatKhau.SelectAll();
                txtMatKhau.Focus();
            }
        }

        private void TxtTenDangNhap_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                txtMatKhau.Focus();
            }
        }

        private void TxtMatKhau_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnDangNhap_Click(sender, e);
            }
        }

        private void XuLyDangNhapThanhCong(TaiKhoan taiKhoan)
        {
            // THIET LAP PHIEN DANG NHAP TRUOC KHI TAO MAINWINDOW
            // Giup cac ViewModel con (nhu QuanLyTaiKhoanViewModel) nhan dung quyen LaAdmin ngay lap tuc
            PhienDangNhap.KhoiTao(taiKhoan.TenDangNhap, taiKhoan.VaiTro);

            MainWindow mainWindow = new MainWindow();

            mainWindow.HienThiTaiKhoan(
                taiKhoan.TenDangNhap,
                taiKhoan.VaiTro);

            Application.Current.MainWindow = mainWindow;

            mainWindow.Show();

            this.Close();
        }

        private void Thoat_Click(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void QuenMatKhau_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(
                "Để cấp lại mật khẩu hoặc mở khóa tài khoản, vui lòng liên hệ Ban Quản trị Hệ thống Đào tạo (Admin).",
                "Quên mật khẩu",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void TroGiup_Click(object sender, MouseButtonEventArgs e)
        {
            string msg = "HƯỚNG DẪN ĐĂNG NHẬP HỆ THỐNG:\n\n" +
                         "1. Sử dụng tài khoản và mật khẩu đã được phân quyền bởi Quản trị viên (Admin).\n" +
                         "2. Chú ý kiểm tra phím Caps Lock và bộ gõ Tiếng Việt khi nhập mật khẩu.\n" +
                         "3. Nếu quên mật khẩu, tài khoản bị khóa hoặc cần cấp mới tài khoản, vui lòng liên hệ Ban Quản trị Hệ thống để được hỗ trợ.";
            MessageBox.Show(
                msg,
                "Trợ giúp đăng nhập",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
    }
}
