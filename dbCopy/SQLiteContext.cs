using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace dbCopy;

public partial class SQLiteContext : DbContext
{
    public SQLiteContext()
    {
    }

    public SQLiteContext(DbContextOptions<SQLiteContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Event_sqlite> Events { get; set; }

    public virtual DbSet<EventCount> EventCounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=C:\\Csharp\\GWTime\\dbCopy\\sqlite\\n240808.sqlite");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event_sqlite>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Index).HasColumnName("Index_");
            entity.Property(e => e.OperatorId).HasColumnName("OperatorID");
        });

        modelBuilder.Entity<EventCount>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("EventCount");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
