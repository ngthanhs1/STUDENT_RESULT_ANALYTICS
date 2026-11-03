
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using RESULT_ANALYTICS.ViewModels;

namespace RESULT_ANALYTICS.Views
{
    public partial class HeThongImportView : UserControl
    {
        public HeThongImportView()
        {
            InitializeComponent();

            // Chi khoi tao ViewModel khi chay ung dung
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DataContext = new HeThongImportViewModel();
            }
        }

        // ========================================
        // THEM TAI KHOAN
        // ========================================
        private void ThemTaiKhoan_Click(
            object sender, RoutedEventArgs e)
        {
            HeThongImportViewModel vm =
                DataContext as HeThongImportViewModel;

            if (vm == null || vm.TaiKhoanVM == null)
                return;

            string matKhau = MatKhauThemBox.Password;

            if (vm.TaiKhoanVM.ThemTaiKhoanCommand.CanExecute(matKhau))
            {
                vm.TaiKhoanVM.ThemTaiKhoanCommand.Execute(matKhau);
            }

            // Xoa mat khau khoi PasswordBox
            MatKhauThemBox.Clear();
        }

        // ========================================
        // DAT LAI MAT KHAU
        // ========================================
        private void DatLaiMatKhau_Click(
            object sender, RoutedEventArgs e)
        {
            HeThongImportViewModel vm =
                DataContext as HeThongImportViewModel;

            if (vm == null || vm.TaiKhoanVM == null)
                return;

            string matKhauMoi = MatKhauDatLaiBox.Password;

            if (vm.TaiKhoanVM.DoiMatKhauCommand.CanExecute(matKhauMoi))
            {
                vm.TaiKhoanVM.DoiMatKhauCommand.Execute(matKhauMoi);
            }

            // Xoa mat khau khoi PasswordBox
            MatKhauDatLaiBox.Clear();
        }
    }
}
