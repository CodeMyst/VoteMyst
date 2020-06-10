using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace VoteMyst.Database 
{
    /// <summary>
    /// Provides utility to handle <see cref="Authorization"/>s.
    /// </summary>
    public class AuthorizationHelper 
    {
        private readonly VoteMystContext context;

        public AuthorizationHelper(VoteMystContext context) 
        {
            this.context = context;
        }

        /// <summary>
        /// Registers a user via an authorization service.
        /// </summary>
        public Authorization AddAuthorizedUser(UserAccount user, string serviceUserId, Service serviceType)
        {
            Authorization authorization = new Authorization() 
            {
                User = user,
                ServiceUserID = ComputeSha256Hash(serviceUserId),
                Service = serviceType,
                Valid = true
            };

            context.Authorizations.Add(authorization);
            context.SaveChanges();

            return authorization;
        }

        /// <summary>
        /// Retrieves a user via a hashed user ID from a service.
        /// </summary>
        public UserAccount GetAuthorizedUser(Service serviceType, string serviceUserId)
            => context.Authorizations
                .Where(auth => auth.Service == serviceType)
                .Where(auth => auth.ServiceUserID == ComputeSha256Hash(serviceUserId))
                .Join(context.UserAccounts,
                    auth => auth.User.ID,
                    user => user.ID,
                    (auth, user) => user)
                .FirstOrDefault();

        /// <summary>
        /// Hashes the specified string.
        /// </summary>
        private string ComputeSha256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] bytes = sha256.ComputeHash(inputBytes);

                StringBuilder builder = new StringBuilder();  
                
                for (int i = 0; i < bytes.Length; i++)    
                    builder.Append(bytes[i].ToString("x2"));  

                return builder.ToString();
            }
        }
    }
}