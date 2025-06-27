using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace STD.Models.DB;

public partial class AuditManager_DB : DbContext
{
    public AuditManager_DB()
    {
    }

    public AuditManager_DB(DbContextOptions<AuditManager_DB> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditIssueActivityVerify> AuditIssueActivityVerifies { get; set; }

    public virtual DbSet<AuditIssueMain> AuditIssueMains { get; set; }

    public virtual DbSet<AuditIssueSub> AuditIssueSubs { get; set; }

    public virtual DbSet<AuditManagement> AuditManagements { get; set; }

    public virtual DbSet<AuditManagementSub> AuditManagementSubs { get; set; }

    public virtual DbSet<AuditManegementSegment> AuditManegementSegments { get; set; }

    public virtual DbSet<AuditManegementSegmentSub> AuditManegementSegmentSubs { get; set; }

    public virtual DbSet<AuditManegementSegmentSubVerify> AuditManegementSegmentSubVerifies { get; set; }

    public virtual DbSet<EngagementPlan> EngagementPlans { get; set; }

    public virtual DbSet<Faculty> Faculties { get; set; }

    public virtual DbSet<RoutingSlip> RoutingSlips { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<UserAuditor> UserAuditors { get; set; }

    public virtual DbSet<WorkingPaper> WorkingPapers { get; set; }

    public virtual DbSet<WorkingPaperActivity> WorkingPaperActivities { get; set; }

    public virtual DbSet<WorkingPaperActivitySub> WorkingPaperActivitySubs { get; set; }

    public virtual DbSet<WorkingPaperActivitySubVerify> WorkingPaperActivitySubVerifies { get; set; }

    public virtual DbSet<WorkingPaperActivityVerify> WorkingPaperActivityVerifies { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("data source=(localdb)\\Local; initial catalog=AuditManager; user id=; password=; multipleactiveresultsets=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditIssueActivityVerify>(entity =>
        {
            entity.HasKey(e => e.AuditIssueActivityId).HasName("PK_AuditIssueActivitiy");

            entity.ToTable("AuditIssueActivityVerify");

            entity.Property(e => e.AuditIssueActivityId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("AuditIssueActivityID");
            entity.Property(e => e.AuditIssueSubId).HasColumnName("AuditIssueSubID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<AuditIssueMain>(entity =>
        {
            entity.ToTable("AuditIssueMain");

            entity.Property(e => e.AuditIssueMainId).HasColumnName("AuditIssueMainID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<AuditIssueSub>(entity =>
        {
            entity.ToTable("AuditIssueSub");

            entity.Property(e => e.AuditIssueSubId)
                .HasComment("1 จุดประสงค์ 2 อ้างอิง 3 กิจกรรมการควบคุม")
                .HasColumnName("AuditIssueSubID");
            entity.Property(e => e.AuditIssueMainId).HasColumnName("AuditIssueMainID");
            entity.Property(e => e.AuditIssueSubType).HasDefaultValue(1);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<AuditManagement>(entity =>
        {
            entity.ToTable("AuditManagement");

            entity.Property(e => e.AuditManagementId).HasColumnName("AuditManagementID");
            entity.Property(e => e.AuditIssueMainId).HasColumnName("AuditIssueMainID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.Status).HasDefaultValue(1);
        });

        modelBuilder.Entity<AuditManagementSub>(entity =>
        {
            entity.ToTable("AuditManagementSub");

            entity.Property(e => e.AuditManagementSubId).HasColumnName("AuditManagementSubID");
            entity.Property(e => e.AuditIssueSubId).HasColumnName("AuditIssueSubID");
            entity.Property(e => e.AuditManagementId).HasColumnName("AuditManagementID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<AuditManegementSegment>(entity =>
        {
            entity.ToTable("AuditManegementSegment");

            entity.Property(e => e.AuditManegementSegmentId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("AuditManegementSegmentID");
            entity.Property(e => e.AuditIssueMainId).HasColumnName("AuditIssueMainID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
        });

        modelBuilder.Entity<AuditManegementSegmentSub>(entity =>
        {
            entity.ToTable("AuditManegementSegmentSub");

            entity.Property(e => e.AuditManegementSegmentSubId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("AuditManegementSegmentSubID");
            entity.Property(e => e.AuditIssueSubId).HasColumnName("AuditIssueSubID");
            entity.Property(e => e.AuditManegementSegmentId).HasColumnName("AuditManegementSegmentID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<AuditManegementSegmentSubVerify>(entity =>
        {
            entity.ToTable("AuditManegementSegmentSubVerify");

            entity.Property(e => e.AuditManegementSegmentSubVerifyId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("AuditManegementSegmentSubVerifyID");
            entity.Property(e => e.AuditIssueActivityId).HasColumnName("AuditIssueActivityID");
            entity.Property(e => e.AuditManegementSegmentSubId).HasColumnName("AuditManegementSegmentSubID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<EngagementPlan>(entity =>
        {
            entity.ToTable("EngagementPlan");

            entity.Property(e => e.EngagementPlanId).HasColumnName("EngagementPlanID");
            entity.Property(e => e.AuditManagementId).HasColumnName("AuditManagementID");
            entity.Property(e => e.AuditTimeEnd).HasPrecision(0);
            entity.Property(e => e.AuditTimeStart).HasPrecision(0);
            entity.Property(e => e.AuditorId).HasColumnName("AuditorID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Wpcode)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("WPCode");
        });

        modelBuilder.Entity<Faculty>(entity =>
        {
            entity.ToTable("Faculty");

            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FacultyCode)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<RoutingSlip>(entity =>
        {
            entity.ToTable("RoutingSlip");

            entity.Property(e => e.RoutingSlipId).HasColumnName("RoutingSlipID");
            entity.Property(e => e.AuditManagementId).HasColumnName("AuditManagementID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.ToTable("Schedule");

            entity.Property(e => e.ScheduleId).HasColumnName("ScheduleID");
            entity.Property(e => e.AuditManagementId).HasColumnName("AuditManagementID");
            entity.Property(e => e.CreateData).HasColumnType("datetime");
            entity.Property(e => e.ScheduleTime).HasPrecision(0);
        });

        modelBuilder.Entity<UserAuditor>(entity =>
        {
            entity.HasKey(e => e.AuditorId);

            entity.ToTable("UserAuditor");

            entity.Property(e => e.AuditorId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("AuditorID");
            entity.Property(e => e.AuditorCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(200);
            entity.Property(e => e.LastName).HasMaxLength(200);
            entity.Property(e => e.Password).IsUnicode(false);
            entity.Property(e => e.Role)
                .HasDefaultValue(1)
                .HasComment("1 Auditor 2 Director 99 Admin");
            entity.Property(e => e.Username)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<WorkingPaper>(entity =>
        {
            entity.ToTable("WorkingPaper");

            entity.Property(e => e.WorkingPaperId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("WorkingPaperID");
            entity.Property(e => e.ApproveDate).HasColumnType("datetime");
            entity.Property(e => e.AuditIssueMainId).HasColumnName("AuditIssueMainID");
            entity.Property(e => e.AuditManegementSegmentId).HasColumnName("AuditManegementSegmentID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.FacultyId).HasColumnName("FacultyID");
            entity.Property(e => e.ProducerBy).HasComment("ผู้จัดทำ");
            entity.Property(e => e.ReviewerBy).HasComment("ผู้สอบทาน");
        });

        modelBuilder.Entity<WorkingPaperActivity>(entity =>
        {
            entity.ToTable("WorkingPaperActivity");

            entity.Property(e => e.WorkingPaperActivityId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("WorkingPaperActivityID");
            entity.Property(e => e.AuditIssueSubId).HasColumnName("AuditIssueSubID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Issue).HasComment("1 เป็นประเด็น 2 ไม่เป็นประเด็น");
            entity.Property(e => e.WorkingPaperId).HasColumnName("WorkingPaperID");
        });

        modelBuilder.Entity<WorkingPaperActivitySub>(entity =>
        {
            entity.ToTable("WorkingPaperActivitySub");

            entity.Property(e => e.WorkingPaperActivitySubId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("WorkingPaperActivitySubID");
            entity.Property(e => e.AuditIssueSubId).HasColumnName("AuditIssueSubID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Issue).HasComment("1 เป็นประเด็น 2 ไม่เป็นประเด็น");
            entity.Property(e => e.WorkingPaperActivityId).HasColumnName("WorkingPaperActivityID");
        });

        modelBuilder.Entity<WorkingPaperActivitySubVerify>(entity =>
        {
            entity.ToTable("WorkingPaperActivitySubVerify");

            entity.Property(e => e.WorkingPaperActivitySubVerifyId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("WorkingPaperActivitySubVerifyID");
            entity.Property(e => e.AuditIssueActivityId).HasColumnName("AuditIssueActivityID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Issue).HasComment("1 เป็นประเด็น 2 ไม่เป็นประเด็น");
            entity.Property(e => e.WorkingPaperActivitySubId).HasColumnName("WorkingPaperActivitySubID");
        });

        modelBuilder.Entity<WorkingPaperActivityVerify>(entity =>
        {
            entity.ToTable("WorkingPaperActivityVerify");

            entity.Property(e => e.WorkingPaperActivityVerifyId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("WorkingPaperActivityVerifyID");
            entity.Property(e => e.AuditIssueActivityId).HasColumnName("AuditIssueActivityID");
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Issue).HasComment("1 เป็นประเด็น 2 ไม่เป็นประเด็น");
            entity.Property(e => e.WorkingPaperActivityId).HasColumnName("WorkingPaperActivityID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
