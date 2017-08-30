using System;
using System.Collections.Generic;
using System.Text;
using Janitra.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Janitra.Data
{
	public class JanitraContext : DbContext
	{
		public DbSet<User> Users { get; set; }

		/// <inheritdoc />
		public JanitraContext(DbContextOptions<JanitraContext> options)
			: base(options)
		{
		}
	}
}