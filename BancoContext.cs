using Microsoft.EntityFrameworkCore;
using System;
namespace BancoDIO

{
    public class BancoContext : DbContext
    {
        public DbSet<Conta> Contas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(@"Data Source=BancoLite.db");
    }
}