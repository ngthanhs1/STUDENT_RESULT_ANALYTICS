using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RESULT_ANALYTICS.Models
{
    internal class SinhVienThieuDiem
    {
        // Thong tin sinh vien
        public string MaSV { get; set; }
        public string HoTen { get; set; }

        // Thong tin mon hoc
        public string MaMonHoc { get; set; }
        public string TenMonHoc { get; set; }

        // Thong tin hoc ky
        public string MaHocKy { get; set; }
        public string TenHocKy { get; set; }
        public string NamHoc { get; set; }

        // Diem thanh phan co the NULL
        public decimal? DiemChuyenCan { get; set; }
        public decimal? DiemGiuaKy { get; set; }
        public decimal? DiemCuoiKy { get; set; }

        // Noi dung canh bao tu SQL Server
        public string NoiDungCanhBao { get; set; }
    }
}

