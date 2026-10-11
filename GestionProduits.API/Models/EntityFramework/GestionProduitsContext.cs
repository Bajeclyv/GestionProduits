using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

// using System.Data.Entity.Infrastructure.Annotations;

namespace GestionProduits.API.Models.EntityFramework
{

    public partial class GestionProduitsContext : DbContext
    {
        public GestionProduitsContext()
        {
        }

        public GestionProduitsContext(DbContextOptions<GestionProduitsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Produit> Produits { get; set; }
        public virtual DbSet<TypeProduit> TypeProduits { get; set; }
        public virtual DbSet<Marque> Marques { get; set; }

        public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Server=localhost,5432;Database=produitsdb;UserId=postgres;Password=postgres;TrustServerCertificate=True");

            // optionsBuilder.UseLoggerFactory(MyLoggerFactory).EnableSensitiveDataLogging()
            //               .UseNpgsql();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.Entity<Produit>()
                        .HasIndex(t => t.NomProduit);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}