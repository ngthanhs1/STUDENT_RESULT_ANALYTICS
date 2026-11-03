using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using RESULT_ANALYTICS.ViewModels;

namespace RESULT_ANALYTICS.Views
{
    public partial class QuanLyTaiKhoanView : UserControl
    {
        public QuanLyTaiKhoanView()
        {
            InitializeComponent();
        }

        // ========================================
        // THEM TAI KHOAN
        // ========================================
        private void ThemTaiKhoan_Click(object sender, RoutedEventArgs e)
        {
            QuanLyTaiKhoanViewModel vm = DataContext as QuanLyTaiKhoanViewModel;
            if (vm == null) return;

            string matKhau = MatKhauThemBox.Password;

            if (vm.ThemTaiKhoanCommand.CanExecute(matKhau))
            {
                vm.ThemTaiKhoanCommand.Execute(matKhau);
            }

            MatKhauThemBox.Clear();
        }

        // ========================================
        // DAT LAI MAT KHAU
        // ========================================
        private void DatLaiMatKhau_Click(object sender, RoutedEventArgs e)
        {
            QuanLyTaiKhoanViewModel vm = DataContext as QuanLyTaiKhoanViewModel;
            if (vm == null) return;

            string matKhauMoi = MatKhauDatLaiBox.Password;

            if (vm.DoiMatKhauCommand.CanExecute(matKhauMoi))
            {
                vm.DoiMatKhauCommand.Execute(matKhauMoi);
            }

            MatKhauDatLaiBox.Clear();
        }
    }
}
