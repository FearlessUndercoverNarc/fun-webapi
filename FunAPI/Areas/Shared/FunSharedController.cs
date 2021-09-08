using FunAPI.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FunAPI.Areas.Shared
{
    [Route("/shared/[controller]/[action]")]
    [ValidateModelFilter]
    [ResponseCache(NoStore = true, Duration = 0)]
    public class FunSharedController : Controller
    {
    }
}