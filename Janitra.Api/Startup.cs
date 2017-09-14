using System;
using System.IO;
using System.Reflection;
using Janitra.Controllers;
using Janitra.Services;
using Janitra.Data;
using Janitra.Data.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace Janitra
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
			services.AddCurrentUserService();

			services.AddSingleton<IFileStorageService>(new AzureBlobStorageService(Configuration.GetConnectionString("AzureStorage")));
			//services.AddSingleton<IFileStorageService, NullFileStorageService>();

			services.Configure<OAuthControllerOptions>(Configuration.GetSection("OAuth"));

			services.AddSwaggerGen(c =>
			{
				var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
				var commentsFileName = Assembly.GetExecutingAssembly().GetName().Name + ".XML";
				var commentsFile = Path.Combine(baseDirectory, commentsFileName);

				c.AddSecurityDefinition("Bearer", new ApiKeyScheme { Type = "apiKey", Name = "Authorization", In = "header", Description = "Call /oauth/github then put the token in here as \"Bearer .......\"" });
				c.DescribeAllEnumsAsStrings();
				c.IncludeXmlComments(commentsFile);
				c.SwaggerDoc("v1", new Info { Title = "Janitra API", Version = "v1" });
			});

			services.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
				})
				.AddCookie();
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
			app.UseMvc(routes => { routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"); });

			app.UseSwagger();
			app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
		}
	}
}