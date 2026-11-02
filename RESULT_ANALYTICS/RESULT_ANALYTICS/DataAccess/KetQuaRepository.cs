
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
    internal class KetQuaRepository
    {
        // Lay danh sach ket qua hoc tap
        public List<KetQua> LayDanhSach()
        {
            List<KetQua> danhSach = new List<KetQua>();

            string sql = @"
                SELECT
                    kq.MaKetQua,
                    kq.MaSV,
                    sv.HoTen,
                    kq.MaMonHoc,
                    mh.TenMonHoc,
                    kq.MaHocKy,
                    hk.TenHocKy,
                    kq.DiemChuyenCan,
                    kq.DiemGiuaKy,
                    kq.DiemCuoiKy,
                    kq.DiemTongKet
                FROM dbo.KetQua kq
                INNER JOIN dbo.SinhVien sv
                    ON kq.MaSV = sv.MaSV
                INNER JOIN dbo.MonHoc mh
                    ON kq.MaMonHoc = mh.MaMonHoc
                INNER JOIN dbo.HocKy hk
                    ON kq.MaHocKy = hk.MaHocKy
                ORDER BY kq.MaKetQua";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            KetQua ketQua = new KetQua();

                            ketQua.MaKetQua = Convert.ToInt32(reader["MaKetQua"]);
                            ketQua.MaSV = reader["MaSV"].ToString();
                            ketQua.HoTen = reader["HoTen"].ToString();
                            ketQua.MaMonHoc = reader["MaMonHoc"].ToString();
                            ketQua.TenMonHoc = reader["TenMonHoc"].ToString();
                            ketQua.MaHocKy = reader["MaHocKy"].ToString();
                            ketQua.TenHocKy = reader["TenHocKy"].ToString();

                            ketQua.DiemChuyenCan = DocDiem(reader, "DiemChuyenCan");
                            ketQua.DiemGiuaKy = DocDiem(reader, "DiemGiuaKy");
                            ketQua.DiemCuoiKy = DocDiem(reader, "DiemCuoiKy");
                            ketQua.DiemTongKet = DocDiem(reader, "DiemTongKet");

                            danhSach.Add(ketQua);
                        }
                    }
                }
            }

            return danhSach;
        }

        // Doc diem cho phep NULL
        private decimal? DocDiem(SqlDataReader reader, string tenCot)
        {
            if (reader[tenCot] == DBNull.Value)
            {
                return null;
            }

            return Convert.ToDecimal(reader[tenCot]);
        }

        // Them ket qua hoc tap
        public bool Them(KetQua ketQua)
        {
            KiemTraDuLieu(ketQua);

            string sql = @"
                INSERT INTO dbo.KetQua
                (
                    MaSV,
                    MaMonHoc,
                    MaHocKy,
                    DiemChuyenCan,
                    DiemGiuaKy,
                    DiemCuoiKy
                )
                VALUES
                (
                    @MaSV,
                    @MaMonHoc,
                    @MaHocKy,
                    @DiemChuyenCan,
                    @DiemGiuaKy,
                    @DiemCuoiKy
                )";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    GanThamSo(cmd, ketQua);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Sua diem sinh vien
        public bool Sua(KetQua ketQua)
        {
            KiemTraDuLieu(ketQua);

            string sql = @"
                UPDATE dbo.KetQua
                SET
                    MaSV = @MaSV,
                    MaMonHoc = @MaMonHoc,
                    MaHocKy = @MaHocKy,
                    DiemChuyenCan = @DiemChuyenCan,
                    DiemGiuaKy = @DiemGiuaKy,
                    DiemCuoiKy = @DiemCuoiKy
                WHERE MaKetQua = @MaKetQua";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    GanThamSo(cmd, ketQua);

                    cmd.Parameters.Add("@MaKetQua", SqlDbType.Int)
                        .Value = ketQua.MaKetQua;

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Xoa ket qua hoc tap
        public bool Xoa(int maKetQua)
        {
            if (maKetQua <= 0)
            {
                throw new ArgumentException("Mã kết quả không hợp lệ!");
            }

            string sql = @"
                DELETE FROM dbo.KetQua
                WHERE MaKetQua = @MaKetQua";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaKetQua", SqlDbType.Int)
                        .Value = maKetQua;

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Gan tham so SQL Server
        private void GanThamSo(SqlCommand cmd, KetQua ketQua)
        {
            cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 20)
                .Value = ketQua.MaSV.Trim();

            cmd.Parameters.Add("@MaMonHoc", SqlDbType.VarChar, 20)
                .Value = ketQua.MaMonHoc.Trim();

            cmd.Parameters.Add("@MaHocKy", SqlDbType.VarChar, 20)
                .Value = ketQua.MaHocKy.Trim();

            GanDiem(cmd, "@DiemChuyenCan", ketQua.DiemChuyenCan);
            GanDiem(cmd, "@DiemGiuaKy", ketQua.DiemGiuaKy);
            GanDiem(cmd, "@DiemCuoiKy", ketQua.DiemCuoiKy);
        }

        // Gan diem decimal hoac NULL
        private void GanDiem(SqlCommand cmd, string ten, decimal? diem)
        {
            SqlParameter p = cmd.Parameters.Add(ten, SqlDbType.Decimal);

            p.Precision = 4;
            p.Scale = 2;

            p.Value = diem.HasValue
                ? (object)diem.Value
                : DBNull.Value;
        }

        // Kiem tra du lieu dau vao
        private void KiemTraDuLieu(KetQua ketQua)
        {
            if (ketQua == null)
            {
                throw new ArgumentNullException(nameof(ketQua));
            }

            if (string.IsNullOrWhiteSpace(ketQua.MaSV))
            {
                throw new ArgumentException("Vui lòng chọn sinh viên!");
            }

            if (string.IsNullOrWhiteSpace(ketQua.MaMonHoc))
            {
                throw new ArgumentException("Vui lòng chọn môn học!");
            }

            if (string.IsNullOrWhiteSpace(ketQua.MaHocKy))
            {
                throw new ArgumentException("Vui lòng chọn học kỳ!");
            }

            KiemTraDiem(ketQua.DiemChuyenCan, "Điểm chuyên cần");
            KiemTraDiem(ketQua.DiemGiuaKy, "Điểm giữa kỳ");
            KiemTraDiem(ketQua.DiemCuoiKy, "Điểm cuối kỳ");
        }

        // Kiem tra diem tu 0 den 10
        private void KiemTraDiem(decimal? diem, string tenDiem)
        {
            if (diem.HasValue)
            {
                if (diem.Value < 0 || diem.Value > 10)
                {
                    throw new ArgumentException(
                        tenDiem + " phải nằm trong khoảng 0 đến 10!");
                }

                if (decimal.Round(diem.Value, 2) != diem.Value)
                {
                    throw new ArgumentException(
                        tenDiem + " chỉ được có tối đa 2 chữ số thập phân!");
                }
            }
        }
    }
}
