using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VideoJuegoProyectoServidorCore.Models;

namespace VideoJuegoProyectoServidorCore.Data
{
    public class InfoClienteContext : DbContext
    {
        static string database = "ClienteInfodb.db";

        public DbSet<ClienteInfo> ClienteInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(connectionString: "Filename=" + database,
                sqliteOptionsAction: op =>
                {
                    op.MigrationsAssembly(

                        Assembly.GetExecutingAssembly().FullName

                        );
                });

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClienteInfo>().ToTable("ClienteInformacion");
            modelBuilder.Entity<ClienteInfo>(entity => {

                entity.HasKey(a => a.id);

            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
