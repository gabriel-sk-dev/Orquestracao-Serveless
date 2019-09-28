﻿// <auto-generated />
using System;
using Faturamento.Api.Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Faturamento.Api.Migrations
{
    [DbContext(typeof(EFContexto))]
    [Migration("20190928000218_inicial")]
    partial class inicial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Faturamento.Api.Dominio.Pagamento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CriadoEm");

                    b.Property<int>("PedidoId");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.Property<string>("TransacaoId");

                    b.Property<decimal>("Valor");

                    b.HasKey("Id");

                    b.HasIndex("PedidoId");

                    b.ToTable("Pagamentos");
                });

            modelBuilder.Entity("Faturamento.Api.Dominio.Pedido", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CriadoEm");

                    b.Property<string>("ProcessoId");

                    b.Property<int>("SocioId");

                    b.Property<string>("Status")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Pedidos");
                });

            modelBuilder.Entity("Faturamento.Api.Dominio.Pedido+Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DividaId");

                    b.Property<int?>("PedidoId");

                    b.Property<decimal>("Valor");

                    b.HasKey("Id");

                    b.HasIndex("PedidoId");

                    b.ToTable("Item");
                });

            modelBuilder.Entity("Faturamento.Api.Dominio.Pagamento", b =>
                {
                    b.HasOne("Faturamento.Api.Dominio.Pedido")
                        .WithMany()
                        .HasForeignKey("PedidoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Faturamento.Api.Dominio.Pedido+Item", b =>
                {
                    b.HasOne("Faturamento.Api.Dominio.Pedido")
                        .WithMany("Itens")
                        .HasForeignKey("PedidoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}