using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RESULT_ANALYTICS.Helpers
{
    internal static class PhienDangNhap
    {
        public static string TenDangNhap { get; private set; }
        public static string VaiTro { get; private set; }

        // Kiem tra tai khoan da dang nhap
        public static bool DaDangNhap
        {
            get { return !string.IsNullOrWhiteSpace(TenDangNhap); }
        }

        // Kiem tra quyen Admin
        public static bool LaAdmin
        {
            get
            {
                return DaDangNhap &&
                    string.Equals(VaiTro, "Admin",
                        StringComparison.OrdinalIgnoreCase);
            }
        }

        // Luu thong tin khi dang nhap thanh cong
        public static void KhoiTao(string tenDangNhap, string vaiTro)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap))
                throw new ArgumentException("Tên đăng nhập không hợp lệ!");

            TenDangNhap = tenDangNhap.Trim();
            VaiTro = vaiTro;
        }

        // Xoa phien khi dang xuat
        public static void KetThuc()
        {
            TenDangNhap = null;
            VaiTro = null;
        }
    }
}

