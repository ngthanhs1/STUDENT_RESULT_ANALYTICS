
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RESULT_ANALYTICS.Models;

namespace RESULT_ANALYTICS.DataAccess
{
    internal class LichSuNhapDuLieuRepository
    {
        // =======================================
        // 1. LAY DANH SACH LICH SU IMPORT
        // =======================================
        public List<LichSuNhapDuLieu> LayDanhSach()
        {
            List<LichSuNhapDuLieu> danhSach =
                new List<LichSuNhapDuLieu>();


            string sql = @"
                SELECT TOP (100)
                    MaLichSu,
                    TenFile,
                    LoaiFile,
                    ThoiGianNhap,
                    TongSoDong,
                    SoDongThanhCong AS SoDongThanh,
                    SoDongLoi,
                    TrangThai
                FROM dbo.LichSuNhapDuLieu
                ORDER BY ThoiGianNhap DESC, MaLichSu DESC";


            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        LichSuNhapDuLieu item =
                            new LichSuNhapDuLieu();

                        item.MaLichSu =
                            Convert.ToInt32(reader["MaLichSu"]);

                        item.TenFile =
                            Convert.ToString(reader["TenFile"]);

                        item.LoaiFile =
                            Convert.ToString(reader["LoaiFile"]);

                        item.ThoiGianNhap =
                            Convert.ToDateTime(reader["ThoiGianNhap"]);

                        item.TongSoDong =
                            Convert.ToInt32(reader["TongSoDong"]);

                        item.SoDongThanh =
                            Convert.ToInt32(reader["SoDongThanh"]);

                        item.SoDongLoi =
                            Convert.ToInt32(reader["SoDongLoi"]);

                        item.TrangThai =
                            Convert.ToString(reader["TrangThai"]);

                        danhSach.Add(item);
                    }
                }
            }

            return danhSach;
        }

        // =======================================
        // 2. GHI LICH SU IMPORT
        // =======================================
        public bool GhiLichSu(LichSuNhapDuLieu lichSu)
        {
            if (lichSu == null)
                throw new ArgumentNullException(nameof(lichSu));

            if (string.IsNullOrWhiteSpace(lichSu.TenFile) ||
                lichSu.TenFile.Length > 255)
                throw new ArgumentException(
                    "Tên file không hợp lệ!");

            if (lichSu.TongSoDong < 0 ||
                lichSu.SoDongThanh < 0 ||
                lichSu.SoDongLoi < 0 ||
                lichSu.SoDongThanh + lichSu.SoDongLoi
                    > lichSu.TongSoDong)
                throw new ArgumentException(
                    "Số dòng Import không hợp lệ!");

            string loaiFile =
                (lichSu.LoaiFile ?? "").Trim().ToUpperInvariant();

            if (loaiFile != "CSV" && loaiFile != "XLSX")
                throw new ArgumentException(
                    "Loại file phải là CSV hoặc XLSX!");


            string trangThai = (lichSu.TrangThai ?? "").Trim();

            if (trangThai != "ThanhCong" &&
                trangThai != "MotPhan" &&
                trangThai != "Loi")
            {
                throw new ArgumentException(
                    "Trạng thái Import không hợp lệ!");
            }




            string sql = @"
                INSERT INTO dbo.LichSuNhapDuLieu
                (
                    TenFile,
                    LoaiFile,
                    ThoiGianNhap,
                    TongSoDong,
                    SoDongThanhCong,
                    SoDongLoi,
                    TrangThai
                )
                VALUES
                (
                    @TenFile,
                    @LoaiFile,
                    SYSDATETIME(),
                    @TongSoDong,
                    @SoDongThanh,
                    @SoDongLoi,
                    @TrangThai
                )";



            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@TenFile",
                        SqlDbType.NVarChar, 255).Value =
                        lichSu.TenFile.Trim();

                    cmd.Parameters.Add("@LoaiFile",
                        SqlDbType.VarChar, 10).Value =
                        loaiFile;

                    cmd.Parameters.Add("@TongSoDong",
                        SqlDbType.Int).Value =
                        lichSu.TongSoDong;

                    cmd.Parameters.Add("@SoDongThanh",
                        SqlDbType.Int).Value =
                        lichSu.SoDongThanh;

                    cmd.Parameters.Add("@SoDongLoi",
                        SqlDbType.Int).Value =
                        lichSu.SoDongLoi;

                    cmd.Parameters.Add("@TrangThai",
                        SqlDbType.VarChar, 20).Value =
                        trangThai;

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
