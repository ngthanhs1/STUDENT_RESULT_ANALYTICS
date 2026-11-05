using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using RESULT_ANALYTICS.Models;

namespace RESULT_ANALYTICS.DataAccess
{
    internal class HocKyRepository
    {
        // Lay danh sach cac muc chon hoc ky cho ComboBox
        public List<MucChonHocKy> LayDanhSachMucChon(bool kemMucTatCa = true)
        {
            List<MucChonHocKy> danhSach = new List<MucChonHocKy>();
            if (kemMucTatCa)
            {
                danhSach.Add(new MucChonHocKy
                {
                    MaHocKy = "",
                    TenHienThi = "⭐ Tất cả học kỳ (Toàn khóa)"
                });
            }

            var danhSachHk = LayDanhSach()
                .OrderByDescending(x => x.NamHoc)
                .ThenByDescending(x => x.TenHocKy)
                .ToList();

            foreach (var hk in danhSachHk)
            {
                danhSach.Add(new MucChonHocKy
                {
                    MaHocKy = hk.MaHocKy,
                    TenHienThi = $"{hk.TenHocKy} ({hk.NamHoc})"
                });
            }

            return danhSach;
        }

        // Lay danh sach hoc ky tu SQL Server
        public List<HocKy> LayDanhSach()
        {
            List<HocKy> danhSach = new List<HocKy>();

            string sql = @"SELECT MaHocKy, TenHocKy, NamHoc,
                                  NgayBatDau, NgayKetThuc
                           FROM dbo.HocKy
                           ORDER BY NamHoc DESC, TenHocKy DESC";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            HocKy hocKy = new HocKy
                            {
                                MaHocKy = reader["MaHocKy"].ToString(),
                                TenHocKy = reader["TenHocKy"].ToString(),
                                NamHoc = reader["NamHoc"].ToString(),

                                NgayBatDau = reader["NgayBatDau"] == DBNull.Value
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(reader["NgayBatDau"]),

                                NgayKetThuc = reader["NgayKetThuc"] == DBNull.Value
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(reader["NgayKetThuc"])
                            };

                            danhSach.Add(hocKy);
                        }
                    }
                }
            }

            return danhSach;
        }

        // Them hoc ky moi
        public bool Them(HocKy hocKy)
        {
            KiemTraDuLieu(hocKy);

            string sql = @"INSERT INTO dbo.HocKy
                           (MaHocKy, TenHocKy, NamHoc,
                            NgayBatDau, NgayKetThuc)
                           VALUES
                           (@MaHocKy, @TenHocKy, @NamHoc,
                            @NgayBatDau, @NgayKetThuc)";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    GanThamSo(cmd, hocKy);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Sua thong tin hoc ky
        public bool Sua(HocKy hocKy)
        {
            KiemTraDuLieu(hocKy);

            string sql = @"UPDATE dbo.HocKy
                           SET TenHocKy = @TenHocKy,
                               NamHoc = @NamHoc,
                               NgayBatDau = @NgayBatDau,
                               NgayKetThuc = @NgayKetThuc
                           WHERE MaHocKy = @MaHocKy";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    GanThamSo(cmd, hocKy);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Xoa hoc ky theo ma
        public bool Xoa(string maHocKy)
        {
            if (string.IsNullOrWhiteSpace(maHocKy))
            {
                throw new ArgumentException("Mã học kỳ không được để trống.");
            }

            string sql = @"DELETE FROM dbo.HocKy
                           WHERE MaHocKy = @MaHocKy";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaHocKy", SqlDbType.VarChar, 20)
                        .Value = maHocKy.Trim();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Gan tham so cho cau lenh SQL
        private void GanThamSo(SqlCommand cmd, HocKy hocKy)
        {
            cmd.Parameters.Add("@MaHocKy", SqlDbType.VarChar, 20)
                .Value = hocKy.MaHocKy.Trim();

            cmd.Parameters.Add("@TenHocKy", SqlDbType.NVarChar, 50)
                .Value = hocKy.TenHocKy.Trim();

            cmd.Parameters.Add("@NamHoc", SqlDbType.VarChar, 9)
                .Value = hocKy.NamHoc.Trim();

            cmd.Parameters.Add("@NgayBatDau", SqlDbType.Date)
                .Value = hocKy.NgayBatDau.HasValue
                    ? (object)hocKy.NgayBatDau.Value.Date
                    : DBNull.Value;

            cmd.Parameters.Add("@NgayKetThuc", SqlDbType.Date)
                .Value = hocKy.NgayKetThuc.HasValue
                    ? (object)hocKy.NgayKetThuc.Value.Date
                    : DBNull.Value;
        }

        // Kiem tra du lieu hoc ky
        private void KiemTraDuLieu(HocKy hocKy)
        {
            if (hocKy == null)
            {
                throw new ArgumentNullException(nameof(hocKy));
            }

            if (string.IsNullOrWhiteSpace(hocKy.MaHocKy))
            {
                throw new ArgumentException("Mã học kỳ không được để trống.");
            }

            if (hocKy.MaHocKy.Trim().Length > 20)
            {
                throw new ArgumentException("Mã học kỳ tối đa 20 ký tự.");
            }

            if (string.IsNullOrWhiteSpace(hocKy.TenHocKy))
            {
                throw new ArgumentException("Tên học kỳ không được để trống.");
            }

            if (hocKy.TenHocKy.Trim().Length > 50)
            {
                throw new ArgumentException("Tên học kỳ tối đa 50 ký tự.");
            }

            if (string.IsNullOrWhiteSpace(hocKy.NamHoc))
            {
                throw new ArgumentException("Năm học không được để trống.");
            }

            if (hocKy.NamHoc.Trim().Length > 9)
            {
                throw new ArgumentException("Năm học tối đa 9 ký tự.");
            }

            if (hocKy.NgayBatDau.HasValue && hocKy.NgayKetThuc.HasValue)
            {
                if (hocKy.NgayKetThuc.Value.Date < hocKy.NgayBatDau.Value.Date)
                {
                    throw new ArgumentException(
                        "Ngày kết thúc không được nhỏ hơn ngày bắt đầu.");
                }
            }
        }
    }
}
