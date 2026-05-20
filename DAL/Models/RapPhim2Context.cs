using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Models;

public partial class RapPhim2Context : DbContext
{
    public RapPhim2Context()
    {
    }

    public RapPhim2Context(DbContextOptions<RapPhim2Context> options)
        : base(options)
    {
    }

    public virtual DbSet<BangLuong> BangLuongs { get; set; }

    public virtual DbSet<BaoCaoDoanhThuNgay> BaoCaoDoanhThuNgays { get; set; }

    public virtual DbSet<CaLam> CaLams { get; set; }

    public virtual DbSet<ChamCong> ChamCongs { get; set; }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietPhieuNhapKho> ChiTietPhieuNhapKhos { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<Ghe> Ghes { get; set; }

    public virtual DbSet<GiuGheTam> GiuGheTams { get; set; }

    public virtual DbSet<HoanVe> HoanVes { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KhenThuong> KhenThuongs { get; set; }

    public virtual DbSet<Kho> Khos { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    public virtual DbSet<KyLuat> KyLuats { get; set; }

    public virtual DbSet<LoaiGhe> LoaiGhes { get; set; }

    public virtual DbSet<LoaiSanPham> LoaiSanPhams { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhieuNhapKho> PhieuNhapKhos { get; set; }

    public virtual DbSet<Phim> Phims { get; set; }

    public virtual DbSet<PhongChieu> PhongChieus { get; set; }

    public virtual DbSet<Rap> Raps { get; set; }

    public virtual DbSet<SanPham> SanPhams { get; set; }

    public virtual DbSet<SuatChieu> SuatChieus { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<TheLoai> TheLoais { get; set; }

    public virtual DbSet<VeBan> VeBans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-8RQB10J4;Initial Catalog=RapPhim2;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BangLuong>(entity =>
        {
            entity.HasKey(e => e.MaLuong).HasName("PK__BangLuon__6609A48D12875A2E");

            entity.ToTable("BangLuong");

            entity.Property(e => e.LuongCoBan).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Phat).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Thuong).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TongLuong).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.BangLuongs)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__BangLuong__MaNha__10566F31");
        });

        modelBuilder.Entity<BaoCaoDoanhThuNgay>(entity =>
        {
            entity.HasKey(e => e.MaBaoCao).HasName("PK__BaoCaoDo__25A9188C052803D3");

            entity.ToTable("BaoCaoDoanhThuNgay");

            entity.HasIndex(e => new { e.MaRap, e.Ngay }, "UQ__BaoCaoDo__0FDDEE05B505A720").IsUnique();

            entity.Property(e => e.DoanhThuDichVu).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DoanhThuVe).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TongDoanhThu).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.MaRapNavigation).WithMany(p => p.BaoCaoDoanhThuNgays)
                .HasForeignKey(d => d.MaRap)
                .HasConstraintName("FK__BaoCaoDoa__MaRap__14270015");
        });

        modelBuilder.Entity<CaLam>(entity =>
        {
            entity.HasKey(e => e.MaCa).HasName("PK__CaLam__27258E7B7D61EFE9");

            entity.ToTable("CaLam");

            entity.Property(e => e.TenCa).HasMaxLength(100);
        });

        modelBuilder.Entity<ChamCong>(entity =>
        {
            entity.HasKey(e => e.MaChamCong).HasName("PK__ChamCong__307331A1718744E5");

            entity.ToTable("ChamCong");

            entity.Property(e => e.GioRa).HasColumnType("datetime");
            entity.Property(e => e.GioVao).HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaCaNavigation).WithMany(p => p.ChamCongs)
                .HasForeignKey(d => d.MaCa)
                .HasConstraintName("FK__ChamCong__MaCa__07C12930");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.ChamCongs)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__ChamCong__MaNhan__06CD04F7");
        });

        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK__ChiTietD__3214CC9F2A867D6B");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.Gia).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__ChiTietDo__MaDon__7E37BEF6");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaSanPham)
                .HasConstraintName("FK__ChiTietDo__MaSan__7F2BE32F");
        });

        modelBuilder.Entity<ChiTietPhieuNhapKho>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK__ChiTietP__3214CC9FD33F1EBC");

            entity.ToTable("ChiTietPhieuNhapKho");

            entity.Property(e => e.DonGia).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.MaPhieuNhapNavigation).WithMany(p => p.ChiTietPhieuNhapKhos)
                .HasForeignKey(d => d.MaPhieuNhap)
                .HasConstraintName("FK__ChiTietPh__MaPhi__6477ECF3");

            entity.HasOne(d => d.MaSanPhamNavigation).WithMany(p => p.ChiTietPhieuNhapKhos)
                .HasForeignKey(d => d.MaSanPham)
                .HasConstraintName("FK__ChiTietPh__MaSan__656C112C");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PK__DonHang__129584AD748F4006");

            entity.ToTable("DonHang");

            entity.Property(e => e.MaHoaDon).HasMaxLength(30);
            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TienGiam).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TongThanhToan).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TongTienDichVu).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TongTienVe).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaKhachHang)
                .HasConstraintName("FK__DonHang__MaKhach__6EF57B66");

            entity.HasOne(d => d.MaKhuyenMaiNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaKhuyenMai)
                .HasConstraintName("FK__DonHang__MaKhuye__6FE99F9F");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__DonHang__MaNhanV__6E01572D");
        });

        modelBuilder.Entity<Ghe>(entity =>
        {
            entity.HasKey(e => e.MaGhe).HasName("PK__Ghe__3CD3C67B2DB6A4FD");

            entity.ToTable("Ghe");

            entity.HasIndex(e => new { e.MaPhong, e.HangGhe, e.SoGhe }, "UQ_Ghe").IsUnique();

            entity.Property(e => e.HangGhe)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();

            entity.HasOne(d => d.MaLoaiGheNavigation).WithMany(p => p.Ghes)
                .HasForeignKey(d => d.MaLoaiGhe)
                .HasConstraintName("FK__Ghe__MaLoaiGhe__46E78A0C");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.Ghes)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK__Ghe__MaPhong__47DBAE45");
        });

        modelBuilder.Entity<GiuGheTam>(entity =>
        {
            entity.HasKey(e => e.MaGiuGhe);

            entity.ToTable("GiuGheTam");

            entity.HasIndex(e => new { e.MaSuatChieu, e.MaGhe }, "UQ_GiuGheTam").IsUnique();

            entity.Property(e => e.HetHanLuc).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianGiu)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("DangGiu");

            entity.HasOne(d => d.MaDatVeNavigation).WithMany(p => p.GiuGheTams)
                .HasForeignKey(d => d.MaDatVe)
                .HasConstraintName("FK_GiuGheTam_VeBan");

            entity.HasOne(d => d.MaGheNavigation).WithMany(p => p.GiuGheTams)
                .HasForeignKey(d => d.MaGhe)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GiuGheTam_Ghe");

            entity.HasOne(d => d.MaSuatChieuNavigation).WithMany(p => p.GiuGheTams)
                .HasForeignKey(d => d.MaSuatChieu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GiuGheTam_SuatChieu");
        });

        modelBuilder.Entity<HoanVe>(entity =>
        {
            entity.HasKey(e => e.MaHoanVe).HasName("PK__HoanVe__848559C9ABD811FC");

            entity.ToTable("HoanVe");

            entity.Property(e => e.LyDo).HasMaxLength(255);
            entity.Property(e => e.NgayHoan)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoTienHoan).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaDatVeNavigation).WithMany(p => p.HoanVes)
                .HasForeignKey(d => d.MaDatVe)
                .HasConstraintName("FK__HoanVe__MaDatVe__7A672E12");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.HoanVes)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__HoanVe__MaNhanVi__7B5B524B");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__KhachHan__88D2F0E5D945B4CC");

            entity.ToTable("KhachHang");

            entity.Property(e => e.HoTen).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
        });

        modelBuilder.Entity<KhenThuong>(entity =>
        {
            entity.HasKey(e => e.MaKhenThuong).HasName("PK__KhenThuo__50C85FF7E9F1E2D2");

            entity.ToTable("KhenThuong");

            entity.Property(e => e.LyDo).HasMaxLength(255);
            entity.Property(e => e.SoTienThuong).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.KhenThuongs)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__KhenThuon__MaNha__0A9D95DB");
        });

        modelBuilder.Entity<Kho>(entity =>
        {
            entity.HasKey(e => e.MaKho).HasName("PK__Kho__3BDA9350B3A3B7D8");

            entity.ToTable("Kho");

            entity.HasIndex(e => e.MaSanPham, "UQ__Kho__FAC7442C3CD60793").IsUnique();

            entity.HasOne(d => d.MaSanPhamNavigation).WithOne(p => p.Kho)
                .HasForeignKey<Kho>(d => d.MaSanPham)
                .HasConstraintName("FK__Kho__MaSanPham__5DCAEF64");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKhuyenMai).HasName("PK__KhuyenMa__6F56B3BDE11ADCD0");

            entity.ToTable("KhuyenMai");

            entity.Property(e => e.DonToiThieu).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.GiaTriGiam).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.LoaiGiam).HasMaxLength(50);
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.NgayBatDau).HasColumnType("datetime");
            entity.Property(e => e.NgayKetThuc).HasColumnType("datetime");
            entity.Property(e => e.TenKhuyenMai).HasMaxLength(255);
            entity.Property(e => e.TrangThai).HasMaxLength(50);
        });

        modelBuilder.Entity<KyLuat>(entity =>
        {
            entity.HasKey(e => e.MaKyLuat).HasName("PK__KyLuat__A675BE7CE8B0319D");

            entity.ToTable("KyLuat");

            entity.Property(e => e.LyDo).HasMaxLength(255);
            entity.Property(e => e.SoTienPhat).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.KyLuats)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__KyLuat__MaNhanVi__0D7A0286");
        });

        modelBuilder.Entity<LoaiGhe>(entity =>
        {
            entity.HasKey(e => e.MaLoaiGhe).HasName("PK__LoaiGhe__965BB4C114666064");

            entity.ToTable("LoaiGhe");

            entity.Property(e => e.HeSoGia).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.TenLoai).HasMaxLength(50);
        });

        modelBuilder.Entity<LoaiSanPham>(entity =>
        {
            entity.HasKey(e => e.MaLoaiSp).HasName("PK__LoaiSanP__1224CA7CE7D338FE");

            entity.ToTable("LoaiSanPham");

            entity.Property(e => e.MaLoaiSp).HasColumnName("MaLoaiSP");
            entity.Property(e => e.TenLoai).HasMaxLength(50);
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__NhanVien__77B2CA471A2B7425");

            entity.ToTable("NhanVien");

            entity.HasIndex(e => e.MaTaiKhoan, "UQ__NhanVien__AD7C6528314336E5").IsUnique();

            entity.Property(e => e.HoTen).HasMaxLength(255);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);

            entity.HasOne(d => d.MaRapNavigation).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.MaRap)
                .HasConstraintName("FK__NhanVien__MaRap__534D60F1");

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithOne(p => p.NhanVien)
                .HasForeignKey<NhanVien>(d => d.MaTaiKhoan)
                .HasConstraintName("FK__NhanVien__MaTaiK__5441852A");
        });

        modelBuilder.Entity<PhieuNhapKho>(entity =>
        {
            entity.HasKey(e => e.MaPhieuNhap).HasName("PK__PhieuNha__1470EF3B3F66080B");

            entity.ToTable("PhieuNhapKho");

            entity.Property(e => e.NgayNhap)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.PhieuNhapKhos)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__PhieuNhap__MaNha__619B8048");
        });

        modelBuilder.Entity<Phim>(entity =>
        {
            entity.HasKey(e => e.MaPhim).HasName("PK__Phim__4AC03DE3B652A2D0");

            entity.ToTable("Phim");

            entity.Property(e => e.AnhBia).HasMaxLength(500);
            entity.Property(e => e.QuocGia).HasMaxLength(100);
            entity.Property(e => e.TieuDe).HasMaxLength(255);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasMany(d => d.MaTheLoais).WithMany(p => p.MaPhims)
                .UsingEntity<Dictionary<string, object>>(
                    "PhimTheLoai",
                    r => r.HasOne<TheLoai>().WithMany()
                        .HasForeignKey("MaTheLoai")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Phim_TheL__MaThe__3C69FB99"),
                    l => l.HasOne<Phim>().WithMany()
                        .HasForeignKey("MaPhim")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__Phim_TheL__MaPhi__3B75D760"),
                    j =>
                    {
                        j.HasKey("MaPhim", "MaTheLoai").HasName("PK__Phim_The__F7B3C2D7DA62B92C");
                        j.ToTable("Phim_TheLoai");
                    });
        });

        modelBuilder.Entity<PhongChieu>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__PhongChi__20BD5E5B02CD67C3");

            entity.ToTable("PhongChieu");

            entity.Property(e => e.TenPhong).HasMaxLength(100);

            entity.HasOne(d => d.MaRapNavigation).WithMany(p => p.PhongChieus)
                .HasForeignKey(d => d.MaRap)
                .HasConstraintName("FK__PhongChie__MaRap__412EB0B6");
        });

        modelBuilder.Entity<Rap>(entity =>
        {
            entity.HasKey(e => e.MaRap).HasName("PK__Rap__3961207F97DA2D81");

            entity.ToTable("Rap");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.TenRap).HasMaxLength(255);
            entity.Property(e => e.ThanhPho).HasMaxLength(100);
        });

        modelBuilder.Entity<SanPham>(entity =>
        {
            entity.HasKey(e => e.MaSanPham).HasName("PK__SanPham__FAC7442D915EB1FB");

            entity.ToTable("SanPham");

            entity.Property(e => e.Gia).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaLoaiSp).HasColumnName("MaLoaiSP");
            entity.Property(e => e.Ten).HasMaxLength(255);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaLoaiSpNavigation).WithMany(p => p.SanPhams)
                .HasForeignKey(d => d.MaLoaiSp)
                .HasConstraintName("FK__SanPham__MaLoaiS__59063A47");
        });

        modelBuilder.Entity<SuatChieu>(entity =>
        {
            entity.HasKey(e => e.MaSuatChieu).HasName("PK__SuatChie__CF5984D20C588FB9");

            entity.ToTable("SuatChieu");

            entity.Property(e => e.GiaVeCoBan)
                .HasDefaultValue(90000m)
                .HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ThoiGianBatDau).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianKetThuc).HasColumnType("datetime");

            entity.HasOne(d => d.MaPhimNavigation).WithMany(p => p.SuatChieus)
                .HasForeignKey(d => d.MaPhim)
                .HasConstraintName("FK__SuatChieu__MaPhi__4AB81AF0");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.SuatChieus)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK__SuatChieu__MaPho__4BAC3F29");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__TaiKhoan__AD7C65292D591016");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.TenDangNhap, "UQ__TaiKhoan__55F68FC0068BE769").IsUnique();

            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.TenDangNhap).HasMaxLength(100);
            entity.Property(e => e.VaiTro)
                .HasMaxLength(50)
                .HasDefaultValue("NhanVien");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaThanhToan).HasName("PK__ThanhToa__D4B258440D8983E7");

            entity.ToTable("ThanhToan");

            entity.Property(e => e.NgayThanhToan).HasColumnType("datetime");
            entity.Property(e => e.PhuongThuc).HasMaxLength(50);
            entity.Property(e => e.SoTien).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__ThanhToan__MaDon__02084FDA");
        });

        modelBuilder.Entity<TheLoai>(entity =>
        {
            entity.HasKey(e => e.MaTheLoai).HasName("PK__TheLoai__D73FF34AEC6FC458");

            entity.ToTable("TheLoai");

            entity.Property(e => e.Ten).HasMaxLength(100);
        });

        modelBuilder.Entity<VeBan>(entity =>
        {
            entity.HasKey(e => e.MaDatVe).HasName("PK__VeBan__6A32C593B9E04C0A");

            entity.ToTable("VeBan");

            entity.HasIndex(e => new { e.MaGhe, e.MaSuatChieu }, "UQ_Ghe_SuatChieu").IsUnique();

            entity.Property(e => e.Gia).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.MaVe).HasMaxLength(30);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.VeBans)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__VeBan__MaDonHang__74AE54BC");

            entity.HasOne(d => d.MaGheNavigation).WithMany(p => p.VeBans)
                .HasForeignKey(d => d.MaGhe)
                .HasConstraintName("FK__VeBan__MaGhe__75A278F5");

            entity.HasOne(d => d.MaSuatChieuNavigation).WithMany(p => p.VeBans)
                .HasForeignKey(d => d.MaSuatChieu)
                .HasConstraintName("FK__VeBan__MaSuatChi__76969D2E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
