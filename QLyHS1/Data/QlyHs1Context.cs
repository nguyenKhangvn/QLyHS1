using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QLyHS1.Data;

public partial class QlyHs1Context : DbContext
{
    public QlyHs1Context()
    {
    }

    public QlyHs1Context(DbContextOptions<QlyHs1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<Classroom> Classrooms { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<GrandLevel> GrandLevels { get; set; }

    public virtual DbSet<Schedule> Schedules { get; set; }

    public virtual DbSet<SchoolYear> SchoolYears { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<WeekDay> WeekDays { get; set; }

    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=ADMIN\\SQL2022EX;Initial Catalog=QLyHS1;Integrated Security=True;Encrypt=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Assignment");

            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Subject).WithMany()
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Assignmen__Subje__6FE99F9F");

            entity.HasOne(d => d.Teacher).WithMany()
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Assignmen__EndDa__6EF57B66");
        });

        modelBuilder.Entity<Classroom>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Classroo__3213E83F409A06B2");

            entity.ToTable("Classroom");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt).HasColumnName("createAt");
            entity.Property(e => e.GrandLevelId).HasColumnName("grandLevelID");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TeacherId).HasColumnName("teacherID");
            entity.Property(e => e.UpdateAt).HasColumnName("updateAt");

            entity.HasOne(d => d.GrandLevel).WithMany(p => p.Classrooms)
                .HasForeignKey(d => d.GrandLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classroom__grand__5AEE82B9");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Classrooms)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Classroom__teach__5BE2A6F2");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Grade__3213E83F32CFB12C");

            entity.ToTable("Grade");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createAt");
            entity.Property(e => e.GradeII).HasColumnName("GradeII");
            entity.Property(e => e.SchoolYearId).HasColumnName("SchoolYearID");
            entity.Property(e => e.SemesterId).HasColumnName("SemesterID");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.SchoolYear).WithMany(p => p.Grades)
                .HasForeignKey(d => d.SchoolYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grade__SchoolYea__6D0D32F4");

            entity.HasOne(d => d.Semester).WithMany(p => p.Grades)
                .HasForeignKey(d => d.SemesterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grade__SemesterI__6B24EA82");

            entity.HasOne(d => d.Student).WithMany(p => p.Grades)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grade__StudentID__6A30C649");

            entity.HasOne(d => d.Subject).WithMany(p => p.Grades)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grade__SubjectID__6C190EBB");

            entity.HasOne(d => d.Classroom).WithMany(p => p.Grades)
                .HasForeignKey(d => d.ClassNameID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WClassroom_Grade");
        });

        modelBuilder.Entity<GrandLevel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__GrandLev__3213E83FB8CED19C");

            entity.ToTable("GrandLevel");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Schedule__3213E83FF2E782FF");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClassRoom)
                .HasMaxLength(50)
                .HasColumnName("classRoom");
            entity.Property(e => e.DayOfWeek).HasColumnName("dayOfWeek");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("endTime");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("startTime");
            entity.Property(e => e.SubjectId).HasColumnName("subjectID");
            entity.Property(e => e.TeacherId).HasColumnName("teacherID");

            entity.HasOne(d => d.DayOfWeekNavigation).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.DayOfWeek)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WeekDays_Schedule");

            entity.HasOne(d => d.Subject).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Schedules__subje__5441852A");

            entity.HasOne(d => d.Teacher).WithMany(p => p.Schedules)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_Teacher_Schedule");
        });

        modelBuilder.Entity<SchoolYear>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SchoolYe__3213E83F0DBE269E");

            entity.ToTable("SchoolYear");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Semester__3213E83F8F4F43FE");

            entity.ToTable("Semester");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createAt");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Student__3213E83F0D1478DC");

            entity.ToTable("Student");

            entity.HasIndex(e => e.Email, "UQ__Student__AB6E6164FA99CAE4").IsUnique();

            entity.HasIndex(e => e.Phone, "UQ__Student__B43B145F53FE56B9").IsUnique();

            entity.HasIndex(e => e.PhoneParent, "UQ__Student__EA615CE5281DF667").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.ClassId).HasColumnName("classID");
            entity.Property(e => e.Conduct)
                .HasMaxLength(10)
                .HasColumnName("conduct");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createAt");
            entity.Property(e => e.DateOfBirth).HasColumnName("dateOfBirth");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.PhoneParent)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phoneParent");
            entity.Property(e => e.Status)
                .HasDefaultValue(true)
                .HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updateAt");

            entity.HasOne(d => d.Class).WithMany(p => p.Students)
                .HasForeignKey(d => d.ClassId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Student__classID__6477ECF3");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subject__3213E83FD7047C88");

            entity.ToTable("Subject");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("createAt");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UpdateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updateAt");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Teacher__3213E83F861FCB4F");

            entity.ToTable("Teacher");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.CreateAt).HasColumnName("createAt");
            entity.Property(e => e.DateOfBirth).HasColumnName("dateOfBirth");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Token)
                .HasMaxLength(200)
                .HasColumnName("token");
            entity.Property(e => e.UpdateAt).HasColumnName("updateAt");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("userName");
        });

        modelBuilder.Entity<WeekDay>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__WeekDays__3214EC07BC126F2E");

            entity.Property(e => e.DayName).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
