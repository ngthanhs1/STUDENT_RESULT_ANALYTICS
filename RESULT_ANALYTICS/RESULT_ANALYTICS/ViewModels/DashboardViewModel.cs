
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using RESULT_ANALYTICS.Helpers;
using RESULT_ANALYTICS.Models;
using RESULT_ANALYTICS.DataAccess;

namespace RESULT_ANALYTICS.ViewModels
{
    internal class DashboardViewModel : BindableBase
    {
        private readonly DashboardRepository repository;

        private int tongSinhVien;
        private int tongLop;
        private int tongMonHoc;
        private int soSinhVienThieuDiem;
        private string thongBao;

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

        public string ThongBao
        {
            get { return thongBao; }
            set { SetProperty(ref thongBao, value); }
        }

        public ObservableCollection<CanhBaoThieuDiem> DanhSachCanhBao { get; private set; }

        public ICommand LamMoiCommand { get; private set; }

        // Ham khoi tao
        public DashboardViewModel()
        {
            repository = new DashboardRepository();

            DanhSachCanhBao = new ObservableCollection<CanhBaoThieuDiem>();

            LamMoiCommand = new RelayCommand(p => TaiDuLieu());

            TaiDuLieu();
        }

        // Doc du lieu tu SQL Server
        private void TaiDuLieu()
        {
            try
            {
                ThongKeTongQuan thongKe = repository.LayThongKe();

                List<CanhBaoThieuDiem> canhBao = repository.LayDanhSachCanhBao();

                TongSinhVien = thongKe.TongSinhVien;
                TongLop = thongKe.TongLop;
                TongMonHoc = thongKe.TongMonHoc;
                SoSinhVienThieuDiem = thongKe.SoSinhVienThieuDiem;

                DanhSachCanhBao.Clear();

                foreach (CanhBaoThieuDiem item in canhBao)
                {
                    DanhSachCanhBao.Add(item);
                }

                ThongBao = "Đã cập nhật dữ liệu từ SQL Server.";
            }
            catch (Exception ex)
            {
                ThongBao = "Không thể tải dữ liệu: " + ex.Message;
            }
        }
    }
}
