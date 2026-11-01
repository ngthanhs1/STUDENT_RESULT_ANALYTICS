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
    internal class MonHocRepository
    {
        // Lay danh sach mon hoc tu SQL Server
        public List<MonHoc> LayDanhSach()
        {
            List<MonHoc> danhSach = new List<MonHoc>();

            string sql = @"SELECT MaMonHoc, TenMonHoc, SoTinChi
                           FROM dbo.MonHoc
                           ORDER BY MaMonHoc";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MonHoc monHoc = new MonHoc
                            {
                                MaMonHoc = reader["MaMonHoc"].ToString(),
                                TenMonHoc = reader["TenMonHoc"].ToString(),
                                SoTinChi = Convert.ToInt32(reader["SoTinChi"])
                            };

                            danhSach.Add(monHoc);
                        }
                    }
                }
            }

            return danhSach;
        }

        // Them mon hoc moi
        public bool Them(MonHoc monHoc)
        {
            KiemTraDuLieu(monHoc);

            string sql = @"INSERT INTO dbo.MonHoc
                           (MaMonHoc, TenMonHoc, SoTinChi)
                           VALUES
                           (@MaMonHoc, @TenMonHoc, @SoTinChi)";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaMonHoc", SqlDbType.VarChar, 20).Value = monHoc.MaMonHoc.Trim();
                    cmd.Parameters.Add("@TenMonHoc", SqlDbType.NVarChar, 150).Value = monHoc.TenMonHoc.Trim();
                    cmd.Parameters.Add("@SoTinChi", SqlDbType.Int).Value = monHoc.SoTinChi;

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Sua thong tin mon hoc
        public bool Sua(MonHoc monHoc)
        {
            KiemTraDuLieu(monHoc);

            string sql = @"UPDATE dbo.MonHoc
                           SET TenMonHoc = @TenMonHoc,
                               SoTinChi = @SoTinChi
                           WHERE MaMonHoc = @MaMonHoc";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaMonHoc", SqlDbType.VarChar, 20).Value = monHoc.MaMonHoc.Trim();
                    cmd.Parameters.Add("@TenMonHoc", SqlDbType.NVarChar, 150).Value = monHoc.TenMonHoc.Trim();
                    cmd.Parameters.Add("@SoTinChi", SqlDbType.Int).Value = monHoc.SoTinChi;

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Xoa mon hoc theo ma
        public bool Xoa(string maMonHoc)
        {
            if (string.IsNullOrWhiteSpace(maMonHoc))
            {
                throw new ArgumentException("Mã môn học không được để trống.");
            }

            string sql = @"DELETE FROM dbo.MonHoc
                           WHERE MaMonHoc = @MaMonHoc";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaMonHoc", SqlDbType.VarChar, 20).Value = maMonHoc.Trim();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Kiem tra du lieu truoc khi luu
        private void KiemTraDuLieu(MonHoc monHoc)
        {
            if (monHoc == null)
            {
                throw new ArgumentNullException(nameof(monHoc));
            }

            if (string.IsNullOrWhiteSpace(monHoc.MaMonHoc))
            {
                throw new ArgumentException("Mã môn học không được để trống.");
            }

            if (monHoc.MaMonHoc.Trim().Length > 20)
            {
                throw new ArgumentException("Mã môn học tối đa 20 ký tự.");
            }

            if (string.IsNullOrWhiteSpace(monHoc.TenMonHoc))
            {
                throw new ArgumentException("Tên môn học không được để trống.");
            }

            if (monHoc.TenMonHoc.Trim().Length > 150)
            {
                throw new ArgumentException("Tên môn học tối đa 150 ký tự.");
            }

            if (monHoc.SoTinChi < 1 || monHoc.SoTinChi > 10)
            {
                throw new ArgumentException("Số tín chỉ phải từ 1 đến 10.");
            }
        }
    }
}
