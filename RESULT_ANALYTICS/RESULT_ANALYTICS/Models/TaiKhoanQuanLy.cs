using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESULT_ANALYTICS.Models
{
    internal class TaiKhoanQuanLy
    {
        public int MaTaiKhoan { get; set; }
        public string TenDangNhap { get; set; }
        public string VaiTro { get; set; }
        public bool TrangThai { get; set; }
        public DateTime NgayTao { get; set; }

        // Hien thi trang thai trong DataGrid
        public string TrangThaiHienThi
        {
            get
            {
                return TrangThai ? "Hoạt động" : "Đã khóa";
            }
        }
    }
}
