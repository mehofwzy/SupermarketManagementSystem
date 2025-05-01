using Microsoft.AspNetCore.Mvc;
using SM_Web.Services;

namespace SM_Web.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly JwtHelperService _jwtHelperService;

        public SidebarViewComponent(JwtHelperService jwtHelperService)
        {
            _jwtHelperService = jwtHelperService;
        }

        public IViewComponentResult Invoke()
        {
            //var permissions = _jwtHelperService.GetPermissions();
            //return View("Default", permissions);
            return View("Default");
        }
    }

}
