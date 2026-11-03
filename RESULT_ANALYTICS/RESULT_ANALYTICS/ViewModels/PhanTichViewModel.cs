
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
    // Doi tuong su dung cho ComboBox loc du lieu
    internal class BoLocPhanTich
    {
        public string Ma { get; set; }
        public string TenHienThi { get; set; }
    }

    // Doi tuong hien thi thong ke bieu do
    internal class ThongKeHocLuc
    {
        public string XepLoai { get; set; }
        public int SoLuong { get; set; }
        public string TyLe { get; set; }
        public double ChieuRongCot { get; set; }
    }

    internal class PhanTichViewModel : BindableBase
    {
        private readonly PhanTichRepository repository;

        // =========================================
        // 1. DANH SACH HIEN THI
        // =========================================

        public ObservableCollection<DiemTrungBinhHocKy> DanhSachDiemTrungBinh { get; private set; }
        public ObservableCollection<PhanLoaiHocLuc> DanhSachPhanLoai { get; private set; }
        public ObservableCollection<BoLocPhanTich> DanhSachLopLoc { get; private set; }
        public ObservableCollection<BoLocPhanTich> DanhSachHocKyLoc { get; private set; }
        public ObservableCollection<ThongKeHocLuc> DanhSachThongKe { get; private set; }

        // =========================================
        // 2. THONG TIN LOC
        // =========================================

        private string maLopLoc;
        public string MaLopLoc
        {
            get { return maLopLoc; }
            set { SetProperty(ref maLopLoc, value); }
        }

        private string maHocKyLoc;
        public string MaHocKyLoc
        {
            get { return maHocKyLoc; }
            set { SetProperty(ref maHocKyLoc, value); }
        }

        // =========================================
        // 3. THONG KE TONG QUAN
        // =========================================

        private int tongSinhVien;
        public int TongSinhVien
        {
            get { return tongSinhVien; }
            set { SetProperty(ref tongSinhVien, value); }
        }

        private int soKetQuaDuDiem;
        public int SoKetQuaDuDiem
        {
            get { return soKetQuaDuDiem; }
            set { SetProperty(ref soKetQuaDuDiem, value); }
        }

        private int soKetQuaThieuDiem;
        public int SoKetQuaThieuDiem
        {
            get { return soKetQuaThieuDiem; }
            set { SetProperty(ref soKetQuaThieuDiem, value); }
        }

        private string diemTrungBinhChung;
        public string DiemTrungBinhChung
        {
            get { return diemTrungBinhChung; }
            set { SetProperty(ref diemTrungBinhChung, value); }
        }

        private string gpaHe4Chung;
        public string GpaHe4Chung
        {
            get { return gpaHe4Chung; }
            set { SetProperty(ref gpaHe4Chung, value); }
        }

        private bool hienThiBieuDo = true;
        public bool HienThiBieuDo
        {
            get { return hienThiBieuDo; }
            set { SetProperty(ref hienThiBieuDo, value); }
        }

        private string thongBao;
        public string ThongBao
        {
            get { return thongBao; }
            set { SetProperty(ref thongBao, value); }
        }

        // =========================================
        // 4. CAC COMMAND
        // =========================================

        public ICommand LocDuLieuCommand { get; private set; }
        public ICommand LamMoiCommand { get; private set; }
        public ICommand ChuyenTrangThaiBieuDoCommand { get; private set; }

        // =========================================
        // 5. HAM KHOI TAO
        // =========================================

        public PhanTichViewModel()
        {
            repository = new PhanTichRepository();

            DanhSachDiemTrungBinh =
                new ObservableCollection<DiemTrungBinhHocKy>();

            DanhSachPhanLoai =
                new ObservableCollection<PhanLoaiHocLuc>();

            DanhSachLopLoc =
                new ObservableCollection<BoLocPhanTich>();

            DanhSachHocKyLoc =
                new ObservableCollection<BoLocPhanTich>();

            DanhSachThongKe =
                new ObservableCollection<ThongKeHocLuc>();

            MaLopLoc = "";
            MaHocKyLoc = "";
            DiemTrungBinhChung = "--";

            LocDuLieuCommand = new RelayCommand(p => LocDuLieu());
            LamMoiCommand = new RelayCommand(p => LamMoi());
            ChuyenTrangThaiBieuDoCommand = new RelayCommand(p => HienThiBieuDo = !HienThiBieuDo);
            GpaHe4Chung = "--";

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
        // 6. TAI BO LOC LOP VA HOC KY
        // =========================================

        private void TaiBoLoc()
        {
            List<DiemTrungBinhHocKy> danhSach =
                repository.LayDanhSachDiemTrungBinh();

            DanhSachLopLoc.Clear();
            DanhSachHocKyLoc.Clear();

            DanhSachLopLoc.Add(new BoLocPhanTich
            {
                Ma = "",
                TenHienThi = "Tất cả lớp"
            });

            DanhSachHocKyLoc.Add(new BoLocPhanTich
            {
                Ma = "",
                TenHienThi = "Tất cả học kỳ"
            });

            HashSet<string> lopDaThem = new HashSet<string>();
            HashSet<string> hocKyDaThem = new HashSet<string>();

            foreach (DiemTrungBinhHocKy item in danhSach)
            {
                if (!string.IsNullOrWhiteSpace(item.MaLop)
                    && lopDaThem.Add(item.MaLop))
                {
                    DanhSachLopLoc.Add(new BoLocPhanTich
                    {
                        Ma = item.MaLop,
                        TenHienThi = item.MaLop
                    });
                }

                if (!string.IsNullOrWhiteSpace(item.MaHocKy)
                    && hocKyDaThem.Add(item.MaHocKy))
                {
                    DanhSachHocKyLoc.Add(new BoLocPhanTich
                    {
                        Ma = item.MaHocKy,
                        TenHienThi = item.TenHocKy + " - " + item.NamHoc
                    });
                }
            }
        }

        // =========================================
        // 7. LOC VA TAI DU LIEU
        // =========================================

        private void LocDuLieu()
        {
            try
            {
                string maLop = string.IsNullOrWhiteSpace(MaLopLoc)
                    ? null : MaLopLoc;

                string maHocKy = string.IsNullOrWhiteSpace(MaHocKyLoc)
                    ? null : MaHocKyLoc;

                List<DiemTrungBinhHocKy> danhSachDiem =
                    repository.LayDanhSachDiemTrungBinh(maLop, maHocKy);

                List<PhanLoaiHocLuc> danhSachLoai =
                    repository.LayDanhSachPhanLoai(maLop, maHocKy);

                DanhSachDiemTrungBinh.Clear();
                foreach (DiemTrungBinhHocKy item in danhSachDiem)
                {
                    DanhSachDiemTrungBinh.Add(item);
                }

                DanhSachPhanLoai.Clear();
                foreach (PhanLoaiHocLuc item in danhSachLoai)
                {
                    DanhSachPhanLoai.Add(item);
                }

                TinhThongKe(danhSachLoai);

                ThongBao = "Đã tải " + danhSachLoai.Count
                    + " kết quả phân tích học lực.";
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // =========================================
        // 8. TINH THONG KE HOC LUC
        // =========================================

        private void TinhThongKe(List<PhanLoaiHocLuc> danhSach)
        {
            HashSet<string> sinhVien = new HashSet<string>();

            Dictionary<string, int> thongKe =
                new Dictionary<string, int>(
                    StringComparer.CurrentCultureIgnoreCase);

            string[] cacLoai = new string[]
            {
                "Xuất sắc",
                "Giỏi",
                "Khá",
                "Trung bình",
                "Yếu",
                "Kém"
            };

            foreach (string loai in cacLoai)
            {
                thongKe[loai] = 0;
            }

            int duDiem = 0;
            int thieuDiem = 0;

            decimal tongDiem = 0;

            foreach (PhanLoaiHocLuc item in danhSach)
            {
                if (!string.IsNullOrWhiteSpace(item.MaSV))
                {
                    sinhVien.Add(item.MaSV);
                }

                // Chi xep loai khi tat ca mon da co diem
                bool daDuDiem =
                    item.TongSoMon > 0
                    && item.SoMonThieuDiem == 0
                    && item.DiemTrungBinh.HasValue;

                if (!daDuDiem)
                {
                    thieuDiem++;
                    continue;
                }

                duDiem++;
                tongDiem += item.DiemTrungBinh.Value;

                string loaiHocLuc = string.IsNullOrWhiteSpace(item.XepLoai)
                    ? "Chưa xác định"
                    : item.XepLoai.Trim();

                if (!thongKe.ContainsKey(loaiHocLuc))
                {
                    thongKe[loaiHocLuc] = 0;
                }

                thongKe[loaiHocLuc]++;
            }

            TongSinhVien = sinhVien.Count;
            SoKetQuaDuDiem = duDiem;
            SoKetQuaThieuDiem = thieuDiem;

            if (duDiem > 0)
            {
                decimal dtb = tongDiem / duDiem;
                DiemTrungBinhChung = dtb.ToString("0.00", CultureInfo.CurrentCulture);
                decimal? he4 = GpaHelper.QuyDoiDiemHe4(dtb);
                GpaHe4Chung = he4.HasValue ? he4.Value.ToString("0.00", CultureInfo.CurrentCulture) : "--";
            }
            else
            {
                DiemTrungBinhChung = "--";
                GpaHe4Chung = "--";
            }

            // Tim so luong lon nhat de ve bieu do
            int lonNhat = 0;

            foreach (int soLuong in thongKe.Values)
            {
                if (soLuong > lonNhat)
                {
                    lonNhat = soLuong;
                }
            }

            DanhSachThongKe.Clear();

            foreach (KeyValuePair<string, int> item in thongKe)
            {
                double tyLe = duDiem > 0
                    ? item.Value * 100.0 / duDiem
                    : 0;

                double chieuRong = lonNhat > 0
                    ? item.Value * 240.0 / lonNhat
                    : 0;

                DanhSachThongKe.Add(new ThongKeHocLuc
                {
                    XepLoai = item.Key,
                    SoLuong = item.Value,
                    TyLe = tyLe.ToString("0.0") + "%",
                    ChieuRongCot = chieuRong
                });
            }
        }

        // =========================================
        // 9. LAM MOI DU LIEU
        // =========================================

        private void LamMoi()
        {
            try
            {
                MaLopLoc = "";
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
        // 10. THONG BAO LOI
        // =========================================

        private void BaoLoi(Exception ex)
        {
            ThongBao = "Lỗi tải dữ liệu: " + ex.Message;

            MessageBox.Show(
                ThongBao,
                "Phân tích học lực",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}
