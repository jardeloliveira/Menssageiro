﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ZapWeb.BancoDados;

namespace ZapWeb.Migrations
{
    [DbContext(typeof(ContextDB))]
    partial class ContextDBModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1");

            modelBuilder.Entity("ZapWeb.Models.Grupos", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Nome")
                        .HasColumnType("TEXT");

                    b.Property<string>("Usuarios")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Grupos");
                });

            modelBuilder.Entity("ZapWeb.Models.Mensagem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("DataCriacao")
                        .HasColumnType("TEXT");

                    b.Property<string>("Grupo")
                        .HasColumnType("TEXT");

                    b.Property<string>("Texto")
                        .HasColumnType("TEXT");

                    b.Property<string>("Usuario")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Mensagem");
                });

            modelBuilder.Entity("ZapWeb.Models.Usuarios", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConnectionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Nome")
                        .HasColumnType("TEXT");

                    b.Property<string>("Senha")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Usuarios");
                });
#pragma warning restore 612, 618
        }
    }
}
