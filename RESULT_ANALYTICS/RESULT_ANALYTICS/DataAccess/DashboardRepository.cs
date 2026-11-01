
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
        // Lay so lieu tong quan
        public ThongKeTongQuan LayThongKe()
        {
            ThongKeTongQuan thongKe = new ThongKeTongQuan();

            string sql = @"
                SELECT
                    (SELECT COUNT(*) FROM dbo.SinhVien) AS TongSinhVien,
                    (SELECT COUNT(*) FROM dbo.Lop) AS TongLop,
                    (SELECT COUNT(*) FROM dbo.MonHoc) AS TongMonHoc,
                    (SELECT COUNT(DISTINCT MaSV)
                     FROM dbo.vw_SinhVienThieuDiem) AS SoSinhVienThieuDiem";

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
                        }
                    }
                }
            }

            return thongKe;
        }

        // Lay danh sach sinh vien thieu diem
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
                            CanhBaoThieuDiem canhBao = new CanhBaoThieuDiem();

                            canhBao.MaSV = reader["MaSV"].ToString();
                            canhBao.HoTen = reader["HoTen"].ToString();
                            canhBao.MaMonHoc = reader["MaMonHoc"].ToString();
                            canhBao.TenMonHoc = reader["TenMonHoc"].ToString();
                            canhBao.MaHocKy = reader["MaHocKy"].ToString();
                            canhBao.NoiDungCanhBao = reader["NoiDungCanhBao"].ToString();

                            danhSach.Add(canhBao);
                        }
                    }
                }
            }

            return danhSach;
        }
    }
}
