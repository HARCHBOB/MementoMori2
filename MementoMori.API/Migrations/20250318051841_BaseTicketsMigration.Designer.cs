﻿// <auto-generated />
using System;
using MementoMori.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MementoMori.API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250318051841_BaseTicketsMigration")]
    partial class BaseTicketsMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("MementoMori.API.Entities.Card", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Answer")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("DeckId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("DeckId");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("MementoMori.API.Entities.Deck", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("CardCount")
                        .HasColumnType("bigint");

                    b.Property<Guid>("CreatorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("boolean");

                    b.Property<DateOnly>("Modified")
                        .HasColumnType("date");

                    b.Property<double>("Rating")
                        .HasColumnType("double precision");

                    b.Property<long>("RatingCount")
                        .HasColumnType("bigint");

                    b.Property<int[]>("Tags")
                        .HasColumnType("integer[]");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.ToTable("Decks");
                });

            modelBuilder.Entity("MementoMori.API.Entities.Ticket", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("TicketType")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("MementoMori.API.Entities.TicketComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("TicketId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("TicketId");

                    b.HasIndex("UserId");

                    b.ToTable("TicketComments");
                });

            modelBuilder.Entity("MementoMori.API.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CardColor")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("MementoMori.API.Entities.UserCardData", b =>
                {
                    b.Property<Guid>("CardId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("DeckId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<double>("EaseFactor")
                        .HasColumnType("double precision");

                    b.Property<int>("Interval")
                        .HasColumnType("integer");

                    b.Property<DateTime>("LastReviewed")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Repetitions")
                        .HasColumnType("integer");

                    b.HasKey("CardId", "DeckId", "UserId");

                    b.ToTable("UserCards");
                });

            modelBuilder.Entity("MementoMori.API.Entities.Card", b =>
                {
                    b.HasOne("MementoMori.API.Entities.Deck", null)
                        .WithMany("Cards")
                        .HasForeignKey("DeckId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MementoMori.API.Entities.Deck", b =>
                {
                    b.HasOne("MementoMori.API.Entities.User", "Creator")
                        .WithMany()
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("MementoMori.API.Entities.Ticket", b =>
                {
                    b.HasOne("MementoMori.API.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MementoMori.API.Entities.TicketComment", b =>
                {
                    b.HasOne("MementoMori.API.Entities.Ticket", "Ticket")
                        .WithMany("TicketComments")
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MementoMori.API.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Ticket");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MementoMori.API.Entities.Deck", b =>
                {
                    b.Navigation("Cards");
                });

            modelBuilder.Entity("MementoMori.API.Entities.Ticket", b =>
                {
                    b.Navigation("TicketComments");
                });
#pragma warning restore 612, 618
        }
    }
}
