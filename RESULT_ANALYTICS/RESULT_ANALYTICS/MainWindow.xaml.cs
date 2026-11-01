
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

namespace RESULT_ANALYTICS
{
    public partial class MainWindow : Window
    {
        // Ham khoi tao
        public MainWindow()
        {
            InitializeComponent();
        }

        // Hien thi thong tin tai khoan
        public void HienThiTaiKhoan(
            string tenDangNhap,
            string vaiTro)
        {
            txtNguoiDung.Text =
                "Người đăng nhập: " + tenDangNhap;

            txtVaiTro.Text =
                "Vai trò: " + vaiTro;
        }
    }
}
