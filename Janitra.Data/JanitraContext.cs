using Janitra.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Janitra.Data
{
	public class JanitraContext : DbContext
	{
		public DbSet<CitraBuild> CitraBuilds { get; set; }
		public DbSet<JanitraBot> JanitraBots { get; set; }
		public DbSet<TestDefinition> TestDefinitions { get; set; }
		public DbSet<TestResult> TestResults { get; set; }
		public DbSet<TestRom> TestRoms { get; set; }
		public DbSet<User> Users { get; set; }

		/// <inheritdoc />
		public JanitraContext(DbContextOptions<JanitraContext> options)
			: base(options)
		{
		}

		/// <inheritdoc />
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//FK CitraBuild* -> AddedByUser
			modelBuilder.Entity<CitraBuild>()
				.HasOne(cb => cb.AddedByUser)
				.WithMany(u => u.AddedCitraBuilds)
				.HasForeignKey(cb => cb.AddedByUserId)
				.OnDelete(DeleteBehavior.Restrict);

			//UK CitraBuild.GitHash
			modelBuilder.Entity<CitraBuild>()
				.HasIndex(cb => cb.GitHash)
				.IsUnique();


			//FK JanitraBot* -> AddedByUser
			modelBuilder.Entity<JanitraBot>()
				.HasOne(jb => jb.AddedByUser)
				.WithMany(u => u.AddedJanitraBots)
				.HasForeignKey(jb => jb.AddedByUserId)
				.OnDelete(DeleteBehavior.Restrict);


			//FK TestDefinition* -> AddedByUser
			modelBuilder.Entity<TestDefinition>()
				.HasOne(td => td.AddedByUser)
				.WithMany(u => u.AddedTestDefinitions)
				.HasForeignKey(td => td.AddedByUserId)
				.OnDelete(DeleteBehavior.Restrict);

			//FK TestDefinition* -> TestRom
			modelBuilder.Entity<TestDefinition>()
				.HasOne(td => td.TestRom)
				.WithMany(tr => tr.UsedByTestDefinitions)
				.HasForeignKey(td => td.TestRomId);

			//UK TestDefinition.TestName
			modelBuilder.Entity<TestDefinition>()
				.HasIndex(td => td.TestName)
				.IsUnique();


			//FK TestResult* -> CitraBuild
			modelBuilder.Entity<TestResult>()
				.HasOne(tr => tr.CitraBuild)
				.WithMany(cb => cb.TestResults)
				.HasForeignKey(tr => tr.CitraBuildId);

			//FK TestResult* -> JanitraBot
			modelBuilder.Entity<TestResult>()
				.HasOne(tr => tr.JanitraBot)
				.WithMany(cb => cb.TestResults)
				.HasForeignKey(tr => tr.JanitraBotId);

			//FK TestResult* -> TestDefinition
			modelBuilder.Entity<TestResult>()
				.HasOne(tr => tr.TestDefinition)
				.WithMany(cb => cb.TestResults)
				.HasForeignKey(tr => tr.TestDefinitionId);

			//UK TestResult.[CitraBuildId + TestDefinitionId + JanitraBotId]
			modelBuilder.Entity<TestResult>()
				.HasIndex(tr => new { tr.CitraBuildId, tr.TestDefinitionId, tr.JanitraBotId })
				.IsUnique();

			//FK TestRom* -> AddedByUser
			modelBuilder.Entity<TestRom>()
				.HasOne(td => td.AddedByUser)
				.WithMany(u => u.AddedTestRoms)
				.HasForeignKey(td => td.AddedByUserId)
				.OnDelete(DeleteBehavior.Restrict);


			//UK User.[OAuthProvider + OAuthId]
			modelBuilder.Entity<User>()
				.HasIndex(u => new { u.OAuthProvider, u.OAuthId })
				.IsUnique();
		}
	}
}