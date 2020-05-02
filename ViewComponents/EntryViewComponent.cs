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
        public UserDataHelper _userHelper;

        public EntryViewComponent(UserDataHelper userHelper)
        {
            _userHelper = userHelper;
        }

        public Task<IViewComponentResult> InvokeAsync(Entry entry, bool displayUser = false) 
        {
            UserData author = _userHelper.GetUser(entry.UserId);

            // TODO: Display the user that posted the entry
            ViewBag.DisplayUser = displayUser;
            ViewBag.Username = author.Username;
            ViewBag.Avatar = $"/assets/avatars/{author.DisplayId}.png";
            ViewBag.Entry = entry;
            
            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}