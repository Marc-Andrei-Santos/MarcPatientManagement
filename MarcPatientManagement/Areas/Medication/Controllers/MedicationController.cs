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

            var totalRecords = allData.Count();
            var pagedData = allData.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.TotalRecords = totalRecords;

            return View(pagedData);
        }


        // GET: Medication/Create
        public ActionResult Create()
        {
            var model = new MedicationEntity
            {
                ModifiedDate = DateTime.Now
            };
            return View(model);
        }

        // POST: Medication/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MedicationEntity entity)
        {
            if (ModelState.IsValid)
            {
                entity.ModifiedDate = DateTime.Now;
                var existing = _bll.GetFiltered(entity.Patient, entity.Drug, null, entity.ModifiedDate.Date);
                if (existing.Any())
                {
                    ModelState.AddModelError("Drug", MessageUtil.DuplicateRecord);
                    TempData["ToastMessage"] = MessageUtil.DuplicateRecord;
                    TempData["ToastType"] = "danger";
                    return View(entity);
                }

                var result = _bll.Insert(entity);

                if (result.IsSuccess)
                {
                    TempData["ToastMessage"] = result.MessageList[0];
                    TempData["ToastType"] = "success"; 
                    return RedirectToAction("Index");
                }

                foreach (var msg in result.MessageList)
                {
                    if (msg == MessageUtil.InvalidPatient)
                        ModelState.AddModelError("Patient", msg);
                    if (msg == MessageUtil.InvalidDrug)
                        ModelState.AddModelError("Drug", msg);
                    if (msg == MessageUtil.InvalidDosage)
                        ModelState.AddModelError("Dosage", msg);
                }

                TempData["ToastMessage"] = string.Join(", ", result.MessageList);
                TempData["ToastType"] = "danger";
            }
            else
            {
                TempData["ToastMessage"] = "Validation error.";
                TempData["ToastType"] = "danger";
            }

            return View(entity);
        }

        // GET: Medication/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var entity = _bll.GetById(id.Value);
            if (entity == null)
                return HttpNotFound();

            return View(entity);
        }

        // POST: Medication/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MedicationEntity entity)
        {
            if (ModelState.IsValid)
            {
                var existing = _bll.GetFiltered(entity.Patient, entity.Drug, null, entity.ModifiedDate.Date)
                                  .Where(x => x.Id != entity.Id)
                                  .ToList();

                if (existing.Any())
                {
                    ModelState.AddModelError("Drug", MessageUtil.DuplicateRecord);
                    TempData["ToastMessage"] = MessageUtil.DuplicateRecord;
                    TempData["ToastType"] = "danger";
                    return View(entity);
                }

                var result = _bll.Update(entity);

                if (result.IsSuccess)
                {
                    TempData["ToastMessage"] = result.MessageList[0];
                    TempData["ToastType"] = "warning";
                    return RedirectToAction("Index");
                }

                foreach (var msg in result.MessageList)
                {
                    if (msg == MessageUtil.InvalidPatient)
                        ModelState.AddModelError("Patient", msg);
                    if (msg == MessageUtil.InvalidDrug)
                        ModelState.AddModelError("Drug", msg);
                    if (msg == MessageUtil.InvalidDosage || msg == MessageUtil.NegativeDosage)
                        ModelState.AddModelError("Dosage", msg);
                }

                TempData["ToastMessage"] = string.Join(", ", result.MessageList);
                TempData["ToastType"] = "danger";
            }
            else
            {
                TempData["ToastMessage"] = "Validation error.";
                TempData["ToastType"] = "danger";
            }

            return View(entity);
        }

        // POST: Medication/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var result = _bll.Delete(id);

            TempData["ToastMessage"] = string.Join(", ", result.MessageList);
            TempData["ToastType"] = result.IsSuccess ? "danger" : "danger"; // ✅ always red for delete

            return RedirectToAction("Index");
        }

        // Check duplicate
        [HttpPost]
        public JsonResult CheckDuplicate(string patient, string drug, DateTime date, int? id = null)
        {
            var existing = _bll.GetFiltered(patient, drug, null, date.Date);

            if (id.HasValue)
                existing = existing.Where(x => x.Id != id.Value).ToList();

            if (existing.Any())
                return Json(new { isDuplicate = true, message = MessageUtil.DuplicateRecord });

            return Json(new { isDuplicate = false });
        }
    }
}
