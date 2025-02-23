﻿// <auto-generated />
using System;
using API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API.Migrations
{
    [DbContext(typeof(DbApiContext))]
    partial class DbApiContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("API.Models.Claim", b =>
                {
                    b.Property<int>("ClaimID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CLAIM_ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ClaimID"));

                    b.Property<DateTime?>("ApprovedDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("APPROVED_DATE");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("FILE_PATH");

                    b.Property<int>("Hours")
                        .HasColumnType("int")
                        .HasColumnName("HOURS");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("NOTES");

                    b.Property<double>("Rate")
                        .HasColumnType("float")
                        .HasColumnName("RATE");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("STATUS");

                    b.Property<DateTime>("UploadDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("UPLOAD_DATE");

                    b.Property<int>("UserID")
                        .HasColumnType("int")
                        .HasColumnName("User_ID");

                    b.Property<int>("User_ID")
                        .HasColumnType("int");

                    b.HasKey("ClaimID");

                    b.HasIndex("UserID");

                    b.ToTable("CLAIM", null, t =>
                        {
                            t.Property("User_ID")
                                .HasColumnName("User_ID1");
                        });
                });

            modelBuilder.Entity("API.Models.Invoice", b =>
                {
                    b.Property<int>("InvoiceID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("INVOICE_ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InvoiceID"));

                    b.Property<int>("CLAIM_ID")
                        .HasColumnType("int");

                    b.Property<int>("ClaimID")
                        .HasColumnType("int")
                        .HasColumnName("CLAIM_ID");

                    b.Property<DateTime>("InvoiceDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("INVOICE_DATE");

                    b.Property<double>("TotalAmount")
                        .HasColumnType("float")
                        .HasColumnName("TOTAL_AMOUNT");

                    b.Property<int>("UserID")
                        .HasColumnType("int")
                        .HasColumnName("User_ID");

                    b.Property<int>("User_ID")
                        .HasColumnType("int");

                    b.HasKey("InvoiceID");

                    b.HasIndex("CLAIM_ID");

                    b.HasIndex("UserID");

                    b.ToTable("HR", null, t =>
                        {
                            t.Property("CLAIM_ID")
                                .HasColumnName("CLAIM_ID1");

                            t.Property("User_ID")
                                .HasColumnName("User_ID1");
                        });
                });

            modelBuilder.Entity("API.Models.Role", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("Id");

                    b.Property<string>("ConcurrencyStamp")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ConcurrencyStamp");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<string>("NormalizedName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("NormalizedName");

                    b.HasKey("Id");

                    b.ToTable("Role");
                });

            modelBuilder.Entity("API.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("User_ID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserID"));

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("EMAIL");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("FIRST_NAME");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("LAST_NAME");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("PASSWORD");

                    b.Property<string>("ROLE_ID")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ROLE_ID");

                    b.HasKey("UserID");

                    b.HasIndex("ROLE_ID");

                    b.ToTable("USERS", null, t =>
                        {
                            t.Property("ROLE_ID")
                                .HasColumnName("ROLE_ID1");
                        });
                });

            modelBuilder.Entity("API.Models.Claim", b =>
                {
                    b.HasOne("API.Models.User", "User")
                        .WithMany("Claims")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("API.Models.Invoice", b =>
                {
                    b.HasOne("API.Models.Claim", "Claim")
                        .WithMany()
                        .HasForeignKey("CLAIM_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Claim");

                    b.Navigation("User");
                });

            modelBuilder.Entity("API.Models.User", b =>
                {
                    b.HasOne("API.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("ROLE_ID");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("API.Models.User", b =>
                {
                    b.Navigation("Claims");
                });
#pragma warning restore 612, 618
        }
    }
}
