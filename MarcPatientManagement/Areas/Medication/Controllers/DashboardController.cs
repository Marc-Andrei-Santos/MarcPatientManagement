using BLL;
using System.Web.Mvc;

namespace AL.Areas.Medication.Controllers
{
    public class DashboardController : Controller
    {
        private readonly MedicationBLL _bll = new MedicationBLL();

        public ActionResult Index()
        {
            ViewBag.ActiveTab = "Dashboard";
            var model = _bll.GetAll(); 
            return View(model);
        }
    }
}
