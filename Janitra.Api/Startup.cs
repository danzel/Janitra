using System.Text;
using Janitra.Api.Controllers;
using Janitra.Data;
using Janitra.Data.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Janitra.Api
{
	public class Startup
	{
		public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			Configuration = configuration;
			Environment = env;
		}

		public IConfiguration Configuration { get; }
		public IHostingEnvironment Environment { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<JanitraContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Janitra")));
			services.Add(ServiceDescriptor.Transient<UserRepository, UserRepository>());
			services.Configure<OAuthControllerOptions>(Configuration.GetSection("OAuth"));

			services.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(cfg =>
				{
					//if (Environment.IsDevelopment())
					//	cfg.RequireHttpsMetadata = false;
					//cfg.SaveToken = true;

					cfg.TokenValidationParameters = new TokenValidationParameters
					{
						ValidIssuer = Configuration["OAuth:JwtIssuer"],
						ValidAudience = Configuration["OAuth:JwtIssuer"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["OAuth:JwtKey"])),
						NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
					};
				});
			services.AddDistributedMemoryCache();
			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseAuthentication();
			app.UseMvc();
		}
	}
}