
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESULT_ANALYTICS.Models
{
    internal class SinhVien
    {
        public string MaSV { get; set; }

        public string HoTen { get; set; }

        public DateTime? NgaySinh { get; set; }

        public string GioiTinh { get; set; }

        public string Email { get; set; }

        public string MaLop { get; set; }

        // Thuoc tinh hien thi ten lop
        public string TenLop { get; set; }
    }
}
