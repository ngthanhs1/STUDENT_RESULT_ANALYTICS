using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESULT_ANALYTICS.Models
{
    internal class Lop
    {
        public string MaLop { get; set; }

        public string TenLop { get; set; }

        public string KhoaHoc { get; set; }

        public string Khoa { get; set; }

        // Ten hien thi truc quan tren giao dien: MaLop - TenLop (Khoa)
        public string TenHienThi
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Khoa))
                    return $"{MaLop} - {TenLop} ({Khoa})";
                return $"{MaLop} - {TenLop}";
            }
        }
    }
}
