using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Models;

public partial class RapPhimContext : DbContext
{
    public RapPhimContext()
    {
    }

    public RapPhimContext(DbContextOptions<RapPhimContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }

    public virtual DbSet<ChiTietPhieuNhapKho> ChiTietPhieuNhapKhos { get; set; }

    public virtual DbSet<DichVu> DichVus { get; set; }

    public virtual DbSet<DonHang> DonHangs { get; set; }

    public virtual DbSet<Ghe> Ghes { get; set; }

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<Kho> Khos { get; set; }

    public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }

    public virtual DbSet<LoaiDichVu> LoaiDichVus { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<PhieuNhapKho> PhieuNhapKhos { get; set; }

    public virtual DbSet<Phim> Phims { get; set; }

    public virtual DbSet<PhongChieu> PhongChieus { get; set; }

    public virtual DbSet<Rap> Raps { get; set; }

    public virtual DbSet<SuatChieu> SuatChieus { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    public virtual DbSet<ThanhToan> ThanhToans { get; set; }

    public virtual DbSet<TheLoai> TheLoais { get; set; }

    public virtual DbSet<VeBan> VeBans { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-8RQB10J4;Database=RapPhim;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChiTietDonHang>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK__ChiTietD__3214CC9FE82F4289");

            entity.ToTable("ChiTietDonHang");

            entity.Property(e => e.Gia).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.MaDichVuNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDichVu)
                .HasConstraintName("FK__ChiTietDo__MaDic__76969D2E");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ChiTietDonHangs)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__ChiTietDo__MaDon__75A278F5");
        });

        modelBuilder.Entity<ChiTietPhieuNhapKho>(entity =>
        {
            entity.HasKey(e => e.Ma).HasName("PK__ChiTietP__3214CC9FB6D3C1AE");

            entity.ToTable("ChiTietPhieuNhapKho");

            entity.Property(e => e.DonGia).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.MaDichVuNavigation).WithMany(p => p.ChiTietPhieuNhapKhos)
                .HasForeignKey(d => d.MaDichVu)
                .HasConstraintName("FK__ChiTietPh__MaDic__619B8048");

            entity.HasOne(d => d.MaPhieuNhapNavigation).WithMany(p => p.ChiTietPhieuNhapKhos)
                .HasForeignKey(d => d.MaPhieuNhap)
                .HasConstraintName("FK__ChiTietPh__MaPhi__60A75C0F");
        });

        modelBuilder.Entity<DichVu>(entity =>
        {
            entity.HasKey(e => e.MaDichVu).HasName("PK__DichVu__C0E6DE8F5E612895");

            entity.ToTable("DichVu");

            entity.Property(e => e.Gia).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Ten).HasMaxLength(255);
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaLoaiNavigation).WithMany(p => p.DichVus)
                .HasForeignKey(d => d.MaLoai)
                .HasConstraintName("FK__DichVu__MaLoai__5629CD9C");
        });

        modelBuilder.Entity<DonHang>(entity =>
        {
            entity.HasKey(e => e.MaDonHang).HasName("PK__DonHang__129584AD249BC567");

            entity.ToTable("DonHang");

            entity.Property(e => e.NgayDat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaKhachHangNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaKhachHang)
                .HasConstraintName("FK__DonHang__MaKhach__6B24EA82");

            entity.HasOne(d => d.MaKhuyenMaiNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaKhuyenMai)
                .HasConstraintName("FK__DonHang__MaKhuye__6C190EBB");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.DonHangs)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__DonHang__MaNhanV__6A30C649");
        });

        modelBuilder.Entity<Ghe>(entity =>
        {
            entity.HasKey(e => e.MaGhe).HasName("PK__Ghe__3CD3C67B05FAE730");

            entity.ToTable("Ghe");

            entity.HasIndex(e => new { e.MaPhong, e.HangGhe, e.SoGhe }, "UQ_Ghe").IsUnique();

            entity.Property(e => e.HangGhe)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.LoaiGhe).HasMaxLength(50);

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.Ghes)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK__Ghe__MaPhong__44FF419A");
        });

        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__HoaDon__835ED13B7DE17703");

            entity.ToTable("HoaDon");

            entity.Property(e => e.MaHoaDon).ValueGeneratedNever();
            entity.Property(e => e.NgayTao).HasColumnType("datetime");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__HoaDon__MaDonHan__7C4F7684");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.MaKhachHang).HasName("PK__KhachHan__88D2F0E52BD6BCE0");

            entity.ToTable("KhachHang");

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.HoTen).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
        });

        modelBuilder.Entity<Kho>(entity =>
        {
            entity.HasKey(e => e.MaKho).HasName("PK__Kho__3BDA9350FB4B540B");

            entity.ToTable("Kho");

            entity.HasIndex(e => e.MaDichVu, "UQ__Kho__C0E6DE8E16E35508").IsUnique();

            entity.Property(e => e.MaKho).ValueGeneratedNever();

            entity.HasOne(d => d.MaDichVuNavigation).WithOne(p => p.Kho)
                .HasForeignKey<Kho>(d => d.MaDichVu)
                .HasConstraintName("FK__Kho__MaDichVu__59FA5E80");
        });

        modelBuilder.Entity<KhuyenMai>(entity =>
        {
            entity.HasKey(e => e.MaKhuyenMai).HasName("PK__KhuyenMa__6F56B3BDDDE4E68C");

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

        modelBuilder.Entity<LoaiDichVu>(entity =>
        {
            entity.HasKey(e => e.MaLoai).HasName("PK__LoaiDich__730A57591174E9FC");

            entity.ToTable("LoaiDichVu");

            entity.Property(e => e.TenLoai).HasMaxLength(50);
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.MaNhanVien).HasName("PK__NhanVien__77B2CA470A469857");

            entity.ToTable("NhanVien");

            entity.HasIndex(e => e.MaTaiKhoan, "UQ__NhanVien__AD7C65281575B1DF").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.HoTen).HasMaxLength(255);
            entity.Property(e => e.Luong).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);

            entity.HasOne(d => d.MaTaiKhoanNavigation).WithOne(p => p.NhanVien)
                .HasForeignKey<NhanVien>(d => d.MaTaiKhoan)
                .HasConstraintName("FK__NhanVien__MaTaiK__5165187F");
        });

        modelBuilder.Entity<PhieuNhapKho>(entity =>
        {
            entity.HasKey(e => e.MaPhieuNhap).HasName("PK__PhieuNha__1470EF3BAC399BF2");

            entity.ToTable("PhieuNhapKho");

            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.NgayNhap)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaNhanVienNavigation).WithMany(p => p.PhieuNhapKhos)
                .HasForeignKey(d => d.MaNhanVien)
                .HasConstraintName("FK__PhieuNhap__MaNha__5DCAEF64");
        });

        modelBuilder.Entity<Phim>(entity =>
        {
            entity.HasKey(e => e.MaPhim).HasName("PK__Phim__4AC03DE3F1547AED");

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
                        j.HasKey("MaPhim", "MaTheLoai").HasName("PK__Phim_The__F7B3C2D7C8CC8C33");
                        j.ToTable("Phim_TheLoai");
                    });
        });

        modelBuilder.Entity<PhongChieu>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__PhongChi__20BD5E5B4C551103");

            entity.ToTable("PhongChieu");

            entity.Property(e => e.TenPhong).HasMaxLength(100);

            entity.HasOne(d => d.MaRapNavigation).WithMany(p => p.PhongChieus)
                .HasForeignKey(d => d.MaRap)
                .HasConstraintName("FK__PhongChie__MaRap__412EB0B6");
        });

        modelBuilder.Entity<Rap>(entity =>
        {
            entity.HasKey(e => e.MaRap).HasName("PK__Rap__3961207F93A4B141");

            entity.ToTable("Rap");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.TenRap).HasMaxLength(255);
            entity.Property(e => e.ThanhPho).HasMaxLength(100);
        });

        modelBuilder.Entity<SuatChieu>(entity =>
        {
            entity.HasKey(e => e.MaSuatChieu).HasName("PK__SuatChie__CF5984D22EE8164D");

            entity.ToTable("SuatChieu");

            entity.Property(e => e.Gia).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ThoiGianBatDau).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianKetThuc).HasColumnType("datetime");

            entity.HasOne(d => d.MaPhimNavigation).WithMany(p => p.SuatChieus)
                .HasForeignKey(d => d.MaPhim)
                .HasConstraintName("FK__SuatChieu__MaPhi__48CFD27E");

            entity.HasOne(d => d.MaPhongNavigation).WithMany(p => p.SuatChieus)
                .HasForeignKey(d => d.MaPhong)
                .HasConstraintName("FK__SuatChieu__MaPho__49C3F6B7");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.MaTaiKhoan).HasName("PK__TaiKhoan__AD7C652905D20E79");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.TenDangNhap, "UQ__TaiKhoan__55F68FC033853DBD").IsUnique();

            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.TenDangNhap).HasMaxLength(100);
            entity.Property(e => e.VaiTro)
                .HasMaxLength(50)
                .HasDefaultValue("NhanVien");
        });

        modelBuilder.Entity<ThanhToan>(entity =>
        {
            entity.HasKey(e => e.MaThanhToan).HasName("PK__ThanhToa__D4B25844AE2D656F");

            entity.ToTable("ThanhToan");

            entity.Property(e => e.NgayThanhToan).HasColumnType("datetime");
            entity.Property(e => e.PhuongThuc).HasMaxLength(50);
            entity.Property(e => e.SoTien).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.TrangThai).HasMaxLength(50);

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.ThanhToans)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__ThanhToan__MaDon__797309D9");
        });

        modelBuilder.Entity<TheLoai>(entity =>
        {
            entity.HasKey(e => e.MaTheLoai).HasName("PK__TheLoai__D73FF34A7D86522F");

            entity.ToTable("TheLoai");

            entity.Property(e => e.Ten).HasMaxLength(100);
        });

        modelBuilder.Entity<VeBan>(entity =>
        {
            entity.HasKey(e => e.MaDatVe).HasName("PK__VeBan__6A32C593A5A11851");

            entity.ToTable("VeBan");

            entity.HasIndex(e => new { e.MaGhe, e.MaSuatChieu }, "UQ_Ghe_SuatChieu").IsUnique();

            entity.Property(e => e.Gia).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.MaDonHangNavigation).WithMany(p => p.VeBans)
                .HasForeignKey(d => d.MaDonHang)
                .HasConstraintName("FK__VeBan__MaDonHang__70DDC3D8");

            entity.HasOne(d => d.MaGheNavigation).WithMany(p => p.VeBans)
                .HasForeignKey(d => d.MaGhe)
                .HasConstraintName("FK__VeBan__MaGhe__71D1E811");

            entity.HasOne(d => d.MaSuatChieuNavigation).WithMany(p => p.VeBans)
                .HasForeignKey(d => d.MaSuatChieu)
                .HasConstraintName("FK__VeBan__MaSuatChi__72C60C4A");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
