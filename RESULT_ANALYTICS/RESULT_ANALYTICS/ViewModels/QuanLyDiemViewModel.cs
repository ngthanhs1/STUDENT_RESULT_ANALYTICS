using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using RESULT_ANALYTICS.DataAccess;
using RESULT_ANALYTICS.Helpers;
using RESULT_ANALYTICS.Models;

namespace RESULT_ANALYTICS.ViewModels
{
    internal class TyLeDiemOption
    {
        public string MaTyLe { get; set; }
        public string TenHienThi { get; set; }
    }

    internal class QuanLyDiemViewModel : BindableBase
    {
        private readonly KetQuaRepository ketQuaRepository;
        private readonly SinhVienRepository sinhVienRepository;
        private readonly MonHocRepository monHocRepository;
        private readonly HocKyRepository hocKyRepository;

        // Danh sach hien thi
        public ObservableCollection<KetQua> DanhSachKetQua { get; private set; }
        public ObservableCollection<SinhVien> DanhSachSinhVien { get; private set; }
        public ObservableCollection<MonHoc> DanhSachMonHoc { get; private set; }
        public ObservableCollection<HocKy> DanhSachHocKy { get; private set; }
        public ObservableCollection<TyLeDiemOption> DanhSachTyLeDiem { get; private set; }

        // Ket qua dang duoc chon
        private KetQua ketQuaDuocChon;
        public KetQua KetQuaDuocChon
        {
            get { return ketQuaDuocChon; }
            set
            {
                if (SetProperty(ref ketQuaDuocChon, value) && value != null)
                {
                    MaKetQuaDangSua = value.MaKetQua;
                    MaSVNhap = value.MaSV;
                    MaMonHocNhap = value.MaMonHoc;
                    MaHocKyNhap = value.MaHocKy;

                    TyLeDiemNhap = string.IsNullOrWhiteSpace(value.TyLeDiem) ? "10-30-60" : value.TyLeDiem;

                    DiemChuyenCanNhap = ChuyenThanhChuoi(value.DiemChuyenCan);
                    DiemGiuaKyNhap = ChuyenThanhChuoi(value.DiemGiuaKy);
                    DiemCuoiKyNhap = ChuyenThanhChuoi(value.DiemCuoiKy);

                    ThongBao = "";
                }
            }
        }

        private int maKetQuaDangSua;
        public int MaKetQuaDangSua
        {
            get { return maKetQuaDangSua; }
            set { SetProperty(ref maKetQuaDangSua, value); }
        }

        // Thong tin nhap
        private string maSVNhap;
        public string MaSVNhap
        {
            get { return maSVNhap; }
            set { SetProperty(ref maSVNhap, value); }
        }

        private string maMonHocNhap;
        public string MaMonHocNhap
        {
            get { return maMonHocNhap; }
            set { SetProperty(ref maMonHocNhap, value); }
        }

        private string maHocKyNhap;
        public string MaHocKyNhap
        {
            get { return maHocKyNhap; }
            set { SetProperty(ref maHocKyNhap, value); }
        }

        // Ty le tinh diem (10-30-60 hoac 10-40-50)
        private string tyLeDiemNhap = "10-30-60";
        public string TyLeDiemNhap
        {
            get { return tyLeDiemNhap; }
            set
            {
                if (SetProperty(ref tyLeDiemNhap, value))
                {
                    OnPropertyChanged(nameof(NhanChuyenCan));
                    OnPropertyChanged(nameof(NhanGiuaKy));
                    OnPropertyChanged(nameof(NhanCuoiKy));
                }
            }
        }

        // Nhan dong cho cac cot diem
        public string NhanChuyenCan => "Điểm chuyên cần (10%)";
        public string NhanGiuaKy => TyLeDiemNhap == "10-40-50" ? "Điểm giữa kỳ / TH (40%)" : "Điểm giữa kỳ (30%)";
        public string NhanCuoiKy => TyLeDiemNhap == "10-40-50" ? "Điểm thi cuối kỳ (50%)" : "Điểm thi cuối kỳ (60%)";

        private string diemChuyenCanNhap;
        public string DiemChuyenCanNhap
        {
            get { return diemChuyenCanNhap; }
            set { SetProperty(ref diemChuyenCanNhap, value); }
        }

        private string diemGiuaKyNhap;
        public string DiemGiuaKyNhap
        {
            get { return diemGiuaKyNhap; }
            set { SetProperty(ref diemGiuaKyNhap, value); }
        }

        private string diemCuoiKyNhap;
        public string DiemCuoiKyNhap
        {
            get { return diemCuoiKyNhap; }
            set { SetProperty(ref diemCuoiKyNhap, value); }
        }

        private string thongBao;
        public string ThongBao
        {
            get { return thongBao; }
            set { SetProperty(ref thongBao, value); }
        }

        // Commands
        public ICommand ThemDiemCommand { get; private set; }
        public ICommand SuaDiemCommand { get; private set; }
        public ICommand XoaDiemCommand { get; private set; }
        public ICommand LamTrongCommand { get; private set; }
        public ICommand LamMoiCommand { get; private set; }

        // Khoi tao
        public QuanLyDiemViewModel()
        {
            ketQuaRepository = new KetQuaRepository();
            sinhVienRepository = new SinhVienRepository();
            monHocRepository = new MonHocRepository();
            hocKyRepository = new HocKyRepository();

            DanhSachKetQua = new ObservableCollection<KetQua>();
            DanhSachSinhVien = new ObservableCollection<SinhVien>();
            DanhSachMonHoc = new ObservableCollection<MonHoc>();
            DanhSachHocKy = new ObservableCollection<HocKy>();

            DanhSachTyLeDiem = new ObservableCollection<TyLeDiemOption>
            {
                new TyLeDiemOption { MaTyLe = "10-30-60", TenHienThi = "10% - 30% - 60% (Môn lý thuyết)" },
                new TyLeDiemOption { MaTyLe = "10-40-50", TenHienThi = "10% - 40% - 50% (Môn có thực hành)" }
            };

            ThemDiemCommand = new RelayCommand(p => ThemDiem());
            SuaDiemCommand = new RelayCommand(p => SuaDiem());
            XoaDiemCommand = new RelayCommand(p => XoaDiem());
            LamTrongCommand = new RelayCommand(p => LamTrong());
            LamMoiCommand = new RelayCommand(p => LamMoi());

            try
            {
                TaiDuLieu();
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // Lay danh sach tu SQL Server
        private void TaiDuLieu()
        {
            DanhSachSinhVien.Clear();
            foreach (SinhVien sv in sinhVienRepository.LayDanhSach())
            {
                DanhSachSinhVien.Add(sv);
            }

            DanhSachMonHoc.Clear();
            foreach (MonHoc mh in monHocRepository.LayDanhSach())
            {
                DanhSachMonHoc.Add(mh);
            }

            DanhSachHocKy.Clear();
            foreach (HocKy hk in hocKyRepository.LayDanhSach())
            {
                DanhSachHocKy.Add(hk);
            }

            TaiDanhSachDiem();
        }

        private void TaiDanhSachDiem()
        {
            DanhSachKetQua.Clear();

            foreach (KetQua kq in ketQuaRepository.LayDanhSach())
            {
                DanhSachKetQua.Add(kq);
            }
        }

        // Chuyen diem thanh chuoi de hien thi
        private string ChuyenThanhChuoi(decimal? diem)
        {
            return diem.HasValue
                ? diem.Value.ToString(CultureInfo.CurrentCulture)
                : "";
        }

        // Cho phep bo trong diem hoac nhap 7,5 / 7.5
        private decimal? DocDiem(string giaTri, string tenDiem)
        {
            if (string.IsNullOrWhiteSpace(giaTri))
                return null;

            string chuoi = giaTri.Trim().Replace(',', '.');

            decimal diem;
            bool hopLe = decimal.TryParse(
                chuoi,
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out diem);

            if (!hopLe)
                throw new ArgumentException(tenDiem + " không hợp lệ!");

            if (diem < 0 || diem > 10)
                throw new ArgumentException(
                    tenDiem + " phải từ 0 đến 10!");

            if (decimal.Round(diem, 2) != diem)
                throw new ArgumentException(
                    tenDiem + " tối đa 2 chữ số thập phân!");

            return diem;
        }

        // Lay du lieu tu form
        private KetQua TaoKetQuaTuForm()
        {
            if (string.IsNullOrWhiteSpace(MaSVNhap))
                throw new ArgumentException("Vui lòng chọn sinh viên!");

            if (string.IsNullOrWhiteSpace(MaMonHocNhap))
                throw new ArgumentException("Vui lòng chọn môn học!");

            if (string.IsNullOrWhiteSpace(MaHocKyNhap))
                throw new ArgumentException("Vui lòng chọn học kỳ!");

            return new KetQua
            {
                MaKetQua = MaKetQuaDangSua,
                MaSV = MaSVNhap.Trim(),
                MaMonHoc = MaMonHocNhap.Trim(),
                MaHocKy = MaHocKyNhap.Trim(),
                TyLeDiem = string.IsNullOrWhiteSpace(TyLeDiemNhap) ? "10-30-60" : TyLeDiemNhap,
                DiemChuyenCan = DocDiem(DiemChuyenCanNhap, NhanChuyenCan),
                DiemGiuaKy = DocDiem(DiemGiuaKyNhap, NhanGiuaKy),
                DiemCuoiKy = DocDiem(DiemCuoiKyNhap, NhanCuoiKy)
            };
        }

        // Them diem
        private void ThemDiem()
        {
            try
            {
                KetQua kq = TaoKetQuaTuForm();

                if (ketQuaRepository.Them(kq))
                {
                    TaiDanhSachDiem();
                    LamTrong();
                    ThongBao = "Thêm điểm thành công!";
                }
                else
                {
                    ThongBao = "Không thể thêm điểm!";
                }
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // Sua diem
        private void SuaDiem()
        {
            if (KetQuaDuocChon == null || MaKetQuaDangSua <= 0)
            {
                MessageBox.Show(
                    "Vui lòng chọn một dòng điểm để sửa!",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            try
            {
                KetQua kq = TaoKetQuaTuForm();

                if (ketQuaRepository.Sua(kq))
                {
                    TaiDanhSachDiem();
                    LamTrong();
                    ThongBao = "Cập nhật điểm thành công!";
                }
                else
                {
                    ThongBao = "Không tìm thấy kết quả cần sửa!";
                }
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // Xoa diem
        private void XoaDiem()
        {
            if (KetQuaDuocChon == null)
            {
                MessageBox.Show(
                    "Vui lòng chọn một dòng điểm để xóa!",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }

            MessageBoxResult xacNhan = MessageBox.Show(
                "Bạn có chắc muốn xóa kết quả điểm này?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (xacNhan != MessageBoxResult.Yes)
                return;

            try
            {
                int maKetQua = KetQuaDuocChon.MaKetQua;

                if (ketQuaRepository.Xoa(maKetQua))
                {
                    TaiDanhSachDiem();
                    LamTrong();
                    ThongBao = "Xóa điểm thành công!";
                }
                else
                {
                    ThongBao = "Không tìm thấy kết quả cần xóa!";
                }
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // Lam trong form
        private void LamTrong()
        {
            KetQuaDuocChon = null;
            MaKetQuaDangSua = 0;

            MaSVNhap = null;
            MaMonHocNhap = null;
            MaHocKyNhap = null;
            TyLeDiemNhap = "10-30-60";

            DiemChuyenCanNhap = "";
            DiemGiuaKyNhap = "";
            DiemCuoiKyNhap = "";

            ThongBao = "";
        }

        // Lam moi toan bo du lieu
        private void LamMoi()
        {
            try
            {
                LamTrong();
                TaiDuLieu();
                ThongBao = "Đã làm mới danh sách điểm!";
            }
            catch (Exception ex)
            {
                BaoLoi(ex);
            }
        }

        // Xu ly loi
        private void BaoLoi(Exception ex)
        {
            string noiDung = ex.Message;

            SqlException loiSql = ex as SqlException;

            if (loiSql != null)
            {
                if (loiSql.Number == 2627 || loiSql.Number == 2601)
                    noiDung = "Kết quả điểm này đã tồn tại!";
                else if (loiSql.Number == 547)
                    noiDung = "Dữ liệu đang liên kết hoặc mã chọn không hợp lệ!";
            }

            ThongBao = noiDung;

            MessageBox.Show(
                noiDung,
                "Thông báo",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}
