using System.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using RESULT_ANALYTICS.DataAccess;
using RESULT_ANALYTICS.Helpers;

namespace RESULT_ANALYTICS.ViewModels
{
    internal class MainViewModel : BindableBase
    {
        // Cac the hien ViewModel cua he thong
        private readonly DashboardViewModel dashboard;
        private readonly QuanLySinhVienViewModel quanLySinhVien;
        private readonly QuanLyMonHocHocKyViewModel quanLyMonHoc;
        private readonly QuanLyDiemViewModel quanLyDiem;
        private readonly PhanTichViewModel phanTich;
        private readonly ThongKeCanhBaoViewModel thongKeCanhBao;
        private readonly HeThongImportViewModel heThongImport;
        private QuanLyTaiKhoanViewModel quanLyTaiKhoan;

        private object noiDungHienTai;
        private string tenTrang;
        private string phanHeHienTai;
        private string mucDangChon;
        private string tenDangNhap;
        private string vaiTro;
        private bool dangODashboard;

        public object NoiDungHienTai
        {
            get { return noiDungHienTai; }
            set { SetProperty(ref noiDungHienTai, value); }
        }

        public string TenTrang
        {
            get { return tenTrang; }
            set { SetProperty(ref tenTrang, value); }
        }

        public string PhanHeHienTai
        {
            get { return phanHeHienTai; }
            set { SetProperty(ref phanHeHienTai, value); }
        }

        public string MucDangChon
        {
            get { return mucDangChon; }
            set
            {
                if (SetProperty(ref mucDangChon, value))
                {
                    OnPropertyChanged(nameof(IsDashboardSelected));
                    OnPropertyChanged(nameof(IsSinhVienLopSelected));
                    OnPropertyChanged(nameof(IsMonHocHocKySelected));
                    OnPropertyChanged(nameof(IsNhapDiemSelected));
                    OnPropertyChanged(nameof(IsImportDiemSelected));
                    OnPropertyChanged(nameof(IsPhanTichSelected));
                    OnPropertyChanged(nameof(IsCanhBaoSelected));
                    OnPropertyChanged(nameof(IsHeThongSelected));
                }
            }
        }

        // ===============================================
        // CAC THUOC TINH NHAN DIEN MENU DANG CHON (ACTIVE)
        // ===============================================
        public bool IsDashboardSelected => mucDangChon == "Dashboard";
        public bool IsSinhVienLopSelected => mucDangChon == "SinhVienLop";
        public bool IsMonHocHocKySelected => mucDangChon == "MonHocHocKy";
        public bool IsNhapDiemSelected => mucDangChon == "NhapDiem";
        public bool IsImportDiemSelected => mucDangChon == "ImportDiem";
        public bool IsPhanTichSelected => mucDangChon == "PhanTich";
        public bool IsCanhBaoSelected => mucDangChon == "CanhBao";
        public bool IsHeThongSelected => mucDangChon == "HeThong";

        public string TenDangNhap
        {
            get { return tenDangNhap; }
            set { SetProperty(ref tenDangNhap, value); }
        }

        public string VaiTro
        {
            get { return vaiTro; }
            set
            {
                if (SetProperty(ref vaiTro, value))
                {
                    OnPropertyChanged(nameof(LaAdmin));
                    OnPropertyChanged(nameof(ThongTinNguoiDung));
                }
            }
        }

        public bool LaAdmin
        {
            get { return vaiTro == "Admin" || PhienDangNhap.LaAdmin; }
        }

        public bool DangODashboard
        {
            get { return dangODashboard; }
            set { SetProperty(ref dangODashboard, value); }
        }

        // ===============================================
        // THONG TIN DONG: NIEN KHOA & TRANG THAI KET NOI
        // ===============================================
        private string nienKhoaHienThi = "Niên khóa: 2025 - 2026";
        public string NienKhoaHienThi
        {
            get { return nienKhoaHienThi; }
            set { SetProperty(ref nienKhoaHienThi, value); }
        }

        private bool dangTrucTuyen = true;
        public bool DangTrucTuyen
        {
            get { return dangTrucTuyen; }
            set
            {
                if (SetProperty(ref dangTrucTuyen, value))
                {
                    OnPropertyChanged(nameof(TrangThaiKetNoi));
                    OnPropertyChanged(nameof(MauTrangThaiKetNoi));
                    OnPropertyChanged(nameof(MauNenTrangThaiKetNoi));
                }
            }
        }

        public string TrangThaiKetNoi => DangTrucTuyen ? "Trực tuyến" : "Mất kết nối";
        public string MauTrangThaiKetNoi => DangTrucTuyen ? "#166534" : "#991B1B";
        public string MauNenTrangThaiKetNoi => DangTrucTuyen ? "#DCFCE7" : "#FEE2E2";

        // Cac lenh dieu huong theo phan he
        public ICommand TrangChuCommand { get; private set; }
        public ICommand QuanLyLopSinhVienCommand { get; private set; }
        public ICommand MonHocHocKyCommand { get; private set; }
        public ICommand NhapDiemCommand { get; private set; }
        public ICommand ImportDiemCommand { get; private set; }
        public ICommand PhanTichCommand { get; private set; }
        public ICommand CanhBaoCommand { get; private set; }
        public ICommand HeThongCommand { get; private set; }

        // Thuoc tinh thu gon / mo rong thanh ben (Hamburger)
        private bool thanhBenThuGon = false;

        public bool ThanhBenThuGon
        {
            get { return thanhBenThuGon; }
            set
            {
                if (SetProperty(ref thanhBenThuGon, value))
                {
                    OnPropertyChanged(nameof(DoRongThanhBen));
                    OnPropertyChanged(nameof(HienThiChuThanhBen));
                    OnPropertyChanged(nameof(HienThiIconThanhBen));
                    OnPropertyChanged(nameof(CanLeNoiDungMenu));
                }
            }
        }

        public double DoRongThanhBen
        {
            get { return thanhBenThuGon ? 60.0 : 230.0; }
        }

        public Visibility HienThiChuThanhBen
        {
            get { return thanhBenThuGon ? Visibility.Collapsed : Visibility.Visible; }
        }

        public Visibility HienThiIconThanhBen
        {
            get { return thanhBenThuGon ? Visibility.Visible : Visibility.Collapsed; }
        }

        public HorizontalAlignment CanLeNoiDungMenu
        {
            get { return thanhBenThuGon ? HorizontalAlignment.Center : HorizontalAlignment.Left; }
        }

        public string ThongTinNguoiDung
        {
            get { return $"{tenDangNhap} ({vaiTro})"; }
        }

        public ICommand ToggleThanhBenCommand { get; private set; }

        public MainViewModel()
        {
            // Khoi tao san cac ViewModel nghiep vu
            dashboard = new DashboardViewModel();
            dashboard.YeuCauDieuHuong += (muc) =>
            {
                if (muc == "SinhVien") QuanLyLopSinhVienCommand?.Execute(null);
                else if (muc == "NhapDiem") NhapDiemCommand?.Execute(null);
                else if (muc == "ImportDiem") ImportDiemCommand?.Execute(null);
                else if (muc == "PhanTich") PhanTichCommand?.Execute(null);
            };
            quanLySinhVien = new QuanLySinhVienViewModel();
            quanLyMonHoc = new QuanLyMonHocHocKyViewModel();
            quanLyDiem = new QuanLyDiemViewModel();
            phanTich = new PhanTichViewModel();
            thongKeCanhBao = new ThongKeCanhBaoViewModel();
            heThongImport = new HeThongImportViewModel();

            // Neu da dang nhap voi quyen Admin, khoi tao quanLyTaiKhoan
            if (PhienDangNhap.LaAdmin)
            {
                quanLyTaiKhoan = new QuanLyTaiKhoanViewModel();
            }

            ToggleThanhBenCommand = new RelayCommand(p =>
            {
                ThanhBenThuGon = !ThanhBenThuGon;
            });

            // 1. TONG QUAN
            TrangChuCommand = new RelayCommand(p => MoTrangChu());

            // 2. QUAN LY DAO TAO
            QuanLyLopSinhVienCommand = new RelayCommand(p => MoQuanLySinhVien());

            MonHocHocKyCommand = new RelayCommand(p =>
            {
                PhanHeHienTai = "QUẢN LÝ ĐÀO TẠO";
                TenTrang = "Học phần & Niên khóa";
                MucDangChon = "MonHocHocKy";
                DangODashboard = false;
                NoiDungHienTai = quanLyMonHoc;
            });

            // 3. KHAO THI & DIEM SO
            NhapDiemCommand = new RelayCommand(p =>
            {
                PhanHeHienTai = "KHẢO THÍ & ĐIỂM SỐ";
                TenTrang = "Sổ điểm học phần";
                MucDangChon = "NhapDiem";
                DangODashboard = false;
                NoiDungHienTai = quanLyDiem;
            });

            ImportDiemCommand = new RelayCommand(p =>
            {
                PhanHeHienTai = "KHẢO THÍ & ĐIỂM SỐ";
                TenTrang = "Nhập điểm thi từ File (CSV / Excel)";
                MucDangChon = "ImportDiem";
                DangODashboard = false;
                NoiDungHienTai = heThongImport;
            });

            // 4. HOC VU & BAO CAO
            PhanTichCommand = new RelayCommand(p =>
            {
                PhanHeHienTai = "HỌC VỤ & BÁO CÁO";
                TenTrang = "Bảng điểm GPA & Phân tích học lực";
                MucDangChon = "PhanTich";
                DangODashboard = false;
                NoiDungHienTai = phanTich;
            });

            CanhBaoCommand = new RelayCommand(p =>
            {
                PhanHeHienTai = "HỌC VỤ & BÁO CÁO";
                TenTrang = "Thống kê Đạt/Trượt & Cảnh báo học vụ";
                MucDangChon = "CanhBao";
                DangODashboard = false;
                NoiDungHienTai = thongKeCanhBao;
            });

            // 5. QUAN TRI HE THONG (CHI DANG CHO ADMIN)
            HeThongCommand = new RelayCommand(p =>
            {
                if (!LaAdmin)
                {
                    MessageBox.Show(
                        "Chức năng này chỉ dành cho Quản trị viên (Admin)!",
                        "Phân quyền hệ thống",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (quanLyTaiKhoan == null)
                {
                    quanLyTaiKhoan = new QuanLyTaiKhoanViewModel();
                }
                else
                {
                    quanLyTaiKhoan.LamMoiCommand?.Execute(null);
                }

                PhanHeHienTai = "QUẢN TRỊ HỆ THỐNG";
                TenTrang = "Quản trị Hệ thống & Phân quyền cán bộ";
                MucDangChon = "HeThong";
                DangODashboard = false;
                NoiDungHienTai = quanLyTaiKhoan;
            });

            MoTrangChu();
            CapNhatThongTinHeThong();
        }

        public void GanTaiKhoan(string tenDangNhap, string vaiTro)
        {
            TenDangNhap = tenDangNhap;
            VaiTro = vaiTro;
            PhienDangNhap.KhoiTao(tenDangNhap, vaiTro);
            OnPropertyChanged(nameof(LaAdmin));
            OnPropertyChanged(nameof(ThongTinNguoiDung));

            if (LaAdmin && quanLyTaiKhoan == null)
            {
                quanLyTaiKhoan = new QuanLyTaiKhoanViewModel();
            }

            CapNhatThongTinHeThong();
        }

        public void CapNhatThongTinHeThong()
        {
            try
            {
                DangTrucTuyen = DatabaseHelper.TestConnection();

                var hocKyRepo = new HocKyRepository();
                var danhSachHocKy = hocKyRepo.LayDanhSach();
                if (danhSachHocKy != null && danhSachHocKy.Count > 0)
                {
                    var hkMoi = danhSachHocKy.OrderByDescending(x => x.MaHocKy).FirstOrDefault();
                    if (hkMoi != null)
                    {
                        NienKhoaHienThi = $"Niên khóa: {hkMoi.NamHoc} ({hkMoi.TenHocKy})";
                    }
                }
                else
                {
                    NienKhoaHienThi = "Niên khóa: 2025 - 2026";
                }
            }
            catch
            {
                DangTrucTuyen = false;
                NienKhoaHienThi = "Niên khóa: 2025 - 2026";
            }
        }

        private void MoTrangChu()
        {
            PhanHeHienTai = "TỔNG QUAN";
            TenTrang = "Bàn làm việc (Dashboard)";
            MucDangChon = "Dashboard";
            DangODashboard = true;
            NoiDungHienTai = dashboard;
        }

        private void MoQuanLySinhVien()
        {
            PhanHeHienTai = "QUẢN LÝ ĐÀO TẠO";
            TenTrang = "Hồ sơ Sinh viên & Lớp học";
            MucDangChon = "SinhVienLop";
            DangODashboard = false;
            NoiDungHienTai = quanLySinhVien;
        }
    }
}
