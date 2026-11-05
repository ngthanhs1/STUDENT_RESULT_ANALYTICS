# HƯỚNG DẪN BỘ DỮ LIỆU MẪU IMPORT ĐIỂM (STUDENT RESULT ANALYTICS)

Thư mục `Sample_Data/` chứa đầy đủ các file dữ liệu chuẩn (.xlsx và .csv) để kiểm thử, demo đồ án và nạp dữ liệu vào hệ thống.

---

## 1. Cấu trúc chuẩn của File Import
Tất cả các file đều tuân thủ chính xác 6 cột theo quy định của hệ thống:
| Cột | Ý nghĩa | Kiểu dữ liệu | Ví dụ |
| :--- | :--- | :--- | :--- |
| **MaSV** | Mã định danh sinh viên | Chuỗi (tối đa 20 ký tự) | `23010192`, `SV001` |
| **MaMonHoc** | Mã môn học đào tạo | Chuỗi (tối đa 20 ký tự) | `MH001`, `MH004`, `MH007` |
| **MaHocKy** | Mã học kỳ niên khóa | Chuỗi (tối đa 20 ký tự) | `HK1_2025_2026`, `HK2_2025_2026` |
| **DiemChuyenCan** | Điểm chuyên cần (10%) | Số thập phân (0.0 - 10.0) | `8.5` |
| **DiemGiuaKy** | Điểm kiểm tra giữa kỳ (30%) | Số thập phân (0.0 - 10.0) | `8.0` |
| **DiemCuoiKy** | Điểm thi kết thúc học phần (60%) | Số thập phân (0.0 - 10.0) | `9.0` |

---

## 2. Danh mục các File mẫu có sẵn

### 📘 File 04: `04_BangDiem_LopCNTT02_HK1.xlsx` (Excel - 33 dòng)
* **Đối tượng:** Toàn bộ sinh viên lớp CNTT02 (bao gồm `2302006`, `23010192` đến `23010200`, `SV003`).
* **Học kỳ & Môn học:** Học kỳ 1 (2025-2026), 3 môn: Mạng máy tính (`MH004`), Kiến trúc máy tính & HĐH (`MH005`), Lập trình Web (`MH006`).
* **Mục đích:** Nạp dữ liệu hoàn chỉnh cho cả lớp, kiểm tra tính ĐTB và xếp loại học lực của lớp CNTT02.

### 📗 File 05: `05_BangDiem_LopCNTT01_HK2.csv` (CSV - 21 dòng)
* **Đối tượng:** Sinh viên lớp CNTT01 (`SV001`, `SV002`, `SV004` đến `SV008`).
* **Học kỳ & Môn học:** Học kỳ 2 (2025-2026), 3 môn: `MH004`, `MH005`, `MH006`.
* **Mục đích:** Kiểm tra khả năng nạp file định dạng CSV mã hóa UTF-8.

### 📙 File 06: `06_BangDiem_MonHoc_TriTueNhanTao_MH007.xlsx` (Excel - 18 dòng)
* **Đối tượng:** 18 sinh viên thuộc nhiều lớp khác nhau (`CNTT01`, `CNTT02`, `KTPM01`, `KHMT01`).
* **Môn học:** Trí tuệ nhân tạo cơ sở (`MH007`), Học kỳ 2 (2025-2026).
* **Mục đích:** Kiểm tra sổ điểm học phần theo từng môn học và biểu đồ phân bố điểm môn học trên Dashboard.

### 🚨 File 07: `07_BangDiem_KiemThu_CanhBaoHocVu.xlsx` (Excel - 12 dòng)
* **Đặc điểm đặc biệt:** Chứa cả trường hợp sinh viên có kết quả trượt và sinh viên thiếu cột điểm:
  * Sinh viên `SV011`, `SV012`, `SV013` có điểm thi cuối kỳ thấp ($\le 3.0$), kéo điểm tổng kết $< 4.0$ (Trượt môn).
  * Sinh viên `SV009`, `SV010` để trống cột điểm Cuối kỳ (Vắng thi / Chưa thi).
  * Sinh viên `SV011` để trống cột điểm Giữa kỳ.
* **Mục đích:** Kiểm thử ngay tính năng **"Cảnh báo học vụ"** và **"Danh sách sinh viên thiếu điểm"** trên hệ thống.

### 📊 File 08: `08_BangDiem_TongHop_NienKhoa2024_2025.csv` (CSV - 40 dòng)
* **Đối tượng:** 10 sinh viên lớp CNTT02 trong niên khóa trước (2024-2025) qua cả 2 học kỳ.
* **Mục đích:** Tạo cơ sở dữ liệu lịch sử nhiều năm học để theo dõi sự tiến bộ học lực của sinh viên qua các kỳ.

### ⚠️ File 03: `03_Diem_CoLoi_DemoKiemTra.csv` (File cố tình tạo lỗi)
* **Mục đích:** Demo cho giảng viên xem cơ chế bắt lỗi và bảo vệ dữ liệu của hệ thống:
  * Lỗi điểm âm (`-2.0`), điểm $> 10$ (`15.0`), điểm chữ (`abc`).
  * Lỗi mã sinh viên lạ (`SV999`), mã môn lạ (`MH999`), mã học kỳ không tồn tại (`HK999`).

---

## 3. Cách thức nạp vào hệ thống
1. Mở phần mềm, đăng nhập tài khoản Quản trị (`admin` / `123456`) hoặc Nhân viên (`nhanvien` / `123456`).
2. Vào menu **Khảo thí & Điểm số** $\rightarrow$ **Nhập điểm từ File (Excel/CSV)**.
3. Bấm **CHỌN FILE...**, duyệt tới thư mục `Sample_Data/` và chọn file bạn muốn nạp.
4. Bấm **THỰC HIỆN IMPORT**.
5. Sau khi nạp xong:
   * Vào **Sổ điểm học phần** để xem danh sách điểm chi tiết.
   * Vào **Tra cứu GPA & Học lực** để xem bảng xếp loại và biểu đồ học lực.
   * Vào **Thống kê Đạt/Trượt & Cảnh báo** để xem danh sách trượt/thiếu điểm.
