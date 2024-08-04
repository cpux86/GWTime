using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace dbCopy;

public partial class Gwt0905Context : DbContext
{
    public Gwt0905Context()
    {
    }

    public Gwt0905Context(DbContextOptions<Gwt0905Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Reader> Readers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Запрос> Запросs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=10.65.68.252; Database=GWT0905; User ID=sa; Password=LaMp368&;Integrated Security=false;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasIndex(e => new { e.Code, e.ReaderId, e.UserId, e.DateTime }, "IX_Events_Code_ReaderId_UserId_dateTime").IsUnique();

            entity.HasIndex(e => e.ReaderId, "IX_Events_ReaderId");

            entity.HasIndex(e => e.UserId, "IX_Events_UserId");

            entity.Property(e => e.DateTime)
                .HasPrecision(2)
                .HasColumnName("dateTime");

            entity.HasOne(d => d.Reader).WithMany(p => p.Events).HasForeignKey(d => d.ReaderId);

            entity.HasOne(d => d.User).WithMany(p => p.Events).HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Reader>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.GroupId, "IX_Users_GroupId");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.LastUsedKeyDate).HasPrecision(2);

            entity.HasOne(d => d.Group).WithMany(p => p.Users).HasForeignKey(d => d.GroupId);
        });

        modelBuilder.Entity<Запрос>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Запрос");

            entity.Property(e => e.DateTime).HasColumnName("dateTime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
