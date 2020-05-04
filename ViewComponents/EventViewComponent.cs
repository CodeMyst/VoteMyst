using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using VoteMyst.Database.Models;

namespace VoteMyst.ViewComponents
{
    public class EventViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(Event ev) 
        {
            ViewBag.EventName = ev.Title;
            ViewBag.Description = ev.Description;
            ViewBag.StartDate = ev.StartDate;
            ViewBag.SubmitEndDate = ev.EndDate;
            ViewBag.VoteEndDate = ev.VoteEndDate;
            ViewBag.Finished = ev.VoteEndDate < DateTime.UtcNow;

            if (ViewBag.Finished) 
            {
                ViewBag.Winners = new string [] 
                {
                    "Aqua",
                    "Yilly",
                    "Cide",
                    "Job",
                    "Dutchy",
                    "Huccy"
                };
            };
            
            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}