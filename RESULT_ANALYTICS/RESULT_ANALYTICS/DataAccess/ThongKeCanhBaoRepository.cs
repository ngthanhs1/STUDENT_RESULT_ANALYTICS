
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RESULT_ANALYTICS.Models;

namespace RESULT_ANALYTICS.DataAccess
{
    internal class ThongKeCanhBaoRepository
    {
        // =========================================
        // 1. LAY THONG KE DAT VA TRUOT
        // =========================================
        public List<ThongKeDatTruot> LayThongKeDatTruot(
            string maMonHoc = null, string maHocKy = null)
        {
            List<ThongKeDatTruot> danhSach =
                new List<ThongKeDatTruot>();

            string sql = @"
                SELECT
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
                WHERE (@MaMonHoc IS NULL OR MaMonHoc = @MaMonHoc)
                  AND (@MaHocKy IS NULL OR MaHocKy = @MaHocKy)
                ORDER BY MaHocKy, MaMonHoc";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    GanBoLoc(cmd, maMonHoc, maHocKy);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ThongKeDatTruot item =
                                new ThongKeDatTruot();

                            item.MaMonHoc = DocChuoi(reader, "MaMonHoc");
                            item.TenMonHoc = DocChuoi(reader, "TenMonHoc");
                            item.SoTinChi = DocSoNguyen(reader, "SoTinChi");

                            item.MaHocKy = DocChuoi(reader, "MaHocKy");
                            item.TenHocKy = DocChuoi(reader, "TenHocKy");
                            item.NamHoc = DocChuoi(reader, "NamHoc");

                            item.TongSinhVien =
                                DocSoNguyen(reader, "TongSinhVien");

                            item.SoSinhVienDat =
                                DocSoNguyen(reader, "SoSinhVienDat");

                            item.SoSinhVienTruot =
                                DocSoNguyen(reader, "SoSinhVienTruot");

                            item.SoSinhVienThieuDiem =
                                DocSoNguyen(reader, "SoSinhVienThieuDiem");

                            item.TyLeDat =
                                DocSoThapPhan(reader, "TyLeDat");

                            danhSach.Add(item);
                        }
                    }
                }
            }

            return danhSach;
        }

        // =========================================
        // 2. LAY DANH SACH SINH VIEN THIEU DIEM
        // =========================================
        public List<SinhVienThieuDiem> LaySinhVienThieuDiem(
            string maMonHoc = null, string maHocKy = null)
        {
            List<SinhVienThieuDiem> danhSach =
                new List<SinhVienThieuDiem>();

            string sql = @"
                SELECT
                    MaSV,
                    HoTen,
                    MaMonHoc,
                    TenMonHoc,
                    MaHocKy,
                    TenHocKy,
                    NamHoc,
                    DiemChuyenCan,
                    DiemGiuaKy,
                    DiemCuoiKy,
                    NoiDungCanhBao
                FROM dbo.vw_SinhVienThieuDiem
                WHERE (@MaMonHoc IS NULL OR MaMonHoc = @MaMonHoc)
                  AND (@MaHocKy IS NULL OR MaHocKy = @MaHocKy)
                ORDER BY MaHocKy, MaMonHoc, MaSV";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    GanBoLoc(cmd, maMonHoc, maHocKy);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            SinhVienThieuDiem item =
                                new SinhVienThieuDiem();

                            item.MaSV = DocChuoi(reader, "MaSV");
                            item.HoTen = DocChuoi(reader, "HoTen");

                            item.MaMonHoc = DocChuoi(reader, "MaMonHoc");
                            item.TenMonHoc = DocChuoi(reader, "TenMonHoc");

                            item.MaHocKy = DocChuoi(reader, "MaHocKy");
                            item.TenHocKy = DocChuoi(reader, "TenHocKy");
                            item.NamHoc = DocChuoi(reader, "NamHoc");

                            item.DiemChuyenCan =
                                DocSoThapPhan(reader, "DiemChuyenCan");

                            item.DiemGiuaKy =
                                DocSoThapPhan(reader, "DiemGiuaKy");

                            item.DiemCuoiKy =
                                DocSoThapPhan(reader, "DiemCuoiKy");

                            item.NoiDungCanhBao =
                                DocChuoi(reader, "NoiDungCanhBao");

                            danhSach.Add(item);
                        }
                    }
                }
            }

            return danhSach;
        }

        // =========================================
        // 3. GAN BO LOC MON HOC VA HOC KY
        // =========================================
        private void GanBoLoc(
            SqlCommand cmd, string maMonHoc, string maHocKy)
        {
            cmd.Parameters.Add("@MaMonHoc", SqlDbType.VarChar, 20)
                .Value = string.IsNullOrWhiteSpace(maMonHoc)
                    ? (object)DBNull.Value
                    : maMonHoc.Trim();

            cmd.Parameters.Add("@MaHocKy", SqlDbType.VarChar, 20)
                .Value = string.IsNullOrWhiteSpace(maHocKy)
                    ? (object)DBNull.Value
                    : maHocKy.Trim();
        }

        // =========================================
        // 4. DOC DU LIEU TU SQL SERVER
        // =========================================
        private string DocChuoi(SqlDataReader reader, string tenCot)
        {
            return reader[tenCot] == DBNull.Value
                ? ""
                : Convert.ToString(reader[tenCot]);
        }

        private int DocSoNguyen(SqlDataReader reader, string tenCot)
        {
            return reader[tenCot] == DBNull.Value
                ? 0
                : Convert.ToInt32(reader[tenCot]);
        }

        private decimal? DocSoThapPhan(
            SqlDataReader reader, string tenCot)
        {
            return reader[tenCot] == DBNull.Value
                ? (decimal?)null
                : Convert.ToDecimal(reader[tenCot]);
        }
    }
}
