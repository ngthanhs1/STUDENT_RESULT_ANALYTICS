# STUDENT RESULT ANALYTICS (HỆ THỐNG QUẢN LÝ ĐÀO TẠO VÀ PHÂN TÍCH KẾT QUẢ HỌC TẬP ĐẠI HỌC)

> **Dự án Bài tập lớn học phần:** Công nghệ .NET  
> **Đơn vị đào tạo:** Khoa Công nghệ thông tin - Trường Đại học Phenikaa  
> **Nền tảng phát triển:** C# .NET Framework 4.7.2 (WPF / XAML)  
> **Cơ sở dữ liệu:** Microsoft SQL Server (Somee Cloud Server & Localhost)  
> **Kiến trúc phần mềm:** Model - View - ViewModel (MVVM) + Repository Pattern  

---

## 1. Giới thiệu tổng quan
**Student Result Analytics** là phần mềm quản lý học vụ và phân tích điểm thi cấp trường đại học được phát triển trên nền tảng **C# WPF**. Hệ thống giải quyết trọn vẹn bài toán quản lý hồ sơ sinh viên, lớp sinh hoạt theo từng **Khoa / Viện**, sổ điểm học phần điện tử, nạp dữ liệu hàng loạt từ file Excel/CSV, tự động quy đổi thang điểm tín chỉ chuẩn MOET ($A^+ = 4.0, A = 3.7$) và tính điểm trung bình học kỳ (GPA) gia quyền theo số tín chỉ môn học.

Hệ thống được thiết kế theo phong cách giao diện hiện đại với thanh điều hướng (Sidebar) màu xanh hải quân trang nhã (`#1B426E`), biểu tượng thương hiệu chính thức, bộ lọc học kỳ toàn hệ thống, hỗ trợ đăng nhập an toàn với nút bật/tắt hiển thị mật khẩu thông minh.

---

## 2. Các tính năng và phân hệ nghiệp vụ nổi bật

### 2.1. Xác thực & Phân quyền bảo mật (Authentication & RBAC)
- Màn hình đăng nhập thẩm mỹ cao với ảnh nền sinh viên tốt nghiệp có chiều sâu.
- **Nút icon con mắt (👁 / 🙈)** cho phép bật/tắt hiển thị mật khẩu hai chiều tức thì, hỗ trợ gõ phím Enter nhanh chóng.
- Phân quyền chặt chẽ giữa 2 nhóm người dùng:
  - **Quản trị viên (`Admin`):** Toàn quyền quản trị tài khoản, đặt lại mật khẩu, khóa/mở người dùng và giám sát hệ thống.
  - **Cán bộ Đào tạo & Khảo thí (`NhanVien`):** Thực hiện các nghiệp vụ quản lý sinh viên, học phần, sổ điểm, import và tra cứu thống kê. Phân hệ Quản trị tài khoản được tự động ẩn để đảm bảo an toàn.

### 2.2. Bàn làm việc tổng quan (Executive Academic Dashboard)
- 6 Thẻ KPI đào tạo toàn trường: Tổng sinh viên, Lớp sinh hoạt, Học phần đào tạo, Điểm TB toàn khóa, Tỷ lệ đạt chung, Số sinh viên cần bổ sung điểm.
- Biểu đồ tiến độ phân bố học lực toàn khóa (Xuất sắc, Giỏi, Khá, Trung bình, Yếu/Kém, Chưa đủ điểm).
- Bảng thống kê kết quả Đạt / Trượt theo từng môn học và Danh sách cảnh báo học vụ thời gian thực.
- **Bộ lọc học kỳ toàn hệ thống (`📅 Kỳ xét:` ComboBox):** Cho phép xem dữ liệu toàn khóa hoặc lọc tức thì theo từng học kỳ cụ thể.

### 2.3. Quản lý Hồ sơ Sinh viên, Lớp học & Khoa
- **Quản lý danh mục Lớp học:** Quản lý mã lớp, tên lớp, khóa học (ví dụ K18) và **Khoa / Viện quản lý** (Khoa CNTT, Khoa KHMT, Khoa Điện - Điện tử, Khoa Kinh tế & Kinh doanh...).
- **Quản lý Hồ sơ Sinh viên:**
  - **Cơ chế tự động thông minh:** Khi nhập Mã sinh viên (ví dụ `23010191`), ô Email liên hệ tự động sinh thành `23010191@st.phenikaa-uni.edu.vn`.
  - Kiểm tra ràng buộc hợp lệ: Tiền tố email bắt buộc phải khớp chính xác với Mã SV.
  - Dropdown chọn lớp sinh hoạt hiển thị trực quan: `Mã lớp - Tên lớp (Khoa quản lý)`.
  - Bảng danh sách sinh viên hiển thị đầy đủ thông tin kèm cột **Khoa / Viện quản lý**.

### 2.4. Quản lý Học phần & Niên khóa
- Quản lý danh mục môn học đào tạo, định nghĩa số tín chỉ từ 1 đến 10 (dùng cho giải thuật gia quyền tín chỉ khi tính GPA).
- Quản lý niên khóa và khung thời gian các học kỳ (Học kỳ 1, Học kỳ 2 qua các năm học 2024-2025, 2025-2026).

### 2.5. Sổ điểm học phần điện tử (Gradebook)
- Chọn học kỳ và môn học để mở sổ điểm chi tiết của từng lớp.
- Nhập điểm thành phần: Điểm chuyên cần (10%), Điểm giữa kỳ (30%), Điểm cuối kỳ (60%).
- Tự động tính toán điểm tổng kết thang 10:
  $$\text{Điểm TK} = \text{Chuyên cần} \times 0.1 + \text{Giữa kỳ} \times 0.3 + \text{Cuối kỳ} \times 0.6$$
- Tự động quy đổi sang thang điểm chữ và thang điểm 4.0 chuẩn tín chỉ:
  - $9.0 - 10.0 \rightarrow A^+ (4.0)$
  - $8.5 - 8.9 \rightarrow A (3.7)$
  - $8.0 - 8.4 \rightarrow B^+ (3.5)$
  - $7.0 - 7.9 \rightarrow B (3.0)$
  - $6.5 - 6.9 \rightarrow C^+ (2.5)$
  - $5.5 - 6.4 \rightarrow C (2.0)$
  - $5.0 - 5.4 \rightarrow D^+ (1.5)$
  - $4.0 - 4.9 \rightarrow D (1.0)$
  - Dưới $4.0 \rightarrow F (0.0)$ (Học lại)

### 2.6. Nhập điểm thi hàng loạt từ File Excel/CSV (Bulk Import Engine)
- Hỗ trợ chọn tệp Microsoft Excel (`.xlsx`) hoặc tệp văn bản định dạng `.csv`.
- Sử dụng thư viện EPPlus đọc dữ liệu ngầm siêu tốc, nạp 1.000 dòng điểm trong dưới 3 giây.
- Cơ chế xác thực chặt chẽ: Kiểm tra sinh viên có tồn tại, môn học và học kỳ có hợp lệ, điểm số từ 0.0 đến 10.0.
- 4 Thẻ KPI kết quả: Tổng dòng, Thành công, Dòng lỗi, Trạng thái.
- Tự động lưu nhật ký lịch sử nạp vào bảng `dbo.LichSuImport`.

### 2.7. Tra cứu GPA & Phân loại học lực
- Tra cứu bảng điểm tổng hợp của sinh viên theo từng học kỳ hoặc toàn khóa học.
- Tính toán điểm GPA có trọng số tín chỉ chuẩn xác:
  $$\text{GPA} = \frac{\sum_{i=1}^{n} (\text{Điểm môn } i \times \text{Số tín chỉ môn } i)}{\sum_{i=1}^{n} \text{Số tín chỉ môn } i}$$
- Phân loại học lực: Xuất sắc ($\ge 3.60$), Giỏi ($3.20 - 3.59$), Khá ($2.50 - 3.19$), Trung bình ($2.00 - 2.49$), Yếu ($1.00 - 1.99$), Kém ($< 1.00$).
- Xuất bảng điểm tổng hợp ra file Excel.

### 2.8. Thống kê Đạt/Trượt & Cảnh báo học vụ
- Thống kê số lượng và tỷ lệ % sinh viên đạt/trượt theo từng môn học.
- Tự động phát hiện và trích xuất danh sách sinh viên bị cảnh báo học vụ (GPA học kỳ $< 2.0$ hoặc nợ môn F) để cố vấn học tập kịp thời hỗ trợ.

---

## 3. Kiến trúc hệ thống & Công nghệ sử dụng

```
+-------------------------------------------------------------+
|                      PRESENTATION LAYER                     |
|            WPF / XAML Views (MainWindow, Dashboard, etc.)   |
+-------------------------------------------------------------+
                              ▲
                              │ Data Binding & RelayCommands
                              ▼
+-------------------------------------------------------------+
|                      VIEWMODEL LAYER                        |
|        Business Logic, State Management, INotifyPropertyChanged|
+-------------------------------------------------------------+
                              ▲
                              │ Service Calls & DTOs
                              ▼
+-------------------------------------------------------------+
|                    DATA ACCESS LAYER (DAL)                  |
|    Repository Pattern (SinhVien, KetQua, Lop, MonHoc, etc.) |
|    ADO.NET (SqlConnection, SqlCommand with Parameters)      |
+-------------------------------------------------------------+
                              ▲
                              │ SQL Queries & Stored Views
                              ▼
+-------------------------------------------------------------+
|                      DATABASE LAYER                         |
|     Microsoft SQL Server (Somee Cloud / Localhost)          |
+-------------------------------------------------------------+
```

---

## 4. Cấu trúc thư mục dự án

```
STUDENT_RESULT_ANALYTICS/
│
├── Database_Azure_Setup.sql        # Script tạo CSDL, bảng, views và nạp dữ liệu mẫu
├── BAO_CAO_BAI_TAP_LON_...docx     # Báo cáo BTL định dạng Word chuẩn hướng dẫn
├── BAO_CAO_BAI_TAP_LON_...md       # Báo cáo BTL định dạng Markdown
├── SLIDE_THUYET_TRINH_...pptx      # Slide thuyết trình bảo vệ đồ án
├── README.md                       # Hướng dẫn và tài liệu dự án
│
├── Sample_Data/                    # Thư mục chứa các file Excel / CSV mẫu
│   ├── 04_BangDiem_LopCNTT02_HK1.xlsx
│   ├── 05_BangDiem_LopCNTT01_HK2.csv
│   ├── 06_BangDiem_MonHoc_TriTueNhanTao_MH007.xlsx
│   ├── 07_BangDiem_KiemThu_CanhBaoHocVu.xlsx
│   └── 08_BangDiem_TongHop_NienKhoa2024_2025.csv
│
└── RESULT_ANALYTICS/
    ├── RESULT_ANALYTICS.sln        # Visual Studio Solution File
    └── RESULT_ANALYTICS/
        ├── App.config              # Cấu hình chuỗi kết nối CSDL SQL Server
        ├── App.xaml / App.xaml.cs
        ├── MainWindow.xaml / .cs   # Giao diện chính và Sidebar điều hướng
        │
        ├── Assets/                 # Tài nguyên hình ảnh, logo, icon
        │   ├── logo.png            # Logo chính thức của dự án
        │   ├── bg_login.jpg        # Ảnh nền màn hình đăng nhập
        │   └── paper_plane.png     # Biểu tượng máy bay giấy
        │
        ├── DataAccess/             # Tầng truy xuất CSDL (Repository)
        │   ├── DatabaseHelper.cs   # Quản lý kết nối SqlConnection
        │   ├── SinhVienRepository.cs
        │   ├── LopRepository.cs
        │   ├── MonHocRepository.cs
        │   ├── HocKyRepository.cs
        │   ├── KetQuaRepository.cs
        │   ├── TaiKhoanRepository.cs
        │   └── DashboardRepository.cs
        │
        ├── Helpers/                # Các lớp tiện ích
        │   ├── GpaHelper.cs        # Quy đổi thang điểm A+ (4.0), A (3.7)...
        │   ├── PhienDangNhap.cs    # Quản lý phiên làm việc & phân quyền
        │   └── RelayCommand.cs     # ICommand cho MVVM
        │
        ├── Models/                 # Tầng thực thể dữ liệu
        │   ├── SinhVien.cs
        │   ├── Lop.cs
        │   ├── MonHoc.cs
        │   ├── HocKy.cs
        │   ├── KetQua.cs
        │   └── TaiKhoan.cs
        │
        ├── ViewModels/             # Tầng xử lý logic điều khiển (MVVM)
        │   ├── MainViewModel.cs
        │   ├── LoginViewModel.cs
        │   ├── DashboardViewModel.cs
        │   ├── QuanLySinhVienViewModel.cs
        │   ├── QuanLyDiemViewModel.cs
        │   ├── HeThongImportViewModel.cs
        │   ├── PhanTichViewModel.cs
        │   ├── ThongKeCanhBaoViewModel.cs
        │   └── QuanLyTaiKhoanViewModel.cs
        │
        └── Views/                  # Tầng giao diện người dùng (UserControl / Window)
            ├── LoginWindow.xaml / .cs
            ├── DashboardView.xaml / .cs
            ├── QuanLySinhVienView.xaml / .cs
            ├── QuanLyMonHocHocKyView.xaml / .cs
            ├── QuanLyDiemView.xaml / .cs
            ├── HeThongImportView.xaml / .cs
            ├── PhanTichView.xaml / .cs
            ├── ThongKeCanhBaoView.xaml / .cs
            └── QuanLyTaiKhoanView.xaml / .cs
```

---

## 5. Hướng dẫn cài đặt và khởi chạy

### 5.1. Yêu cầu môi trường
- Hệ điều hành: Windows 10 hoặc Windows 11 (64-bit).
- Nền tảng: .NET Framework 4.7.2 trở lên.
- Công cụ phát triển: Microsoft Visual Studio 2022 (khuyến nghị) hoặc MSBuild 17.0+.

### 5.2. Cấu hình Cơ sở dữ liệu
Chuỗi kết nối cơ sở dữ liệu được cấu hình trong tệp [`App.config`](file:///d:/Code_Hoc/STUDENT_RESULT_ANALYTICS/RESULT_ANALYTICS/RESULT_ANALYTICS/App.config).

1. **Sử dụng Cloud SQL Server (Somee) - Cấu hình mặc định sẵn có:**
   ```xml
   <connectionStrings>
       <add name="StudentDB"
            connectionString="Data Source=StudentResultAnalyticsDB.mssql.somee.com;Initial Catalog=StudentResultAnalyticsDB;User ID=tthanhng098_SQLLogin_1;Password=3n36kbatyu;Encrypt=True;TrustServerCertificate=True;"
            providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

2. **Sử dụng Localhost SQL Server (Nếu chạy offline nội bộ):**
   - Chạy toàn bộ script [`Database_Azure_Setup.sql`](file:///d:/Code_Hoc/STUDENT_RESULT_ANALYTICS/Database_Azure_Setup.sql) trên SQL Server Management Studio (SSMS).
   - Đổi chuỗi kết nối trong `App.config` thành:
     ```xml
     connectionString="Data Source=.;Initial Catalog=StudentResultAnalyticsDB;Integrated Security=True;Encrypt=False;"
     ```

### 5.3. Biên dịch và Khởi chạy ứng dụng
- **Cách 1: Khởi chạy bằng Visual Studio 2022**
  1. Mở tệp `RESULT_ANALYTICS\RESULT_ANALYTICS.sln`.
  2. Chọn chế độ cấu hình `Debug` hoặc `Release` (nền tảng `Any CPU` hoặc `x64`).
  3. Nhấn phím `F5` hoặc bấm nút `Start` để biên dịch và khởi chạy.

- **Cách 2: Biên dịch qua dòng lệnh MSBuild**
  ```powershell
  & "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "d:\Code_Hoc\STUDENT_RESULT_ANALYTICS\RESULT_ANALYTICS\RESULT_ANALYTICS.sln" /t:Rebuild /p:Configuration=Debug
  ```
  File thực thi sinh ra tại:  
  `RESULT_ANALYTICS\RESULT_ANALYTICS\bin\Debug\RESULT_ANALYTICS.exe`

---

## 6. Tài khoản đăng nhập mặc định

| Tên đăng nhập | Mật khẩu | Họ và tên | Vai trò (Role) | Quyền hạn truy cập |
|:---:|:---:|:---|:---:|:---|
| **`admin`** | **`admin123`** | Quản trị viên hệ thống | **Admin** | **Toàn quyền:** Bao gồm cả Quản trị tài khoản, Phân quyền & Quản lý đào tạo |
| **`nhanvien`** | **`nv123`** | Cán bộ Đào tạo & Khảo thí | **NhanVien** | Thực hiện quản lý sinh viên, học phần, sổ điểm, import file và tra cứu GPA (Ẩn menu Quản trị) |

---

## 7. Hướng dẫn nạp bảng điểm mẫu từ thư mục `Sample_Data/`
Dự án đi kèm bộ file dữ liệu mẫu chuẩn cấu trúc tại thư mục [`Sample_Data/`](file:///d:/Code_Hoc/STUDENT_RESULT_ANALYTICS/Sample_Data):
1. Đăng nhập vào phần mềm bằng tài khoản `admin` hoặc `nhanvien`.
2. Trên thanh Sidebar, chọn mục **"Nhập điểm từ File (Excel/CSV)"**.
3. Bấm **"CHỌN FILE..."** và duyệt tới thư mục `Sample_Data/`, ví dụ chọn file:
   - `04_BangDiem_LopCNTT02_HK1.xlsx` (Bảng điểm lớp CNTT 02)
   - `05_BangDiem_LopCNTT01_HK2.csv` (Bảng điểm lớp CNTT 01 tệp CSV)
   - `07_BangDiem_KiemThu_CanhBaoHocVu.xlsx` (Bảng điểm kiểm thử cảnh báo học vụ)
4. Bấm **"THỰC HIỆN IMPORT"**.
5. Quan sát 4 thẻ KPI cập nhật số dòng thành công và kiểm tra nhật ký trong bảng **Lịch sử các lần Import**.
6. Chuyển sang mục **"Bàn làm việc (Dashboard)"** hoặc **"Tra cứu GPA & Học lực"** để xem số liệu phân tích cập nhật tức thì.

---

## 8. Giấy phép & Bản quyền
Dự án được thực hiện phục vụ mục đích học tập và nghiên cứu trong học phần Công nghệ .NET tại Trường Đại học Phenikaa. Bản quyền mã nguồn thuộc về nhóm sinh viên thực hiện. Mọi hành vi sao chép vui lòng trích dẫn nguồn đầy đủ.
