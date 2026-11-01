
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RESULT_ANALYTICS.Models;
using RESULT_ANALYTICS.DataAccess;
using RESULT_ANALYTICS.Helpers;

namespace RESULT_ANALYTICS.Services
{
    internal class AuthService
    {
        private readonly TaiKhoanRepository repository;

        // Ham khoi tao
        public AuthService()
        {
            repository = new TaiKhoanRepository();
        }

        // Ham xu ly dang nhap
        public TaiKhoan DangNhap(
            string tenDangNhap,
            string matKhau)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap) ||
                string.IsNullOrEmpty(matKhau))
            {
                return null;
            }

            TaiKhoan taiKhoan =
                repository.LayTheoTenDangNhap(
                    tenDangNhap.Trim());

            // Tai khoan khong ton tai
            if (taiKhoan == null)
            {
                return null;
            }

            // Tai khoan da bi khoa
            if (!taiKhoan.TrangThai)
            {
                return null;
            }

            // Kiem tra mat khau PBKDF2
            bool hopLe = PasswordHasher.VerifyPassword(
                matKhau,
                taiKhoan.MatKhauHash);

            if (!hopLe)
            {
                return null;
            }

            return taiKhoan;
        }
    }
}
