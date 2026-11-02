using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RESULT_ANALYTICS.Models
{
    internal class PhanLoaiHocLuc
    {
        // Thong tin sinh vien
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public string MaLop { get; set; }

        // Thong tin hoc ky
        public string MaHocKy { get; set; }
        public string TenHocKy { get; set; }
        public string NamHoc { get; set; }

        // Thong ke mon hoc
        public int TongSoMon { get; set; }
        public int SoMonThieuDiem { get; set; }

        // Ket qua phan tich
        public decimal? DiemTrungBinh { get; set; }
        public string XepLoai { get; set; }
        public string CanhBao { get; set; }
    }
}
