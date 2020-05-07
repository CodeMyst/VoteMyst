using System.Linq;
using System.Security.Cryptography;
using System.Text;
using VoteMyst.Database.Models;

namespace VoteMyst.Database 
{
    public class AuthorizationHelper 
    {
        private readonly VoteMystContext context;

        public AuthorizationHelper(VoteMystContext context) 
        {
            this.context = context;
        }

        public Authorization AddAuthorizedUser(int userId, string serviceUserId, ServiceType serviceType)
        {
            Authorization authorization = new Authorization() 
            {
                UserId = userId,
                ServiceUserId = ComputeSha256Hash(serviceUserId),
                ServiceType = serviceType,
                Valid = true
            };

            context.Authorizations.Add(authorization);

            context.SaveChanges();

            return authorization;
        }

        public UserData GetAuthorizedUser(ServiceType serviceType, string serviceUserId)
            => context.Authorizations
                .Where(auth => auth.ServiceType == serviceType)
                .Where(auth => auth.ServiceUserId == ComputeSha256Hash(serviceUserId))
                .Join(context.UserData,
                    auth => auth.UserId,
                    user => user.UserId,
                    (auth, user) => user)
                .FirstOrDefault();

        public Authorization GetAuthorization(int authId)
            => context.Authorizations
                .FirstOrDefault(x => x.AuthId == authId);

        public Authorization[] GetAllAuthorizationsOfUser(int userId)
            => context.Authorizations
                .Where(x => x.UserId == userId)
                .ToArray();


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