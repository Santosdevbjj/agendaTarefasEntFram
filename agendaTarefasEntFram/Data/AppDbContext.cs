using AgendaTarefas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AgendaTarefas.Data
{
    /// <summary>
    /// DbContext principal da aplicação. Registra a entidade Tarefa e converte o enum para string.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // DbSet que representa a tabela 'Tarefas' no banco.
        public DbSet<Tarefa> Tarefas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurações da entidade Tarefa
            modelBuilder.Entity<Tarefa>(entity =>
            {
                entity.ToTable("Tarefas"); // nome explícito da tabela

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Titulo)
                      .IsRequired()
                      .HasMaxLength(200);

                entity.Property(e => e.Descricao)
                      .HasMaxLength(1000);

                entity.Property(e => e.Data)
                      .IsRequired();

                // Salva enum StatusTarefa como string no banco para legibilidade.
                entity.Property(e => e.Status)
                      .HasConversion(new EnumToStringConverter<StatusTarefa>())
                      .HasMaxLength(50)
                      .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
