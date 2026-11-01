using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RESULT_ANALYTICS.Models
{
    internal class HocKy
    {
        // Ma hoc ky
        public string MaHocKy { get; set; }

        // Ten hoc ky
        public string TenHocKy { get; set; }

        // Nam hoc
        public string NamHoc { get; set; }

        // Ngay bat dau
        public DateTime? NgayBatDau { get; set; }

        // Ngay ket thuc
        public DateTime? NgayKetThuc { get; set; }
    }
}

