using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using RESULT_ANALYTICS.Models;

namespace RESULT_ANALYTICS.DataAccess
{
    internal class PhanTichRepository
    {
        // =========================================
        // 1. LAY DIEM TRUNG BINH THEO HOC KY
        // =========================================
        public List<DiemTrungBinhHocKy> LayDanhSachDiemTrungBinh(
            string maLop = null, string maHocKy = null)
        {
            List<DiemTrungBinhHocKy> danhSach =
                new List<DiemTrungBinhHocKy>();

            string sql = @"
                SELECT
                    MaSV,
                    HoTen,
                    MaLop,
                    MaHocKy,
                    TenHocKy,
                    NamHoc,
                    TongSoMon,
                    TongSoTinChi,
                    SoMonThieuDiem,
                    SoTinChiDaCoDiem,
                    DiemTrungBinhTamTinh,
                    DiemTrungBinhHe4,
                    TrangThai
                FROM dbo.vw_DiemTrungBinhHocKy
                WHERE (@MaLop IS NULL OR MaLop = @MaLop)
                  AND (@MaHocKy IS NULL OR MaHocKy = @MaHocKy)
                ORDER BY MaLop, MaSV";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    GanBoLoc(cmd, maLop, maHocKy);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DiemTrungBinhHocKy item =
                                new DiemTrungBinhHocKy();

                            item.MaSV = DocChuoi(reader, "MaSV");
                            item.HoTen = DocChuoi(reader, "HoTen");
                            item.MaLop = DocChuoi(reader, "MaLop");

                            item.MaHocKy = DocChuoi(reader, "MaHocKy");
                            item.TenHocKy = DocChuoi(reader, "TenHocKy");
                            item.NamHoc = DocChuoi(reader, "NamHoc");

                            item.TongSoMon = DocSoNguyen(
                                reader, "TongSoMon");

                            item.TongSoTinChi = DocSoNguyen(
                                reader, "TongSoTinChi");

                            item.SoMonThieuDiem = DocSoNguyen(
                                reader, "SoMonThieuDiem");

                            item.SoTinChiDaCoDiem = DocSoNguyen(
                                reader, "SoTinChiDaCoDiem");

                            item.DiemTrungBinhTamTinh = DocSoThapPhan(
                                reader, "DiemTrungBinhTamTinh");

                            item.DiemTrungBinhHe4 = DocSoThapPhan(
                                reader, "DiemTrungBinhHe4");

                            item.TrangThai = DocChuoi(reader, "TrangThai");

                            danhSach.Add(item);
                        }
                    }
                }
            }

            return danhSach;
        }

        // =========================================
        // 2. LAY KET QUA PHAN LOAI HOC LUC
        // =========================================
        public List<PhanLoaiHocLuc> LayDanhSachPhanLoai(
            string maLop = null, string maHocKy = null)
        {
            List<PhanLoaiHocLuc> danhSach =
                new List<PhanLoaiHocLuc>();

            string sql = @"
                SELECT
                    MaSV,
                    HoTen,
                    MaLop,
                    MaHocKy,
                    TenHocKy,
                    NamHoc,
                    TongSoMon,
                    SoMonThieuDiem,
                    DiemTrungBinh,
                    DiemTrungBinhHe4,
                    XepLoai,
                    CanhBao
                FROM dbo.vw_PhanLoaiHocLuc
                WHERE (@MaLop IS NULL OR MaLop = @MaLop)
                  AND (@MaHocKy IS NULL OR MaHocKy = @MaHocKy)
                ORDER BY MaLop, MaSV";

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    GanBoLoc(cmd, maLop, maHocKy);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PhanLoaiHocLuc item =
                                new PhanLoaiHocLuc();

                            item.MaSV = DocChuoi(reader, "MaSV");
                            item.HoTen = DocChuoi(reader, "HoTen");
                            item.MaLop = DocChuoi(reader, "MaLop");

                            item.MaHocKy = DocChuoi(reader, "MaHocKy");
                            item.TenHocKy = DocChuoi(reader, "TenHocKy");
                            item.NamHoc = DocChuoi(reader, "NamHoc");

                            item.TongSoMon = DocSoNguyen(
                                reader, "TongSoMon");

                            item.SoMonThieuDiem = DocSoNguyen(
                                reader, "SoMonThieuDiem");

                            item.DiemTrungBinh = DocSoThapPhan(
                                reader, "DiemTrungBinh");

                            item.DiemTrungBinhHe4 = DocSoThapPhan(
                                reader, "DiemTrungBinhHe4");

                            item.XepLoai = DocChuoi(reader, "XepLoai");
                            item.CanhBao = DocChuoi(reader, "CanhBao");

                            danhSach.Add(item);
                        }
                    }
                }
            }

            return danhSach;
        }

        // =========================================
        // 3. GAN THAM SO LOC DU LIEU
        // =========================================
        private void GanBoLoc(
            SqlCommand cmd, string maLop, string maHocKy)
        {
            cmd.Parameters.Add("@MaLop", SqlDbType.VarChar, 20)
                .Value = string.IsNullOrWhiteSpace(maLop)
                    ? (object)DBNull.Value
                    : maLop.Trim();

            cmd.Parameters.Add("@MaHocKy", SqlDbType.VarChar, 20)
                .Value = string.IsNullOrWhiteSpace(maHocKy)
                    ? (object)DBNull.Value
                    : maHocKy.Trim();
        }

        // =========================================
        // 4. HAM DOC DU LIEU TU SQL SERVER
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
