using System;
using System.Collections.Generic;
using BlogWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogWeb.Data;

public partial class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Experience> Experiences { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Experience>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Experience_pkey");

            entity.ToTable("Experience");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Company).HasColumnName("company");
            entity.Property(e => e.EndDate)
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("endDate");
            entity.Property(e => e.Highlights)
                .HasDefaultValueSql("''::text")
                .HasColumnName("highlights");
            entity.Property(e => e.Order)
                .HasDefaultValue(0)
                .HasColumnName("order");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.StartDate)
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("startDate");
            entity.Property(e => e.Summary).HasColumnName("summary");
            entity.Property(e => e.Tech)
                .HasDefaultValueSql("''::text")
                .HasColumnName("tech");
            entity.Property(e => e.Type)
                .HasDefaultValueSql("'WORK'::text")
                .HasColumnName("type");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Post_pkey");

            entity.ToTable("Post");

            entity.HasIndex(e => e.Slug, "Post_slug_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Body).HasColumnName("body");
            entity.Property(e => e.Category)
                .HasDefaultValueSql("'Street'::text")
                .HasColumnName("category");
            entity.Property(e => e.CoverImage).HasColumnName("coverImage");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("createdAt");
            entity.Property(e => e.Excerpt).HasColumnName("excerpt");
            entity.Property(e => e.Published)
                .HasDefaultValue(false)
                .HasColumnName("published");
            entity.Property(e => e.Slug).HasColumnName("slug");
            entity.Property(e => e.Tags)
                .HasDefaultValueSql("ARRAY[]::text[]")
                .HasColumnName("tags");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Profile_pkey");

            entity.ToTable("Profile");

            entity.Property(e => e.Id)
                .HasDefaultValue(1)
                .HasColumnName("id");
            entity.Property(e => e.AvatarUrl).HasColumnName("avatarUrl");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Github).HasColumnName("github");
            entity.Property(e => e.Linkedin).HasColumnName("linkedin");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PenAvatarUrl).HasColumnName("penAvatarUrl");
            entity.Property(e => e.PenBio).HasColumnName("penBio");
            entity.Property(e => e.PenName).HasColumnName("penName");
            entity.Property(e => e.ResumeUrl).HasColumnName("resumeUrl");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Tagline).HasColumnName("tagline");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("timestamp(3) without time zone")
                .HasColumnName("updatedAt");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Project_pkey");

            entity.ToTable("Project");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ImageUrl).HasColumnName("imageUrl");
            entity.Property(e => e.Link).HasColumnName("link");
            entity.Property(e => e.Order)
                .HasDefaultValue(0)
                .HasColumnName("order");
            entity.Property(e => e.Problem).HasColumnName("problem");
            entity.Property(e => e.Result).HasColumnName("result");
            entity.Property(e => e.Solution).HasColumnName("solution");
            entity.Property(e => e.Title).HasColumnName("title");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Skill_pkey");

            entity.ToTable("Skill");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Category)
                .HasDefaultValueSql("'other'::text")
                .HasColumnName("category");
            entity.Property(e => e.IconUrl).HasColumnName("iconUrl");
            entity.Property(e => e.Level)
                .HasDefaultValue(70)
                .HasColumnName("level");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Order)
                .HasDefaultValue(0)
                .HasColumnName("order");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
