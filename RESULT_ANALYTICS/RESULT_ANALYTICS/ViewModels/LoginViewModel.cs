
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using RESULT_ANALYTICS.Helpers;
using RESULT_ANALYTICS.Models;
using RESULT_ANALYTICS.Services;

namespace RESULT_ANALYTICS.ViewModels
{
    internal class LoginViewModel : BindableBase
    {
        private readonly AuthService authService;

        private string tenDangNhap;
        private string thongBao;

        // Ten dang nhap
        public string TenDangNhap
        {
            get { return tenDangNhap; }
            set { SetProperty(ref tenDangNhap, value); }
        }

        // Thong bao
        public string ThongBao
        {
            get { return thongBao; }
            set { SetProperty(ref thongBao, value); }
        }

        // Lenh dang nhap
        public ICommand DangNhapCommand { get; private set; }

        // Su kien dang nhap thanh cong
        public event Action<TaiKhoan> DangNhapThanhCong;

        // Ham khoi tao
        public LoginViewModel()
        {
            authService = new AuthService();

            ThongBao = "";

            DangNhapCommand = new RelayCommand(ThucHienDangNhap);
        }

        // Xu ly dang nhap
        private void ThucHienDangNhap(object parameter)
        {
            string matKhau = parameter as string;

            if (string.IsNullOrWhiteSpace(TenDangNhap) ||
                string.IsNullOrEmpty(matKhau))
            {
                ThongBao = "Vui lòng nhập đầy đủ thông tin!";
                return;
            }

            try
            {
                TaiKhoan taiKhoan = authService.DangNhap(
                    TenDangNhap.Trim(), matKhau);

                if (taiKhoan == null)
                {
                    ThongBao = "Tài khoản hoặc mật khẩu không đúng!";
                    return;
                }

                ThongBao = "";

                DangNhapThanhCong?.Invoke(taiKhoan);
            }
            catch (Exception)
            {
                ThongBao = "Lỗi kết nối hoặc xác thực SQL Server!";
            }
        }
    }
}
