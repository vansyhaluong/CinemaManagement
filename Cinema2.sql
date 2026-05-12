CREATE DATABASE RapPhim2
GO 
USE RapPhim2
GO

------ Bảng Phim
CREATE TABLE Phim (
    MaPhim INT IDENTITY PRIMARY KEY,
    TieuDe NVARCHAR(255),
    MoTa NVARCHAR(MAX),
    ThoiLuong INT,
    NgayKhoiChieu DATE,
QuocGia NVARCHAR(100),
    AnhBia NVARCHAR(500),
    TrangThai NVARCHAR(50)
);

go
---------- Bảng Thể Loại
CREATE TABLE TheLoai (
    MaTheLoai INT IDENTITY PRIMARY KEY,
    Ten NVARCHAR(100)
);

go

----------- Bảng Phim_TheLoai
CREATE TABLE Phim_TheLoai (
    MaPhim INT,
    MaTheLoai INT,
    PRIMARY KEY (MaPhim, MaTheLoai),
    FOREIGN KEY (MaPhim) REFERENCES Phim(MaPhim),
    FOREIGN KEY (MaTheLoai) REFERENCES TheLoai(MaTheLoai)
);

go

--------- Bảng Rạp
CREATE TABLE Rap (
    MaRap INT IDENTITY PRIMARY KEY,
    TenRap NVARCHAR(255),
    DiaChi NVARCHAR(255),
    ThanhPho NVARCHAR(100)
);
go

-------- Bảng PhongChieu
CREATE TABLE PhongChieu (
    MaPhong INT IDENTITY PRIMARY KEY,
    MaRap INT,
    TenPhong NVARCHAR(100),
    FOREIGN KEY (MaRap) REFERENCES Rap(MaRap)
);
go
CREATE TABLE LoaiGhe (
    MaLoaiGhe INT IDENTITY PRIMARY KEY,
    TenLoai NVARCHAR(50),
    
    HeSoGia DECIMAL(5,2)
);
GO
------ Bảng Ghe
CREATE TABLE Ghe (
    MaGhe INT IDENTITY PRIMARY KEY,
    MaPhong INT,
    HangGhe CHAR(1),
    SoGhe INT,
    MaLoaiGhe INT,
 FOREIGN KEY (MaLoaiGhe) REFERENCES LoaiGhe(MaLoaiGhe),
    FOREIGN KEY (MaPhong) REFERENCES PhongChieu(MaPhong),
    CONSTRAINT UQ_Ghe UNIQUE (MaPhong, HangGhe, SoGhe)
);
go
alter table Ghe
add TrangThai nvarchar(100) default N'Trống'
go
-------- Bảng SuatChieu
CREATE TABLE SuatChieu (
    MaSuatChieu INT IDENTITY PRIMARY KEY,
    MaPhim INT,
    MaPhong INT,
    ThoiGianBatDau DATETIME,
    ThoiGianKetThuc DATETIME,
  
    FOREIGN KEY (MaPhim) REFERENCES Phim(MaPhim),
    FOREIGN KEY (MaPhong) REFERENCES PhongChieu(MaPhong)
);

------- Bảng TaiKhoan
GO
CREATE TABLE TaiKhoan (
    MaTaiKhoan INT IDENTITY PRIMARY KEY,
    TenDangNhap NVARCHAR(100) UNIQUE,
    MatKhau NVARCHAR(255),
    VaiTro NVARCHAR(50) DEFAULT 'NhanVien'

);
GO

CREATE TABLE NhanVien (
    MaNhanVien INT IDENTITY PRIMARY KEY,
    MaTaiKhoan INT UNIQUE,
    HoTen NVARCHAR(255),
    SoDienThoai NVARCHAR(20),
   MaRap INT,
    FOREIGN KEY (MaRap) REFERENCES Rap(MaRap),
    FOREIGN KEY (MaTaiKhoan) REFERENCES TaiKhoan(MaTaiKhoan)
);

go
CREATE TABLE LoaiSanPham (
    MaLoaiSP INT IDENTITY PRIMARY KEY,
    TenLoai NVARCHAR(50) -- DoAn, Nuoc, Combo
);

go
CREATE TABLE SanPham (
    MaSanPham INT IDENTITY PRIMARY KEY,
    Ten NVARCHAR(255),
    Gia DECIMAL(10,2),
    TrangThai NVARCHAR(50),
    MaLoaiSP INT,
    FOREIGN KEY (MaLoaiSP) REFERENCES LoaiSanPham(MaLoaiSP)
);

go
CREATE TABLE Kho (
    MaKho INT IDENTITY PRIMARY KEY,
    MaSanPham INT UNIQUE,
    SoLuongTon INT CHECK (SoLuongTon >=0),

    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);
go
CREATE TABLE PhieuNhapKho (
    MaPhieuNhap INT IDENTITY PRIMARY KEY,
    NgayNhap DATETIME DEFAULT GETDATE(),
    MaNhanVien INT,

    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien)
);

go
CREATE TABLE ChiTietPhieuNhapKho (
    Ma INT IDENTITY PRIMARY KEY,
    MaPhieuNhap INT,
    MaSanPham INT,
    SoLuong INT,
    DonGia DECIMAL(10,2),

    FOREIGN KEY (MaPhieuNhap) REFERENCES PhieuNhapKho(MaPhieuNhap),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);

go

-------- Bảng KhachHang
CREATE TABLE KhachHang (
    MaKhachHang INT IDENTITY PRIMARY KEY,
    HoTen NVARCHAR(255),
    SoDienThoai NVARCHAR(20),
    NgayTao DATETIME DEFAULT GETDATE()
);
GO
CREATE TABLE KhuyenMai (
    MaKhuyenMai INT IDENTITY PRIMARY KEY,
    TenKhuyenMai NVARCHAR(255),
    MoTa NVARCHAR(500),

    LoaiGiam NVARCHAR(50),      -- PhanTram / TienMat
    GiaTriGiam DECIMAL(10,2),  -- 20 (%) hoặc 50000

    DonToiThieu DECIMAL(10,2), -- đơn tối thiểu để áp dụng
    NgayBatDau DATETIME,
    NgayKetThuc DATETIME,
    SoLuong INT,               -- số lượt dùng
    TrangThai NVARCHAR(50)     -- HoatDong / HetHan / TamDung
);
go
CREATE TABLE DonHang (
    MaDonHang INT IDENTITY PRIMARY KEY,
    MaKhachHang INT,
    NgayDat DATETIME DEFAULT GETDATE(),

    TrangThai NVARCHAR(50),
    MaNhanVien INT,
MaKhuyenMai INT,
    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien),
    FOREIGN KEY (MaKhachHang) REFERENCES KhachHang(MaKhachHang),
FOREIGN KEY (MaKhuyenMai) REFERENCES KhuyenMai(MaKhuyenMai)
);

go
CREATE TABLE VeBan (
    MaDatVe INT IDENTITY PRIMARY KEY,
    MaDonHang INT,
    MaSuatChieu INT,
    MaGhe INT,
    Gia DECIMAL(10,2) CHECK (Gia >= 0),
TrangThai NVARCHAR(50),
    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang),
    FOREIGN KEY (MaGhe) REFERENCES Ghe(MaGhe),
    FOREIGN KEY (MaSuatChieu) REFERENCES SuatChieu(MaSuatChieu),
    CONSTRAINT UQ_Ghe_SuatChieu UNIQUE (MaGhe, MaSuatChieu)
);
GO
CREATE TABLE HoanVe (
    MaHoanVe INT IDENTITY PRIMARY KEY,
    MaDatVe INT,
    NgayHoan DATETIME DEFAULT GETDATE(),
    LyDo NVARCHAR(255),
    SoTienHoan DECIMAL(10,2),
    TrangThai NVARCHAR(50), -- DaHoan / TuChoi / ChoDuyet
    MaNhanVien INT,

    FOREIGN KEY (MaDatVe) REFERENCES VeBan(MaDatVe),
    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien)
);
GO
CREATE TABLE ChiTietDonHang (
    Ma INT IDENTITY PRIMARY KEY,
    MaDonHang INT,
    MaSanPham INT,
    SoLuong INT,
    Gia DECIMAL(10,2),

    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang),
    FOREIGN KEY (MaSanPham) REFERENCES SanPham(MaSanPham)
);

go
CREATE TABLE ThanhToan (
    MaThanhToan INT IDENTITY PRIMARY KEY,
    MaDonHang INT,
    PhuongThuc NVARCHAR(50),
    NgayThanhToan DATETIME,
    SoTien DECIMAL(10,2),
    TrangThai NVARCHAR(50),

    FOREIGN KEY (MaDonHang) REFERENCES DonHang(MaDonHang)
);
go
CREATE TABLE CaLam (
    MaCa INT IDENTITY PRIMARY KEY,
    TenCa NVARCHAR(100),      -- Ca sáng, Ca chiều...
    GioBatDau TIME,
    GioKetThuc TIME
);
go
CREATE TABLE ChamCong (
    MaChamCong INT IDENTITY PRIMARY KEY,
    MaNhanVien INT,
    Ngay DATE,
    MaCa INT,

    GioVao DATETIME,
    GioRa DATETIME,

    TrangThai NVARCHAR(50), -- Đi làm / Trễ / Nghỉ / Vắng

    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien),
    FOREIGN KEY (MaCa) REFERENCES CaLam(MaCa)
);
go
CREATE TABLE KhenThuong (
    MaKhenThuong INT IDENTITY PRIMARY KEY,
    MaNhanVien INT,
    Ngay DATE,
    LyDo NVARCHAR(255),
    SoTienThuong DECIMAL(10,2),

    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien)
);
go
CREATE TABLE KyLuat (
    MaKyLuat INT IDENTITY PRIMARY KEY,
    MaNhanVien INT,
    Ngay DATE,
    LyDo NVARCHAR(255),
    SoTienPhat DECIMAL(10,2),

    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien)
);

go
CREATE TABLE BangLuong (
    MaLuong INT IDENTITY PRIMARY KEY,
    MaNhanVien INT,
    Thang INT,
    Nam INT,

    LuongCoBan DECIMAL(10,2),
    Thuong DECIMAL(10,2),
    Phat DECIMAL(10,2),
    TongLuong DECIMAL(10,2),

    FOREIGN KEY (MaNhanVien) REFERENCES NhanVien(MaNhanVien)
);
go
CREATE TABLE BaoCaoDoanhThuNgay (
    MaBaoCao INT IDENTITY PRIMARY KEY,
    MaRap INT,
    Ngay DATE,

    DoanhThuVe DECIMAL(18,2),
    DoanhThuDichVu DECIMAL(18,2),
    TongDoanhThu DECIMAL(18,2),
    SoDonHang INT,
    SoVeBan INT,

    FOREIGN KEY (MaRap) REFERENCES Rap(MaRap),
    UNIQUE(MaRap, Ngay)
);
go
/* ===============================
   1. THỂ LOẠI
   =============================== */
INSERT INTO TheLoai(Ten)
VALUES
(N'Hành động'),
(N'Phiêu lưu'),
(N'Khoa học viễn tưởng'),
(N'Hoạt hình'),
(N'Kinh dị'),
(N'Trinh thám'),
(N'Giả tưởng'),
(N'Hài kịch'),
(N'Chiến tranh'),
(N'Chính kịch'),
(N'Tài liệu'),
(N'Tình cảm');
go
INSERT INTO Phim (TieuDe, MoTa, ThoiLuong, NgayKhoiChieu, QuocGia, AnhBia, TrangThai)
VALUES
(N'Avengers: Endgame', 
 N'Biệt đội Avengers tập hợp để đảo ngược thảm họa do Thanos gây ra.', 
 181, '2019-04-26', N'Mỹ',
 'pack://application:,,,/Images/avenger.jpg', N'Đang chiếu'),

(N'Spider-Man: No Way Home', 
 N'Peter Parker gặp rắc rối khi danh tính bị lộ và đa vũ trụ mở ra.', 
 148, '2021-12-17', N'Mỹ',
 'pack://application:,,,/Images/spiderman.jpg', N'Đang chiếu'),

(N'Frozen 2',
N'Elsa khám phá sức mạnh bí ẩn.',
103,'2019-11-22',N'Mỹ',
'pack://application:,,,/Images/frozen2.jpg',
N'Ngừng chiếu'),

(N'Conan Movie',
N'Thám tử Conan phá án.',
110,'2024-01-10',N'Nhật Bản',
'pack://application:,,,/Images/conan.jpg',
N'Đang chiếu');
go
/* ===============================
   4. RẠP
   =============================== */
INSERT INTO Rap(TenRap, DiaChi, ThanhPho)
VALUES
(N'CGV Vincom Đồng Khởi', N'72 Lê Thánh Tôn, Quận 1', N'Tp. Hồ Chí Minh'),
(N'CGV Giga Mall', N'240-242 Phạm Văn Đồng, Thủ Đức', N'Tp. Hồ Chí Minh'),
(N'CGV Aeon Bình Tân', N'1 Đường 17A, Bình Tân', N'Tp. Hồ Chí Minh'),
(N'CGV Vincom Bà Triệu', N'Tầng 6, VinCom Center Hà Nội, 191 Bà Triệu, Q. Hai Bà Trưng', N'Tp. Hà Nội'),
(N'CGV CT Plaza', N'Tầng 10, CT Plaza, 60A Trường Sơn, P.2, Q. Tân Bình', N'Tp. Hồ Chí Minh'),
(N'CGV Vincom Thủ Đức', N'Tầng 5, TTTM Vincom Thủ Đức, 216 Võ Văn Ngân, P. Bình Thọ, Q. Thủ Đức', N'Tp. Hồ Chí Minh'),
(N'CGV Vincom Gò Vấp', N'Tầng 5 TTTM Vincom Plaza Gò Vấp, 12 Phan Văn Trị, P.7, Q. Gò Vấp', N'Tp. Hồ Chí Minh')
go
/* ===============================
   5. PHÒNG CHIẾU
   =============================== */
INSERT INTO PhongChieu(MaRap, TenPhong)
VALUES
(1,N'Phòng ScreenX'),
(1,N'Phòng IMAX'),
(1,N'Phòng Sweetbox'),
(2,N'Phòng ScreenX'),
(2,N'Phòng IMAX'),
(3,N'Phòng Sweetbox'),
(3,N'Phòng IMAX'),
(4,N'Phòng IMAX'),
(4,N'Phòng 4DX'),
(5,N'Phòng 4DX'),
(5,N'Phòng IMAX'),
(5,N'Phòng ScreenX')

GO
INSERT INTO LoaiGhe(TenLoai, HeSoGia)
VALUES
(N'Thường',1.0),
(N'VIP',1.3),
(N'SweetBox',2.0);
GO
INSERT INTO Ghe(MaPhong, HangGhe, SoGhe, MaLoaiGhe)
VALUES
(1,'A',1,1),
(1,'A',2,1),
(1,'A',3,1),
(1,'B',1,2),
(1,'B',2,2),
(1,'C',1,3),

(2,'A',1,1),
(2,'A',2,1),
(2,'B',1,2),

(3,'A',1,1),
(3,'A',2,1),

(4,'A',1,1),
(4,'A',2,1),
(4,'B',1,2),
(4,'C',1,2),

(5,'A',1,1),
(5,'D',2,1),
(5,'B',1,2),
(5,'C',1,2)
GO
INSERT INTO Ghe(MaPhong, HangGhe, SoGhe, MaLoaiGhe)
VALUES
(1,'A',4,1),
(1,'B',3,1),
(1,'B',4,2),
(2,'A',3,1),
(2,'B',2,2),
(2,'C',1,3),
(2,'C',2,3),
(2,'C',3,3),
(2,'C',4,3),
(3,'A',3,1),
(3,'A',4,1),
(3,'B',1,2),
(3,'B',2,2),
(3,'B',3,2),
(3,'B',4,2),
(3,'C',1,1),
(3,'C',2,1),
(3,'D',1,1),
(3,'D',2,1),
(4,'C',2,3),
(4,'C',3,3),
(4,'D',1,2),
(4,'D',2,2),
(5,'A',2,1),
(5,'A',3,1),
(5,'A',4,1),
(5,'B',2,2),
(5,'B',3,2),
(5,'C',2,2),
(5,'C',3,2),
(5,'D',1,1),
(5,'D',3,1)
go
INSERT INTO SuatChieu(MaPhim, MaPhong, ThoiGianBatDau, ThoiGianKetThuc)
VALUES
(1,1,'2026-05-10 09:00','2026-05-10 12:01'),
(2,2,'2026-05-10 13:00','2026-05-10 15:28'),
(4,3,'2026-05-10 18:00','2026-05-10 19:50'),
(1,4,'2026-05-11 20:00','2026-05-11 23:01'),
(4,5,'2026-05-12 21:00','2026-05-12 23:01'),
(2,6,'2026-05-13 13:00','2026-05-13 16:05'),
(3,7,'2026-05-14 22:00','2026-05-15 01:30'),
(1,8,'2026-05-15 17:00','2026-05-15 20:00')

GO
INSERT INTO TaiKhoan(TenDangNhap, MatKhau, VaiTro)
VALUES
(N'admin','123',N'Admin')
INSERT INTO TaiKhoan(TenDangNhap, MatKhau)
VALUES
(N'nv01','123'),
(N'nv02','1234'),
(N'nv03','12345'),
(N'nv04','123456'),
(N'nv05','1234567'),
(N'nv06','0000'),
(N'nv07','1122'),
(N'nv08','1111')
GO
INSERT INTO NhanVien(MaTaiKhoan, HoTen, SoDienThoai, MaRap)
VALUES
(2,N'Nguyễn Văn Tân','0909000001',1),
(3,N'Trần Thị Phương Dung','0909000002',2),
(4,N'Trần Thị Thu Thảo','0909000003',2),
(5,N'Trần Văn Tiên','0909000004',1),
(6,N'Nguyễn Văn Tèo','0909000005',3),
(7,N'Võ Nguyễn Nhựt Long','0909000006',4),
(8,N'Lê Thiện Nhân','0909000007',5),
(9,N'Lê Đoàn Tiến Đạt','0909000008',5)

go
/* ===============================
   11. LOẠI SẢN PHẨM
   =============================== */
INSERT INTO LoaiSanPham(TenLoai)
VALUES
(N'Đồ ăn'),
(N'Nước')

go
/* ===============================
   12. SẢN PHẨM
   =============================== */
INSERT INTO SanPham(Ten, Gia, TrangThai, MaLoaiSP)
VALUES
(N'Bắp rang bơ',65000,N'Đang bán',1),
(N'Coca Cola',30000,N'Đang bán',2),
(N'Pepsi',30000,N'Đang bán',2),
(N'Bánh trứng',25000,N'Đang bán',1),
(N'Mandu',50000,N'Đang bán',1),
(N'7 Up',20000,N'Đang bán',2),
(N'Sting Dâu',20000,N'Đang bán',2)
go
/* ===============================
   13. KHO
   =============================== */
INSERT INTO Kho(MaSanPham, SoLuongTon)
VALUES
(1,100),
(2,150),
(3,250),
(4,120),
(5,30),
(6,150),
(7,100)

go

/* ===============================
   14. KHÁCH HÀNG
   =============================== */
INSERT INTO KhachHang(HoTen, SoDienThoai)
VALUES
(N'Lê Minh Khang','0911111111'),
(N'Phạm Ngọc Lan','0922222222'),
(N'Hoàng Tuấn','0933333333');
go
/* ===============================
   15. KHUYẾN MÃI
   =============================== */
INSERT INTO KhuyenMai
(TenKhuyenMai, MoTa, LoaiGiam, GiaTriGiam, DonToiThieu,
NgayBatDau, NgayKetThuc, SoLuong, TrangThai)
VALUES
(N'Giảm 20%',N'Áp dụng đơn từ 200k',N'PhanTram',20,200000,
'2026-05-01','2026-05-31',100,N'HoatDong'),

(N'Giảm 50K',N'Áp dụng cuối tuần',N'TienMat',50000,300000,
'2026-05-01','2026-06-30',50,N'HoatDong');
go
/* ===============================
   20. CA LÀM
   =============================== */
INSERT INTO CaLam(TenCa, GioBatDau, GioKetThuc)
VALUES
(N'Ca sáng','08:00','12:00'),
(N'Ca chiều','13:00','17:00'),
(N'Ca tối','18:00','22:00');
go
/* ===============================
   21. CHẤM CÔNG
   =============================== */
INSERT INTO ChamCong(MaNhanVien, Ngay, MaCa, GioVao, GioRa, TrangThai)
VALUES
(1,'2026-05-09',1,'2026-05-09 08:00','2026-05-09 12:00',N'Đi làm'),
(2,'2026-05-09',2,'2026-05-09 13:05','2026-05-09 17:00',N'Trễ');
go

/* ===============================
   22. KHEN THƯỞNG
   =============================== */
INSERT INTO KhenThuong(MaNhanVien, Ngay, LyDo, SoTienThuong)
VALUES
(1,'2026-05-09',N'Nhân viên xuất sắc',500000);

go
/* ===============================
   23. KỶ LUẬT
   =============================== */
INSERT INTO KyLuat(MaNhanVien, Ngay, LyDo, SoTienPhat)
VALUES
(2,'2026-05-09',N'Đi trễ',100000);

go
/* ===============================
   24. BẢNG LƯƠNG
   =============================== */
INSERT INTO BangLuong
(MaNhanVien, Thang, Nam, LuongCoBan, Thuong, Phat, TongLuong)
VALUES
(1,5,2026,7000000,500000,0,7500000),
(2,5,2026,7000000,0,100000,6900000);
go
INSERT INTO Phim_TheLoai (MaPhim, MaTheLoai)
VALUES
-- Avengers: Endgame
(1,1), -- Hành động
(1,2), -- Phiêu lưu
(1,3), -- Khoa học viễn tưởng

-- Spider-Man: No Way Home
(2,1), -- Hành động
(2,2), -- Phiêu lưu
(2,3), -- Khoa học viễn tưởng
(2,8), -- Hài kịch

-- Frozen 2
(3,4), -- Hoạt hình
(3,7), -- Giả tưởng
(3,12), -- Tình cảm

-- Conan Movie
(4,6), -- Trinh thám
(4,1), -- Hành động
(4,10); -- Chính kịch







