
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using RESULT_ANALYTICS.DataAccess;
using RESULT_ANALYTICS.Helpers;
using RESULT_ANALYTICS.Models;
using ClosedXML.Excel;


namespace RESULT_ANALYTICS.Services
{
    // Ket qua sau khi Import
    internal class KetQuaImportResult
    {
        public int TongSoDong { get; set; }
        public int SoDongThanh { get; set; }
        public int SoDongLoi { get; set; }
        public bool DaLuuLichSu { get; set; }
        public string CanhBaoLichSu { get; set; }

        public List<string> DanhSachLoi { get; private set; }

        public KetQuaImportResult()
        {
            DanhSachLoi = new List<string>();
        }

        public string TrangThai
        {
            get
            {
                if (SoDongLoi == 0 && SoDongThanh > 0)
                    return "ThanhCong";

                if (SoDongThanh > 0)
                    return "MotPhan";

                return "ThatBai";
            }
        }
    }

    internal class ImportService
    {
        private readonly LichSuNhapDuLieuRepository lichSuRepository;

        public ImportService()
        {
            lichSuRepository = new LichSuNhapDuLieuRepository();
        }

        // ============================================
        // 1. IMPORT DIEM SINH VIEN TU FILE CSV
        // ============================================
        public KetQuaImportResult ImportCsvKetQua(string duongDan)
        {
            if (!PhienDangNhap.DaDangNhap)
                throw new UnauthorizedAccessException(
                    "Vui lòng đăng nhập trước khi Import!");

            if (string.IsNullOrWhiteSpace(duongDan) ||
                !File.Exists(duongDan))
                throw new FileNotFoundException(
                    "Không tìm thấy file CSV cần Import!");

            if (!string.Equals(
                Path.GetExtension(duongDan), ".csv",
                StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(
                    "Vui lòng chọn file có đuôi .csv!");

            FileInfo thongTinFile = new FileInfo(duongDan);

            if (thongTinFile.Length > 5 * 1024 * 1024)
                throw new ArgumentException(
                    "File CSV không được lớn hơn 5 MB!");

            // File CSV can duoc luu bang ma hoa UTF-8
            string noiDung = File.ReadAllText(
                duongDan, new UTF8Encoding(false, true));

            char dauPhanCach = XacDinhDauPhanCach(noiDung);

            List<string[]> cacDong = TachCsv(noiDung, dauPhanCach);

            if (cacDong.Count == 0)
                throw new ArgumentException("File CSV đang trống!");

            KiemTraTieuDe(cacDong[0]);

            if (cacDong.Count > 5001)
                throw new ArgumentException(
                    "Mỗi lần Import tối đa 5000 dòng dữ liệu!");

            KetQuaImportResult ketQua = new KetQuaImportResult();

            using (SqlConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                // Bo qua dong tieu de
                for (int i = 1; i < cacDong.Count; i++)
                {
                    string[] dong = cacDong[i];

                    if (DongTrong(dong))
                        continue;

                    ketQua.TongSoDong++;

                    try
                    {
                        if (dong.Length != 6)
                            throw new ArgumentException(
                                "Dòng dữ liệu phải có đúng 6 cột!");

                        string maSV = KiemTraMa(dong[0], "Mã sinh viên");
                        string maMon = KiemTraMa(dong[1], "Mã môn học");
                        string maHocKy = KiemTraMa(dong[2], "Mã học kỳ");

                        decimal? chuyenCan =
                            DocDiem(dong[3], "Điểm chuyên cần");

                        decimal? giuaKy =
                            DocDiem(dong[4], "Điểm giữa kỳ");

                        decimal? cuoiKy =
                            DocDiem(dong[5], "Điểm cuối kỳ");

                        bool thanhCong = ThemKetQua(
                            conn, maSV, maMon, maHocKy,
                            chuyenCan, giuaKy, cuoiKy);

                        if (!thanhCong)
                            throw new InvalidOperationException(
                                "Kết quả sinh viên - môn học - học kỳ đã tồn tại!");

                        ketQua.SoDongThanh++;
                    }
                    catch (Exception ex)
                    {
                        ketQua.SoDongLoi++;

                        string noiDungLoi = ex.Message;

                        SqlException loiSql = ex as SqlException;

                        if (loiSql != null)
                        {
                            if (loiSql.Number == 547)
                                noiDungLoi =
                                    "Mã sinh viên, môn học hoặc học kỳ không tồn tại!";
                            else if (loiSql.Number == 2601 ||
                                     loiSql.Number == 2627)
                                noiDungLoi = "Kết quả đã tồn tại!";
                        }

                        // Chi giu 30 loi dau tien de hien thi
                        if (ketQua.DanhSachLoi.Count < 30)
                        {
                            ketQua.DanhSachLoi.Add(
                                "Bản ghi " + i + ": " + noiDungLoi);
                        }
                    }
                }
            }

            if (ketQua.TongSoDong == 0)
                throw new ArgumentException(
                    "File CSV không có dòng dữ liệu để Import!");

            // Ghi lich su vao SQL Server
            try
            {
                ketQua.DaLuuLichSu = lichSuRepository.GhiLichSu(
                    new LichSuNhapDuLieu
                    {
                        TenFile = Path.GetFileName(duongDan),
                        LoaiFile = "CSV",
                        TongSoDong = ketQua.TongSoDong,
                        SoDongThanh = ketQua.SoDongThanh,
                        SoDongLoi = ketQua.SoDongLoi,
                        TrangThai = ketQua.TrangThai
                    });

                if (!ketQua.DaLuuLichSu)
                    ketQua.CanhBaoLichSu =
                        "Import đã xử lý nhưng chưa lưu được lịch sử!";
            }
            catch (Exception ex)
            {
                ketQua.DaLuuLichSu = false;
                ketQua.CanhBaoLichSu =
                    "Không thể ghi lịch sử Import: " + ex.Message;
            }

            return ketQua;
        }


        // ============================================
        // 8. IMPORT DIEM SINH VIEN TU EXCEL
        // ============================================
        public KetQuaImportResult ImportExcelKetQua(string duongDan)
        {
            if (!PhienDangNhap.DaDangNhap)
                throw new UnauthorizedAccessException(
                    "Vui lòng đăng nhập trước khi Import!");

            if (string.IsNullOrWhiteSpace(duongDan) ||
                !File.Exists(duongDan))
                throw new FileNotFoundException(
                    "Không tìm thấy file Excel cần Import!");

            if (!string.Equals(
                Path.GetExtension(duongDan), ".xlsx",
                StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(
                    "Chỉ hỗ trợ file Excel có đuôi .xlsx!");

            FileInfo thongTinFile = new FileInfo(duongDan);

            if (thongTinFile.Length > 5 * 1024 * 1024)
                throw new ArgumentException(
                    "File Excel không được lớn hơn 5 MB!");

            KetQuaImportResult ketQua = new KetQuaImportResult();

            using (XLWorkbook workbook = new XLWorkbook(duongDan))
            {
                if (workbook.Worksheets.Count == 0)
                    throw new ArgumentException(
                        "File Excel không có Sheet dữ liệu!");

                // Lay Sheet dau tien
                IXLWorksheet sheet = workbook.Worksheet(1);

                IXLRow dongCuoi = sheet.LastRowUsed();

                if (dongCuoi == null || dongCuoi.RowNumber() < 2)
                    throw new ArgumentException(
                        "File Excel không có dữ liệu để Import!");

                int soDongCuoi = dongCuoi.RowNumber();

                if (soDongCuoi > 5001)
                    throw new ArgumentException(
                        "Mỗi lần Import tối đa 5000 dòng dữ liệu!");

                // Kiem tra so cot tieu de
                IXLCell cotCuoiTieuDe = sheet.Row(1).LastCellUsed();

                if (cotCuoiTieuDe == null ||
                    cotCuoiTieuDe.Address.ColumnNumber != 6)
                    throw new ArgumentException(
                        "Dòng tiêu đề phải có đúng 6 cột!");

                string[] tieuDe = new string[6];

                for (int c = 1; c <= 6; c++)
                {
                    if (sheet.Cell(1, c).HasFormula)
                        throw new ArgumentException(
                            "Dòng tiêu đề không được chứa công thức!");

                    tieuDe[c - 1] = sheet.Cell(1, c).GetString();
                }

                // Su dung ham kiem tra tieu de CSV da co
                KiemTraTieuDe(tieuDe);

                using (SqlConnection conn =
                    DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    // Dong 1 la tieu de, bat dau tu dong 2
                    for (int r = 2; r <= soDongCuoi; r++)
                    {
                        IXLRow dongExcel = sheet.Row(r);

                        if (dongExcel.IsEmpty())
                            continue;

                        ketQua.TongSoDong++;

                        try
                        {
                            IXLCell cotCuoi = dongExcel.LastCellUsed();

                            if (cotCuoi != null &&
                                cotCuoi.Address.ColumnNumber > 6)
                                throw new ArgumentException(
                                    "Dòng dữ liệu có nhiều hơn 6 cột!");

                            string[] duLieu = new string[6];

                            for (int c = 1; c <= 6; c++)
                            {
                                IXLCell o = sheet.Cell(r, c);

                                // Chi nhan gia tri, khong xu ly cong thuc
                                if (o.HasFormula)
                                    throw new ArgumentException(
                                        "Không hỗ trợ ô chứa công thức!");

                                duLieu[c - 1] = o.GetString();
                            }

                            if (DongTrong(duLieu))
                            {
                                ketQua.TongSoDong--;
                                continue;
                            }

                            string maSV = KiemTraMa(
                                duLieu[0], "Mã sinh viên");

                            string maMonHoc = KiemTraMa(
                                duLieu[1], "Mã môn học");

                            string maHocKy = KiemTraMa(
                                duLieu[2], "Mã học kỳ");

                            decimal? diemChuyenCan = DocDiem(
                                duLieu[3], "Điểm chuyên cần");

                            decimal? diemGiuaKy = DocDiem(
                                duLieu[4], "Điểm giữa kỳ");

                            decimal? diemCuoiKy = DocDiem(
                                duLieu[5], "Điểm cuối kỳ");

                            // Dung lai ham ThemKetQua cua CSV
                            bool thanhCong = ThemKetQua(
                                conn,
                                maSV,
                                maMonHoc,
                                maHocKy,
                                diemChuyenCan,
                                diemGiuaKy,
                                diemCuoiKy);

                            if (!thanhCong)
                                throw new InvalidOperationException(
                                    "Kết quả sinh viên - môn học - học kỳ đã tồn tại!");

                            ketQua.SoDongThanh++;
                        }
                        catch (Exception ex)
                        {
                            ketQua.SoDongLoi++;

                            string noiDungLoi = ex.Message;

                            SqlException loiSql = ex as SqlException;

                            if (loiSql != null)
                            {
                                if (loiSql.Number == 547)
                                    noiDungLoi =
                                        "Mã sinh viên, môn học hoặc học kỳ không tồn tại!";
                                else if (loiSql.Number == 2601 ||
                                         loiSql.Number == 2627)
                                    noiDungLoi =
                                        "Kết quả đã tồn tại!";
                            }

                            // Luu toi da 30 thong bao loi
                            if (ketQua.DanhSachLoi.Count < 30)
                            {
                                ketQua.DanhSachLoi.Add(
                                    "Dòng Excel " + r + ": " +
                                    noiDungLoi);
                            }
                        }
                    }
                }
            }

            if (ketQua.TongSoDong == 0)
                throw new ArgumentException(
                    "File Excel không có dòng dữ liệu hợp lệ để xử lý!");

            // Luu lich su Import Excel
            LuuLichSuExcel(duongDan, ketQua);

            return ketQua;
        }

        // ============================================
        // 9. GHI LICH SU IMPORT EXCEL
        // ============================================
        private void LuuLichSuExcel(
            string duongDan, KetQuaImportResult ketQua)
        {
            try
            {
                LichSuNhapDuLieu lichSu =
                    new LichSuNhapDuLieu
                    {
                        TenFile = Path.GetFileName(duongDan),
                        LoaiFile = "XLSX",
                        TongSoDong = ketQua.TongSoDong,
                        SoDongThanh = ketQua.SoDongThanh,
                        SoDongLoi = ketQua.SoDongLoi,
                        TrangThai = ketQua.TrangThai
                    };

                ketQua.DaLuuLichSu =
                    lichSuRepository.GhiLichSu(lichSu);

                if (!ketQua.DaLuuLichSu)
                {
                    ketQua.CanhBaoLichSu =
                        "Import đã xử lý nhưng chưa lưu được lịch sử!";
                }
            }
            catch (Exception ex)
            {
                ketQua.DaLuuLichSu = false;

                ketQua.CanhBaoLichSu =
                    "Không thể ghi lịch sử Import Excel: " +
                    ex.Message;
            }
        }

        // ============================================
        // 2. THEM DIEM, KHONG CHO TRUNG KET QUA
        // ============================================
        private bool ThemKetQua(
            SqlConnection conn,
            string maSV,
            string maMon,
            string maHocKy,
            decimal? chuyenCan,
            decimal? giuaKy,
            decimal? cuoiKy)
        {
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
                SELECT
                    @MaSV,
                    @MaMonHoc,
                    @MaHocKy,
                    @DiemChuyenCan,
                    @DiemGiuaKy,
                    @DiemCuoiKy
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM dbo.KetQua WITH (UPDLOCK, HOLDLOCK)
                    WHERE MaSV = @MaSV
                      AND MaMonHoc = @MaMonHoc
                      AND MaHocKy = @MaHocKy
                )";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@MaSV", SqlDbType.VarChar, 20)
                    .Value = maSV;

                cmd.Parameters.Add("@MaMonHoc", SqlDbType.VarChar, 20)
                    .Value = maMon;

                cmd.Parameters.Add("@MaHocKy", SqlDbType.VarChar, 20)
                    .Value = maHocKy;

                GanDiem(cmd, "@DiemChuyenCan", chuyenCan);
                GanDiem(cmd, "@DiemGiuaKy", giuaKy);
                GanDiem(cmd, "@DiemCuoiKy", cuoiKy);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        private void GanDiem(
            SqlCommand cmd, string tenThamSo, decimal? diem)
        {
            SqlParameter p = cmd.Parameters.Add(
                tenThamSo, SqlDbType.Decimal);

            p.Precision = 4;
            p.Scale = 2;

            p.Value = diem.HasValue
                ? (object)diem.Value
                : DBNull.Value;
        }

        // ============================================
        // 3. KIEM TRA MA SINH VIEN, MON HOC, HOC KY
        // ============================================
        private string KiemTraMa(string giaTri, string tenTruong)
        {
            string ma = (giaTri ?? "").Trim();

            if (ma.Length == 0 || ma.Length > 20)
                throw new ArgumentException(
                    tenTruong + " phải có từ 1 đến 20 ký tự!");

            return ma;
        }

        // ============================================
        // 4. DOC DIEM, CHO PHEP NULL
        // ============================================
        private decimal? DocDiem(string giaTri, string tenDiem)
        {
            if (string.IsNullOrWhiteSpace(giaTri))
                return null;

            string chuoi = giaTri.Trim().Replace(',', '.');

            decimal diem;

            bool hopLe = decimal.TryParse(
                chuoi,
                NumberStyles.AllowLeadingSign |
                NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture,
                out diem);

            if (!hopLe)
                throw new ArgumentException(
                    tenDiem + " không hợp lệ!");

            if (diem < 0 || diem > 10)
                throw new ArgumentException(
                    tenDiem + " phải nằm trong khoảng 0 đến 10!");

            if (decimal.Round(diem, 2) != diem)
                throw new ArgumentException(
                    tenDiem + " chỉ được tối đa 2 chữ số thập phân!");

            return diem;
        }

        // ============================================
        // 5. KIEM TRA TIEU DE CSV
        // ============================================
        private void KiemTraTieuDe(string[] tieuDe)
        {
            string[] cotBatBuoc =
            {
                "MaSV",
                "MaMonHoc",
                "MaHocKy",
                "DiemChuyenCan",
                "DiemGiuaKy",
                "DiemCuoiKy"
            };

            if (tieuDe.Length != cotBatBuoc.Length)
                throw new ArgumentException(
                    "File CSV phải có đúng 6 cột!");

            for (int i = 0; i < cotBatBuoc.Length; i++)
            {
                string tenCot = tieuDe[i].Trim().TrimStart('\uFEFF');

                if (!string.Equals(
                    tenCot,
                    cotBatBuoc[i],
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException(
                        "Sai tiêu đề cột " + (i + 1) +
                        ". Yêu cầu: " + cotBatBuoc[i]);
                }
            }
        }

        // ============================================
        // 6. XAC DINH DAU PHAN CACH
        // ============================================
        private char XacDinhDauPhanCach(string noiDung)
        {
            int viTriXuốngDong = noiDung.IndexOfAny(
                new char[] { '\r', '\n' });

            string dongDau = viTriXuốngDong >= 0
                ? noiDung.Substring(0, viTriXuốngDong)
                : noiDung;

            return dongDau.Contains(";") ? ';' : ',';
        }

        // ============================================
        // 7. TACH CSV, HO TRO GIA TRI CO DAU NGOAC KEP
        // ============================================
        private List<string[]> TachCsv(string noiDung, char dauPhanCach)
        {
            List<string[]> ketQua = new List<string[]>();
            List<string> cacCot = new List<string>();
            StringBuilder giaTri = new StringBuilder();

            bool trongNgoacKep = false;

            for (int i = 0; i < noiDung.Length; i++)
            {
                char kyTu = noiDung[i];

                if (kyTu == '"')
                {
                    if (trongNgoacKep &&
                        i + 1 < noiDung.Length &&
                        noiDung[i + 1] == '"')
                    {
                        giaTri.Append('"');
                        i++;
                    }
                    else
                    {
                        trongNgoacKep = !trongNgoacKep;
                    }
                }
                else if (kyTu == dauPhanCach && !trongNgoacKep)
                {
                    cacCot.Add(giaTri.ToString());
                    giaTri.Clear();
                }
                else if ((kyTu == '\r' || kyTu == '\n') &&
                         !trongNgoacKep)
                {
                    cacCot.Add(giaTri.ToString());
                    giaTri.Clear();

                    ketQua.Add(cacCot.ToArray());
                    cacCot.Clear();

                    if (kyTu == '\r' &&
                        i + 1 < noiDung.Length &&
                        noiDung[i + 1] == '\n')
                        i++;
                }
                else
                {
                    giaTri.Append(kyTu);
                }
            }

            if (trongNgoacKep)
                throw new FormatException(
                    "File CSV có dấu ngoặc kép chưa đóng!");

            if (giaTri.Length > 0 || cacCot.Count > 0)
            {
                cacCot.Add(giaTri.ToString());
                ketQua.Add(cacCot.ToArray());
            }

            return ketQua;
        }

        private bool DongTrong(string[] dong)
        {
            foreach (string giaTri in dong)
            {
                if (!string.IsNullOrWhiteSpace(giaTri))
                    return false;
            }

            return true;
        }
    }
}
