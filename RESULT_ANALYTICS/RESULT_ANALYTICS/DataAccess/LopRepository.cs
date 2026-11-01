
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
    internal class LopRepository
    {
        // Lay danh sach tat ca cac lop
        public List<Lop> LayDanhSach()
        {
            List<Lop> danhSach = new List<Lop>();

            string sql = @"SELECT MaLop, TenLop, KhoaHoc
                           FROM dbo.Lop
                           ORDER BY MaLop";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Lop lop = new Lop();

                            lop.MaLop = reader["MaLop"].ToString();
                            lop.TenLop = reader["TenLop"].ToString();

                            lop.KhoaHoc = reader["KhoaHoc"] == DBNull.Value
                                ? null
                                : reader["KhoaHoc"].ToString();

                            danhSach.Add(lop);
                        }
                    }
                }
            }

            return danhSach;
        }

        // Them lop moi
        public bool Them(Lop lop)
        {
            KiemTraDuLieu(lop);

            string sql = @"INSERT INTO dbo.Lop
                           (MaLop, TenLop, KhoaHoc)
                           VALUES
                           (@MaLop, @TenLop, @KhoaHoc)";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaLop", SqlDbType.VarChar, 20).Value = lop.MaLop.Trim();

                    cmd.Parameters.Add("@TenLop", SqlDbType.NVarChar, 100).Value = lop.TenLop.Trim();

                    cmd.Parameters.Add("@KhoaHoc", SqlDbType.VarChar, 20).Value =
                        string.IsNullOrWhiteSpace(lop.KhoaHoc)
                        ? (object)DBNull.Value
                        : lop.KhoaHoc.Trim();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Sua thong tin lop
        public bool Sua(Lop lop)
        {
            KiemTraDuLieu(lop);

            string sql = @"UPDATE dbo.Lop
                           SET TenLop = @TenLop,
                               KhoaHoc = @KhoaHoc
                           WHERE MaLop = @MaLop";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaLop", SqlDbType.VarChar, 20).Value = lop.MaLop.Trim();

                    cmd.Parameters.Add("@TenLop", SqlDbType.NVarChar, 100).Value = lop.TenLop.Trim();

                    cmd.Parameters.Add("@KhoaHoc", SqlDbType.VarChar, 20).Value =
                        string.IsNullOrWhiteSpace(lop.KhoaHoc)
                        ? (object)DBNull.Value
                        : lop.KhoaHoc.Trim();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Xoa lop theo ma lop
        public bool Xoa(string maLop)
        {
            if (string.IsNullOrWhiteSpace(maLop))
            {
                throw new ArgumentException("Mã lớp không được để trống!");
            }

            string sql = @"DELETE FROM dbo.Lop
                           WHERE MaLop = @MaLop";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaLop", SqlDbType.VarChar, 20).Value = maLop.Trim();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Tim kiem lop theo ma, ten va khoa hoc
        public List<Lop> TimKiem(string tuKhoa)
        {
            List<Lop> danhSach = new List<Lop>();

            string sql = @"SELECT MaLop, TenLop, KhoaHoc
                           FROM dbo.Lop
                           WHERE MaLop LIKE @TuKhoa
                              OR TenLop LIKE @TuKhoa
                              OR KhoaHoc LIKE @TuKhoa
                           ORDER BY MaLop";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@TuKhoa", SqlDbType.NVarChar, 4000).Value =
                        "%" + (tuKhoa ?? "").Trim() + "%";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Lop lop = new Lop();

                            lop.MaLop = reader["MaLop"].ToString();
                            lop.TenLop = reader["TenLop"].ToString();

                            lop.KhoaHoc = reader["KhoaHoc"] == DBNull.Value
                                ? null
                                : reader["KhoaHoc"].ToString();

                            danhSach.Add(lop);
                        }
                    }
                }
            }

            return danhSach;
        }

        // Kiem tra du lieu truoc khi luu
        private void KiemTraDuLieu(Lop lop)
        {
            if (lop == null)
            {
                throw new ArgumentNullException(nameof(lop));
            }

            if (string.IsNullOrWhiteSpace(lop.MaLop))
            {
                throw new ArgumentException("Mã lớp không được để trống!");
            }

            if (string.IsNullOrWhiteSpace(lop.TenLop))
            {
                throw new ArgumentException("Tên lớp không được để trống!");
            }

            if (lop.MaLop.Trim().Length > 20)
            {
                throw new ArgumentException("Mã lớp tối đa 20 ký tự!");
            }

            if (lop.TenLop.Trim().Length > 100)
            {
                throw new ArgumentException("Tên lớp tối đa 100 ký tự!");
            }

            if (!string.IsNullOrWhiteSpace(lop.KhoaHoc) &&
                lop.KhoaHoc.Trim().Length > 20)
            {
                throw new ArgumentException("Khóa học tối đa 20 ký tự!");
            }
        }
    }
}
