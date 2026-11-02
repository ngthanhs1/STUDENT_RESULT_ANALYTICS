
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using RESULT_ANALYTICS.DataAccess;
using RESULT_ANALYTICS.Helpers;
using RESULT_ANALYTICS.Models;

namespace RESULT_ANALYTICS.ViewModels
{
    // Doi tuong cho ComboBox loc du lieu
    internal class BoLocThongKeCanhBao
    {
        public string Ma { get; set; }
        public string TenHienThi { get; set; }
    }

    // Du lieu hien thi bieu do dat, truot, thieu diem
    internal class BieuDoThongKeDatTruot
    {
        public string TenNhom { get; set; }

        public int SoDat { get; set; }
        public int SoTruot { get; set; }
        public int SoThieuDiem { get; set; }

        public double ChieuRongDat { get; set; }
        public double ChieuRongTruot { get; set; }
        public double ChieuRongThieuDiem { get; set; }
    }

    internal class ThongKeCanhBaoViewModel : BindableBase
    {
        private readonly ThongKeCanhBaoRepository repository;
        private readonly MonHocRepository monHocRepository;
        private readonly HocKyRepository hocKyRepository;

        // =========================================
        // 1. DANH SACH HIEN THI
        // =========================================

        public ObservableCollection<ThongKeDatTruot> DanhSachThongKe { get; private set; }

        public ObservableCollection<SinhVienThieuDiem> DanhSachCanhBao { get; private set; }

        public ObservableCollection<BoLocThongKeCanhBao> DanhSachMonHocLoc { get; private set; }

        public ObservableCollection<BoLocThongKeCanhBao> DanhSachHocKyLoc { get; private set; }

        public ObservableCollection<BieuDoThongKeDatTruot> DanhSachBieuDo { get; private set; }

        // =========================================
        // 2. THONG TIN BO LOC
        // =========================================

        private string maMonHocLoc;
        public string MaMonHocLoc
        {
            get { return maMonHocLoc; }
            set { SetProperty(ref maMonHocLoc, value); }
        }

        private string maHocKyLoc;
        public string MaHocKyLoc
        {
            get { return maHocKyLoc; }
            set { SetProperty(ref maHocKyLoc, value); }
        }

        // =========================================
        // 3. CAC CHI SO THONG KE
        // =========================================

        private int tongLuotKetQua;
        public int TongLuotKetQua
        {
            get { return tongLuotKetQua; }
            set { SetProperty(ref tongLuotKetQua, value); }
        }

        private int tongLuotDat;
        public int TongLuotDat
        {
            get { return tongLuotDat; }
            set { SetProperty(ref tongLuotDat, value); }
        }

        private int tongLuotTruot;
        public int TongLuotTruot
        {
            get { return tongLuotTruot; }
            set { SetProperty(ref tongLuotTruot, value); }
        }

        private int tongLuotThieuDiem;
        public int TongLuotThieuDiem
        {
            get { return tongLuotThieuDiem; }
            set { SetProperty(ref tongLuotThieuDiem, value); }
        }

        private int soCanhBao;
        public int SoCanhBao
        {
            get { return soCanhBao; }
            set { SetProperty(ref soCanhBao, value); }
        }

        private string tyLeDatChung;
        public string TyLeDatChung
        {
            get { return tyLeDatChung; }
            set { SetProperty(ref tyLeDatChung, value); }
        }

        private string thongBao;
        public string ThongBao
        {
            get { return thongBao; }
            set { SetProperty(ref thongBao, value); }
        }

        // =========================================
        // 4. COMMAND
        // =========================================

        public ICommand LocDuLieuCommand { get; private set; }
        public ICommand LamMoiCommand { get; private set; }

        // =========================================
        // 5. HAM KHOI TAO
        // =========================================

        public ThongKeCanhBaoViewModel()
        {
            repository = new ThongKeCanhBaoRepository();
            monHocRepository = new MonHocRepository();
            hocKyRepository = new HocKyRepository();

            DanhSachThongKe =
                new ObservableCollection<ThongKeDatTruot>();

            DanhSachCanhBao =
                new ObservableCollection<SinhVienThieuDiem>();

            DanhSachMonHocLoc =
                new ObservableCollection<BoLocThongKeCanhBao>();

            DanhSachHocKyLoc =
                new ObservableCollection<BoLocThongKeCanhBao>();

            DanhSachBieuDo =
                new ObservableCollection<BieuDoThongKeDatTruot>();

            MaMonHocLoc = "";
            MaHocKyLoc = "";

            TyLeDatChung = "0.00%";
            ThongBao = "";

            LocDuLieuCommand =
                new RelayCommand(p => LocDuLieu());

            LamMoiCommand =
                new RelayCommand(p => LamMoi());

            try
            {
                TaiBoLoc();
                LocDuLieu();
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // =========================================
        // 6. TAI DANH SACH MON HOC VA HOC KY
        // =========================================

        private void TaiBoLoc()
        {
            DanhSachMonHocLoc.Clear();
            DanhSachHocKyLoc.Clear();

            // Lua chon tat ca
            DanhSachMonHocLoc.Add(new BoLocThongKeCanhBao
            {
                Ma = "",
                TenHienThi = "Tất cả môn học"
            });

            DanhSachHocKyLoc.Add(new BoLocThongKeCanhBao
            {
                Ma = "",
                TenHienThi = "Tất cả học kỳ"
            });

            // Lay mon hoc tu SQL Server
            foreach (MonHoc mh in monHocRepository.LayDanhSach())
            {
                DanhSachMonHocLoc.Add(new BoLocThongKeCanhBao
                {
                    Ma = mh.MaMonHoc,
                    TenHienThi = mh.MaMonHoc + " - " + mh.TenMonHoc
                });
            }

            // Lay hoc ky tu SQL Server
            foreach (HocKy hk in hocKyRepository.LayDanhSach())
            {
                DanhSachHocKyLoc.Add(new BoLocThongKeCanhBao
                {
                    Ma = hk.MaHocKy,
                    TenHienThi = hk.TenHocKy + " - " + hk.NamHoc
                });
            }
        }

        // =========================================
        // 7. LOC VA TAI DU LIEU
        // =========================================

        private void LocDuLieu()
        {
            try
            {
                string maMonHoc = string.IsNullOrWhiteSpace(MaMonHocLoc)
                    ? null : MaMonHocLoc.Trim();

                string maHocKy = string.IsNullOrWhiteSpace(MaHocKyLoc)
                    ? null : MaHocKyLoc.Trim();

                // Lay thong ke dat, truot
                List<ThongKeDatTruot> thongKe =
                    repository.LayThongKeDatTruot(maMonHoc, maHocKy);

                // Lay danh sach canh bao
                List<SinhVienThieuDiem> canhBao =
                    repository.LaySinhVienThieuDiem(maMonHoc, maHocKy);

                // Cap nhat DataGrid thong ke
                DanhSachThongKe.Clear();

                foreach (ThongKeDatTruot item in thongKe)
                {
                    DanhSachThongKe.Add(item);
                }

                // Cap nhat DataGrid canh bao
                DanhSachCanhBao.Clear();

                foreach (SinhVienThieuDiem item in canhBao)
                {
                    DanhSachCanhBao.Add(item);
                }

                // Tinh cac chi so tong hop
                TinhThongKeTongHop(thongKe, canhBao);

                // Cap nhat du lieu bieu do
                TaoDuLieuBieuDo(thongKe);

                ThongBao = "Đã tải " + thongKe.Count
                    + " dòng thống kê và "
                    + canhBao.Count + " cảnh báo thiếu điểm.";
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // =========================================
        // 8. TINH THONG KE TONG HOP
        // =========================================

        private void TinhThongKeTongHop(
            List<ThongKeDatTruot> thongKe,
            List<SinhVienThieuDiem> canhBao)
        {
            int tongLuot = 0;
            int dat = 0;
            int truot = 0;
            int thieu = 0;

            foreach (ThongKeDatTruot item in thongKe)
            {
                tongLuot += item.TongSinhVien;
                dat += item.SoSinhVienDat;
                truot += item.SoSinhVienTruot;
                thieu += item.SoSinhVienThieuDiem;
            }

            TongLuotKetQua = tongLuot;
            TongLuotDat = dat;
            TongLuotTruot = truot;
            TongLuotThieuDiem = thieu;

            SoCanhBao = canhBao.Count;

            // Ty le dat chi tinh trong cac ket qua du diem
            int tongDaDuDiem = dat + truot;

            if (tongDaDuDiem > 0)
            {
                decimal tyLe =
                    (decimal)dat * 100m / tongDaDuDiem;

                TyLeDatChung = tyLe.ToString(
                    "0.00", CultureInfo.CurrentCulture) + "%";
            }
            else
            {
                TyLeDatChung = "--";
            }
        }

        // =========================================
        // 9. TAO DU LIEU CHO BIEU DO
        // =========================================

        private void TaoDuLieuBieuDo(
            List<ThongKeDatTruot> thongKe)
        {
            DanhSachBieuDo.Clear();

            // Tim tong so sinh vien lon nhat
            int lonNhat = 0;

            foreach (ThongKeDatTruot item in thongKe)
            {
                if (item.TongSinhVien > lonNhat)
                {
                    lonNhat = item.TongSinhVien;
                }
            }

            // Chieu rong toi da cua mot cot la 260
            foreach (ThongKeDatTruot item in thongKe)
            {
                double tyLeChieuRong = lonNhat > 0
                    ? 260.0 / lonNhat
                    : 0;

                BieuDoThongKeDatTruot bieuDo =
                    new BieuDoThongKeDatTruot();

                bieuDo.TenNhom =
                    item.MaMonHoc + " - " + item.TenMonHoc
                    + " (" + item.TenHocKy
                    + " " + item.NamHoc + ")";

                bieuDo.SoDat = item.SoSinhVienDat;
                bieuDo.SoTruot = item.SoSinhVienTruot;
                bieuDo.SoThieuDiem =
                    item.SoSinhVienThieuDiem;

                bieuDo.ChieuRongDat =
                    item.SoSinhVienDat * tyLeChieuRong;

                bieuDo.ChieuRongTruot =
                    item.SoSinhVienTruot * tyLeChieuRong;

                bieuDo.ChieuRongThieuDiem =
                    item.SoSinhVienThieuDiem * tyLeChieuRong;

                DanhSachBieuDo.Add(bieuDo);
            }
        }

        // =========================================
        // 10. LAM MOI DU LIEU
        // =========================================

        private void LamMoi()
        {
            try
            {
                MaMonHocLoc = "";
                MaHocKyLoc = "";

                TaiBoLoc();
                LocDuLieu();
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // =========================================
        // 11. XU LY LOI
        // =========================================

        private void BaoLoi(Exception ex)
        {
            ThongBao = "Lỗi thống kê: " + ex.Message;

            MessageBox.Show(
                ThongBao,
                "Thống kê và cảnh báo",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}
