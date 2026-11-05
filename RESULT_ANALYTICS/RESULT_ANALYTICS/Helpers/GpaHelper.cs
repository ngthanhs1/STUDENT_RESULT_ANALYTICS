using System;

namespace RESULT_ANALYTICS.Helpers
{
    /// <summary>
    /// Helper quy doi diem sang he chu va he 4.0 theo Quy che dao tao dai hoc tin chi (Thong tu 08/2021/TT-BGDDT)
    /// </summary>
    internal static class GpaHelper
    {
        /// <summary>
        /// Quy doi diem thang 10 sang diem chu (A+, A, B+, B, C+, C, D+, D, F)
        /// </summary>
        public static string QuyDoiDiemChu(decimal? diem10)
        {
            if (!diem10.HasValue) return "";

            decimal d = diem10.Value;
            if (d >= 9.0m) return "A+";
            if (d >= 8.5m) return "A";
            if (d >= 8.0m) return "B+";
            if (d >= 7.0m) return "B";
            if (d >= 6.5m) return "C+";
            if (d >= 5.5m) return "C";
            if (d >= 5.0m) return "D+";
            if (d >= 4.0m) return "D";
            return "F";
        }

        /// <summary>
        /// Quy doi diem thang 10 sang diem he 4 (4.0, 3.7, 3.5, 3.0, 2.5, 2.0, 1.5, 1.0, 0.0)
        /// </summary>
        public static decimal? QuyDoiDiemHe4(decimal? diem10)
        {
            if (!diem10.HasValue) return null;

            decimal d = diem10.Value;
            if (d >= 9.0m) return 4.0m;
            if (d >= 8.5m) return 3.7m;
            if (d >= 8.0m) return 3.5m;
            if (d >= 7.0m) return 3.0m;
            if (d >= 6.5m) return 2.5m;
            if (d >= 5.5m) return 2.0m;
            if (d >= 5.0m) return 1.5m;
            if (d >= 4.0m) return 1.0m;
            return 0.0m;
        }

        /// <summary>
        /// Kiem tra mon hoc Dat hay Truot (Tu 4.0 tro len la Dat)
        /// </summary>
        public static string LayKetQuaDatTruot(decimal? diem10)
        {
            if (!diem10.HasValue) return "";
            return diem10.Value >= 4.0m ? "Đạt" : "Trượt";
        }

        /// <summary>
        /// Xep loai hoc luc dua theo GPA He 4 (Theo quy che tin chi dai hoc)
        /// </summary>
        public static string XepLoaiTheoGpaHe4(decimal? gpaHe4)
        {
            if (!gpaHe4.HasValue) return "Chưa đủ điểm";

            decimal g = gpaHe4.Value;
            if (g >= 3.60m) return "Xuất sắc";
            if (g >= 3.20m) return "Giỏi";
            if (g >= 2.50m) return "Khá";
            if (g >= 2.00m) return "Trung bình";
            if (g >= 1.00m) return "Yếu";
            return "Kém";
        }
    }
}
