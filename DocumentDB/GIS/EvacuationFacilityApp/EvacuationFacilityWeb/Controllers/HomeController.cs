using System.Web.Mvc;

using EvacuationFacilityLib.Repositories;

namespace EvacuationFacilityWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(double latitude, double longitude, int distance)
        {
            EvacuationFacilityInfoRepository.Initialize();
            ViewBag.EvacuationFacilities = EvacuationFacilityInfoRepository.Search(longitude, latitude, distance);

            return View("result");
        }
    }
}