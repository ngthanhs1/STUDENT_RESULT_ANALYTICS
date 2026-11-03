using RESULT_ANALYTICS.Helpers;

namespace RESULT_ANALYTICS.Models
{
    internal class DiemTrungBinhHocKy
    {
        // Thong tin sinh vien
        public string MaSV { get; set; }
        public string HoTen { get; set; }
        public string MaLop { get; set; }

        // Thong tin hoc ky
        public string MaHocKy { get; set; }
        public string TenHocKy { get; set; }
        public string NamHoc { get; set; }

        // Thong ke mon hoc va tin chi
        public int TongSoMon { get; set; }
        public int TongSoTinChi { get; set; }
        public int SoMonThieuDiem { get; set; }
        public int SoTinChiDaCoDiem { get; set; }

        // Diem trung binh thang 10
        public decimal? DiemTrungBinhTamTinh { get; set; }

        // Diem trung binh GPA he 4.0
        public decimal? DiemTrungBinhHe4 { get; set; }

        // Diem chu tuong ung
        public string DiemChu => GpaHelper.QuyDoiDiemChu(DiemTrungBinhTamTinh);

        // Du diem hay chua du diem
        public string TrangThai { get; set; }
    }
}
