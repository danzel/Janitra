using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Janitra.Api.Services
{
	public class CurrentUser
	{
		public User User { get; }

		public CurrentUser(User user)
		{
			User = user;
		}
	}

    public static class CurrentUserService
	{
		public static void AddCurrentUserService(this IServiceCollection services)
		{
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.Add(ServiceDescriptor.Scoped(async s =>
			{
				//if (s.GetService<IHttpContextAccessor>() == null)
				//	return new CurrentUser(null);

				var janitraContext = s.GetService<JanitraContext>();

				var context = s.GetService<IHttpContextAccessor>().HttpContext;
				if (context != null && context.User.Identity.IsAuthenticated)
					return new CurrentUser(await janitraContext.Users.SingleAsync(u => u.UserId == int.Parse(context.User.Identity.Name)));
				return new CurrentUser(null);
			}));
		}
	}
}
