using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RESULT_ANALYTICS.Models
{
    internal class ThongKeDatTruot
    {
        // Thong tin mon hoc
        public string MaMonHoc { get; set; }
        public string TenMonHoc { get; set; }
        public int SoTinChi { get; set; }

        // Thong tin hoc ky
        public string MaHocKy { get; set; }
        public string TenHocKy { get; set; }
        public string NamHoc { get; set; }

        // Thong ke ket qua hoc tap
        public int TongSinhVien { get; set; }
        public int SoSinhVienDat { get; set; }
        public int SoSinhVienTruot { get; set; }
        public int SoSinhVienThieuDiem { get; set; }

        // Ty le dat do SQL Server tinh
        public decimal? TyLeDat { get; set; }
    }
}

