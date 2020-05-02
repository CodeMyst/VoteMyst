using System.Linq;
using VoteMyst.Database.Models;

namespace VoteMyst.Database 
{
    public class AuthorizationHelper 
    {
        private readonly VoteMystContext context;
        private readonly UserDataHelper userHelper;

        public AuthorizationHelper(VoteMystContext context, UserDataHelper userHelper) 
        {
            this.context = context;
            this.userHelper = userHelper;
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

        public UserData GetAuthorizedUser(ServiceType serviceType, string serviceUserId)
            => context.Authorizations
                .Where(auth => auth.ServiceType == serviceType)
                .Where(auth => auth.ServiceUserId == serviceUserId)
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
    }
}