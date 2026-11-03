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
        // 1. DANH SACH TAI KHOAN & KPI
        // =====================================

        public ObservableCollection<TaiKhoanQuanLy> DanhSachTaiKhoan
        {
            get;
            private set;
        }

        public string[] DanhSachVaiTro { get; private set; }

        public int TongTaiKhoan
        {
            get { return DanhSachTaiKhoan.Count; }
        }

        public int SoTaiKhoanHoatDong
        {
            get
            {
                int count = 0;
                foreach (var tk in DanhSachTaiKhoan)
                {
                    if (tk.TrangThai) count++;
                }
                return count;
            }
        }

        public int SoTaiKhoanBiKhoa
        {
            get
            {
                int count = 0;
                foreach (var tk in DanhSachTaiKhoan)
                {
                    if (!tk.TrangThai) count++;
                }
                return count;
            }
        }

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
        // 3. THONG TIN NHAP & TRANG THAI KHOA / MO
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
            set
            {
                if (SetProperty(ref trangThaiNhap, value))
                {
                    OnPropertyChanged(nameof(TrangThaiMoKhoa));
                    OnPropertyChanged(nameof(TrangThaiKhoa));
                    OnPropertyChanged(nameof(ChuThichTrangThai));
                }
            }
        }

        // Ho tro chon truc tiep bang 2 nut Mo khoa / Khoa (Segmented control)
        public bool TrangThaiMoKhoa
        {
            get { return trangThaiNhap; }
            set
            {
                if (value)
                {
                    TrangThaiNhap = true;
                }
            }
        }

        public bool TrangThaiKhoa
        {
            get { return !trangThaiNhap; }
            set
            {
                if (value)
                {
                    TrangThaiNhap = false;
                }
            }
        }

        public string ChuThichTrangThai
        {
            get
            {
                return trangThaiNhap
                    ? "Trạng thái: Đang MỞ KHÓA (Tài khoản được phép đăng nhập bình thường)"
                    : "Trạng thái: Đang BỊ KHÓA (Tài khoản bị chặn đăng nhập vào hệ thống)";
            }
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
        public ICommand ChuyenTrangThaiKhoaNhanhCommand { get; private set; }
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

            ThemTaiKhoanCommand =
                new RelayCommand(p => ThemTaiKhoan(p as string));

            CapNhatTaiKhoanCommand =
                new RelayCommand(p => CapNhatTaiKhoan());

            DoiMatKhauCommand =
                new RelayCommand(p => DoiMatKhau(p as string));

            ChuyenTrangThaiKhoaNhanhCommand =
                new RelayCommand(p => ChuyenTrangThaiKhoaNhanh());

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

            CapNhatKpi();
        }

        private void CapNhatKpi()
        {
            OnPropertyChanged(nameof(TongTaiKhoan));
            OnPropertyChanged(nameof(SoTaiKhoanHoatDong));
            OnPropertyChanged(nameof(SoTaiKhoanBiKhoa));
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
                    matKhau.Length < 6)
                    throw new ArgumentException(
                        "Mật khẩu phải có ít nhất 6 ký tự!");

                string matKhauHash =
                    PasswordHasher.HashPassword(matKhau);

                bool thanhCong = repository.Them(
                    TenDangNhapNhap.Trim(),
                    matKhauHash,
                    VaiTroNhap);

                if (thanhCong)
                {
                    if (!TrangThaiNhap)
                    {
                        TaiKhoanQuanLy vuaThem = repository.LayDanhSach()
                            .Find(x => x.TenDangNhap.Equals(TenDangNhapNhap.Trim(), StringComparison.OrdinalIgnoreCase));
                        if (vuaThem != null)
                        {
                            repository.CapNhat(vuaThem.MaTaiKhoan, VaiTroNhap, false);
                        }
                    }

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
                        "Vui lòng chọn tài khoản cần sửa trong bảng bên dưới!");

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
        // 11. CHUYEN TRANG THAI KHOA / MO NHANH
        // =====================================

        private void ChuyenTrangThaiKhoaNhanh()
        {
            try
            {
                KiemTraQuyen();

                if (TaiKhoanDuocChon == null)
                    throw new ArgumentException("Vui lòng chọn một tài khoản trong bảng để Khóa/Mở khóa!");

                bool laTaiKhoanHienTai =
                    string.Equals(
                        TaiKhoanDuocChon.TenDangNhap,
                        PhienDangNhap.TenDangNhap,
                        StringComparison.OrdinalIgnoreCase);

                bool trangThaiMoi = !TaiKhoanDuocChon.TrangThai;

                if (laTaiKhoanHienTai && !trangThaiMoi)
                {
                    throw new InvalidOperationException("Không thể tự khóa tài khoản đang đăng nhập!");
                }

                bool thanhCong = repository.CapNhat(
                    TaiKhoanDuocChon.MaTaiKhoan,
                    TaiKhoanDuocChon.VaiTro,
                    trangThaiMoi);

                if (thanhCong)
                {
                    ThongBao = trangThaiMoi
                        ? $"Đã mở khóa tài khoản '{TaiKhoanDuocChon.TenDangNhap}' thành công!"
                        : $"Đã khóa tài khoản '{TaiKhoanDuocChon.TenDangNhap}' thành công!";
                    LamTrong();
                    TaiDanhSach();
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
        // 12. DAT LAI MAT KHAU
        // =====================================

        private void DoiMatKhau(string matKhauMoi)
        {
            try
            {
                KiemTraQuyen();

                if (TaiKhoanDuocChon == null)
                    throw new ArgumentException(
                        "Vui lòng chọn tài khoản cần đặt lại mật khẩu trong bảng!");

                if (string.IsNullOrEmpty(matKhauMoi) ||
                    matKhauMoi.Length < 6)
                    throw new ArgumentException(
                        "Mật khẩu mới phải có ít nhất 6 ký tự!");

                string hashMoi =
                    PasswordHasher.HashPassword(matKhauMoi);

                bool thanhCong = repository.DoiMatKhau(
                    TaiKhoanDuocChon.MaTaiKhoan,
                    hashMoi);

                ThongBao = thanhCong
                    ? $"Đặt lại mật khẩu cho tài khoản '{TaiKhoanDuocChon.TenDangNhap}' thành công!"
                    : "Không tìm thấy tài khoản!";
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // =====================================
        // 13. LAM TRONG
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
        // 14. LAM MOI
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
        // 15. XU LY LOI
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
