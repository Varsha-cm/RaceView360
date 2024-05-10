using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Api.Models;

public partial class RaceViewContext : DbContext
{
    public RaceViewContext()
    {
    }

    public RaceViewContext(DbContextOptions<RaceViewContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Circuit> Circuits { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<DriverSeasonMapping> DriverSeasonMappings { get; set; }

    public virtual DbSet<DriverStanding> DriverStandings { get; set; }

    public virtual DbSet<LapTime> LapTimes { get; set; }

    public virtual DbSet<PracticeResult> PracticeResults { get; set; }

    public virtual DbSet<QualifyingResult> QualifyingResults { get; set; }

    public virtual DbSet<Race> Races { get; set; }

    public virtual DbSet<RaceResult> RaceResults { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Round> Rounds { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<TeamStanding> TeamStandings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLExpress01;Database=RaceView;Trusted_Connection=True;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Circuit>(entity =>
        {
            entity.HasKey(e => e.CircuitId).HasName("PK__Circuits__7D21697024EE91F7");

            entity.HasIndex(e => e.Name, "UQ__Circuits__737584F639D545BE").IsUnique();

            entity.HasIndex(e => e.CircuitCode, "UQ__Circuits__F04B7DF1AADE5120").IsUnique();

            entity.Property(e => e.CircuitId).HasColumnName("CircuitID");
            entity.Property(e => e.RaceDistance).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CircuitCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Country)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Length).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Location)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .IsUnicode(false)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.DriverId).HasName("PK__Drivers__F1B1CD24F6F312A6");

            entity.HasIndex(e => e.DriverCode, "UQ__Drivers__0BF84B47B1B3001D").IsUnique();

            entity.HasIndex(e => new { e.FirstName, e.LastName }, "UQ__Drivers__2457AEF02D79F7A2").IsUnique();

            entity.Property(e => e.DriverId).HasColumnName("DriverID");
            entity.Property(e => e.DateOfBirth).HasColumnType("date");
            entity.Property(e => e.DriverCode)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nationality)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Url)
                .IsUnicode(false)
                .HasColumnName("URL");
        });

        modelBuilder.Entity<DriverSeasonMapping>(entity =>
        {
            entity.HasKey(e => e.DriverSeasonMapping1).HasName("PK__DriverSe__138A6360B6177FDC");

            entity.ToTable("DriverSeasonMapping");

            entity.Property(e => e.DriverSeasonMapping1).HasColumnName("DriverSeasonMapping");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DriverId).HasColumnName("DriverID");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Driver).WithMany(p => p.DriverSeasonMappings)
                .HasForeignKey(d => d.DriverId)
                .HasConstraintName("FK__DriverSea__Drive__3CDEFCE5");

            entity.HasOne(d => d.Team).WithMany(p => p.DriverSeasonMappings)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK__DriverSea__TeamI__3DD3211E");
        });

        modelBuilder.Entity<DriverStanding>(entity =>
        {
            entity.HasKey(e => e.DriverStandingsId).HasName("PK__DriverSt__F3A89C7D04CA2822");

            entity.Property(e => e.DriverStandingsId).HasColumnName("DriverStandingsID");
            entity.Property(e => e.DriverId).HasColumnName("DriverID");
            entity.Property(e => e.RaceId).HasColumnName("RaceID");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");

            entity.HasOne(d => d.Driver).WithMany(p => p.DriverStandings)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DriverSta__Drive__231F2AE2");

            entity.HasOne(d => d.Race).WithMany(p => p.DriverStandings)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DriverSta__RaceI__2136E270");

            entity.HasOne(d => d.Team).WithMany(p => p.DriverStandings)
                .HasForeignKey(d => d.TeamId)
                .HasConstraintName("FK__DriverSta__TeamI__222B06A9");
        });

        modelBuilder.Entity<LapTime>(entity =>
        {
            entity.HasKey(e => e.LapId).HasName("PK__LapTime__E61AFF955590E2CD");

            entity.ToTable("LapTime");

            entity.Property(e => e.LapId).HasColumnName("LapID");
            entity.Property(e => e.DriverId).HasColumnName("DriverID");
            entity.Property(e => e.LapTime1).HasColumnName("LapTime");
            entity.Property(e => e.RaceId).HasColumnName("RaceID");
            entity.Property(e => e.RoundId).HasColumnName("RoundID");
            entity.Property(e => e.RoundType)
                .HasMaxLength(20)
                .HasConversion<string>()
                .IsUnicode(false);

            entity.HasOne(d => d.Driver).WithMany(p => p.LapTimes)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LapTime__DriverI__2AC04CAA");

            entity.HasOne(d => d.Race).WithMany(p => p.LapTimes)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LapTime__RaceID__29CC2871");

            entity.HasOne(d => d.Round).WithMany(p => p.LapTimes)
                .HasForeignKey(d => d.RoundId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__LapTime__RoundID__2BB470E3");
        });

        modelBuilder.Entity<PracticeResult>(entity =>
        {
            entity.HasKey(e => e.PracticeResultId).HasName("PK__Practice__66915A3F3A800D68");

            entity.Property(e => e.PracticeResultId).HasColumnName("PracticeResultID");
            entity.Property(e => e.DriverId).HasColumnName("DriverID");
            entity.Property(e => e.PracticeType)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RaceId).HasColumnName("RaceID");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");

            entity.HasOne(d => d.Driver).WithMany(p => p.PracticeResults)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PracticeR__Drive__2F8501C7");

            entity.HasOne(d => d.Race).WithMany(p => p.PracticeResults)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PracticeR__RaceI__2E90DD8E");

            entity.HasOne(d => d.Team).WithMany(p => p.PracticeResults)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PracticeR__TeamI__30792600");
        });

        modelBuilder.Entity<QualifyingResult>(entity =>
        {
            entity.HasKey(e => e.QualifyId).HasName("PK__Qualifyi__872DF3C44ED7D602");

            entity.Property(e => e.QualifyId).HasColumnName("QualifyID");
            entity.Property(e => e.DriverId).HasColumnName("DriverID");
            entity.Property(e => e.RaceId).HasColumnName("RaceID");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");

            entity.HasOne(d => d.Driver).WithMany(p => p.QualifyingResults)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Qualifyin__Drive__3449B6E4");

            entity.HasOne(d => d.Race).WithMany(p => p.QualifyingResults)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Qualifyin__RaceI__335592AB");

            entity.HasOne(d => d.Team).WithMany(p => p.QualifyingResults)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Qualifyin__TeamI__353DDB1D");
        });

        modelBuilder.Entity<Race>(entity =>
        {
            entity.HasKey(e => e.RaceId).HasName("PK__Races__05FBD6D464FA5292");

            entity.HasIndex(e => e.RaceCode, "UQ__Races__54465BEBF75EA609").IsUnique();

            entity.Property(e => e.RaceId).HasColumnName("RaceID");
            entity.Property(e => e.CircuitId).HasColumnName("CircuitID");
            entity.Property(e => e.Practice1DateTime).HasColumnType("datetime");
            entity.Property(e => e.Practice2DateTime).HasColumnType("datetime");
            entity.Property(e => e.Practice3DateTime).HasColumnType("datetime");
            entity.Property(e => e.QualifyDateTime).HasColumnType("datetime");
            entity.Property(e => e.RaceCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.RaceDateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RaceName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.SeasonId).HasColumnName("SeasonID");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Url).IsUnicode(false);

            entity.HasOne(d => d.Circuit).WithMany(p => p.Races)
                .HasForeignKey(d => d.CircuitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Races__CircuitID__1995C0A8");

            entity.HasOne(d => d.Season).WithMany(p => p.Races)
                .HasForeignKey(d => d.SeasonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Races__SeasonID__1A89E4E1");
        });

        modelBuilder.Entity<RaceResult>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__RaceResu__9769022813239664");

            entity.Property(e => e.ResultId).HasColumnName("ResultID");
            entity.Property(e => e.DriverId).HasColumnName("DriverID");
            entity.Property(e => e.PointsEarned).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.RaceId).HasColumnName("RaceID");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");

            entity.HasOne(d => d.Driver).WithMany(p => p.RaceResults)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RaceResul__Drive__390E6C01");

            entity.HasOne(d => d.Race).WithMany(p => p.RaceResults)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RaceResul__RaceI__381A47C8");

            entity.HasOne(d => d.Team).WithMany(p => p.RaceResults)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RaceResul__TeamI__3A02903A");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A771D08CF");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.RoleName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<Round>(entity =>
        {
            entity.HasKey(e => e.RoundId).HasName("PK__Rounds__94D84E1A5CFE3B52");

            entity.Property(e => e.RoundId).HasColumnName("RoundID");
            entity.Property(e => e.EndTime).HasColumnType("datetime");
            entity.Property(e => e.RaceId).HasColumnName("RaceID");
            entity.Property(e => e.RoundStatus)
                .HasMaxLength(50)
                .HasConversion<string>()
                .IsUnicode(false)
                .HasDefaultValueSql("('not started')");
            entity.Property(e => e.RoundType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.StartTime).HasColumnType("datetime");

            entity.HasOne(d => d.Race).WithMany(p => p.Rounds)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rounds__RaceID__1E5A75C5");
        });

        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasKey(e => e.SeasonId).HasName("PK__Seasons__C1814E1865A3714A");

            entity.HasIndex(e => e.Year, "UQ__Seasons__D4BD6054DF8513FD").IsUnique();

            entity.Property(e => e.SeasonId).HasColumnName("SeasonID");
            entity.Property(e => e.Url).IsUnicode(false);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PK__Teams__123AE7B9628B72C4");

            entity.HasIndex(e => e.TeamName, "UQ__Teams__4E21CAACE6495405").IsUnique();

            entity.HasIndex(e => e.TeamCode, "UQ__Teams__55013508AD7D251A").IsUnique();

            entity.Property(e => e.TeamId).HasColumnName("TeamID");
            entity.Property(e => e.Nationality)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TeamCode)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.TeamName)
                .IsRequired()
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Url).IsUnicode(false);
        });

        modelBuilder.Entity<TeamStanding>(entity =>
        {
            entity.HasKey(e => e.TeamStandingsId).HasName("PK__TeamStan__47C3FFE77745C6F1");

            entity.Property(e => e.TeamStandingsId).HasColumnName("TeamStandingsID");
            entity.Property(e => e.RaceId).HasColumnName("RaceID");
            entity.Property(e => e.TeamId).HasColumnName("TeamID");

            entity.HasOne(d => d.Race).WithMany(p => p.TeamStandings)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeamStand__RaceI__25FB978D");

            entity.HasOne(d => d.Team).WithMany(p => p.TeamStandings)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TeamStand__TeamI__26EFBBC6");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACE5E8B1E4");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534A142509E").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__RoleID__44801EAD");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
