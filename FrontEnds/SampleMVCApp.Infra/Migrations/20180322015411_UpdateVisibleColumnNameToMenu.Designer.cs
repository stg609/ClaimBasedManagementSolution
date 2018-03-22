﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using SampleMVCApp.Infra;
using System;

namespace SampleMVCApp.Infra.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20180322015411_UpdateVisibleColumnNameToMenu")]
    partial class UpdateVisibleColumnNameToMenu
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SampleMVCApp.Domain.MenuDTO", b =>
                {
                    b.Property<int>("Key")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Claims");

                    b.Property<string>("Name");

                    b.Property<int>("Order");

                    b.Property<int>("ParentMenuKey");

                    b.Property<string>("Url");

                    b.Property<bool>("Visible");

                    b.HasKey("Key");

                    b.ToTable("Menus");
                });
#pragma warning restore 612, 618
        }
    }
}
