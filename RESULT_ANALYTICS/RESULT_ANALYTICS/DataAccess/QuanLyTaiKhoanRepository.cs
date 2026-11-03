
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RESULT_ANALYTICS.Models;
using RESULT_ANALYTICS.Helpers;

namespace RESULT_ANALYTICS.DataAccess
{
    internal class QuanLyTaiKhoanRepository
    {
        // 1. Lay danh sach tai khoan
        public List<TaiKhoanQuanLy> LayDanhSach()
        {
            KiemTraQuyenAdmin();

            List<TaiKhoanQuanLy> danhSach =
                new List<TaiKhoanQuanLy>();

            string sql = @"
                SELECT MaTaiKhoan, TenDangNhap,
                       VaiTro, TrangThai, NgayTao
                FROM dbo.TaiKhoan
                ORDER BY MaTaiKhoan";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TaiKhoanQuanLy tk = new TaiKhoanQuanLy();

                        tk.MaTaiKhoan = Convert.ToInt32(
                            reader["MaTaiKhoan"]);

                        tk.TenDangNhap =
                            reader["TenDangNhap"].ToString();

                        tk.VaiTro = reader["VaiTro"].ToString();

                        tk.TrangThai =
                            Convert.ToBoolean(reader["TrangThai"]);

                        tk.NgayTao =
                            Convert.ToDateTime(reader["NgayTao"]);

                        danhSach.Add(tk);
                    }
                }
            }

            return danhSach;
        }

        // 2. Them tai khoan
        // Tham so matKhauHash phai duoc bam bang
        // PasswordHasher hien co, khong truyen mat khau goc
        public bool Them(string tenDangNhap,
                         string matKhauHash,
                         string vaiTro)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap)
                || tenDangNhap.Trim().Length > 50)
            {
                throw new ArgumentException(
                    "Tên đăng nhập phải từ 1 đến 50 ký tự!");
            }

            KiemTraMatKhauHash(matKhauHash);
            vaiTro = ChuanHoaVaiTro(vaiTro);

            string sql = @"
                INSERT INTO dbo.TaiKhoan
                (
                    TenDangNhap,
                    MatKhauHash,
                    VaiTro,
                    TrangThai,
                    NgayTao
                )
                SELECT
                    @TenDangNhap,
                    @MatKhauHash,
                    @VaiTro,
                    1,
                    SYSDATETIME()
                WHERE NOT EXISTS
                (
                    SELECT 1 FROM dbo.TaiKhoan
                    WHERE TenDangNhap = @TenDangNhap
                )";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@TenDangNhap",
                        SqlDbType.VarChar, 50).Value =
                        tenDangNhap.Trim();

                    cmd.Parameters.Add("@MatKhauHash",
                        SqlDbType.NVarChar, 255).Value =
                        matKhauHash;

                    cmd.Parameters.Add("@VaiTro",
                        SqlDbType.VarChar, 20).Value =
                        vaiTro;

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 3. Cap nhat vai tro va trang thai tai khoan
        public bool CapNhat(int maTaiKhoan,
                            string vaiTro,
                            bool trangThai)
        {
            if (maTaiKhoan <= 0)
                throw new ArgumentException(
                    "Mã tài khoản không hợp lệ!");

            vaiTro = ChuanHoaVaiTro(vaiTro);

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlTransaction tran =
                    conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        string vaiTroCu = null;
                        bool trangThaiCu = false;

                        // Khoa ban ghi trong qua trinh cap nhat
                        string sqlKiemTra = @"
                            SELECT VaiTro, TrangThai
                            FROM dbo.TaiKhoan WITH (UPDLOCK, HOLDLOCK)
                            WHERE MaTaiKhoan = @MaTaiKhoan";

                        using (SqlCommand cmd =
                            new SqlCommand(sqlKiemTra, conn, tran))
                        {
                            cmd.Parameters.Add("@MaTaiKhoan",
                                SqlDbType.Int).Value = maTaiKhoan;

                            using (SqlDataReader reader =
                                cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    vaiTroCu =
                                        reader["VaiTro"].ToString();

                                    trangThaiCu =
                                        Convert.ToBoolean(
                                            reader["TrangThai"]);
                                }
                            }
                        }

                        if (vaiTroCu == null)
                        {
                            tran.Rollback();
                            return false;
                        }

                        bool laAdminDangHoatDong =
                            vaiTroCu == "Admin" && trangThaiCu;

                        bool vanLaAdminDangHoatDong =
                            vaiTro == "Admin" && trangThai;

                        // Khong cho khoa hoac ha quyen
                        // tai khoan Admin hoat dong cuoi cung
                        if (laAdminDangHoatDong
                            && !vanLaAdminDangHoatDong)
                        {
                            string sqlDemAdmin = @"
                                SELECT COUNT(*)
                                FROM dbo.TaiKhoan
                                    WITH (UPDLOCK, HOLDLOCK)
                                WHERE VaiTro = 'Admin'
                                  AND TrangThai = 1
                                  AND MaTaiKhoan <> @MaTaiKhoan";

                            using (SqlCommand cmd =
                                new SqlCommand(sqlDemAdmin, conn, tran))
                            {
                                cmd.Parameters.Add("@MaTaiKhoan",
                                    SqlDbType.Int).Value = maTaiKhoan;

                                int soAdminKhac =
                                    Convert.ToInt32(cmd.ExecuteScalar());

                                if (soAdminKhac == 0)
                                {
                                    throw new InvalidOperationException(
                                        "Không thể khóa hoặc hạ quyền " +
                                        "Admin đang hoạt động cuối cùng!");
                                }
                            }
                        }

                        string sql = @"
                            UPDATE dbo.TaiKhoan
                            SET VaiTro = @VaiTro,
                                TrangThai = @TrangThai
                            WHERE MaTaiKhoan = @MaTaiKhoan";

                        int soDong;

                        using (SqlCommand cmd =
                            new SqlCommand(sql, conn, tran))
                        {
                            cmd.Parameters.Add("@MaTaiKhoan",
                                SqlDbType.Int).Value = maTaiKhoan;

                            cmd.Parameters.Add("@VaiTro",
                                SqlDbType.VarChar, 20).Value = vaiTro;

                            cmd.Parameters.Add("@TrangThai",
                                SqlDbType.Bit).Value = trangThai;

                            soDong = cmd.ExecuteNonQuery();
                        }

                        tran.Commit();
                        return soDong > 0;
                    }
                    catch
                    {
                        tran.Rollback();
                        throw;
                    }
                }
            }
        }

        // 4. Doi mat khau da bam
        public bool DoiMatKhau(int maTaiKhoan,
                               string matKhauHash)
        {
            if (maTaiKhoan <= 0)
                throw new ArgumentException(
                    "Mã tài khoản không hợp lệ!");

            KiemTraMatKhauHash(matKhauHash);

            string sql = @"
                UPDATE dbo.TaiKhoan
                SET MatKhauHash = @MatKhauHash
                WHERE MaTaiKhoan = @MaTaiKhoan";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@MaTaiKhoan",
                        SqlDbType.Int).Value = maTaiKhoan;

                    cmd.Parameters.Add("@MatKhauHash",
                        SqlDbType.NVarChar, 255).Value =
                        matKhauHash;

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // 5. Chuan hoa vai tro
        private string ChuanHoaVaiTro(string vaiTro)
        {
            if (string.Equals(vaiTro, "Admin",
                StringComparison.OrdinalIgnoreCase))
                return "Admin";

            if (string.Equals(vaiTro, "NhanVien",
                StringComparison.OrdinalIgnoreCase))
                return "NhanVien";

            throw new ArgumentException(
                "Vai trò chỉ được là Admin hoặc NhanVien!");
        }

        // 6. Kiem tra hash mat khau
        private void KiemTraMatKhauHash(string matKhauHash)
        {
            if (string.IsNullOrWhiteSpace(matKhauHash)
                || matKhauHash.Length > 255)
            {
                throw new ArgumentException(
                    "Mật khẩu băm không hợp lệ!");
            }
        }

        private void KiemTraQuyenAdmin()
        {
            if (!PhienDangNhap.LaAdmin)
            {
                throw new UnauthorizedAccessException(
                    "Chỉ Admin mới được quản lý tài khoản!");
            }
        }

    }
}
