using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

using VoteMyst.Database;
using VoteMyst.Database.Models;

namespace VoteMyst.ViewComponents
{
    public class EntryViewComponent : ViewComponent
    {
        private readonly UserProfileBuilder _builder;
        private readonly DatabaseHelperProvider _helpers;

        public EntryViewComponent(UserProfileBuilder builder, DatabaseHelperProvider helpers)
        {
            _builder = builder;
            _helpers = helpers;
        }

        public Task<IViewComponentResult> InvokeAsync(Entry entry, bool displayUser = false) 
        {
            UserData user = _builder.FromPrincipal(UserClaimsPrincipal);
            UserData author = _helpers.Users.GetUser(entry.UserId);

            // TODO: Display the user that posted the entry
            ViewBag.DisplayUser = displayUser;
            ViewBag.User = author;
            ViewBag.Entry = entry;
            ViewBag.HasVoted = _helpers.Votes.GetVoteByUserOnEntry(user, entry) != null;
            
            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}