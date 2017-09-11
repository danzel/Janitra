using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Janitra.Api.Services;
using Janitra.Data.Models;
using Janitra.Data.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Janitra.Api.Controllers
{
	public class OAuthControllerOptions
	{
		public string GithubClientId { get; set; }
		public string GithubClientSecret { get; set; }
	}

	/// <summary>
	/// Responsible for authentication
	/// </summary>
	[ApiExplorerSettings(IgnoreApi = true)]
	public class AuthController : Controller
	{
		//https://developer.github.com/apps/building-integrations/setting-up-and-registering-oauth-apps/about-authorization-options-for-oauth-apps/

		private readonly IOptions<OAuthControllerOptions> _options;
		private readonly IDistributedCache _cache;
		private readonly UserRepository _userRepository;
		private readonly CurrentUser _currentUser;

		/// <summary>
		/// Constructor
		/// </summary>
		public AuthController(IOptions<OAuthControllerOptions> options, IDistributedCache cache, UserRepository userRepository, CurrentUser currentUser)
		{
			_options = options;
			_cache = cache;
			_userRepository = userRepository;
			_currentUser = currentUser;
		}

		public IActionResult Index()
		{
			return View(_currentUser);
		}

		public IActionResult LogOut()
		{
			HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

			return Redirect("/");
		}

		/// <summary>
		/// Begin an OAuth authentication with github
		/// </summary>
		public async Task<IActionResult> Github()
		{
			var state = SecureRandomStringGenerator.Generate();

			await _cache.SetAsync("github:state:" + state, new byte[0], new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

			return Redirect("https://github.com/login/oauth/authorize?client_id=" + _options.Value.GithubClientId + "&state=" + state);
		}

		/// <summary>
		/// Finish an OAuth authentication with github. Redirects you to /
		/// </summary>
		[HttpGet("auth/github/callback")]
		[ProducesResponseType(StatusCodes.Status302Found)]
		[ProducesResponseType(StatusCodes.Status403Forbidden)]
		public async Task<IActionResult> GithubCallback([FromQuery] string code, [FromQuery] string state)
		{
			//Validate state
			var stateByte = await _cache.GetAsync("github:state:" + state);
			if (stateByte == null)
				return Forbid(); //TODO: Nicer error handling
			await _cache.RemoveAsync("github:state:" + state);

			//Post to github to verify the code
			var client = new HttpClient();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			var res = await client.PostAsync("https://github.com/login/oauth/access_token", new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("client_id", _options.Value.GithubClientId),
				new KeyValuePair<string, string>("client_secret", _options.Value.GithubClientSecret),
				new KeyValuePair<string, string>("code", code)
			}));
			var result = JsonConvert.DeserializeObject<OAuthAccessToken>(await res.Content.ReadAsStringAsync());
			if (result.access_token == null)
				return Forbid(); //TODO: Nicer error handling

			//get the user details
			client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("JanitraApi", "1.0"));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", result.access_token);
			res = await client.GetAsync("https://api.github.com/user");
			var githubUser = JsonConvert.DeserializeObject<GithubUser>(await res.Content.ReadAsStringAsync());
			if (githubUser.login == null)
				return Forbid(); //TODO: Nicer error handling

			var user = await _userRepository.GetOrCreateUser("github", githubUser.id.ToString(), githubUser.login);

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.UserId.ToString())
			};
			if (user.UserLevel == UserLevel.Developer)
				claims.Add(new Claim(ClaimTypes.Role, "Developer"));
			var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

			return Redirect("/");
		}

		private class OAuthAccessToken
		{
			public string access_token { get; set; }
			public string scope { get; set; }
			public string token_type { get; set; }
		}

		private class GithubUser
		{
			public string login { get; set; }
			public long id { get; set; }
			public string name { get; set; }
		}
	}
}