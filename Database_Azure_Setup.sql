-- ======================================================================
-- SCRIPT KHOI TAO CSDL CHO MICROSOFT AZURE SQL DATABASE
-- He thong: Student Result Analytics
-- ======================================================================

-- 1. TAO BANG LOP
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Lop')
BEGIN
    CREATE TABLE dbo.Lop (
        MaLop VARCHAR(20) PRIMARY KEY,
        TenLop NVARCHAR(100) NOT NULL,
        KhoaHoc NVARCHAR(20) NULL,
        Khoa NVARCHAR(100) NULL
    );
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Lop' AND COLUMN_NAME = 'Khoa')
    BEGIN
        ALTER TABLE dbo.Lop ADD Khoa NVARCHAR(100) NULL;
    END
END
GO

-- 2. TAO BANG SINH VIEN
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SinhVien')
BEGIN
    CREATE TABLE dbo.SinhVien (
        MaSV VARCHAR(20) PRIMARY KEY,
        HoTen NVARCHAR(100) NOT NULL,
        NgaySinh DATE NULL,
        GioiTinh NVARCHAR(10) NULL,
        Email VARCHAR(100) NULL,
        MaLop VARCHAR(20) NOT NULL REFERENCES dbo.Lop(MaLop)
    );
END
GO

-- 3. TAO BANG MON HOC
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MonHoc')
BEGIN
    CREATE TABLE dbo.MonHoc (
        MaMonHoc VARCHAR(20) PRIMARY KEY,
        TenMonHoc NVARCHAR(150) NOT NULL,
        SoTinChi INT NOT NULL
    );
END
GO

-- 4. TAO BANG HOC KY
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'HocKy')
BEGIN
    CREATE TABLE dbo.HocKy (
        MaHocKy VARCHAR(20) PRIMARY KEY,
        TenHocKy NVARCHAR(50) NOT NULL,
        NamHoc VARCHAR(9) NOT NULL,
        NgayBatDau DATE NULL,
        NgayKetThuc DATE NULL
    );
END
GO

-- 5. TAO BANG KET QUA
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'KetQua')
BEGIN
    CREATE TABLE dbo.KetQua (
        MaKetQua INT IDENTITY(1,1) PRIMARY KEY,
        MaSV VARCHAR(20) NOT NULL REFERENCES dbo.SinhVien(MaSV),
        MaMonHoc VARCHAR(20) NOT NULL REFERENCES dbo.MonHoc(MaMonHoc),
        MaHocKy VARCHAR(20) NOT NULL REFERENCES dbo.HocKy(MaHocKy),
        DiemChuyenCan DECIMAL(4,2) NULL,
        DiemGiuaKy DECIMAL(4,2) NULL,
        DiemCuoiKy DECIMAL(4,2) NULL,
        TyLeDiem VARCHAR(20) NOT NULL DEFAULT ('10-30-60'),
        DiemTongKet AS (CONVERT(DECIMAL(4,2), ROUND(
            CASE 
                WHEN TyLeDiem = '10-40-50' THEN (DiemChuyenCan * 0.1 + DiemGiuaKy * 0.4) + DiemCuoiKy * 0.5
                ELSE (DiemChuyenCan * 0.1 + DiemGiuaKy * 0.3) + DiemCuoiKy * 0.6
            END, 2))) PERSISTED
    );
END
GO

-- 6. TAO BANG TAI KHOAN
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TaiKhoan')
BEGIN
    CREATE TABLE dbo.TaiKhoan (
        MaTaiKhoan INT IDENTITY(1,1) PRIMARY KEY,
        TenDangNhap VARCHAR(50) NOT NULL UNIQUE,
        MatKhauHash NVARCHAR(255) NOT NULL,
        VaiTro VARCHAR(20) NOT NULL DEFAULT ('NhanVien'),
        TrangThai BIT NOT NULL DEFAULT (1),
        NgayTao DATETIME2 NOT NULL DEFAULT (SYSUTCDATETIME())
    );
END
GO

-- 7. TAO BANG LICH SU NHAP DU LIEU
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LichSuNhapDuLieu')
BEGIN
    CREATE TABLE dbo.LichSuNhapDuLieu (
        MaLichSu INT IDENTITY(1,1) PRIMARY KEY,
        TenFile NVARCHAR(255) NOT NULL,
        LoaiFile VARCHAR(10) NOT NULL,
        ThoiGianNhap DATETIME2 NOT NULL DEFAULT (SYSDATETIME()),
        TongSoDong INT NOT NULL DEFAULT (0),
        SoDongThanhCong INT NOT NULL DEFAULT (0),
        SoDongLoi INT NOT NULL DEFAULT (0),
        TrangThai VARCHAR(20) NOT NULL DEFAULT ('ThanhCong'),
        GhiChu NVARCHAR(500) NULL,
        MaTaiKhoan INT NULL REFERENCES dbo.TaiKhoan(MaTaiKhoan)
    );
END
GO

-- 8. TAO CAC VIEWS HE THONG
CREATE OR ALTER VIEW dbo.vw_DiemTrungBinhHocKy
AS
SELECT
    sv.MaSV,
    sv.HoTen,
    sv.MaLop,
    hk.MaHocKy,
    hk.TenHocKy,
    hk.NamHoc,
    COUNT(*) AS TongSoMon,
    SUM(mh.SoTinChi) AS TongSoTinChi,
    SUM(CASE WHEN k.DiemTongKet IS NULL THEN 1 ELSE 0 END) AS SoMonThieuDiem,
    SUM(CASE WHEN k.DiemTongKet IS NOT NULL THEN mh.SoTinChi ELSE 0 END) AS SoTinChiDaCoDiem,
    CAST(
        SUM(CASE WHEN k.DiemTongKet IS NOT NULL THEN k.DiemTongKet * mh.SoTinChi ELSE 0 END)
        / NULLIF(SUM(CASE WHEN k.DiemTongKet IS NOT NULL THEN mh.SoTinChi ELSE 0 END), 0)
        AS DECIMAL(5,2)
    ) AS DiemTrungBinhTamTinh,
    CAST(
        SUM(CASE
            WHEN k.DiemTongKet IS NOT NULL
            THEN (CASE
                WHEN k.DiemTongKet >= 9.0 THEN 4.0
                WHEN k.DiemTongKet >= 8.5 THEN 3.7
                WHEN k.DiemTongKet >= 8.0 THEN 3.5
                WHEN k.DiemTongKet >= 7.0 THEN 3.0
                WHEN k.DiemTongKet >= 6.5 THEN 2.5
                WHEN k.DiemTongKet >= 5.5 THEN 2.0
                WHEN k.DiemTongKet >= 5.0 THEN 1.5
                WHEN k.DiemTongKet >= 4.0 THEN 1.0
                ELSE 0.0
            END) * mh.SoTinChi
            ELSE 0
        END)
        / NULLIF(SUM(CASE WHEN k.DiemTongKet IS NOT NULL THEN mh.SoTinChi ELSE 0 END), 0)
        AS DECIMAL(4,2)
    ) AS DiemTrungBinhHe4,
    CASE
        WHEN SUM(CASE WHEN k.DiemTongKet IS NULL THEN 1 ELSE 0 END) > 0
        THEN N'Chưa đủ điểm'
        ELSE N'Đủ điểm'
    END AS TrangThai
FROM dbo.KetQua AS k
JOIN dbo.SinhVien AS sv ON k.MaSV = sv.MaSV
JOIN dbo.MonHoc AS mh ON k.MaMonHoc = mh.MaMonHoc
JOIN dbo.HocKy AS hk ON k.MaHocKy = hk.MaHocKy
GROUP BY sv.MaSV, sv.HoTen, sv.MaLop, hk.MaHocKy, hk.TenHocKy, hk.NamHoc;
GO
CREATE OR ALTER VIEW dbo.vw_PhanLoaiHocLuc
AS
SELECT
    d.MaSV,
    d.HoTen,
    d.MaLop,
    d.MaHocKy,
    d.TenHocKy,
    d.NamHoc,
    d.TongSoMon,
    d.SoMonThieuDiem,
    d.DiemTrungBinhTamTinh AS DiemTrungBinh,
    d.DiemTrungBinhHe4,
    CASE
        WHEN d.SoMonThieuDiem > 0 OR d.DiemTrungBinhTamTinh IS NULL THEN N'Chưa đủ điểm'
        WHEN d.DiemTrungBinhHe4 >= 3.60 THEN N'Xuất sắc'
        WHEN d.DiemTrungBinhHe4 >= 3.20 THEN N'Giỏi'
        WHEN d.DiemTrungBinhHe4 >= 2.50 THEN N'Khá'
        WHEN d.DiemTrungBinhHe4 >= 2.00 THEN N'Trung bình'
        WHEN d.DiemTrungBinhHe4 >= 1.00 THEN N'Yếu'
        ELSE N'Kém'
    END AS XepLoai,
    CASE
        WHEN d.SoMonThieuDiem > 0 OR d.DiemTrungBinhTamTinh IS NULL THEN N'Cần bổ sung điểm'
        WHEN d.DiemTrungBinhHe4 < 2.00 THEN N'Cảnh báo học tập'
        ELSE N'Bình thường'
    END AS CanhBao
FROM dbo.vw_DiemTrungBinhHocKy AS d;
GO
CREATE OR ALTER VIEW dbo.vw_SinhVienThieuDiem
AS
SELECT
    sv.MaSV,
    sv.HoTen,
    mh.MaMonHoc,
    mh.TenMonHoc,
    hk.MaHocKy,
    hk.TenHocKy,
    hk.NamHoc,
    k.DiemChuyenCan,
    k.DiemGiuaKy,
    k.DiemCuoiKy,
    CONCAT_WS(N', ',
        CASE WHEN k.DiemChuyenCan IS NULL THEN N'Thiếu điểm chuyên cần' END,
        CASE WHEN k.DiemGiuaKy IS NULL THEN N'Thiếu điểm giữa kỳ' END,
        CASE WHEN k.DiemCuoiKy IS NULL THEN N'Thiếu điểm cuối kỳ' END
    ) AS NoiDungCanhBao
FROM dbo.KetQua AS k
JOIN dbo.SinhVien AS sv ON k.MaSV = sv.MaSV
JOIN dbo.MonHoc AS mh ON k.MaMonHoc = mh.MaMonHoc
JOIN dbo.HocKy AS hk ON k.MaHocKy = hk.MaHocKy
WHERE k.DiemChuyenCan IS NULL OR k.DiemGiuaKy IS NULL OR k.DiemCuoiKy IS NULL;
GO
CREATE OR ALTER VIEW dbo.vw_ThongKeDatTruot
AS
SELECT
    mh.MaMonHoc,
    mh.TenMonHoc,
    mh.SoTinChi,
    hk.MaHocKy,
    hk.TenHocKy,
    hk.NamHoc,
    COUNT(k.MaKetQua) AS TongSinhVien,
    SUM(CASE WHEN k.DiemTongKet >= 4.0 THEN 1 ELSE 0 END) AS SoSinhVienDat,
    SUM(CASE WHEN k.DiemTongKet < 4.0 THEN 1 ELSE 0 END) AS SoSinhVienTruot,
    SUM(CASE WHEN k.DiemTongKet IS NULL THEN 1 ELSE 0 END) AS SoSinhVienThieuDiem,
    ISNULL(CAST(
        100.0 * SUM(CASE WHEN k.DiemTongKet >= 4.0 THEN 1 ELSE 0 END)
        / NULLIF(COUNT(k.DiemTongKet), 0)
        AS DECIMAL(5,2)
    ), 0.00) AS TyLeDat
FROM dbo.KetQua AS k
JOIN dbo.MonHoc AS mh ON k.MaMonHoc = mh.MaMonHoc
JOIN dbo.HocKy AS hk ON k.MaHocKy = hk.MaHocKy
GROUP BY mh.MaMonHoc, mh.TenMonHoc, mh.SoTinChi, hk.MaHocKy, hk.TenHocKy, hk.NamHoc;
GO

-- 9. NAP DU LIEU MAU BAN DAU

-- Lop:
IF NOT EXISTS (SELECT 1 FROM dbo.Lop WHERE MaLop = 'CNTT01') INSERT INTO dbo.Lop VALUES ('CNTT01', N'Công nghệ thông tin 01', N'K18', N'Khoa Công nghệ thông tin');
IF NOT EXISTS (SELECT 1 FROM dbo.Lop WHERE MaLop = 'CNTT02') INSERT INTO dbo.Lop VALUES ('CNTT02', N'Công nghệ thông tin 02', N'K18', N'Khoa Công nghệ thông tin');
IF NOT EXISTS (SELECT 1 FROM dbo.Lop WHERE MaLop = 'KTPM01') INSERT INTO dbo.Lop VALUES ('KTPM01', N'Kỹ thuật phần mềm 01', N'K18', N'Khoa Công nghệ thông tin');
IF NOT EXISTS (SELECT 1 FROM dbo.Lop WHERE MaLop = 'KHMT01') INSERT INTO dbo.Lop VALUES ('KHMT01', N'Khoa học máy tính 01', N'K18', N'Khoa Khoa học máy tính');
IF NOT EXISTS (SELECT 1 FROM dbo.Lop WHERE MaLop = 'HTTT01') INSERT INTO dbo.Lop VALUES ('HTTT01', N'Hệ thống thông tin 01', N'K18', N'Khoa Hệ thống thông tin');

-- MonHoc:
IF NOT EXISTS (SELECT 1 FROM dbo.MonHoc WHERE MaMonHoc = 'MH001') INSERT INTO dbo.MonHoc VALUES ('MH001', N'Lập trình C#', 3);
IF NOT EXISTS (SELECT 1 FROM dbo.MonHoc WHERE MaMonHoc = 'MH002') INSERT INTO dbo.MonHoc VALUES ('MH002', N'Cơ sở dữ liệu', 3);
IF NOT EXISTS (SELECT 1 FROM dbo.MonHoc WHERE MaMonHoc = 'MH003') INSERT INTO dbo.MonHoc VALUES ('MH003', N'Cấu trúc dữ liệu', 2);
IF NOT EXISTS (SELECT 1 FROM dbo.MonHoc WHERE MaMonHoc = 'MH004') INSERT INTO dbo.MonHoc VALUES ('MH004', N'Mạng máy tính', 3);
IF NOT EXISTS (SELECT 1 FROM dbo.MonHoc WHERE MaMonHoc = 'MH005') INSERT INTO dbo.MonHoc VALUES ('MH005', N'Kiến trúc máy tính & HĐH', 3);
IF NOT EXISTS (SELECT 1 FROM dbo.MonHoc WHERE MaMonHoc = 'MH006') INSERT INTO dbo.MonHoc VALUES ('MH006', N'Lập trình Web nâng cao', 3);
IF NOT EXISTS (SELECT 1 FROM dbo.MonHoc WHERE MaMonHoc = 'MH007') INSERT INTO dbo.MonHoc VALUES ('MH007', N'Trí tuệ nhân tạo cơ sở', 3);
IF NOT EXISTS (SELECT 1 FROM dbo.MonHoc WHERE MaMonHoc = 'MH008') INSERT INTO dbo.MonHoc VALUES ('MH008', N'An toàn thông tin', 3);

-- HocKy:
IF NOT EXISTS (SELECT 1 FROM dbo.HocKy WHERE MaHocKy = 'HK1_2024_2025') INSERT INTO dbo.HocKy VALUES ('HK1_2024_2025', N'Học kỳ 1 (2024-2025)', '2024-2025', '2024-09-01', '2025-01-15');
IF NOT EXISTS (SELECT 1 FROM dbo.HocKy WHERE MaHocKy = 'HK2_2024_2025') INSERT INTO dbo.HocKy VALUES ('HK2_2024_2025', N'Học kỳ 2 (2024-2025)', '2024-2025', '2025-02-01', '2025-06-30');
IF NOT EXISTS (SELECT 1 FROM dbo.HocKy WHERE MaHocKy = 'HK1_2025_2026') INSERT INTO dbo.HocKy VALUES ('HK1_2025_2026', N'Học kỳ 1', '2025-2026', '2025-09-01', '2026-01-15');
IF NOT EXISTS (SELECT 1 FROM dbo.HocKy WHERE MaHocKy = 'HK2_2025_2026') INSERT INTO dbo.HocKy VALUES ('HK2_2025_2026', N'Học kỳ 2', '2025-2026', '2026-02-01', '2026-06-30');

-- SinhVien:
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = '23010191') INSERT INTO dbo.SinhVien VALUES ('23010191', N'Thành Ng', NULL, N'Nam', '23010191st.phenikaaa.eu.vn', 'CNTT02');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = '2302006') INSERT INTO dbo.SinhVien VALUES ('2302006', N'Nguyễn Văn', '2026-11-08', N'Nữ', '', 'CNTT02');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = '23010192') INSERT INTO dbo.SinhVien VALUES ('23010192', N'Trịnh Quốc Bảo', '2005-04-18', N'Nam', '23010192@phenikaa-uni.edu.vn', 'CNTT02');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = '23010193') INSERT INTO dbo.SinhVien VALUES ('23010193', N'Phạm Thu Hà', '2005-09-22', N'Nữ', '23010193@phenikaa-uni.edu.vn', 'CNTT02');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = '23010194') INSERT INTO dbo.SinhVien VALUES ('23010194', N'Hoàng Minh Đức', '2005-01-15', N'Nam', '23010194@phenikaa-uni.edu.vn', 'CNTT02');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = '23010195') INSERT INTO dbo.SinhVien VALUES ('23010195', N'Đỗ Thị Mai', '2005-11-30', N'Nữ', '23010195@phenikaa-uni.edu.vn', 'CNTT02');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = '23010196') INSERT INTO dbo.SinhVien VALUES ('23010196', N'Vũ Hải Nam', '2005-06-08', N'Nam', '23010196@phenikaa-uni.edu.vn', 'CNTT02');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = '23010197') INSERT INTO dbo.SinhVien VALUES ('23010197', N'Nguyễn Kiều Trang', '2005-03-25', N'Nữ', '23010197@phenikaa-uni.edu.vn', 'CNTT02');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = '23010198') INSERT INTO dbo.SinhVien VALUES ('23010198', N'Bùi Tuấn Anh', '2005-12-12', N'Nam', '23010198@phenikaa-uni.edu.vn', 'CNTT02');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = '23010199') INSERT INTO dbo.SinhVien VALUES ('23010199', N'Lê Phương Thảo', '2005-07-04', N'Nữ', '23010199@phenikaa-uni.edu.vn', 'CNTT02');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = '23010200') INSERT INTO dbo.SinhVien VALUES ('23010200', N'Đặng Gia Huy', '2005-08-19', N'Nam', '23010200@phenikaa-uni.edu.vn', 'CNTT02');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV001') INSERT INTO dbo.SinhVien VALUES ('SV001', N'Nguyễn Văn An', '2005-05-12', N'Nam', 'sv001@example.com', 'CNTT01');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV002') INSERT INTO dbo.SinhVien VALUES ('SV002', N'Trần Thị Bình', '2005-08-20', N'Nữ', 'sv002@example.com', 'CNTT01');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV003') INSERT INTO dbo.SinhVien VALUES ('SV003', N'Lê Minh Cường', '2005-02-15', N'Nam', 'sv003@example.com', 'CNTT02');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV004') INSERT INTO dbo.SinhVien VALUES ('SV004', N'Ngô Bá Đạt', '2005-05-10', N'Nam', 'sv004@example.com', 'CNTT01');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV005') INSERT INTO dbo.SinhVien VALUES ('SV005', N'Lương Thị Ánh', '2005-10-14', N'Nữ', 'sv005@example.com', 'CNTT01');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV006') INSERT INTO dbo.SinhVien VALUES ('SV006', N'Phan Thanh Tùng', '2005-02-28', N'Nam', 'sv006@example.com', 'CNTT01');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV007') INSERT INTO dbo.SinhVien VALUES ('SV007', N'Dương Ngọc Mai', '2005-12-05', N'Nữ', 'sv007@example.com', 'CNTT01');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV008') INSERT INTO dbo.SinhVien VALUES ('SV008', N'Hà Quang Dũng', '2005-07-17', N'Nam', 'sv008@example.com', 'CNTT01');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV009') INSERT INTO dbo.SinhVien VALUES ('SV009', N'Võ Thị Hồng Nhung', '2005-09-03', N'Nữ', 'sv009@example.com', 'KTPM01');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV010') INSERT INTO dbo.SinhVien VALUES ('SV010', N'Tạ Minh Khang', '2005-04-12', N'Nam', 'sv010@example.com', 'KTPM01');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV011') INSERT INTO dbo.SinhVien VALUES ('SV011', N'Đinh Bảo Ngọc', '2005-08-27', N'Nữ', 'sv011@example.com', 'KTPM01');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV012') INSERT INTO dbo.SinhVien VALUES ('SV012', N'Cao Văn Phát', '2005-01-21', N'Nam', 'sv012@example.com', 'KHMT01');
IF NOT EXISTS (SELECT 1 FROM dbo.SinhVien WHERE MaSV = 'SV013') INSERT INTO dbo.SinhVien VALUES ('SV013', N'Mai Hoàng Yến', '2005-06-15', N'Nữ', 'sv013@example.com', 'KHMT01');

-- TaiKhoan:
IF NOT EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = 'admin') INSERT INTO dbo.TaiKhoan (TenDangNhap, MatKhauHash, VaiTro, TrangThai) VALUES ('admin', N'PBKDF2-SHA256$600000$+SlNkxL/iA6i+JQ5grKB9Q==$hFQLHQb4OcUclNzSbWl2hpOOIQXwsfTqyoNa5nTFYB4=', 'Admin', 1);
IF NOT EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = 'nhanvien') INSERT INTO dbo.TaiKhoan (TenDangNhap, MatKhauHash, VaiTro, TrangThai) VALUES ('nhanvien', N'PBKDF2-SHA256$600000$Wdo7uTYp+8QwuwRFa2IRHA==$aIdeScqltg+OXd+L3ovYN16WoZlT952IsDvgm/HeYYY=', 'NhanVien', 1);
IF NOT EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = 'nhanvien1') INSERT INTO dbo.TaiKhoan (TenDangNhap, MatKhauHash, VaiTro, TrangThai) VALUES ('nhanvien1', N'PBKDF2-SHA256$600000$SoapY5vn4tUcMQvy6nKYIQ==$cfZwpyNLxZ6iTdNFNv1Op3ZP9QY6PWvUIyd3azzje/I=', 'NhanVien', 1);

-- KetQua:
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV001' AND MaMonHoc = 'MH001' AND MaHocKy = 'HK1_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV001', 'MH001', 'HK1_2025_2026', 9.00, 8.00, 9.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV001' AND MaMonHoc = 'MH002' AND MaHocKy = 'HK1_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV001', 'MH002', 'HK1_2025_2026', 8.00, 7.00, 8.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV001' AND MaMonHoc = 'MH003' AND MaHocKy = 'HK1_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV001', 'MH003', 'HK1_2025_2026', 9.00, 9.00, 9.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV002' AND MaMonHoc = 'MH001' AND MaHocKy = 'HK1_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV002', 'MH001', 'HK1_2025_2026', 7.00, 6.00, 7.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV002' AND MaMonHoc = 'MH002' AND MaHocKy = 'HK1_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV002', 'MH002', 'HK1_2025_2026', 6.00, 7.00, 6.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV002' AND MaMonHoc = 'MH003' AND MaHocKy = 'HK1_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV002', 'MH003', 'HK1_2025_2026', 8.00, 7.00, 8.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV003' AND MaMonHoc = 'MH001' AND MaHocKy = 'HK1_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV003', 'MH001', 'HK1_2025_2026', 5.00, 4.00, 4.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV003' AND MaMonHoc = 'MH002' AND MaHocKy = 'HK1_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV003', 'MH002', 'HK1_2025_2026', 6.00, 4.00, 5.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV003' AND MaMonHoc = 'MH003' AND MaHocKy = 'HK1_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV003', 'MH003', 'HK1_2025_2026', 5.00, 2.00, 6.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV001' AND MaMonHoc = 'MH001' AND MaHocKy = 'HK2_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV001', 'MH001', 'HK2_2025_2026', 8.00, 7.50, 9.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV002' AND MaMonHoc = 'MH002' AND MaHocKy = 'HK2_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV002', 'MH002', 'HK2_2025_2026', 7.00, 8.00, 8.50, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV003' AND MaMonHoc = 'MH001' AND MaHocKy = 'HK2_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV003', 'MH001', 'HK2_2025_2026', 9.00, 8.00, 8.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV001' AND MaMonHoc = 'MH003' AND MaHocKy = 'HK2_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV001', 'MH003', 'HK2_2025_2026', 6.50, 7.50, 8.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV001' AND MaMonHoc = 'MH002' AND MaHocKy = 'HK2_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV001', 'MH002', 'HK2_2025_2026', 8.00, 7.00, 8.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV002' AND MaMonHoc = 'MH001' AND MaHocKy = 'HK2_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV002', 'MH001', 'HK2_2025_2026', 9.00, 8.00, 7.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV002' AND MaMonHoc = 'MH003' AND MaHocKy = 'HK2_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV002', 'MH003', 'HK2_2025_2026', 6.00, 8.00, 7.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV003' AND MaMonHoc = 'MH002' AND MaHocKy = 'HK2_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV003', 'MH002', 'HK2_2025_2026', 7.00, 6.00, 8.00, '10-30-60');
IF NOT EXISTS (SELECT 1 FROM dbo.KetQua WHERE MaSV = 'SV003' AND MaMonHoc = 'MH003' AND MaHocKy = 'HK2_2025_2026') INSERT INTO dbo.KetQua (MaSV, MaMonHoc, MaHocKy, DiemChuyenCan, DiemGiuaKy, DiemCuoiKy, TyLeDiem) VALUES ('SV003', 'MH003', 'HK2_2025_2026', 8.00, 7.00, 7.00, '10-30-60');
GO
PRINT N'KHOI TAO CSDL AZURE SQL THANH CONG!';
