using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using VoteMyst.Database.Models;

namespace VoteMyst.ViewComponents
{
    public class AvatarViewComponent : ViewComponent
    {
        private readonly AvatarHelper _avatarHelper;

        public AvatarViewComponent(AvatarHelper avatarHelper)
        {
            _avatarHelper = avatarHelper;
        }

        public Task<IViewComponentResult> InvokeAsync(UserData user) 
        {
            ViewBag.AvatarUrl = _avatarHelper.GetRelativeAvatarUrl(user, out string initials);
            ViewBag.AvatarInitials = initials;

            return Task.FromResult<IViewComponentResult>(View());
        }
    }
}