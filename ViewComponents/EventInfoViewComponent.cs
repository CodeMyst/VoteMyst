using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using VoteMyst.Database.Models;

namespace VoteMyst.ViewComponents
{
    public class EventInfoViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(Event ev) 
        {
            ViewBag.Event = ev;
            ViewBag.Finished = ev.VoteEndDate < DateTime.UtcNow;
            
            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}