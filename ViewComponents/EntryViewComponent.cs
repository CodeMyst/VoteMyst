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
        private readonly DatabaseHelperProvider _helpers;

        public EntryViewComponent(DatabaseHelperProvider helpers)
        {
            _helpers = helpers;
        }

        public Task<IViewComponentResult> InvokeAsync(Entry entry, bool displayUser = false) 
        {
            UserData author = _helpers.Users.GetUser(entry.UserId);

            // TODO: Display the user that posted the entry
            ViewBag.DisplayUser = displayUser;
            ViewBag.User = author;
            ViewBag.Entry = entry;
            
            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}