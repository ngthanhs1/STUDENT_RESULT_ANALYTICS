using System;
using RESULT_ANALYTICS.Helpers;

namespace RESULT_ANALYTICS.Models
{
    internal class KetQua
    {
        // Ma ket qua
        public int MaKetQua { get; set; }

        // Thong tin sinh vien
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public string HoTenSinhVien => HoTen;

        // Thong tin mon hoc
        public string MaMonHoc { get; set; }
        public string TenMonHoc { get; set; }

        // Thong tin hoc ky
        public string MaHocKy { get; set; }
        public string TenHocKy { get; set; }

        // Ty le tinh diem: 10-30-60 (Ly thuyet) hoac 10-40-50 (Co thuc hanh)
        public string TyLeDiem { get; set; } = "10-30-60";
        public string TyLeHienThi => TyLeDiem == "10-40-50" ? "10-40-50 (TH)" : "10-30-60 (LT)";

        // Diem thanh phan
        public decimal? DiemChuyenCan { get; set; }
        public decimal? DiemGiuaKy { get; set; }
        public decimal? DiemCuoiKy { get; set; }

        // Diem tong ket do SQL Server tu tinh
        public decimal? DiemTongKet { get; set; }

        // Quy doi diem chu (A, B+, B, C+, C, D+, D, F)
        public string DiemChu => GpaHelper.QuyDoiDiemChu(DiemTongKet);

        // Quy doi GPA thang 4.0 (4.0, 3.5, 3.0, 2.5, 2.0, 1.5, 1.0, 0.0)
        public decimal? DiemHe4 => GpaHelper.QuyDoiDiemHe4(DiemTongKet);

        // Ket qua mon hoc: Dat / Truot
        public string KetQuaMon => GpaHelper.LayKetQuaDatTruot(DiemTongKet);
    }
}
