
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
    internal class SinhVienRepository
    {
        // Lay danh sach sinh vien
        public List<SinhVien> LayDanhSach()
        {
            string sql = @"
                SELECT
                    sv.MaSV,
                    sv.HoTen,
                    sv.NgaySinh,
                    sv.GioiTinh,
                    sv.Email,
                    sv.MaLop,
                    l.TenLop,
                    l.Khoa
                FROM dbo.SinhVien sv
                LEFT JOIN dbo.Lop l ON sv.MaLop = l.MaLop
                ORDER BY sv.MaSV";

            return DocDanhSach(sql, null);
        }

        // Them sinh vien moi
        public bool Them(SinhVien sinhVien)
        {
            KiemTraDuLieu(sinhVien);

            string sql = @"
                INSERT INTO dbo.SinhVien
                (MaSV, HoTen, NgaySinh, GioiTinh, Email, MaLop)
                VALUES
                (@MaSV, @HoTen, @NgaySinh, @GioiTinh, @Email, @MaLop)";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    GanThamSo(cmd, sinhVien);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Sua thong tin sinh vien
        public bool Sua(SinhVien sinhVien)
        {
            KiemTraDuLieu(sinhVien);

            string sql = @"
                UPDATE dbo.SinhVien
                SET
                    HoTen = @HoTen,
                    NgaySinh = @NgaySinh,
                    GioiTinh = @GioiTinh,
                    Email = @Email,
                    MaLop = @MaLop
                WHERE MaSV = @MaSV";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    GanThamSo(cmd, sinhVien);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Xoa sinh vien theo ma
        public bool Xoa(string maSV)
        {
            if (string.IsNullOrWhiteSpace(maSV))
            {
                throw new ArgumentException(
                    "Mã sinh viên không được để trống!");
            }

            string sql = @"
                DELETE FROM dbo.SinhVien
                WHERE MaSV = @MaSV";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 20)
                        .Value = maSV.Trim();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Tim kiem sinh vien
        public List<SinhVien> TimKiem(string tuKhoa)
        {
            if (string.IsNullOrWhiteSpace(tuKhoa))
            {
                return LayDanhSach();
            }

            string sql = @"
                SELECT
                    sv.MaSV,
                    sv.HoTen,
                    sv.NgaySinh,
                    sv.GioiTinh,
                    sv.Email,
                    sv.MaLop,
                    l.TenLop,
                    l.Khoa
                FROM dbo.SinhVien sv
                LEFT JOIN dbo.Lop l ON sv.MaLop = l.MaLop
                WHERE
                    sv.MaSV LIKE @TuKhoa
                    OR sv.HoTen LIKE @TuKhoa
                    OR sv.Email LIKE @TuKhoa
                    OR sv.MaLop LIKE @TuKhoa
                    OR l.TenLop LIKE @TuKhoa
                    OR l.Khoa LIKE @TuKhoa
                ORDER BY sv.MaSV";

            return DocDanhSach(sql, tuKhoa);
        }

        // Doc du lieu tu SQL Server
        private List<SinhVien> DocDanhSach(
            string sql, string tuKhoa)
        {
            List<SinhVien> danhSach = new List<SinhVien>();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    if (tuKhoa != null)
                    {
                        cmd.Parameters.Add("@TuKhoa", SqlDbType.NVarChar, 4000)
                            .Value = "%" + tuKhoa.Trim() + "%";
                    }

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SinhVien sinhVien = new SinhVien();

                            sinhVien.MaSV = reader["MaSV"].ToString();
                            sinhVien.HoTen = reader["HoTen"].ToString();

                            sinhVien.NgaySinh =
                                reader["NgaySinh"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(reader["NgaySinh"]);

                            sinhVien.GioiTinh =
                                reader["GioiTinh"] == DBNull.Value
                                ? null
                                : reader["GioiTinh"].ToString();

                            sinhVien.Email =
                                reader["Email"] == DBNull.Value
                                ? null
                                : reader["Email"].ToString();

                            sinhVien.MaLop = reader["MaLop"].ToString();

                            sinhVien.TenLop =
                                reader["TenLop"] == DBNull.Value
                                ? null
                                : reader["TenLop"].ToString();

                            sinhVien.Khoa =
                                reader["Khoa"] == DBNull.Value
                                ? ""
                                : reader["Khoa"].ToString();

                            danhSach.Add(sinhVien);
                        }
                    }
                }
            }

            return danhSach;
        }

        // Gan tham so cho SQL Command
        private void GanThamSo(
            SqlCommand cmd, SinhVien sinhVien)
        {
            cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 20)
                .Value = sinhVien.MaSV.Trim();

            cmd.Parameters.Add("@HoTen", SqlDbType.NVarChar, 100)
                .Value = sinhVien.HoTen.Trim();

            cmd.Parameters.Add("@NgaySinh", SqlDbType.Date)
                .Value = sinhVien.NgaySinh.HasValue
                ? (object)sinhVien.NgaySinh.Value.Date
                : DBNull.Value;

            cmd.Parameters.Add("@GioiTinh", SqlDbType.NVarChar, 10)
                .Value = string.IsNullOrWhiteSpace(sinhVien.GioiTinh)
                ? (object)DBNull.Value
                : sinhVien.GioiTinh.Trim();

            cmd.Parameters.Add("@Email", SqlDbType.VarChar, 100)
                .Value = string.IsNullOrWhiteSpace(sinhVien.Email)
                ? (object)DBNull.Value
                : sinhVien.Email.Trim();

            cmd.Parameters.Add("@MaLop", SqlDbType.VarChar, 20)
                .Value = sinhVien.MaLop.Trim();
        }

        // Kiem tra du lieu dau vao
        private void KiemTraDuLieu(SinhVien sinhVien)
        {
            if (sinhVien == null)
            {
                throw new ArgumentNullException(nameof(sinhVien));
            }

            if (string.IsNullOrWhiteSpace(sinhVien.MaSV))
            {
                throw new ArgumentException(
                    "Mã sinh viên không được để trống!");
            }

            if (string.IsNullOrWhiteSpace(sinhVien.HoTen))
            {
                throw new ArgumentException(
                    "Họ tên sinh viên không được để trống!");
            }

            if (string.IsNullOrWhiteSpace(sinhVien.MaLop))
            {
                throw new ArgumentException(
                    "Vui lòng chọn lớp học!");
            }

            if (sinhVien.MaSV.Trim().Length > 20)
            {
                throw new ArgumentException(
                    "Mã sinh viên tối đa 20 ký tự!");
            }

            if (sinhVien.HoTen.Trim().Length > 100)
            {
                throw new ArgumentException(
                    "Họ tên tối đa 100 ký tự!");
            }

            if (sinhVien.MaLop.Trim().Length > 20)
            {
                throw new ArgumentException(
                    "Mã lớp tối đa 20 ký tự!");
            }

            if (!string.IsNullOrWhiteSpace(sinhVien.Email))
            {
                string email = sinhVien.Email.Trim();
                if (email.Length > 100)
                {
                    throw new ArgumentException("Email tối đa 100 ký tự!");
                }

                int atIndex = email.IndexOf('@');
                if (atIndex <= 0 || atIndex == email.Length - 1 || !email.Substring(atIndex + 1).Contains("."))
                {
                    throw new ArgumentException(
                        $"Email '{email}' không đúng định dạng!\nEmail sinh viên phải có dạng: {sinhVien.MaSV.Trim()}@domain (Ví dụ: {sinhVien.MaSV.Trim()}@phenikaa-uni.edu.vn)");
                }

                string phanDau = email.Substring(0, atIndex).Trim();
                if (!string.Equals(phanDau, sinhVien.MaSV.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException(
                        $"Phần đầu của email ('{phanDau}') không khớp với Mã sinh viên ('{sinhVien.MaSV.Trim()}')!\nEmail sinh viên bắt buộc phải có dạng: {sinhVien.MaSV.Trim()}@<tên_miền> (Ví dụ: {sinhVien.MaSV.Trim()}@phenikaa-uni.edu.vn).");
                }
            }

            if (!string.IsNullOrWhiteSpace(sinhVien.GioiTinh) &&
                sinhVien.GioiTinh.Trim().Length > 10)
            {
                throw new ArgumentException(
                    "Giới tính tối đa 10 ký tự!");
            }
        }
    }
}
