using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESULT_ANALYTICS.Models
{
    internal class TaiKhoan
    {
        public int MaTaiKhoan { get; set; }

        public string TenDangNhap { get; set; }

        public string MatKhauHash { get; set; }

        public string VaiTro { get; set; }

        public bool TrangThai { get; set; }

        public DateTime NgayTao { get; set; }
    }
}
