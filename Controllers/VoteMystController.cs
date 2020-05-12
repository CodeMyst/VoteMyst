using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using VoteMyst.Database;
using VoteMyst.Database.Models;
using VoteMyst.PermissionSystem;
using VoteMyst.Controllers.Validation;

namespace VoteMyst.Controllers
{
    /// <summary>
    /// The base class for VoteMyst controllers.
    /// </summary>
    public abstract class VoteMystController : Controller
    {
        protected UserProfileBuilder ProfileBuilder { get; }
        protected DatabaseHelperProvider DatabaseHelpers { get; }
        protected PathBuilder PathBuilder { get; }

        public VoteMystController(IServiceProvider serviceProvider)
        {
            ProfileBuilder = serviceProvider.GetService<UserProfileBuilder>();
            PathBuilder = serviceProvider.GetService<PathBuilder>();
            DatabaseHelpers = serviceProvider.GetService<DatabaseHelperProvider>();
        }

        /// <summary>
        /// Returns the currently authenticated user. Returns a guest user if the no one is logged in.
        /// </summary>
        public UserData GetCurrentUser()
            => ProfileBuilder.FromPrincipal(User);

        /// <summary>
        /// Sets up a <see cref="VoteMyst.Controllers.Validation.RequestValidator" /> that can used to validate data.
        /// <para>Inside the validation action the <see cref="VoteMyst.Controllers.Validation.RequestValidator.Verify(bool, string)" /> method should be used.</para>
        /// </summary>
        protected RequestValidator SetupValidation(Action<RequestValidator> validation)
        {
            return new RequestValidator(validation);
        }

        /// <summary>
        /// Injects the result of a <see cref="VoteMyst.Controllers.Validation.ValidationException" /> into the view under "ValidationMessage".
        /// </summary>
        protected void InjectResultIntoView(ValidationException validationException) 
        {
            ViewData["ValidationMessage"] = validationException.Message;
        }
    }
}