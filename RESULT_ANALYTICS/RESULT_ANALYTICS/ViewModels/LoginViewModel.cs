using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using RESULT_ANALYTICS.Helpers;
using RESULT_ANALYTICS.Models;
using RESULT_ANALYTICS.Services;
using RESULT_ANALYTICS.DataAccess;

namespace RESULT_ANALYTICS.ViewModels
{
    internal class LoginViewModel : BindableBase
    {
        private readonly AuthService authService;
        private readonly TaiKhoanRepository taiKhoanRepo;

        private string tenDangNhap;
        private string thongBao;

        public string TenDangNhap
        {
            get { return tenDangNhap; }
            set
            {
                if (SetProperty(ref tenDangNhap, value))
                {
                    if (CoLoi)
                    {
                        ThongBao = "";
                    }
                }
            }
        }

        public string ThongBao
        {
            get { return thongBao; }
            set
            {
                if (SetProperty(ref thongBao, value))
                {
                    OnPropertyChanged(nameof(CoLoi));
                }
            }
        }

        public bool CoLoi
        {
            get { return !string.IsNullOrWhiteSpace(thongBao); }
        }

        public ICommand DangNhapCommand { get; private set; }

        public event Action<TaiKhoan> DangNhapThanhCong;

        public LoginViewModel()
        {
            authService = new AuthService();
            taiKhoanRepo = new TaiKhoanRepository();

            ThongBao = "";

            DangNhapCommand = new RelayCommand(ThucHienDangNhap);
        }

        private void ThucHienDangNhap(object parameter)
        {
            string matKhau = parameter as string;

            if (string.IsNullOrWhiteSpace(TenDangNhap) && string.IsNullOrEmpty(matKhau))
            {
                ThongBao = "Vui lòng nhập tên đăng nhập và mật khẩu!";
                return;
            }

            if (string.IsNullOrWhiteSpace(TenDangNhap))
            {
                ThongBao = "Vui lòng nhập tên đăng nhập!";
                return;
            }

            if (string.IsNullOrEmpty(matKhau))
            {
                ThongBao = "Vui lòng nhập mật khẩu!";
                return;
            }

            try
            {
                TaiKhoan taiKhoan = authService.DangNhap(TenDangNhap.Trim(), matKhau);

                if (taiKhoan == null)
                {
                    TaiKhoan tkTonTai = taiKhoanRepo.LayTheoTenDangNhap(TenDangNhap.Trim());
                    if (tkTonTai == null)
                    {
                        ThongBao = "Tài khoản không tồn tại trong hệ thống!";
                    }
                    else if (!tkTonTai.TrangThai)
                    {
                        ThongBao = "Tài khoản này hiện đang bị khóa!";
                    }
                    else
                    {
                        ThongBao = "Mật khẩu không chính xác! Vui lòng thử lại.";
                    }
                    return;
                }

                ThongBao = "";
                DangNhapThanhCong?.Invoke(taiKhoan);
            }
            catch (Exception ex)
            {
                ThongBao = "Lỗi kết nối SQL Server: " + ex.Message;
            }
        }
    }
}
