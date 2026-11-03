
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using RESULT_ANALYTICS.DataAccess;
using RESULT_ANALYTICS.Helpers;
using RESULT_ANALYTICS.Models;

namespace RESULT_ANALYTICS.ViewModels
{
    internal class QuanLyTaiKhoanViewModel : BindableBase
    {
        private readonly QuanLyTaiKhoanRepository repository;

        // =====================================
        // 1. DANH SACH TAI KHOAN
        // =====================================

        public ObservableCollection<TaiKhoanQuanLy> DanhSachTaiKhoan
        {
            get;
            private set;
        }

        public string[] DanhSachVaiTro { get; private set; }

        // =====================================
        // 2. TAI KHOAN DUOC CHON
        // =====================================

        private TaiKhoanQuanLy taiKhoanDuocChon;

        public TaiKhoanQuanLy TaiKhoanDuocChon
        {
            get { return taiKhoanDuocChon; }
            set
            {
                if (SetProperty(ref taiKhoanDuocChon, value))
                {
                    if (value != null)
                    {
                        TenDangNhapNhap = value.TenDangNhap;
                        VaiTroNhap = value.VaiTro;
                        TrangThaiNhap = value.TrangThai;
                    }
                }
            }
        }

        // =====================================
        // 3. THONG TIN NHAP
        // =====================================

        private string tenDangNhapNhap;

        public string TenDangNhapNhap
        {
            get { return tenDangNhapNhap; }
            set { SetProperty(ref tenDangNhapNhap, value); }
        }

        private string vaiTroNhap;

        public string VaiTroNhap
        {
            get { return vaiTroNhap; }
            set { SetProperty(ref vaiTroNhap, value); }
        }

        private bool trangThaiNhap;

        public bool TrangThaiNhap
        {
            get { return trangThaiNhap; }
            set { SetProperty(ref trangThaiNhap, value); }
        }

        private string thongBao;

        public string ThongBao
        {
            get { return thongBao; }
            set { SetProperty(ref thongBao, value); }
        }

        // =====================================
        // 4. QUYEN HAN
        // =====================================

        public bool LaAdmin
        {
            get { return PhienDangNhap.LaAdmin; }
        }

        // =====================================
        // 5. COMMAND
        // =====================================

        public ICommand ThemTaiKhoanCommand { get; private set; }
        public ICommand CapNhatTaiKhoanCommand { get; private set; }
        public ICommand DoiMatKhauCommand { get; private set; }
        public ICommand LamTrongCommand { get; private set; }
        public ICommand LamMoiCommand { get; private set; }

        // =====================================
        // 6. KHOI TAO
        // =====================================

        public QuanLyTaiKhoanViewModel()
        {
            repository = new QuanLyTaiKhoanRepository();

            DanhSachTaiKhoan =
                new ObservableCollection<TaiKhoanQuanLy>();

            DanhSachVaiTro = new string[]
            {
                "Admin",
                "NhanVien"
            };

            VaiTroNhap = "NhanVien";
            TrangThaiNhap = true;
            ThongBao = "";

            // Mat khau se duoc truyen tu PasswordBox
            // qua CommandParameter khi lam giao dien
            ThemTaiKhoanCommand =
                new RelayCommand(p => ThemTaiKhoan(p as string));

            CapNhatTaiKhoanCommand =
                new RelayCommand(p => CapNhatTaiKhoan());

            DoiMatKhauCommand =
                new RelayCommand(p => DoiMatKhau(p as string));

            LamTrongCommand =
                new RelayCommand(p => LamTrong());

            LamMoiCommand =
                new RelayCommand(p => LamMoi());

            if (LaAdmin)
            {
                try
                {
                    TaiDanhSach();
                }
                catch (Exception ex)
                {
                    BaoLoi(ex);
                }
            }
            else
            {
                ThongBao = "Chỉ Admin được quản lý tài khoản!";
            }
        }

        // =====================================
        // 7. KIEM TRA QUYEN ADMIN
        // =====================================

        private void KiemTraQuyen()
        {
            if (!PhienDangNhap.LaAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Bạn không có quyền quản lý tài khoản!");
            }
        }

        // =====================================
        // 8. TAI DANH SACH TAI KHOAN
        // =====================================

        private void TaiDanhSach()
        {
            KiemTraQuyen();

            DanhSachTaiKhoan.Clear();

            foreach (TaiKhoanQuanLy tk in repository.LayDanhSach())
            {
                DanhSachTaiKhoan.Add(tk);
            }
        }

        // =====================================
        // 9. THEM TAI KHOAN
        // =====================================

        private void ThemTaiKhoan(string matKhau)
        {
            try
            {
                KiemTraQuyen();

                if (string.IsNullOrWhiteSpace(TenDangNhapNhap))
                    throw new ArgumentException(
                        "Vui lòng nhập tên đăng nhập!");

                if (string.IsNullOrEmpty(matKhau) ||
                    matKhau.Length < 8)
                    throw new ArgumentException(
                        "Mật khẩu phải có ít nhất 8 ký tự!");

                // Su dung PasswordHasher hien co
                string matKhauHash =
                    PasswordHasher.HashPassword(matKhau);

                bool thanhCong = repository.Them(
                    TenDangNhapNhap.Trim(),
                    matKhauHash,
                    VaiTroNhap);

                if (thanhCong)
                {
                    LamTrong();
                    TaiDanhSach();
                    ThongBao = "Thêm tài khoản thành công!";
                }
                else
                {
                    ThongBao = "Tên đăng nhập đã tồn tại!";
                }
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // =====================================
        // 10. CAP NHAT TAI KHOAN
        // =====================================

        private void CapNhatTaiKhoan()
        {
            try
            {
                KiemTraQuyen();

                if (TaiKhoanDuocChon == null)
                    throw new ArgumentException(
                        "Vui lòng chọn tài khoản cần sửa!");

                // Khong cho tai khoan dang su dung
                // tu khoa minh hoac tu ha quyen
                bool laTaiKhoanHienTai =
                    string.Equals(
                        TaiKhoanDuocChon.TenDangNhap,
                        PhienDangNhap.TenDangNhap,
                        StringComparison.OrdinalIgnoreCase);

                bool vanLaAdminHoatDong =
                    string.Equals(
                        VaiTroNhap,
                        "Admin",
                        StringComparison.OrdinalIgnoreCase)
                    && TrangThaiNhap;

                if (laTaiKhoanHienTai && !vanLaAdminHoatDong)
                {
                    throw new InvalidOperationException(
                        "Không thể tự khóa hoặc hạ quyền tài khoản đang đăng nhập!");
                }

                bool thanhCong = repository.CapNhat(
                    TaiKhoanDuocChon.MaTaiKhoan,
                    VaiTroNhap,
                    TrangThaiNhap);

                if (thanhCong)
                {
                    LamTrong();
                    TaiDanhSach();
                    ThongBao = "Cập nhật tài khoản thành công!";
                }
                else
                {
                    ThongBao = "Không tìm thấy tài khoản!";
                }
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // =====================================
        // 11. DAT LAI MAT KHAU
        // =====================================

        private void DoiMatKhau(string matKhauMoi)
        {
            try
            {
                KiemTraQuyen();

                if (TaiKhoanDuocChon == null)
                    throw new ArgumentException(
                        "Vui lòng chọn tài khoản cần đặt lại mật khẩu!");

                if (string.IsNullOrEmpty(matKhauMoi) ||
                    matKhauMoi.Length < 8)
                    throw new ArgumentException(
                        "Mật khẩu mới phải có ít nhất 8 ký tự!");

                string hashMoi =
                    PasswordHasher.HashPassword(matKhauMoi);

                bool thanhCong = repository.DoiMatKhau(
                    TaiKhoanDuocChon.MaTaiKhoan,
                    hashMoi);

                ThongBao = thanhCong
                    ? "Đặt lại mật khẩu thành công!"
                    : "Không tìm thấy tài khoản!";
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // =====================================
        // 12. LAM TRONG
        // =====================================

        private void LamTrong()
        {
            TaiKhoanDuocChon = null;
            TenDangNhapNhap = "";
            VaiTroNhap = "NhanVien";
            TrangThaiNhap = true;
            ThongBao = "";
        }

        // =====================================
        // 13. LAM MOI
        // =====================================

        private void LamMoi()
        {
            try
            {
                KiemTraQuyen();

                LamTrong();
                TaiDanhSach();

                ThongBao = "Đã tải lại danh sách tài khoản!";
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // =====================================
        // 14. XU LY LOI
        // =====================================

        private void BaoLoi(Exception ex)
        {
            string noiDung = ex.Message;

            SqlException loiSql = ex as SqlException;

            if (loiSql != null)
            {
                if (loiSql.Number == 2627 ||
                    loiSql.Number == 2601)
                {
                    noiDung = "Tên đăng nhập đã tồn tại!";
                }
                else if (loiSql.Number == 547)
                {
                    noiDung = "Dữ liệu tài khoản đang được liên kết!";
                }
            }

            ThongBao = noiDung;

            MessageBox.Show(
                noiDung,
                "Quản lý tài khoản",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}
