
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using RESULT_ANALYTICS.Helpers;

namespace RESULT_ANALYTICS.ViewModels
{
    internal class MainViewModel : BindableBase
    {
        private readonly DashboardViewModel dashboard;
        private readonly QuanLySinhVienViewModel quanLySinhVien;

        private object noiDungHienTai;
        private string tenTrang;
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

        public string TenDangNhap
        {
            get { return tenDangNhap; }
            set { SetProperty(ref tenDangNhap, value); }
        }

        public string VaiTro
        {
            get { return vaiTro; }
            set { SetProperty(ref vaiTro, value); }
        }

        public bool DangODashboard
        {
            get { return dangODashboard; }
            set { SetProperty(ref dangODashboard, value); }
        }

        // Cac lenh menu
        public ICommand TrangChuCommand { get; private set; }
        public ICommand QuanLyLopSinhVienCommand { get; private set; }
        public ICommand MonHocHocKyCommand { get; private set; }
        public ICommand NhapDiemCommand { get; private set; }
        public ICommand PhanTichCommand { get; private set; }
        public ICommand CanhBaoCommand { get; private set; }
        public ICommand HeThongCommand { get; private set; }

        // Ham khoi tao
        public MainViewModel()
        {
            dashboard = new DashboardViewModel();
            quanLySinhVien = new QuanLySinhVienViewModel();

            TrangChuCommand = new RelayCommand(p => MoTrangChu());

            QuanLyLopSinhVienCommand =
                new RelayCommand(p => MoQuanLySinhVien());


            MonHocHocKyCommand = new RelayCommand(p =>
            {
                TenTrang = "Quản lý môn học và học kỳ";
                NoiDungHienTai = "Quản lý môn học và học kỳ";
            });


            NhapDiemCommand =
                new RelayCommand(p => MoTrang("Nhập và quản lý điểm"));

            PhanTichCommand =
                new RelayCommand(p => MoTrang("Phân tích kết quả học tập"));

            CanhBaoCommand =
                new RelayCommand(p => MoTrang("Thống kê và cảnh báo"));

            HeThongCommand =
                new RelayCommand(p => MoTrang("Phân quyền và nhập dữ liệu"));

            MoTrangChu();
        }

        // Gan thong tin nguoi dang nhap
        public void GanTaiKhoan(string tenDangNhap, string vaiTro)
        {
            TenDangNhap = tenDangNhap;
            VaiTro = vaiTro;
        }

        // Mo Dashboard
        private void MoTrangChu()
        {
            TenTrang = "Dashboard tổng quan";
            DangODashboard = true;
            NoiDungHienTai = dashboard;
        }

        // Mo giao dien quan ly sinh vien
        private void MoQuanLySinhVien()
        {
            TenTrang = "Quản lý lớp và sinh viên";
            DangODashboard = false;
            NoiDungHienTai = quanLySinhVien;
        }

        // Cac trang chua hoan thien
        private void MoTrang(string tenChucNang)
        {
            TenTrang = tenChucNang;
            DangODashboard = false;

            NoiDungHienTai = "Chức năng " + tenChucNang +
                " sẽ được hoàn thiện trong các bước tiếp theo.";
        }
    }
}
