using Microsoft.EntityFrameworkCore;
using ZapWeb.Models;

namespace ZapWeb.BancoDados
{
    public class ContextDB : DbContext
    {
        
        public ContextDB(DbContextOptions<ContextDB> options):base(options)
        {

        }
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Grupos> Grupos { get; set; }
        public DbSet<Mensagem> Mensagem { get; set; }
    }
}
