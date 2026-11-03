
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using RESULT_ANALYTICS.DataAccess;
using RESULT_ANALYTICS.Helpers;
using RESULT_ANALYTICS.Models;
using RESULT_ANALYTICS.Services;

namespace RESULT_ANALYTICS.ViewModels
{
    internal class HeThongImportViewModel : BindableBase
    {
        private readonly ImportService importService;
        private readonly LichSuNhapDuLieuRepository lichSuRepository;

        // ============================================
        // 1. QUAN LY TAI KHOAN
        // ============================================

        public QuanLyTaiKhoanViewModel TaiKhoanVM
        {
            get;
            private set;
        }

        public bool LaAdmin
        {
            get { return PhienDangNhap.LaAdmin; }
        }

        public string TenDangNhap
        {
            get { return PhienDangNhap.TenDangNhap; }
        }

        public string VaiTro
        {
            get { return PhienDangNhap.VaiTro; }
        }

        private int tabDuocChon;
        public int TabDuocChon
        {
            get { return tabDuocChon; }
            set { SetProperty(ref tabDuocChon, value); }
        }

        // ============================================
        // 2. DANH SACH LICH SU VA LOI IMPORT
        // ============================================

        public ObservableCollection<LichSuNhapDuLieu> DanhSachLichSu
        {
            get;
            private set;
        }

        public ObservableCollection<string> DanhSachLoi
        {
            get;
            private set;
        }

        // ============================================
        // 3. FILE DUOC CHON
        // ============================================

        private string duongDanFile;

        public string DuongDanFile
        {
            get { return duongDanFile; }
            set
            {
                if (SetProperty(ref duongDanFile, value))
                {
                    OnPropertyChanged(nameof(TenFileHienThi));
                }
            }
        }

        public string TenFileHienThi
        {
            get
            {
                return string.IsNullOrWhiteSpace(DuongDanFile)
                    ? "Chưa chọn file"
                    : Path.GetFileName(DuongDanFile);
            }
        }

        // ============================================
        // 4. KET QUA IMPORT
        // ============================================

        private int tongSoDong;

        public int TongSoDong
        {
            get { return tongSoDong; }
            set { SetProperty(ref tongSoDong, value); }
        }

        private int soDongThanh;

        public int SoDongThanh
        {
            get { return soDongThanh; }
            set { SetProperty(ref soDongThanh, value); }
        }

        private int soDongLoi;

        public int SoDongLoi
        {
            get { return soDongLoi; }
            set
            {
                if (SetProperty(ref soDongLoi, value))
                {
                    OnPropertyChanged(nameof(CoLoi));
                }
            }
        }

        public bool CoLoi
        {
            get { return soDongLoi > 0; }
        }

        private string trangThaiImport;

        public string TrangThaiImport
        {
            get { return trangThaiImport; }
            set { SetProperty(ref trangThaiImport, value); }
        }

        private string thongBaoImport;

        public string ThongBaoImport
        {
            get { return thongBaoImport; }
            set { SetProperty(ref thongBaoImport, value); }
        }

        private string thongBaoLichSu;

        public string ThongBaoLichSu
        {
            get { return thongBaoLichSu; }
            set { SetProperty(ref thongBaoLichSu, value); }
        }

        // ============================================
        // 5. COMMAND
        // ============================================

        public ICommand ChonFileCommand { get; private set; }
        public ICommand ImportFileCommand { get; private set; }
        public ICommand LamMoiLichSuCommand { get; private set; }

        // ============================================
        // 6. HAM KHOI TAO
        // ============================================

        public HeThongImportViewModel()
        {
            importService = new ImportService();
            lichSuRepository = new LichSuNhapDuLieuRepository();

            DanhSachLichSu =
                new ObservableCollection<LichSuNhapDuLieu>();

            DanhSachLoi =
                new ObservableCollection<string>();

            DuongDanFile = "";
            TrangThaiImport = "Chưa Import";
            ThongBaoImport = "";
            ThongBaoLichSu = "";

            ChonFileCommand =
                new RelayCommand(p => ChonFile());

            ImportFileCommand =
                new RelayCommand(p => ImportFile());

            LamMoiLichSuCommand =
                new RelayCommand(p => LamMoiLichSu());

            // Chi Admin moi khoi tao phan quan ly tai khoan
            if (PhienDangNhap.LaAdmin)
            {
                TaiKhoanVM = new QuanLyTaiKhoanViewModel();
            }


            try
            {
                TaiLichSu();
            }
            catch (Exception ex)
            {
                // Khong hien MessageBox khi giao dien dang khoi tao
                ThongBaoLichSu =
                    "Không thể tải lịch sử Import: " + ex.Message;
            }

        }

        // ============================================
        // 7. CHON FILE CSV HOAC EXCEL
        // ============================================

        private void ChonFile()
        {
            try
            {
                KiemTraDangNhap();

                OpenFileDialog dialog = new OpenFileDialog();

                dialog.Title = "Chọn file điểm sinh viên";

                dialog.Filter =
                    "File CSV và Excel (*.csv;*.xlsx)|*.csv;*.xlsx|" +
                    "File CSV (*.csv)|*.csv|" +
                    "File Excel (*.xlsx)|*.xlsx";

                dialog.Multiselect = false;
                dialog.CheckFileExists = true;

                bool? ketQua = dialog.ShowDialog();

                if (ketQua == true)
                {
                    DuongDanFile = dialog.FileName;

                    XoaKetQuaCu();

                    ThongBaoImport =
                        "Đã chọn file: " + TenFileHienThi;
                }
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // ============================================
        // 8. THUC HIEN IMPORT
        // ============================================

        private void ImportFile()
        {
            try
            {
                KiemTraDangNhap();

                if (string.IsNullOrWhiteSpace(DuongDanFile))
                {
                    throw new ArgumentException(
                        "Vui lòng chọn file CSV hoặc Excel!");
                }

                if (!File.Exists(DuongDanFile))
                {
                    throw new FileNotFoundException(
                        "File đã chọn không còn tồn tại!");
                }

                string phanMoRong = Path.GetExtension(
                    DuongDanFile).ToLowerInvariant();

                if (phanMoRong != ".csv" &&
                    phanMoRong != ".xlsx")
                {
                    throw new ArgumentException(
                        "Chỉ hỗ trợ file CSV hoặc Excel .xlsx!");
                }

                MessageBoxResult xacNhan = MessageBox.Show(
                    "Bạn có chắc muốn Import file:\n" +
                    TenFileHienThi +
                    "\n\nDữ liệu mới sẽ được thêm vào SQL Server." +
                    "\nCác kết quả đã tồn tại sẽ không bị ghi đè.",
                    "Xác nhận Import",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (xacNhan != MessageBoxResult.Yes)
                    return;

                XoaKetQuaCu();

                ThongBaoImport = "Đang thực hiện Import...";

                KetQuaImportResult ketQua;

                // Chon ham Import dua vao duoi file
                if (phanMoRong == ".csv")
                {
                    ketQua = importService.ImportCsvKetQua(
                        DuongDanFile);
                }
                else
                {
                    ketQua = importService.ImportExcelKetQua(
                        DuongDanFile);
                }

                // Cap nhat cac chi so
                TongSoDong = ketQua.TongSoDong;
                SoDongThanh = ketQua.SoDongThanh;
                SoDongLoi = ketQua.SoDongLoi;

                TrangThaiImport = ketQua.TrangThai;

                // Hien thi chi tiet loi
                DanhSachLoi.Clear();

                foreach (string loi in ketQua.DanhSachLoi)
                {
                    DanhSachLoi.Add(loi);
                }

                // Thong bao ket qua
                ThongBaoImport =
                    "Import hoàn tất! Tổng: " +
                    TongSoDong +
                    " | Thành công: " +
                    SoDongThanh +
                    " | Lỗi: " +
                    SoDongLoi;

                if (ketQua.SoDongLoi > ketQua.DanhSachLoi.Count)
                {
                    ThongBaoImport +=
                        ". Chỉ hiển thị 30 lỗi đầu tiên.";
                }

                if (!ketQua.DaLuuLichSu)
                {
                    ThongBaoLichSu =
                        ketQua.CanhBaoLichSu ??
                        "Chưa lưu được lịch sử Import.";
                }
                else
                {
                    ThongBaoLichSu =
                        "Đã lưu lịch sử Import thành công.";
                }

                // Tai lai bang lich su
                try
                {
                    TaiLichSu();
                }
                catch (Exception ex)
                {
                    ThongBaoLichSu +=
                        " Không thể tải lại lịch sử: " +
                        ex.Message;
                }

                MessageBox.Show(
                    ThongBaoImport +
                    "\n\n" + ThongBaoLichSu,
                    "Kết quả Import",
                    MessageBoxButton.OK,
                    ketQua.SoDongLoi == 0
                        ? MessageBoxImage.Information
                        : MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                TrangThaiImport = "Lỗi";
                BaoLoi(ex);
            }
        }

        // ============================================
        // 9. LAY LICH SU IMPORT TU SQL SERVER
        // ============================================

        private void TaiLichSu()
        {
            KiemTraDangNhap();

            DanhSachLichSu.Clear();

            foreach (LichSuNhapDuLieu item in
                lichSuRepository.LayDanhSach())
            {
                DanhSachLichSu.Add(item);
            }
        }

        // ============================================
        // 10. LAM MOI DANH SACH LICH SU
        // ============================================

        private void LamMoiLichSu()
        {
            try
            {
                TaiLichSu();

                ThongBaoLichSu =
                    "Đã tải lại " +
                    DanhSachLichSu.Count +
                    " bản ghi lịch sử Import.";
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // ============================================
        // 11. XOA KET QUA IMPORT CU TREN GIAO DIEN
        // ============================================

        private void XoaKetQuaCu()
        {
            TongSoDong = 0;
            SoDongThanh = 0;
            SoDongLoi = 0;

            TrangThaiImport = "Chưa Import";
            ThongBaoImport = "";
            ThongBaoLichSu = "";

            DanhSachLoi.Clear();
            OnPropertyChanged(nameof(CoLoi));
        }

        // ============================================
        // 12. KIEM TRA DANG NHAP
        // ============================================

        private void KiemTraDangNhap()
        {
            if (!PhienDangNhap.DaDangNhap)
            {
                throw new UnauthorizedAccessException(
                    "Vui lòng đăng nhập để sử dụng chức năng này!");
            }
        }

        // ============================================
        // 13. XU LY LOI
        // ============================================

        private void BaoLoi(Exception ex)
        {
            ThongBaoImport = "Lỗi: " + ex.Message;

            MessageBox.Show(
                ThongBaoImport,
                "Hệ thống và Import",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}
