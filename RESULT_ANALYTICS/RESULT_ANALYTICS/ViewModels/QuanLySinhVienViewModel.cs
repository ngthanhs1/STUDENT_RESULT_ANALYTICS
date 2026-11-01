
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using RESULT_ANALYTICS.DataAccess;
using RESULT_ANALYTICS.Helpers;
using RESULT_ANALYTICS.Models;

namespace RESULT_ANALYTICS.ViewModels
{
    internal class QuanLySinhVienViewModel : BindableBase
    {
        // Repository ket noi SQL Server
        private readonly SinhVienRepository sinhVienRepository;
        private readonly LopRepository lopRepository;

        // Danh sach du lieu
        public ObservableCollection<SinhVien> DanhSachSinhVien { get; private set; }
        public ObservableCollection<Lop> DanhSachLop { get; private set; }

        // Cac bien luu du lieu
        private SinhVien sinhVienDuocChon;
        private string maSV;
        private string hoTen;
        private DateTime? ngaySinh;
        private string gioiTinh;
        private string email;
        private string maLop;
        private string tuKhoa;
        private string thongBao;
        private bool dangSua;

        // Du lieu form quan ly lop
        private Lop lopDuocChon;
        private string maLopNhap;
        private string tenLopNhap;
        private string khoaHocNhap;

        // Sinh vien dang duoc chon
        public SinhVien SinhVienDuocChon
        {
            get { return sinhVienDuocChon; }
            set
            {
                if (SetProperty(ref sinhVienDuocChon, value))
                {
                    if (value != null)
                    {
                        MaSV = value.MaSV;
                        HoTen = value.HoTen;
                        NgaySinh = value.NgaySinh;
                        GioiTinh = value.GioiTinh;
                        Email = value.Email;
                        MaLop = value.MaLop;
                        DangSua = true;
                    }
                }
            }
        }

        // Thuoc tinh nhap lieu
        public string MaSV
        {
            get { return maSV; }
            set { SetProperty(ref maSV, value); }
        }

        public string HoTen
        {
            get { return hoTen; }
            set { SetProperty(ref hoTen, value); }
        }

        public DateTime? NgaySinh
        {
            get { return ngaySinh; }
            set { SetProperty(ref ngaySinh, value); }
        }

        public string GioiTinh
        {
            get { return gioiTinh; }
            set { SetProperty(ref gioiTinh, value); }
        }

        public string Email
        {
            get { return email; }
            set { SetProperty(ref email, value); }
        }

        public string MaLop
        {
            get { return maLop; }
            set { SetProperty(ref maLop, value); }
        }

        public string TuKhoa
        {
            get { return tuKhoa; }
            set { SetProperty(ref tuKhoa, value); }
        }

        public string ThongBao
        {
            get { return thongBao; }
            set { SetProperty(ref thongBao, value); }
        }

        public bool DangSua
        {
            get { return dangSua; }
            set { SetProperty(ref dangSua, value); }
        }

        // Cac lenh dieu khien
        public ICommand ThemCommand { get; private set; }
        public ICommand SuaCommand { get; private set; }
        public ICommand XoaCommand { get; private set; }
        public ICommand TimKiemCommand { get; private set; }
        public ICommand LamMoiCommand { get; private set; }
        public ICommand LamTrongCommand { get; private set; }

        // Ham khoi tao
        public QuanLySinhVienViewModel()
        {
            sinhVienRepository = new SinhVienRepository();
            lopRepository = new LopRepository();

            DanhSachSinhVien = new ObservableCollection<SinhVien>();
            DanhSachLop = new ObservableCollection<Lop>();

            ThemCommand = new RelayCommand(p => ThemSinhVien());
            SuaCommand = new RelayCommand(p => SuaSinhVien());
            XoaCommand = new RelayCommand(p => XoaSinhVien());
            TimKiemCommand = new RelayCommand(p => TimKiemSinhVien());
            LamMoiCommand = new RelayCommand(p => TaiDuLieu());
            LamTrongCommand = new RelayCommand(p => XoaTrangForm());

            ThemLopCommand = new RelayCommand(p => ThemLop());
            SuaLopCommand = new RelayCommand(p => SuaLop());
            XoaLopCommand = new RelayCommand(p => XoaLop());
            LamMoiLopCommand = new RelayCommand(p => LamMoiLop());
            LamTrongLopCommand = new RelayCommand(p => LamTrongLop());

            TaiDuLieu();

        }

        // Tai du lieu tu SQL Server
        private void TaiDuLieu()
        {
            try
            {
                DanhSachLop.Clear();

                foreach (Lop lop in lopRepository.LayDanhSach())
                {
                    DanhSachLop.Add(lop);
                }

                TaiDanhSachSinhVien();

                TuKhoa = "";
                XoaTrangForm();

                ThongBao = "Đã tải dữ liệu thành công.";
            }
            catch (Exception ex)
            {
                HienThiLoi(ex);
            }
        }

        // Cap nhat danh sach sinh vien
        private void TaiDanhSachSinhVien()
        {
            List<SinhVien> danhSach = sinhVienRepository.LayDanhSach();

            DanhSachSinhVien.Clear();

            foreach (SinhVien sv in danhSach)
            {
                DanhSachSinhVien.Add(sv);
            }
        }

        // Lay du lieu tu form
        private SinhVien LayDuLieuForm()
        {
            return new SinhVien
            {
                MaSV = MaSV,
                HoTen = HoTen,
                NgaySinh = NgaySinh,
                GioiTinh = GioiTinh,
                Email = Email,
                MaLop = MaLop
            };
        }

        // Them sinh vien
        private void ThemSinhVien()
        {
            try
            {
                if (SinhVienDuocChon != null)
                {
                    ThongBao = "Hãy nhấn Làm trống trước khi thêm sinh viên mới.";
                    return;
                }

                SinhVien sv = LayDuLieuForm();

                if (sinhVienRepository.Them(sv))
                {
                    TaiDanhSachSinhVien();
                    XoaTrangForm();
                    ThongBao = "Thêm sinh viên thành công!";
                }
                else
                {
                    ThongBao = "Không thể thêm sinh viên.";
                }
            }
            catch (Exception ex)
            {
                HienThiLoi(ex);
            }
        }

        // Sua sinh vien
        private void SuaSinhVien()
        {
            try
            {
                if (SinhVienDuocChon == null)
                {
                    ThongBao = "Vui lòng chọn sinh viên cần sửa!";
                    return;
                }

                SinhVien sv = LayDuLieuForm();

                // Giu nguyen khoa chinh khi sua
                sv.MaSV = SinhVienDuocChon.MaSV;

                if (sinhVienRepository.Sua(sv))
                {
                    TaiDanhSachSinhVien();
                    XoaTrangForm();
                    ThongBao = "Cập nhật sinh viên thành công!";
                }
                else
                {
                    ThongBao = "Không tìm thấy sinh viên cần sửa.";
                }
            }
            catch (Exception ex)
            {
                HienThiLoi(ex);
            }
        }

        // Xoa sinh vien
        private void XoaSinhVien()
        {
            if (SinhVienDuocChon == null)
            {
                ThongBao = "Vui lòng chọn sinh viên cần xóa!";
                return;
            }

            MessageBoxResult ketQua = MessageBox.Show(
                "Bạn có chắc muốn xóa sinh viên " +
                SinhVienDuocChon.HoTen + "?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (ketQua != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                if (sinhVienRepository.Xoa(SinhVienDuocChon.MaSV))
                {
                    TaiDanhSachSinhVien();
                    XoaTrangForm();
                    ThongBao = "Xóa sinh viên thành công!";
                }
                else
                {
                    ThongBao = "Không tìm thấy sinh viên cần xóa.";
                }
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                ThongBao = "Không thể xóa sinh viên vì đã có dữ liệu điểm liên quan.";
                MessageBox.Show(ThongBao, "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                HienThiLoi(ex);
            }
        }

        // Tim kiem sinh vien
        private void TimKiemSinhVien()
        {
            try
            {
                List<SinhVien> ketQua =
                    sinhVienRepository.TimKiem(TuKhoa);

                DanhSachSinhVien.Clear();

                foreach (SinhVien sv in ketQua)
                {
                    DanhSachSinhVien.Add(sv);
                }

                XoaTrangForm();

                ThongBao = "Tìm thấy " + ketQua.Count + " sinh viên.";
            }
            catch (Exception ex)
            {
                HienThiLoi(ex);
            }
        }

        // Xoa trang form nhap
        private void XoaTrangForm()
        {
            SinhVienDuocChon = null;
            DangSua = false;

            MaSV = "";
            HoTen = "";
            NgaySinh = null;
            GioiTinh = "";
            Email = "";
            MaLop = null;
        }

        // Thong bao loi
        private void HienThiLoi(Exception ex)
        {
            if (ex is SqlException sqlEx &&
                (sqlEx.Number == 2627 || sqlEx.Number == 2601))
            {
                ThongBao = "Mã sinh viên hoặc dữ liệu duy nhất đã tồn tại!";
            }
            else if (ex is SqlException fkEx && fkEx.Number == 547)
            {
                ThongBao = "Mã lớp không hợp lệ hoặc vi phạm liên kết dữ liệu!";
            }
            else
            {
                ThongBao = "Lỗi: " + ex.Message;
            }

            MessageBox.Show(ThongBao, "Thông báo",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public Lop LopDuocChon
        {
            get { return lopDuocChon; }
            set
            {
                if (SetProperty(ref lopDuocChon, value))
                {
                    OnPropertyChanged(nameof(DangSuaLop));

                    if (value != null)
                    {
                        MaLopNhap = value.MaLop;
                        TenLopNhap = value.TenLop;
                        KhoaHocNhap = value.KhoaHoc;
                    }
                }
            }
        }

        public string MaLopNhap
        {
            get { return maLopNhap; }
            set { SetProperty(ref maLopNhap, value); }
        }

        public string TenLopNhap
        {
            get { return tenLopNhap; }
            set { SetProperty(ref tenLopNhap, value); }
        }

        public string KhoaHocNhap
        {
            get { return khoaHocNhap; }
            set { SetProperty(ref khoaHocNhap, value); }
        }

        public bool DangSuaLop
        {
            get { return LopDuocChon != null; }
        }

        // Cac lenh quan ly lop
        public ICommand ThemLopCommand { get; private set; }
        public ICommand SuaLopCommand { get; private set; }
        public ICommand XoaLopCommand { get; private set; }
        public ICommand LamMoiLopCommand { get; private set; }
        public ICommand LamTrongLopCommand { get; private set; }


        // Lay du lieu lop tu form
        private Lop LayDuLieuLop()
        {
            return new Lop
            {
                MaLop = MaLopNhap,
                TenLop = TenLopNhap,
                KhoaHoc = KhoaHocNhap
            };
        }

        // Tai danh sach lop tu SQL Server
        private void TaiDanhSachLop()
        {
            List<Lop> danhSach = lopRepository.LayDanhSach();

            DanhSachLop.Clear();

            foreach (Lop lop in danhSach)
            {
                DanhSachLop.Add(lop);
            }
        }

        // Them lop hoc
        private void ThemLop()
        {
            try
            {
                if (LopDuocChon != null)
                {
                    ThongBao = "Nhấn Làm trống lớp trước khi thêm mới!";
                    return;
                }

                Lop lop = LayDuLieuLop();

                if (lopRepository.Them(lop))
                {
                    TaiDanhSachLop();
                    LamTrongLop();
                    ThongBao = "Thêm lớp học thành công!";
                }
                else
                {
                    ThongBao = "Không thể thêm lớp học.";
                }
            }
            catch (Exception ex)
            {
                HienThiLoi(ex);
            }
        }

        // Sua thong tin lop
        private void SuaLop()
        {
            try
            {
                if (LopDuocChon == null)
                {
                    ThongBao = "Vui lòng chọn lớp cần sửa!";
                    return;
                }

                Lop lop = LayDuLieuLop();

                // Khong thay doi khoa chinh
                lop.MaLop = LopDuocChon.MaLop;

                if (lopRepository.Sua(lop))
                {
                    TaiDanhSachLop();
                    TaiDanhSachSinhVien();
                    LamTrongLop();

                    ThongBao = "Cập nhật lớp học thành công!";
                }
                else
                {
                    ThongBao = "Không tìm thấy lớp cần sửa.";
                }
            }
            catch (Exception ex)
            {
                HienThiLoi(ex);
            }
        }

        // Xoa lop hoc
        private void XoaLop()
        {
            if (LopDuocChon == null)
            {
                ThongBao = "Vui lòng chọn lớp cần xóa!";
                return;
            }

            string maLopCanXoa = LopDuocChon.MaLop;

            MessageBoxResult ketQua = MessageBox.Show(
                "Bạn có chắc muốn xóa lớp " + maLopCanXoa + "?",
                "Xác nhận xóa lớp",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (ketQua != MessageBoxResult.Yes)
            {
                return;
            }

            try
            {
                if (lopRepository.Xoa(maLopCanXoa))
                {
                    TaiDanhSachLop();
                    LamTrongLop();
                    ThongBao = "Xóa lớp học thành công!";
                }
                else
                {
                    ThongBao = "Không tìm thấy lớp cần xóa.";
                }
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                ThongBao = "Không thể xóa lớp vì đang có sinh viên thuộc lớp này!";

                MessageBox.Show(
                    ThongBao,
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                HienThiLoi(ex);
            }
        }

        // Lam moi danh sach lop
        private void LamMoiLop()
        {
            try
            {
                TaiDanhSachLop();
                LamTrongLop();

                ThongBao = "Đã làm mới danh sách lớp học.";
            }
            catch (Exception ex)
            {
                HienThiLoi(ex);
            }
        }

        // Lam trong form lop
        private void LamTrongLop()
        {
            LopDuocChon = null;

            MaLopNhap = "";
            TenLopNhap = "";
            KhoaHocNhap = "";
        }

    }
}
