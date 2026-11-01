
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESULT_ANALYTICS.Models
{
    internal class ThongKeTongQuan
    {
        public int TongSinhVien { get; set; }
        public int TongLop { get; set; }
        public int TongMonHoc { get; set; }
        public int SoSinhVienThieuDiem { get; set; }
    }

    internal class CanhBaoThieuDiem
    {
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public string MaMonHoc { get; set; }
        public string TenMonHoc { get; set; }
        public string MaHocKy { get; set; }
        public string NoiDungCanhBao { get; set; }
    }
}
