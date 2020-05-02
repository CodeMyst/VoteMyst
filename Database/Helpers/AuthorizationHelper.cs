using System.Linq;
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
                ServiceUserId = serviceUserId,
                ServiceType = serviceType
            };

            context.Authorizations.Add(authorization);

            context.SaveChanges();

            return authorization;
        }

        public Authorization GetAuthorization(int authId)
            => context.Authorizations
                .First(x => x.AuthId == authId);

        public Authorization[] GetAllAuthorizationsOfUser(int userId)
            => context.Authorizations
                .Where(x => x.UserId == userId)
                .ToArray();
    }
}