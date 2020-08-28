using System;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using VoteMyst.Database;

namespace VoteMyst.Controllers.Api
{
    /// <summary>
    /// The base class for VoteMyst API controllers.
    /// </summary>
    [ApiController]
    public abstract class VoteMystApiController : ControllerBase
    {
        protected UserProfileBuilder ProfileBuilder { get; }
        protected DatabaseHelperProvider DatabaseHelpers { get; }
        protected IWebHostEnvironment Environment { get; }

        public VoteMystApiController(IServiceProvider serviceProvider)
        {
            ProfileBuilder = serviceProvider.GetService<UserProfileBuilder>();
            DatabaseHelpers = serviceProvider.GetService<DatabaseHelperProvider>();
            Environment = serviceProvider.GetService<IWebHostEnvironment>();
        }

        /// <summary>
        /// Returns the currently authenticated user. Returns a guest user if the no one is logged in.
        /// </summary>
        protected UserAccount GetCurrentUser()
            => ProfileBuilder.FromPrincipal(User);
    }
}
