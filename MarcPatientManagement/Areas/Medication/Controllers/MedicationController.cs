using BLL;
using EL;
using UL;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AL.Areas.Medication.Controllers
{
    public class MedicationController : Controller
    {
        private readonly MedicationBLL _bll = new MedicationBLL();

        // GET: Medication/Index
        public ActionResult Index(string patientFilter, string drugFilter, decimal? dosageFilter, DateTime? modifiedDateFilter,
                                  int page = 1, int pageSize = 10)
        {
            ViewBag.ActiveTab = "Medication";
            ViewBag.PatientFilter = patientFilter;
            ViewBag.DrugFilter = drugFilter;
            ViewBag.DosageFilter = dosageFilter;
            ViewBag.ModifiedDateFilter = modifiedDateFilter?.ToString("yyyy-MM-dd");
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            var allData = _bll.GetFiltered(patientFilter, drugFilter, dosageFilter, modifiedDateFilter);
            ViewBag.TotalRecords = allData.Count();

            return View(allData.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }

        // GET: Medication/Create
        public ActionResult Create() => View(new MedicationEntity { ModifiedDate = DateTime.Now });

        // POST: Medication/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MedicationEntity entity)
        {
            entity.ModifiedDate = DateTime.Now;
            return HandleSave(() => _bll.Insert(entity), entity, "success");
        }

        // GET: Medication/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var entity = _bll.GetById(id.Value);
            if (entity == null) return HttpNotFound();
            return View(entity);
        }

        // POST: Medication/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MedicationEntity entity) => HandleSave(() => _bll.Update(entity), entity, "warning");


        // POST: Medication/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var result = _bll.Delete(id);
            TempData["ToastMessage"] = string.Join(", ", result.MessageList);
            TempData["ToastType"] = "danger";
            return RedirectToAction("Index");
        }

        // POST: Medication/CheckDuplicate
        [HttpPost]
        public JsonResult CheckDuplicate(string patient, string drug, DateTime date, decimal? dosage, int? id = null)
        {
            var allRecords = _bll.GetAll();
            if (id.HasValue) allRecords = allRecords.Where(x => x.Id != id.Value).ToList();

            if (id.HasValue)
            {
                var original = _bll.GetById(id.Value);
                if (original != null &&
                    original.Patient == patient &&
                    original.Drug == drug &&
                    original.Dosage == dosage &&
                    original.ModifiedDate.Date == date.Date)
                    return Json(new { isDuplicate = true, message = MessageUtil.NoChanges, isValid = false });
            }

            if (allRecords.Any(x => x.Patient == patient && x.Drug == drug && x.Dosage == dosage && x.ModifiedDate.Date == date.Date))
                return Json(new { isDuplicate = true, message = MessageUtil.RecordAlreadyExists, isValid = false });

            if (allRecords.Any(x => x.Patient == patient && x.Drug == drug && x.ModifiedDate.Date == date.Date))
                return Json(new { isDuplicate = true, message = MessageUtil.DuplicateRecord, isValid = false });

            return Json(new { isDuplicate = false, isValid = true });
        }

        // Helper Handle Save 
        private ActionResult HandleSave(Func<MedicationEntity> saveAction, MedicationEntity entity, string successType)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastMessage"] = "Validation error.";
                TempData["ToastType"] = "danger";
                return View(entity);
            }

            var result = saveAction();

            if (result.IsSuccess)
            {
                TempData["ToastMessage"] = result.MessageList[0];
                TempData["ToastType"] = successType;
                return RedirectToAction("Index");
            }

            foreach (var msg in result.MessageList)
            {
                if (msg == MessageUtil.InvalidPatient) ModelState.AddModelError("Patient", msg);
                else if (msg == MessageUtil.InvalidDrug) ModelState.AddModelError("Drug", msg);
                else if (msg == MessageUtil.InvalidDosage) ModelState.AddModelError("Dosage", msg);
                else if (msg == MessageUtil.DuplicateRecord || msg == MessageUtil.RecordAlreadyExists) ModelState.AddModelError("Drug", msg);
                else if (msg == MessageUtil.NoChanges) ModelState.AddModelError("", msg);
            }

            TempData["ToastMessage"] = string.Join(", ", result.MessageList);
            TempData["ToastType"] = "danger";
            return View(entity);
        }
    }
}
