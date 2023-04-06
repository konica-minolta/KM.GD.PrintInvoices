﻿// <auto-generated />
using System;
using KM.GD.PrintInvoices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KM.GD.PrintInvoices.Migrations
{
    [DbContext(typeof(PrintInvoiceContext))]
    [Migration("20221125114100_AddStatusToprintedItems")]
    partial class AddStatusToprintedItems
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.12")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("KM.GD.PrintInvoices.Models.IndexedFile", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FILE_NAME")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("FULL_PATH")
                        .HasColumnType("varchar(250)");

                    b.HasKey("ID");

                    b.ToTable("IndexedFiles");
                });

            modelBuilder.Entity("KM.GD.PrintInvoices.Models.PrintedOrder", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DATE")
                        .HasColumnType("datetime");

                    b.Property<string>("INVOICE_FILE")
                        .HasColumnType("varchar(250)");

                    b.Property<bool>("ISSUBMISSIONVALID")
                        .HasColumnType("bit");

                    b.Property<int>("N_DOCS")
                        .HasColumnType("int");

                    b.Property<string>("ORDER_ID")
                        .HasColumnType("varchar(50)");

                    b.Property<string>("STATE")
                        .HasColumnType("varchar(50)");

                    b.Property<int>("TOTAL_PAGES")
                        .HasColumnType("int");

                    b.HasKey("ID");

                    b.ToTable("PrintedOrders");
                });

            modelBuilder.Entity("KM.GD.PrintInvoices.Models.PrintedOrderItems", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ORDER_ITEM_ID")
                        .HasColumnType("varchar(50)");

                    b.Property<int?>("PrintedOrderID")
                        .HasColumnType("int");

                    b.Property<string>("STATE")
                        .HasColumnType("varchar(50)");

                    b.HasKey("ID");

                    b.HasIndex("PrintedOrderID");

                    b.ToTable("PrintedOrderItems");
                });

            modelBuilder.Entity("KM.GD.PrintInvoices.Models.PrintedOrderItems", b =>
                {
                    b.HasOne("KM.GD.PrintInvoices.Models.PrintedOrder", null)
                        .WithMany("PritedOrderItems")
                        .HasForeignKey("PrintedOrderID");
                });

            modelBuilder.Entity("KM.GD.PrintInvoices.Models.PrintedOrder", b =>
                {
                    b.Navigation("PritedOrderItems");
                });
#pragma warning restore 612, 618
        }
    }
}
