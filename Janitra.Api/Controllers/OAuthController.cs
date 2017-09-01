using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Janitra.Data.Models;
using Janitra.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Janitra.Api.Controllers
{
	public class OAuthControllerOptions
	{
		public string GithubClientId { get; set; }
		public string GithubClientSecret { get; set; }

		/// <summary>
		/// Where to redirect the user to after authenticating them, will have their JWT appended to the end
		/// </summary>
		public string RedirectUrl { get; set; }

		public string JwtIssuer { get; set; }
		public string JwtKey { get; set; }
		public double JwtLifetimeDays { get; set; }
	}

	[Route("oauth")]
	public class OAuthController : Controller
	{
		//https://developer.github.com/apps/building-integrations/setting-up-and-registering-oauth-apps/about-authorization-options-for-oauth-apps/

		private readonly IOptions<OAuthControllerOptions> _options;
		private readonly IDistributedCache _cache;
		private readonly UserRepository _userRepository;
		private readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

		public OAuthController(IOptions<OAuthControllerOptions> options, IDistributedCache cache, UserRepository userRepository)
		{
			_options = options;
			_cache = cache;
			_userRepository = userRepository;
		}

		[HttpGet("github")]
		public async Task<IActionResult> Github()
		{
			var bytes = new byte[16];
			_rng.GetBytes(bytes);
			var stateBytes = SHA256.Create().ComputeHash(bytes);
			var state = BitConverter.ToString(stateBytes);

			await _cache.SetAsync("github:state:" + state, new byte[0], new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

			return Redirect("https://github.com/login/oauth/authorize?client_id=" + _options.Value.GithubClientId + "&state=" + state);
		}

		[HttpGet("github/callback")]
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

			var jwt = IssueJwt(user);

			return Redirect(_options.Value.RedirectUrl + jwt);
		}

		[Authorize]
		[HttpGet("test")]
		public void TestAuth()
		{
			var user = HttpContext.User;
		}

		private string IssueJwt(User user)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Value.JwtKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};
			//TODO: Add more claims based on their user level

			var token = new JwtSecurityToken(
				issuer: _options.Value.JwtIssuer,
				audience: _options.Value.JwtIssuer,
				claims: claims,
				expires: DateTime.Now.AddDays(_options.Value.JwtLifetimeDays),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
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