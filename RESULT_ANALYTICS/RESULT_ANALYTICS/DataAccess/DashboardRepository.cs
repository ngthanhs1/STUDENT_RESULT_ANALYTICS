using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using RESULT_ANALYTICS.Models;

namespace RESULT_ANALYTICS.DataAccess
{
    internal class DashboardRepository
    {
        // 1. Lay chi so KPI tong quan toan dien (Co ho tro loc theo hoc ky)
        public ThongKeTongQuan LayThongKe(string maHocKy = null)
        {
            ThongKeTongQuan thongKe = new ThongKeTongQuan();
            bool coHocKy = !string.IsNullOrWhiteSpace(maHocKy);

            string sql = @"
                SELECT
                    CASE WHEN @MaHocKy IS NOT NULL 
                         THEN (SELECT COUNT(DISTINCT MaSV) FROM dbo.KetQua WHERE MaHocKy = @MaHocKy)
                         ELSE (SELECT COUNT(*) FROM dbo.SinhVien) END AS TongSinhVien,
                    (SELECT COUNT(*) FROM dbo.Lop) AS TongLop,
                    CASE WHEN @MaHocKy IS NOT NULL 
                         THEN (SELECT COUNT(DISTINCT MaMonHoc) FROM dbo.KetQua WHERE MaHocKy = @MaHocKy)
                         ELSE (SELECT COUNT(*) FROM dbo.MonHoc) END AS TongMonHoc,
                    (SELECT COUNT(DISTINCT MaSV) FROM dbo.vw_SinhVienThieuDiem 
                     WHERE (@MaHocKy IS NULL OR MaHocKy = @MaHocKy)) AS SoSinhVienThieuDiem,
                    (SELECT ISNULL(CAST(AVG(DiemTrungBinh) AS DECIMAL(4,2)), 0) 
                     FROM dbo.vw_PhanLoaiHocLuc 
                     WHERE DiemTrungBinh > 0 AND (@MaHocKy IS NULL OR MaHocKy = @MaHocKy)) AS DiemTBChung,
                    (SELECT CASE WHEN (SUM(SoSinhVienDat) + SUM(SoSinhVienTruot)) > 0 
                                 THEN CAST(SUM(SoSinhVienDat) * 100.0 / (SUM(SoSinhVienDat) + SUM(SoSinhVienTruot)) AS DECIMAL(5,1)) 
                                 ELSE 0 END 
                     FROM dbo.vw_ThongKeDatTruot 
                     WHERE (@MaHocKy IS NULL OR MaHocKy = @MaHocKy)) AS TyLeDatChung";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaHocKy", SqlDbType.VarChar, 20)
                        .Value = coHocKy ? (object)maHocKy.Trim() : DBNull.Value;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            thongKe.TongSinhVien = DocSoNguyen(reader, "TongSinhVien");
                            thongKe.TongLop = DocSoNguyen(reader, "TongLop");
                            thongKe.TongMonHoc = DocSoNguyen(reader, "TongMonHoc");
                            thongKe.SoSinhVienThieuDiem = DocSoNguyen(reader, "SoSinhVienThieuDiem");
                            thongKe.DiemTBChung = DocSoThapPhan(reader, "DiemTBChung");
                            thongKe.TyLeDatChung = DocSoThapPhan(reader, "TyLeDatChung");
                        }
                    }
                }
            }

            return thongKe;
        }

        // 2. Lay phan bo hoc luc toan khoa hoac theo hoc ky
        public List<PhanBoHocLucDashboard> LayPhanBoHocLuc(string maHocKy = null)
        {
            List<PhanBoHocLucDashboard> danhSach = new List<PhanBoHocLucDashboard>();
            bool coHocKy = !string.IsNullOrWhiteSpace(maHocKy);

            string sql = @"
                SELECT XepLoai, COUNT(*) AS SoLuong
                FROM dbo.vw_PhanLoaiHocLuc
                WHERE (@MaHocKy IS NULL OR MaHocKy = @MaHocKy)
                GROUP BY XepLoai
                ORDER BY 
                    CASE XepLoai 
                        WHEN N'Xuất sắc' THEN 1
                        WHEN N'Giỏi' THEN 2
                        WHEN N'Khá' THEN 3
                        WHEN N'Trung bình' THEN 4
                        WHEN N'Yếu' THEN 5
                        WHEN N'Kém' THEN 6
                        ELSE 7
                    END";

            int tong = 0;
            Dictionary<string, int> data = new Dictionary<string, int>();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaHocKy", SqlDbType.VarChar, 20)
                        .Value = coHocKy ? (object)maHocKy.Trim() : DBNull.Value;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string loai = reader["XepLoai"] != DBNull.Value ? reader["XepLoai"].ToString() : "Chưa đủ điểm";
                            int sl = DocSoNguyen(reader, "SoLuong");
                            data[loai] = sl;
                            tong += sl;
                        }
                    }
                }
            }

            // Dinh danh mau sac truc quan cho tung muc
            foreach (var kv in data)
            {
                double tyLe = tong > 0 ? (kv.Value * 100.0 / tong) : 0;
                string mau = "#3B82F6"; // default blue
                if (kv.Key.Contains("Xuất sắc")) mau = "#8B5CF6"; // tim
                else if (kv.Key.Contains("Giỏi")) mau = "#10B981"; // xanh la
                else if (kv.Key.Contains("Khá")) mau = "#3B82F6"; // xanh duong
                else if (kv.Key.Contains("Trung bình")) mau = "#F59E0B"; // vang cam
                else if (kv.Key.Contains("Yếu") || kv.Key.Contains("Kém")) mau = "#EF4444"; // do
                else if (kv.Key.Contains("Chưa") || kv.Key.Contains("thiếu")) mau = "#F97316"; // cam dam

                double barWidth = (tyLe / 100.0) * 180.0;
                if (barWidth < 10 && kv.Value > 0) barWidth = 10;

                danhSach.Add(new PhanBoHocLucDashboard
                {
                    XepLoai = kv.Key,
                    SoLuong = kv.Value,
                    TyLe = $"{tyLe:0.1}%",
                    ChieuRongBar = barWidth,
                    MauSac = mau
                });
            }

            return danhSach;
        }

        // 3. Lay danh sach ket qua Dat / Truot theo hoc phan (hoac theo hoc ky)
        public List<ThongKeDatTruot> LayMonHocDatTruot(string maHocKy = null)
        {
            List<ThongKeDatTruot> danhSach = new List<ThongKeDatTruot>();
            bool coHocKy = !string.IsNullOrWhiteSpace(maHocKy);

            string sql = @"
                SELECT TOP (8)
                    MaMonHoc,
                    TenMonHoc,
                    SoTinChi,
                    MaHocKy,
                    TenHocKy,
                    NamHoc,
                    TongSinhVien,
                    SoSinhVienDat,
                    SoSinhVienTruot,
                    SoSinhVienThieuDiem,
                    TyLeDat
                FROM dbo.vw_ThongKeDatTruot
                WHERE (@MaHocKy IS NULL OR MaHocKy = @MaHocKy)
                ORDER BY TyLeDat ASC, MaMonHoc";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaHocKy", SqlDbType.VarChar, 20)
                        .Value = coHocKy ? (object)maHocKy.Trim() : DBNull.Value;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ThongKeDatTruot item = new ThongKeDatTruot
                            {
                                MaMonHoc = DocChuoi(reader, "MaMonHoc"),
                                TenMonHoc = DocChuoi(reader, "TenMonHoc"),
                                SoTinChi = DocSoNguyen(reader, "SoTinChi"),
                                MaHocKy = DocChuoi(reader, "MaHocKy"),
                                TenHocKy = DocChuoi(reader, "TenHocKy"),
                                NamHoc = DocChuoi(reader, "NamHoc"),
                                TongSinhVien = DocSoNguyen(reader, "TongSinhVien"),
                                SoSinhVienDat = DocSoNguyen(reader, "SoSinhVienDat"),
                                SoSinhVienTruot = DocSoNguyen(reader, "SoSinhVienTruot"),
                                SoSinhVienThieuDiem = DocSoNguyen(reader, "SoSinhVienThieuDiem"),
                                TyLeDat = DocSoThapPhan(reader, "TyLeDat")
                            };

                            danhSach.Add(item);
                        }
                    }
                }
            }

            return danhSach;
        }

        // 4. Lay danh sach sinh vien thieu diem / canh bao (hoac theo hoc ky)
        public List<CanhBaoThieuDiem> LayDanhSachCanhBao(string maHocKy = null)
        {
            List<CanhBaoThieuDiem> danhSach = new List<CanhBaoThieuDiem>();
            bool coHocKy = !string.IsNullOrWhiteSpace(maHocKy);

            string sql = @"
                SELECT TOP (10)
                    MaSV,
                    HoTen,
                    MaMonHoc,
                    TenMonHoc,
                    MaHocKy,
                    NoiDungCanhBao
                FROM dbo.vw_SinhVienThieuDiem
                WHERE (@MaHocKy IS NULL OR MaHocKy = @MaHocKy)
                ORDER BY MaSV, MaMonHoc, MaHocKy";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaHocKy", SqlDbType.VarChar, 20)
                        .Value = coHocKy ? (object)maHocKy.Trim() : DBNull.Value;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CanhBaoThieuDiem canhBao = new CanhBaoThieuDiem
                            {
                                MaSV = DocChuoi(reader, "MaSV"),
                                HoTen = DocChuoi(reader, "HoTen"),
                                MaMonHoc = DocChuoi(reader, "MaMonHoc"),
                                TenMonHoc = DocChuoi(reader, "TenMonHoc"),
                                MaHocKy = DocChuoi(reader, "MaHocKy"),
                                NoiDungCanhBao = DocChuoi(reader, "NoiDungCanhBao")
                            };

                            danhSach.Add(canhBao);
                        }
                    }
                }
            }

            return danhSach;
        }

        // =========================================
        // HELPER FUNCTIONS DOC DU LIEU AN TOAN VOI DBNULL
        // =========================================
        private static string DocChuoi(SqlDataReader reader, string tenCot)
        {
            return reader[tenCot] == DBNull.Value ? "" : Convert.ToString(reader[tenCot]);
        }

        private static int DocSoNguyen(SqlDataReader reader, string tenCot)
        {
            return reader[tenCot] == DBNull.Value ? 0 : Convert.ToInt32(reader[tenCot]);
        }

        private static decimal DocSoThapPhan(SqlDataReader reader, string tenCot)
        {
            return reader[tenCot] == DBNull.Value ? 0m : Convert.ToDecimal(reader[tenCot]);
        }
    }
}
