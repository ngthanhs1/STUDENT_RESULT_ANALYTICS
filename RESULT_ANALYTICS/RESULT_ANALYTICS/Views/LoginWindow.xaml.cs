
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RESULT_ANALYTICS.Models;
using RESULT_ANALYTICS.ViewModels;

namespace RESULT_ANALYTICS.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel viewModel;

        // Ham khoi tao
        public LoginWindow()
        {
            InitializeComponent();

            viewModel = new LoginViewModel();

            DataContext = viewModel;

            viewModel.DangNhapThanhCong += XuLyDangNhapThanhCong;

            Loaded += (s, e) => txtTenDangNhap.Focus();
        }

        // Nut dang nhap
        private void BtnDangNhap_Click(object sender, RoutedEventArgs e)
        {
            string matKhau = txtMatKhau.Password;

            txtMatKhau.Clear();

            viewModel.DangNhapCommand.Execute(matKhau);
        }

        // Nhan Enter de dang nhap
        private void TxtMatKhau_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnDangNhap_Click(sender, e);
            }
        }

        // Xu ly dang nhap thanh cong
        private void XuLyDangNhapThanhCong(TaiKhoan taiKhoan)
        {
            MainWindow mainWindow = new MainWindow();

            mainWindow.HienThiTaiKhoan(
                taiKhoan.TenDangNhap,
                taiKhoan.VaiTro);

            Application.Current.MainWindow = mainWindow;

            mainWindow.Show();

            this.Close();
        }

        // Nut thoat
        private void BtnThoat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
