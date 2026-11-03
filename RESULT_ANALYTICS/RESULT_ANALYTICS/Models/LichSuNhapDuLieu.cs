using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESULT_ANALYTICS.Models
{
    internal class LichSuNhapDuLieu
    {
        // Ma lich su tu tang
        public int MaLichSu { get; set; }

        // Thong tin file
        public string TenFile { get; set; }
        public string LoaiFile { get; set; }

        // Thoi gian nhap
        public DateTime ThoiGianNhap { get; set; }

        // Thong ke ket qua Import
        public int TongSoDong { get; set; }
        public int SoDongThanh { get; set; }
        public int SoDongLoi { get; set; }

        // Trang thai Import
        public string TrangThai { get; set; }
    }
}
