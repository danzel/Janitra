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
		public JanitraBot JanitraBot { get; }

		public CurrentUser()
		{
		}

		public CurrentUser(User user)
		{
			User = user;
		}

		public CurrentUser(JanitraBot janitraBot)
		{
			JanitraBot = janitraBot;
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
				{
					if (context.User.IsInRole("JanitraBot"))
						return new CurrentUser(janitraContext.JanitraBots.Single(u => u.JanitraBotId == int.Parse(context.User.Identity.Name)));
					return new CurrentUser(janitraContext.Users.Single(u => u.UserId == int.Parse(context.User.Identity.Name)));
				}
				return new CurrentUser();
			}));
		}
	}
}