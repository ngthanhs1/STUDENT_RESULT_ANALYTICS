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
        // 1. Lay chi so KPI tong quan toan dien
        public ThongKeTongQuan LayThongKe()
        {
            ThongKeTongQuan thongKe = new ThongKeTongQuan();

            string sql = @"
                SELECT
                    (SELECT COUNT(*) FROM dbo.SinhVien) AS TongSinhVien,
                    (SELECT COUNT(*) FROM dbo.Lop) AS TongLop,
                    (SELECT COUNT(*) FROM dbo.MonHoc) AS TongMonHoc,
                    (SELECT COUNT(DISTINCT MaSV) FROM dbo.vw_SinhVienThieuDiem) AS SoSinhVienThieuDiem,
                    (SELECT ISNULL(CAST(AVG(DiemTrungBinh) AS DECIMAL(4,2)), 0) 
                     FROM dbo.vw_PhanLoaiHocLuc WHERE DiemTrungBinh > 0) AS DiemTBChung,
                    (SELECT CASE WHEN (SUM(SoSinhVienDat) + SUM(SoSinhVienTruot)) > 0 
                                 THEN CAST(SUM(SoSinhVienDat) * 100.0 / (SUM(SoSinhVienDat) + SUM(SoSinhVienTruot)) AS DECIMAL(5,1)) 
                                 ELSE 0 END 
                     FROM dbo.vw_ThongKeDatTruot) AS TyLeDatChung";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            thongKe.TongSinhVien = Convert.ToInt32(reader["TongSinhVien"]);
                            thongKe.TongLop = Convert.ToInt32(reader["TongLop"]);
                            thongKe.TongMonHoc = Convert.ToInt32(reader["TongMonHoc"]);
                            thongKe.SoSinhVienThieuDiem = Convert.ToInt32(reader["SoSinhVienThieuDiem"]);
                            thongKe.DiemTBChung = reader["DiemTBChung"] != DBNull.Value ? Convert.ToDecimal(reader["DiemTBChung"]) : 0;
                            thongKe.TyLeDatChung = reader["TyLeDatChung"] != DBNull.Value ? Convert.ToDecimal(reader["TyLeDatChung"]) : 0;
                        }
                    }
                }
            }

            return thongKe;
        }

        // 2. Lay phan bo hoc luc toan khoa
        public List<PhanBoHocLucDashboard> LayPhanBoHocLuc()
        {
            List<PhanBoHocLucDashboard> danhSach = new List<PhanBoHocLucDashboard>();

            string sql = @"
                SELECT XepLoai, COUNT(*) AS SoLuong
                FROM dbo.vw_PhanLoaiHocLuc
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
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string loai = reader["XepLoai"] != DBNull.Value ? reader["XepLoai"].ToString() : "Chưa đủ điểm";
                            int sl = Convert.ToInt32(reader["SoLuong"]);
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

        // 3. Lay danh sach ket qua Dat / Truot theo hoc phan
        public List<ThongKeDatTruot> LayMonHocDatTruot()
        {
            List<ThongKeDatTruot> danhSach = new List<ThongKeDatTruot>();

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
                ORDER BY TyLeDat ASC, MaMonHoc";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ThongKeDatTruot item = new ThongKeDatTruot
                            {
                                MaMonHoc = reader["MaMonHoc"].ToString(),
                                TenMonHoc = reader["TenMonHoc"].ToString(),
                                SoTinChi = Convert.ToInt32(reader["SoTinChi"]),
                                MaHocKy = reader["MaHocKy"].ToString(),
                                TenHocKy = reader["TenHocKy"].ToString(),
                                NamHoc = reader["NamHoc"].ToString(),
                                TongSinhVien = Convert.ToInt32(reader["TongSinhVien"]),
                                SoSinhVienDat = Convert.ToInt32(reader["SoSinhVienDat"]),
                                SoSinhVienTruot = Convert.ToInt32(reader["SoSinhVienTruot"]),
                                SoSinhVienThieuDiem = Convert.ToInt32(reader["SoSinhVienThieuDiem"]),
                                TyLeDat = Convert.ToDecimal(reader["TyLeDat"])
                            };

                            danhSach.Add(item);
                        }
                    }
                }
            }

            return danhSach;
        }

        // 4. Lay danh sach sinh vien thieu diem / canh bao
        public List<CanhBaoThieuDiem> LayDanhSachCanhBao()
        {
            List<CanhBaoThieuDiem> danhSach = new List<CanhBaoThieuDiem>();

            string sql = @"
                SELECT TOP (10)
                    MaSV,
                    HoTen,
                    MaMonHoc,
                    TenMonHoc,
                    MaHocKy,
                    NoiDungCanhBao
                FROM dbo.vw_SinhVienThieuDiem
                ORDER BY MaSV, MaMonHoc, MaHocKy";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CanhBaoThieuDiem canhBao = new CanhBaoThieuDiem
                            {
                                MaSV = reader["MaSV"].ToString(),
                                HoTen = reader["HoTen"].ToString(),
                                MaMonHoc = reader["MaMonHoc"].ToString(),
                                TenMonHoc = reader["TenMonHoc"].ToString(),
                                MaHocKy = reader["MaHocKy"].ToString(),
                                NoiDungCanhBao = reader["NoiDungCanhBao"].ToString()
                            };

                            danhSach.Add(canhBao);
                        }
                    }
                }
            }

            return danhSach;
        }
    }
}
