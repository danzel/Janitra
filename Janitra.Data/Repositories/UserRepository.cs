using System.Threading.Tasks;
using Janitra.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Janitra.Data.Repositories
{
    public class UserRepository
    {
	    private readonly JanitraContext _context;
	    public UserRepository(JanitraContext context)
	    {
		    _context = context;
	    }

		/// <summary>
		/// Get the user matching the given (oAuthProvider + oAuthId). Creates them if they don't exist.
		/// This method saves changes to the database (if any).
		/// </summary>
		public async Task<User> GetOrCreateUser(string oAuthProvider, string oAuthId, string oAuthName)
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.OAuthProvider == oAuthProvider && u.OAuthId == oAuthId);

			if (user == null)
			{
				user = new User { OAuthProvider = oAuthProvider, OAuthId = oAuthId, OAuthName = oAuthName, UserLevel = UserLevel.Default };
				_context.Add(user);
				await _context.SaveChangesAsync();
			}
			else
			{
				if (user.OAuthName != oAuthName)
				{
					user.OAuthName = oAuthName;
					await _context.SaveChangesAsync();
				}
			}

			return user;
		}
    }
}
