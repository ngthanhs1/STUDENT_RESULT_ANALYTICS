
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
    }
}
