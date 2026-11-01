
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
    internal class QuanLyMonHocHocKyViewModel : BindableBase
    {
        // KHAI BAO REPOSITORY
        private readonly MonHocRepository monHocRepository;
        private readonly HocKyRepository hocKyRepository;

        // DANH SACH DU LIEU
        public ObservableCollection<MonHoc> DanhSachMonHoc { get; private set; }
        public ObservableCollection<HocKy> DanhSachHocKy { get; private set; }

        // =========================================
        // PHAN 1: QUAN LY MON HOC
        // =========================================

        private MonHoc monHocDuocChon;
        private string maMonHocNhap;
        private string tenMonHocNhap;
        private string soTinChiNhap;
        private string thongBaoMonHoc;

        // Mon hoc duoc chon trong DataGrid
        public MonHoc MonHocDuocChon
        {
            get { return monHocDuocChon; }
            set
            {
                if (SetProperty(ref monHocDuocChon, value))
                {
                    OnPropertyChanged(nameof(DangSuaMonHoc));

                    if (value != null)
                    {
                        MaMonHocNhap = value.MaMonHoc;
                        TenMonHocNhap = value.TenMonHoc;
                        SoTinChiNhap = value.SoTinChi.ToString();
                    }
                }
            }
        }

        // Ma mon hoc
        public string MaMonHocNhap
        {
            get { return maMonHocNhap; }
            set { SetProperty(ref maMonHocNhap, value); }
        }

        // Ten mon hoc
        public string TenMonHocNhap
        {
            get { return tenMonHocNhap; }
            set { SetProperty(ref tenMonHocNhap, value); }
        }

        // So tin chi
        public string SoTinChiNhap
        {
            get { return soTinChiNhap; }
            set { SetProperty(ref soTinChiNhap, value); }
        }

        // Thong bao mon hoc
        public string ThongBaoMonHoc
        {
            get { return thongBaoMonHoc; }
            set { SetProperty(ref thongBaoMonHoc, value); }
        }

        // Khoa ma khi dang sua
        public bool DangSuaMonHoc
        {
            get { return MonHocDuocChon != null; }
        }

        // COMMAND MON HOC
        public ICommand ThemMonHocCommand { get; private set; }
        public ICommand SuaMonHocCommand { get; private set; }
        public ICommand XoaMonHocCommand { get; private set; }
        public ICommand LamTrongMonHocCommand { get; private set; }
        public ICommand LamMoiMonHocCommand { get; private set; }

        // =========================================
        // PHAN 2: QUAN LY HOC KY
        // =========================================

        private HocKy hocKyDuocChon;
        private string maHocKyNhap;
        private string tenHocKyNhap;
        private string namHocNhap;
        private DateTime? ngayBatDauNhap;
        private DateTime? ngayKetThucNhap;
        private string thongBaoHocKy;

        // Hoc ky duoc chon trong DataGrid
        public HocKy HocKyDuocChon
        {
            get { return hocKyDuocChon; }
            set
            {
                if (SetProperty(ref hocKyDuocChon, value))
                {
                    OnPropertyChanged(nameof(DangSuaHocKy));

                    if (value != null)
                    {
                        MaHocKyNhap = value.MaHocKy;
                        TenHocKyNhap = value.TenHocKy;
                        NamHocNhap = value.NamHoc;
                        NgayBatDauNhap = value.NgayBatDau;
                        NgayKetThucNhap = value.NgayKetThuc;
                    }
                }
            }
        }

        // Ma hoc ky
        public string MaHocKyNhap
        {
            get { return maHocKyNhap; }
            set { SetProperty(ref maHocKyNhap, value); }
        }

        // Ten hoc ky
        public string TenHocKyNhap
        {
            get { return tenHocKyNhap; }
            set { SetProperty(ref tenHocKyNhap, value); }
        }

        // Nam hoc
        public string NamHocNhap
        {
            get { return namHocNhap; }
            set { SetProperty(ref namHocNhap, value); }
        }

        // Ngay bat dau
        public DateTime? NgayBatDauNhap
        {
            get { return ngayBatDauNhap; }
            set { SetProperty(ref ngayBatDauNhap, value); }
        }

        // Ngay ket thuc
        public DateTime? NgayKetThucNhap
        {
            get { return ngayKetThucNhap; }
            set { SetProperty(ref ngayKetThucNhap, value); }
        }

        // Thong bao hoc ky
        public string ThongBaoHocKy
        {
            get { return thongBaoHocKy; }
            set { SetProperty(ref thongBaoHocKy, value); }
        }

        // Khoa ma khi dang sua
        public bool DangSuaHocKy
        {
            get { return HocKyDuocChon != null; }
        }

        // COMMAND HOC KY
        public ICommand ThemHocKyCommand { get; private set; }
        public ICommand SuaHocKyCommand { get; private set; }
        public ICommand XoaHocKyCommand { get; private set; }
        public ICommand LamTrongHocKyCommand { get; private set; }
        public ICommand LamMoiHocKyCommand { get; private set; }

        // =========================================
        // PHAN 3: HAM KHOI TAO
        // =========================================

        public QuanLyMonHocHocKyViewModel()
        {
            monHocRepository = new MonHocRepository();
            hocKyRepository = new HocKyRepository();

            DanhSachMonHoc = new ObservableCollection<MonHoc>();
            DanhSachHocKy = new ObservableCollection<HocKy>();

            // Khoi tao command mon hoc
            ThemMonHocCommand = new RelayCommand(p => ThemMonHoc());
            SuaMonHocCommand = new RelayCommand(p => SuaMonHoc());
            XoaMonHocCommand = new RelayCommand(p => XoaMonHoc());
            LamTrongMonHocCommand = new RelayCommand(p => LamTrongMonHoc());
            LamMoiMonHocCommand = new RelayCommand(p => LamMoiMonHoc());

            // Khoi tao command hoc ky
            ThemHocKyCommand = new RelayCommand(p => ThemHocKy());
            SuaHocKyCommand = new RelayCommand(p => SuaHocKy());
            XoaHocKyCommand = new RelayCommand(p => XoaHocKy());
            LamTrongHocKyCommand = new RelayCommand(p => LamTrongHocKy());
            LamMoiHocKyCommand = new RelayCommand(p => LamMoiHocKy());

            // Tai du lieu ban dau
            try
            {
                TaiDanhSachMonHoc();
            }
            catch (Exception ex)
            {
                ThongBaoMonHoc = ex.Message;
            }

            try
            {
                TaiDanhSachHocKy();
            }
            catch (Exception ex)
            {
                ThongBaoHocKy = ex.Message;
            }
        }

        // =========================================
        // PHAN 4: XU LY MON HOC
        // =========================================

        // Tai danh sach mon hoc
        private void TaiDanhSachMonHoc()
        {
            var danhSach = monHocRepository.LayDanhSach();

            DanhSachMonHoc.Clear();

            foreach (MonHoc monHoc in danhSach)
            {
                DanhSachMonHoc.Add(monHoc);
            }
        }

        // Lay du lieu mon hoc tu form
        private MonHoc LayDuLieuMonHoc()
        {
            int soTinChi;

            if (!int.TryParse(SoTinChiNhap, out soTinChi))
            {
                throw new ArgumentException("Số tín chỉ phải là số nguyên!");
            }

            return new MonHoc
            {
                MaMonHoc = MaMonHocNhap,
                TenMonHoc = TenMonHocNhap,
                SoTinChi = soTinChi
            };
        }

        // Them mon hoc
        private void ThemMonHoc()
        {
            try
            {
                if (MonHocDuocChon != null)
                {
                    ThongBaoMonHoc = "Nhấn Làm trống trước khi thêm môn học!";
                    return;
                }

                MonHoc monHoc = LayDuLieuMonHoc();

                if (monHocRepository.Them(monHoc))
                {
                    TaiDanhSachMonHoc();
                    LamTrongMonHoc();
                    ThongBaoMonHoc = "Thêm môn học thành công!";
                }
                else
                {
                    ThongBaoMonHoc = "Không thể thêm môn học.";
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                ThongBaoMonHoc = "Mã môn học đã tồn tại!";
            }
            catch (Exception ex)
            {
                ThongBaoMonHoc = "Lỗi: " + ex.Message;
            }
        }

        // Sua mon hoc
        private void SuaMonHoc()
        {
            try
            {
                if (MonHocDuocChon == null)
                {
                    ThongBaoMonHoc = "Vui lòng chọn môn học cần sửa!";
                    return;
                }

                MonHoc monHoc = LayDuLieuMonHoc();

                // Khong cho phep sua khoa chinh
                monHoc.MaMonHoc = MonHocDuocChon.MaMonHoc;

                if (monHocRepository.Sua(monHoc))
                {
                    TaiDanhSachMonHoc();
                    LamTrongMonHoc();
                    ThongBaoMonHoc = "Cập nhật môn học thành công!";
                }
                else
                {
                    ThongBaoMonHoc = "Không tìm thấy môn học cần sửa.";
                }
            }
            catch (Exception ex)
            {
                ThongBaoMonHoc = "Lỗi: " + ex.Message;
            }
        }

        // Xoa mon hoc
        private void XoaMonHoc()
        {
            if (MonHocDuocChon == null)
            {
                ThongBaoMonHoc = "Vui lòng chọn môn học cần xóa!";
                return;
            }

            string maMonHoc = MonHocDuocChon.MaMonHoc;

            MessageBoxResult ketQua = MessageBox.Show(
                "Bạn có chắc muốn xóa môn học " + maMonHoc + "?",
                "Xác nhận xóa môn học",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (ketQua != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                if (monHocRepository.Xoa(maMonHoc))
                {
                    TaiDanhSachMonHoc();
                    LamTrongMonHoc();
                    ThongBaoMonHoc = "Xóa môn học thành công!";
                }
                else
                {
                    ThongBaoMonHoc = "Không tìm thấy môn học cần xóa.";
                }
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                ThongBaoMonHoc =
                    "Không thể xóa môn học vì đang có kết quả học tập liên kết!";
            }
            catch (Exception ex)
            {
                ThongBaoMonHoc = "Lỗi: " + ex.Message;
            }
        }

        // Lam trong form mon hoc
        private void LamTrongMonHoc()
        {
            MonHocDuocChon = null;
            MaMonHocNhap = "";
            TenMonHocNhap = "";
            SoTinChiNhap = "";
        }

        // Lam moi du lieu mon hoc
        private void LamMoiMonHoc()
        {
            try
            {
                TaiDanhSachMonHoc();
                LamTrongMonHoc();
                ThongBaoMonHoc = "Đã làm mới danh sách môn học.";
            }
            catch (Exception ex)
            {
                ThongBaoMonHoc = "Lỗi: " + ex.Message;
            }
        }

        // =========================================
        // PHAN 5: XU LY HOC KY
        // =========================================

        // Tai danh sach hoc ky
        private void TaiDanhSachHocKy()
        {
            var danhSach = hocKyRepository.LayDanhSach();

            DanhSachHocKy.Clear();

            foreach (HocKy hocKy in danhSach)
            {
                DanhSachHocKy.Add(hocKy);
            }
        }

        // Lay du lieu hoc ky tu form
        private HocKy LayDuLieuHocKy()
        {
            return new HocKy
            {
                MaHocKy = MaHocKyNhap,
                TenHocKy = TenHocKyNhap,
                NamHoc = NamHocNhap,
                NgayBatDau = NgayBatDauNhap,
                NgayKetThuc = NgayKetThucNhap
            };
        }

        // Them hoc ky
        private void ThemHocKy()
        {
            try
            {
                if (HocKyDuocChon != null)
                {
                    ThongBaoHocKy = "Nhấn Làm trống trước khi thêm học kỳ!";
                    return;
                }

                HocKy hocKy = LayDuLieuHocKy();

                if (hocKyRepository.Them(hocKy))
                {
                    TaiDanhSachHocKy();
                    LamTrongHocKy();
                    ThongBaoHocKy = "Thêm học kỳ thành công!";
                }
                else
                {
                    ThongBaoHocKy = "Không thể thêm học kỳ.";
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                ThongBaoHocKy = "Mã học kỳ đã tồn tại!";
            }
            catch (Exception ex)
            {
                ThongBaoHocKy = "Lỗi: " + ex.Message;
            }
        }

        // Sua hoc ky
        private void SuaHocKy()
        {
            try
            {
                if (HocKyDuocChon == null)
                {
                    ThongBaoHocKy = "Vui lòng chọn học kỳ cần sửa!";
                    return;
                }

                HocKy hocKy = LayDuLieuHocKy();

                // Khong cho phep sua khoa chinh
                hocKy.MaHocKy = HocKyDuocChon.MaHocKy;

                if (hocKyRepository.Sua(hocKy))
                {
                    TaiDanhSachHocKy();
                    LamTrongHocKy();
                    ThongBaoHocKy = "Cập nhật học kỳ thành công!";
                }
                else
                {
                    ThongBaoHocKy = "Không tìm thấy học kỳ cần sửa.";
                }
            }
            catch (Exception ex)
            {
                ThongBaoHocKy = "Lỗi: " + ex.Message;
            }
        }

        // Xoa hoc ky
        private void XoaHocKy()
        {
            if (HocKyDuocChon == null)
            {
                ThongBaoHocKy = "Vui lòng chọn học kỳ cần xóa!";
                return;
            }

            string maHocKy = HocKyDuocChon.MaHocKy;

            MessageBoxResult ketQua = MessageBox.Show(
                "Bạn có chắc muốn xóa học kỳ " + maHocKy + "?",
                "Xác nhận xóa học kỳ",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (ketQua != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                if (hocKyRepository.Xoa(maHocKy))
                {
                    TaiDanhSachHocKy();
                    LamTrongHocKy();
                    ThongBaoHocKy = "Xóa học kỳ thành công!";
                }
                else
                {
                    ThongBaoHocKy = "Không tìm thấy học kỳ cần xóa.";
                }
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                ThongBaoHocKy =
                    "Không thể xóa học kỳ vì đang có kết quả học tập liên kết!";
            }
            catch (Exception ex)
            {
                ThongBaoHocKy = "Lỗi: " + ex.Message;
            }
        }

        // Lam trong form hoc ky
        private void LamTrongHocKy()
        {
            HocKyDuocChon = null;
            MaHocKyNhap = "";
            TenHocKyNhap = "";
            NamHocNhap = "";
            NgayBatDauNhap = null;
            NgayKetThucNhap = null;
        }

        // Lam moi du lieu hoc ky
        private void LamMoiHocKy()
        {
            try
            {
                TaiDanhSachHocKy();
                LamTrongHocKy();
                ThongBaoHocKy = "Đã làm mới danh sách học kỳ.";
            }
            catch (Exception ex)
            {
                ThongBaoHocKy = "Lỗi: " + ex.Message;
            }
        }
    }
}
