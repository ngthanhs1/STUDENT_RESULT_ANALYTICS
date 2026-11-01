
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
    internal class TaiKhoanRepository
    {
        // Tim tai khoan theo ten dang nhap
        public TaiKhoan LayTheoTenDangNhap(string tenDangNhap)
        {
            TaiKhoan taiKhoan = null;

            string sql = @"
                SELECT TOP (1)
                    MaTaiKhoan,
                    TenDangNhap,
                    MatKhauHash,
                    VaiTro,
                    TrangThai,
                    NgayTao
                FROM dbo.TaiKhoan
                WHERE TenDangNhap = @TenDangNhap";

            using (SqlConnection conn =
                DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@TenDangNhap",
                        SqlDbType.VarChar,
                        50).Value = tenDangNhap;

                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            taiKhoan = new TaiKhoan();

                            taiKhoan.MaTaiKhoan =
                                Convert.ToInt32(
                                    reader["MaTaiKhoan"]);

                            taiKhoan.TenDangNhap =
                                reader["TenDangNhap"].ToString();

                            taiKhoan.MatKhauHash =
                                reader["MatKhauHash"].ToString();

                            taiKhoan.VaiTro =
                                reader["VaiTro"].ToString();

                            taiKhoan.TrangThai =
                                Convert.ToBoolean(
                                    reader["TrangThai"]);

                            taiKhoan.NgayTao =
                                Convert.ToDateTime(
                                    reader["NgayTao"]);
                        }
                    }
                }
            }

            return taiKhoan;
        }
    }
}
