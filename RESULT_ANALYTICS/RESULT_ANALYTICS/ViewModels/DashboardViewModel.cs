using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using RESULT_ANALYTICS.DataAccess;
using RESULT_ANALYTICS.Helpers;
using RESULT_ANALYTICS.Models;

namespace RESULT_ANALYTICS.ViewModels
{
    internal class DashboardViewModel : BindableBase
    {
        private readonly DashboardRepository repository;

        private int tongSinhVien;
        private int tongLop;
        private int tongMonHoc;
        private int soSinhVienThieuDiem;
        private decimal diemTBChung;
        private decimal tyLeDatChung;
        private string thongBao;
        private string nienKhoaHienThi = "Niên khóa: 2025 - 2026";
        private string heThongDbHienThi = "🟢 Hệ thống: SQL Server";
        private string maHocKyLoc = "";

        public int TongSinhVien
        {
            get { return tongSinhVien; }
            set { SetProperty(ref tongSinhVien, value); }
        }

        public int TongLop
        {
            get { return tongLop; }
            set { SetProperty(ref tongLop, value); }
        }

        public int TongMonHoc
        {
            get { return tongMonHoc; }
            set { SetProperty(ref tongMonHoc, value); }
        }

        public int SoSinhVienThieuDiem
        {
            get { return soSinhVienThieuDiem; }
            set { SetProperty(ref soSinhVienThieuDiem, value); }
        }

        public decimal DiemTBChung
        {
            get { return diemTBChung; }
            set
            {
                if (SetProperty(ref diemTBChung, value))
                {
                    OnPropertyChanged(nameof(GpaHe4HienThi));
                }
            }
        }

        public string GpaHe4HienThi
        {
            get
            {
                if (DiemTBChung <= 0) return "";
                decimal? he4 = GpaHelper.QuyDoiDiemHe4(DiemTBChung);
                return he4.HasValue ? $"(GPA 4.0: {he4.Value:N1})" : "";
            }
        }

        public decimal TyLeDatChung
        {
            get { return tyLeDatChung; }
            set { SetProperty(ref tyLeDatChung, value); }
        }

        public string ThongBao
        {
            get { return thongBao; }
            set { SetProperty(ref thongBao, value); }
        }

        public string NienKhoaHienThi
        {
            get { return nienKhoaHienThi; }
            set { SetProperty(ref nienKhoaHienThi, value); }
        }

        public string HeThongDbHienThi
        {
            get { return heThongDbHienThi; }
            set { SetProperty(ref heThongDbHienThi, value); }
        }

        public event Action<string> YeuCauDoiHocKy;

        public string MaHocKyLoc
        {
            get { return maHocKyLoc; }
            set
            {
                if (SetProperty(ref maHocKyLoc, value))
                {
                    OnPropertyChanged(nameof(NhanDiemTB));
                    OnPropertyChanged(nameof(NhanTyLeDat));
                    OnPropertyChanged(nameof(NhanCanhBao));
                    TaiDuLieu();
                    YeuCauDoiHocKy?.Invoke(value);
                }
            }
        }

        public string NhanDiemTB => string.IsNullOrWhiteSpace(MaHocKyLoc) ? "ĐIỂM TB TOÀN KHÓA" : "ĐIỂM TB HỌC KỲ";
        public string NhanTyLeDat => string.IsNullOrWhiteSpace(MaHocKyLoc) ? "TỶ LỆ ĐẠT CHUNG" : "TỶ LỆ ĐẠT HỌC KỲ";
        public string NhanCanhBao => string.IsNullOrWhiteSpace(MaHocKyLoc) ? "CẦN BỔ SUNG ĐIỂM" : "THIẾU ĐIỂM KỲ NÀY";

        public ObservableCollection<MucChonHocKy> DanhSachHocKyLoc { get; private set; }
        public ObservableCollection<PhanBoHocLucDashboard> DanhSachPhanBo { get; private set; }
        public ObservableCollection<ThongKeDatTruot> DanhSachMonHoc { get; private set; }
        public ObservableCollection<CanhBaoThieuDiem> DanhSachCanhBao { get; private set; }

        public ICommand LamMoiCommand { get; private set; }

        // Lenh chuyen trang nhanh
        public ICommand ChuyenDenNhapDiemCommand { get; private set; }
        public ICommand ChuyenDenImportCommand { get; private set; }
        public ICommand ChuyenDenSinhVienCommand { get; private set; }
        public ICommand ChuyenDenPhanTichCommand { get; private set; }

        // Su kien yeu cau MainWindow dieu huong
        public event Action<string> YeuCauDieuHuong;

        public DashboardViewModel()
        {
            repository = new DashboardRepository();

            DanhSachHocKyLoc = new ObservableCollection<MucChonHocKy>();
            DanhSachPhanBo = new ObservableCollection<PhanBoHocLucDashboard>();
            DanhSachMonHoc = new ObservableCollection<ThongKeDatTruot>();
            DanhSachCanhBao = new ObservableCollection<CanhBaoThieuDiem>();

            LamMoiCommand = new RelayCommand(p => TaiDuLieu());

            ChuyenDenNhapDiemCommand = new RelayCommand(p => YeuCauDieuHuong?.Invoke("NhapDiem"));
            ChuyenDenImportCommand = new RelayCommand(p => YeuCauDieuHuong?.Invoke("ImportDiem"));
            ChuyenDenSinhVienCommand = new RelayCommand(p => YeuCauDieuHuong?.Invoke("SinhVien"));
            ChuyenDenPhanTichCommand = new RelayCommand(p => YeuCauDieuHuong?.Invoke("PhanTich"));

            TaiDanhSachHocKy();
            TaiDuLieu();
        }

        public void TaiDanhSachHocKy()
        {
            try
            {
                var hocKyRepo = new HocKyRepository();
                var danhSachHk = hocKyRepo.LayDanhSachMucChon(kemMucTatCa: true);

                DanhSachHocKyLoc.Clear();
                foreach (var hk in danhSachHk)
                {
                    DanhSachHocKyLoc.Add(hk);
                }
            }
            catch { }
        }

        public void TaiDuLieu(string maHocKy = null)
        {
            if (maHocKy != null && maHocKy != maHocKyLoc)
            {
                maHocKyLoc = maHocKy;
                OnPropertyChanged(nameof(MaHocKyLoc));
                OnPropertyChanged(nameof(NhanDiemTB));
                OnPropertyChanged(nameof(NhanTyLeDat));
                OnPropertyChanged(nameof(NhanCanhBao));
            }

            try
            {
                ThongKeTongQuan thongKe = repository.LayThongKe(maHocKyLoc);
                List<PhanBoHocLucDashboard> phanBo = repository.LayPhanBoHocLuc(maHocKyLoc);
                List<ThongKeDatTruot> monHoc = repository.LayMonHocDatTruot(maHocKyLoc);
                List<CanhBaoThieuDiem> canhBao = repository.LayDanhSachCanhBao(maHocKyLoc);

                TongSinhVien = thongKe.TongSinhVien;
                TongLop = thongKe.TongLop;
                TongMonHoc = thongKe.TongMonHoc;
                SoSinhVienThieuDiem = thongKe.SoSinhVienThieuDiem;
                DiemTBChung = thongKe.DiemTBChung;
                TyLeDatChung = thongKe.TyLeDatChung;

                DanhSachPhanBo.Clear();
                foreach (var item in phanBo)
                {
                    DanhSachPhanBo.Add(item);
                }

                DanhSachMonHoc.Clear();
                foreach (var item in monHoc)
                {
                    DanhSachMonHoc.Add(item);
                }

                DanhSachCanhBao.Clear();
                foreach (var item in canhBao)
                {
                    DanhSachCanhBao.Add(item);
                }

                try
                {
                    var hocKyRepo = new HocKyRepository();
                    var danhSachHk = hocKyRepo.LayDanhSach();
                    if (danhSachHk != null && danhSachHk.Count > 0)
                    {
                        var hkMoi = danhSachHk.OrderByDescending(x => x.MaHocKy).FirstOrDefault();
                        if (hkMoi != null)
                        {
                            NienKhoaHienThi = $"Niên khóa: {hkMoi.NamHoc} ({hkMoi.TenHocKy})";
                        }
                    }
                    bool ketNoiOk = DatabaseHelper.TestConnection();
                    HeThongDbHienThi = ketNoiOk ? "🟢 Hệ thống: SQL Server (Trực tuyến)" : "🔴 Hệ thống: SQL Server (Mất kết nối)";
                }
                catch
                {
                    HeThongDbHienThi = "⚪ Hệ thống: SQL Server";
                }

                string tenKy = string.IsNullOrWhiteSpace(maHocKyLoc) ? "Tất cả học kỳ" : maHocKyLoc;
                ThongBao = $"Đã đồng bộ số liệu [{tenKy}] từ SQL Server ({DateTime.Now:HH:mm:ss}).";
            }
            catch (Exception ex)
            {
                ThongBao = "Không thể tải dữ liệu: " + ex.Message;
            }
        }
    }
}
