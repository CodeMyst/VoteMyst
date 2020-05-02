using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;

using VoteMyst.Database.Models;

namespace VoteMyst.ViewComponents
{
    public class EntryViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(Entry entry, bool displayUser = false) 
        {
            // TODO: Display the user that posted the entry
            ViewBag.DisplayUser = displayUser;
            ViewBag.Username = "TODO:Username";
            ViewBag.Avatar = "https://via.placeholder.com/64";
            ViewBag.Entry = entry;
            
            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}