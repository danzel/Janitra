using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Janitra.Data.Repositories;
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

		/// <summary>
		/// Where to redirect the user to after authenticating them, will have their JWT appended to the end
		/// </summary>
		public string RedirectUrl { get; set; }
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
			//TODO: Validate state
			var stateByte = await _cache.GetAsync("github:state:" + state);
			if (stateByte == null)
				return Forbid(); //TODO: Reason or redirect or something instead
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
				return Forbid();

			//get the user details
			client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("JanitraApi", "1.0"));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", result.access_token);
			res = await client.GetAsync("https://api.github.com/user");
			var githubUser = JsonConvert.DeserializeObject<GithubUser>(await res.Content.ReadAsStringAsync());
			if (githubUser.login == null)
				return Forbid();

			var user = await _userRepository.GetOrCreateUser("github", githubUser.id.ToString(), githubUser.login);

			//TODO: JWT
			var jwt = "etsektertkesrter==";

			return Redirect(_options.Value.RedirectUrl + jwt);
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
