# STUDENT RESULT ANALYTICS
**Hệ thống Quản lý Đào tạo và Phân tích Kết quả Học tập Đại học**

---

## 1. Giới thiệu tổng quan
**Student Result Analytics** là ứng dụng desktop cấp trường được xây dựng trên nền tảng **C# .NET WPF**, kết nối trực tiếp với **Microsoft SQL Server** thông qua **ADO.NET thuần** (không phụ thuộc Entity Framework để tối ưu hóa tốc độ và bảo đảm an toàn dữ liệu). 

Hệ thống được thiết kế theo chuẩn phân hệ **Quản lý Đào tạo & Khảo thí Đại học**, tuân thủ kiến trúc **MVVM (Model - View - ViewModel)** kết hợp phân lớp 3 tầng (3-Tier Architecture) rõ ràng, giao diện đa thẻ (TabControl) hiện đại, không cuộn dọc tràn màn hình.

---

## 2. Công nghệ và Kỹ thuật sử dụng
- **Nền tảng**: .NET Framework 4.7.2 (WPF / XAML).
- **Cơ sở dữ liệu**: Microsoft SQL Server (`StudentResultAnalyticsDB`).
- **Truy cập dữ liệu**: ADO.NET (`SqlConnection`, `SqlCommand`, `SqlDataReader`, `SqlDataAdapter`, `SqlParameter` chống SQL Injection 100%).
- **Kiến trúc mã nguồn**: MVVM Pattern (`BindableBase`, `RelayCommand`, `INotifyPropertyChanged`, `ObservableCollection`).
- **Thư viện xử lý Excel**: ClosedXML (0.105.1) xử lý file `.xlsx` tốc độ cao.
- **Bảo mật**: Thuật toán mã hóa mật khẩu `PBKDF2-HMAC-SHA256` với 600.000 vòng lặp (vượt chuẩn bảo mật OWASP).
- **Ràng buộc toàn vẹn CSDL**: Khóa chính, khóa ngoại, `CHECK (0 <= Diem <= 10)`, `UNIQUE (MaSV, MaMonHoc, MaHocKy)`, và cột tính toán tự động `DiemTongKet PERSISTED`.

---

## 3. Cấu trúc 5 Phân hệ Nghiệp vụ Chuẩn Đại học

Giao diện hệ thống được chia thành 5 phân hệ rõ ràng trên thanh Sidebar điều hướng kèm Breadcrumb trên thanh tiêu đề:

### 1. Phân hệ TỔNG QUAN
- **📊 Bàn làm việc (Dashboard)**: Thống kê chỉ số đào tạo toàn trường thời gian thực (Tổng sinh viên, tổng lớp, tổng môn, sinh viên cảnh báo); Bảng vinh danh Top sinh viên xuất sắc và bảng tổng hợp kết quả môn học.

### 2. Phân hệ QUẢN LÝ ĐÀO TẠO
- **👨‍🎓 Hồ sơ Sinh viên & Lớp học** *(Hệ thống 2 Tab)*:
  - *Tab 1: Danh sách & Hồ sơ Sinh viên*: Quản lý thông tin học viên, tra cứu nhanh theo mã/tên/email, lọc sinh viên theo lớp.
  - *Tab 2: Danh mục Lớp học & Niên khóa*: Quản lý mã lớp, tên lớp, khóa học sinh hoạt.
- **📚 Học phần & Niên khóa** *(Hệ thống 2 Tab)*:
  - *Tab 1: Danh mục Học phần / Môn học*: Quản lý môn học, số tín chỉ (dùng để tính GPA có trọng số).
  - *Tab 2: Danh mục Học kỳ & Niên khóa*: Quản lý học kỳ, năm học, ngày bắt đầu và kết thúc kỳ học.

### 3. Phân hệ KHẢO THÍ & ĐIỂM SỐ
- **📝 Sổ điểm học phần**: Vào điểm học phần chi tiết (Điểm chuyên cần 10%, Điểm giữa kỳ 30%, Điểm thi cuối kỳ 60%); Điểm tổng kết do SQL Server tự tính toán chính xác và tự động.
- **📥 Nhập điểm từ File (CSV / Excel)**:
  - *Tab 1: Nhập điểm thi từ File*: Hỗ trợ đọc trực tiếp file `.csv` và `.xlsx` (ClosedXML), kiểm tra dung lượng, làm sạch dữ liệu, phát hiện lỗi trước khi lưu vào CSDL.
  - *Tab 2: Nhật ký & Lịch sử Import (Audit Log)*: Truy vết 100 lần import gần nhất (thời gian, tên file, số dòng hợp lệ, số dòng lỗi, trạng thái).

### 4. Phân hệ HỌC VỤ & BÁO CÁO
- **📈 Tra cứu GPA & Học lực**: Tính toán điểm trung bình có trọng số số tín chỉ (GPA); Phân loại học lực 4 mức (Giỏi, Khá, Trung bình, Yếu); Bộ lọc linh hoạt theo Lớp và Học kỳ; Biểu đồ thanh trực quan hóa tỷ lệ xếp loại.
- **⚠️ Thống kê Đạt/Trượt & Cảnh báo học vụ**: Thống kê tỷ lệ đạt/trượt môn học; Tự động quét và cảnh báo danh sách sinh viên bị thiếu đầu điểm thành phần.

### 5. Phân hệ QUẢN TRỊ HỆ THỐNG
- **⚙️ Quản trị & Phân quyền cán bộ** *(Tab quản trị dành riêng cho Admin)*:
  - Quản lý tài khoản cán bộ đào tạo, phân quyền (Admin / Nhân viên), khóa/mở khóa tài khoản, đặt lại mật khẩu.

---

## 4. Tài khoản Đăng nhập Hệ thống

Hệ thống được thiết lập sẵn 2 tài khoản mẫu phục vụ kiểm thử và chấm bài:

| Tên đăng nhập | Mật khẩu | Vai trò | Quyền hạn |
|:---:|:---:|:---:|:---|
| **`admin`** | `admin123` | **Admin** | Toàn quyền sử dụng tất cả các phân hệ, bao gồm quản trị tài khoản, phân quyền cán bộ, cấu hình hệ thống và Import dữ liệu. |
| **`nhanvien`** | `123456` | **NhanVien** | Sử dụng các nghiệp vụ xem hồ sơ, quản lý môn học, nhập điểm, tra cứu GPA, Import điểm thi (tự động ẩn tab Quản trị tài khoản Admin). |

---

## 5. Thư mục Dữ liệu Mẫu (Demo & Chấm bài)

Trong thư mục `Sample_Data/` của dự án đã chuẩn bị sẵn các file dữ liệu để thực hành và trình diễn:

1. **`Sample_Data/01_Diem_HopLe.csv`**: File CSV hợp lệ chứa điểm học kỳ 1 và 2 của sinh viên mới. Dùng để demo luồng Import thành công 100%.
2. **`Sample_Data/02_Diem_HopLe.xlsx`**: File Excel (.xlsx) chuẩn cấu trúc 6 cột. Dùng để demo tính năng đọc file Excel trực tiếp qua ClosedXML.
3. **`Sample_Data/03_Diem_CoLoi_DemoKiemTra.csv`**: File cố tình thiết kế nhiều loại lỗi (điểm > 10, điểm âm, điểm chứa chữ, mã sinh viên/môn học không tồn tại trong CSDL, bản ghi điểm bị trùng). Dùng để demo khả năng **kiểm tra chất lượng và làm sạch dữ liệu** trước khi ghi vào CSDL.

---

## 6. Hướng dẫn Khởi chạy Dự án

1. **Mở Visual Studio**:
   - Mở file `RESULT_ANALYTICS/RESULT_ANALYTICS.sln`.
   - Nhấn **F5** (hoặc `Ctrl+F5`) để khởi chạy ứng dụng.
2. **Đăng nhập**:
   - Sử dụng tài khoản `admin` / `admin123` hoặc `nhanvien` / `123456`.
3. **Trải nghiệm**:
   - Trải nghiệm chuyển đổi giữa 5 phân hệ chuẩn đại học trên Sidebar.
   - Thao tác các Tab con trên từng màn hình mà không cần cuộn chuột.
   - Nút **ĐĂNG XUẤT** ở góc dưới Sidebar cho phép đổi nhanh giữa tài khoản Admin và Nhân viên để kiểm tra tính năng phân quyền.
