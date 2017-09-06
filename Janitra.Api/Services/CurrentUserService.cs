using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Janitra.Api.Services
{
	public class CurrentUser
	{
		public User User { get; }

		public CurrentUser()
		{
		}

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
			services.Add(ServiceDescriptor.Scoped(s =>
			{
				//if (s.GetService<IHttpContextAccessor>() == null)
				//	return new CurrentUser(null);

				var janitraContext = s.GetService<JanitraContext>();

				var context = s.GetService<IHttpContextAccessor>().HttpContext;
				if (context != null && context.User.Identity.IsAuthenticated)
					return new CurrentUser(janitraContext.Users.Single(u => u.UserId == int.Parse(context.User.Identity.Name)));
				return new CurrentUser();
			}));
		}
	}
}