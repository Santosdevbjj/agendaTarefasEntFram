using System;
using AgendaTarefas.Data;
using AgendaTarefas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AgendaTarefas.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AgendaTarefasModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "8.0.0");

            modelBuilder.Entity("AgendaTarefas.Models.Tarefa", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int");

                b.Property<string>("Descricao")
                    .HasColumnType("nvarchar(1000)")
                    .HasMaxLength(1000);

                b.Property<DateTime>("Data")
                    .HasColumnType("datetime2");

                b.Property<string>("Status")
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasConversion(new EnumToStringConverter<StatusTarefa>())
                    .HasColumnType("nvarchar(50)");

                b.Property<string>("Titulo")
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnType("nvarchar(200)");

                b.HasKey("Id");

                b.ToTable("Tarefas");
            });
        }
    }
}
