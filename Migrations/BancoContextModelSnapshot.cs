﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Servidor_V3.Data;

#nullable disable

namespace Servidor_V3.Migrations
{
    [DbContext(typeof(BancoContext))]
    partial class BancoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Servidor.Models.AdministrativoModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Acoluna1")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Acoluna2")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Acoluna3")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Acoluna4")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Acoluna5")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Acoluna6")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Administrativo", (string)null);
                });

            modelBuilder.Entity("Servidor.Models.ContrachequeModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Cargo")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna1")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna10")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna11")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna12")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna13")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna14")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna15")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna16")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna17")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna18")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna19")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna2")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna20")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna21")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna22")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna23")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna24")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna25")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna3")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna4")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna5")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna6")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna7")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna8")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Ccoluna9")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Contracheque", (string)null);
                });

            modelBuilder.Entity("Servidor.Models.SelectOptionModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Municipio")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Uf")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("SelectOptions", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
