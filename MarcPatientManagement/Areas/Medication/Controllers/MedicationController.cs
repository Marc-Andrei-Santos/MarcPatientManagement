using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BLL;
using EL;
using UL;

namespace AL.Areas.Medication.Controllers
{
    public class MedicationController : Controller
    {
        private readonly MedicationBLL _bll = new MedicationBLL();

        public ActionResult Index()
        {
            ViewBag.ActiveTab = "Medication";

            var allData = _bll.GetAll()
                              .OrderByDescending(x => x.ModifiedDate)
                              .ToList();
            return View(allData);
        }


        // GET: Create
        public ActionResult Create()
        {
            if (Request.Url.Segments.Length > 4) 
            {
                return RedirectToAction("Index");
            }

            return View(new MedicationEntity { ModifiedDate = DateTime.Now });
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MedicationEntity entity)
        {
            entity.ModifiedDate = DateTime.Now;
            return HandleSubmit(() => _bll.Insert(entity), entity, "success");
        }


        // GET: Edit/5
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue || Request.Url.Segments.Length > 5)
            {
                return RedirectToAction("Index");
            }

            var entity = _bll.GetById(id.Value);
            if (entity == null)
                return RedirectToAction("Index");

            return View(entity);
        }


        // POST: Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MedicationEntity entity)
        {
            var existing = _bll.GetById(entity.Id);
            if (existing == null)
                return RedirectToAction("NotFound", "Error", new { area = "" });

            return HandleSubmit(() => _bll.Update(entity), entity, "warning");
        }


        // POST: Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var existing = _bll.GetById(id);
            if (existing == null)
                return RedirectToAction("NotFound", "Error", new { area = "" });

            var result = _bll.Delete(id);
            TempData["ToastMessage"] = string.Join(", ", result.MessageList);
            TempData["ToastType"] = "danger";
            return RedirectToAction("Index");
        }

        // POST: Check Duplicates
        public JsonResult CheckDuplicate(string patient, string drug, decimal? dosage, int? id = null)
        {
            var allRecords = _bll.GetAll();

            if (id.HasValue)
                allRecords = allRecords.Where(x => x.Id != id.Value).ToList();

            // (Patient + Drug + Dosage)
            if (allRecords.Any(x =>
                    x.Patient == patient &&
                    x.Drug == drug &&
                    x.Dosage == dosage))
            {
                return Json(new { isDuplicate = true, message = MessageUtil.RecordAlreadyExists, isValid = false });
            }

            // (Patient + Drug)
            if (allRecords.Any(x =>
                    x.Patient == patient &&
                    x.Drug == drug))
            {
                return Json(new { isDuplicate = true, message = MessageUtil.DuplicateRecord, isValid = false });
            }

            // No changes made (for Edit)
            if (id.HasValue)
            {
                var currentRecord = _bll.GetById(id.Value);
                if (currentRecord != null &&
                    currentRecord.Patient == patient &&
                    currentRecord.Drug == drug &&
                    currentRecord.Dosage == dosage)
                {
                    return Json(new { isDuplicate = true, message = MessageUtil.NoChanges, isValid = false });
                }
            }

            return Json(new { isDuplicate = false, isValid = true });
        }

        // Helper method  (Create and Edit)
        private ActionResult HandleSubmit(Func<MedicationEntity> saveAction, MedicationEntity entity, string successType)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                                        .SelectMany(v => v.Errors)
                                        .Select(e => e.ErrorMessage)
                                        .ToList();

                TempData["ToastMessage"] = errors.Any()
                    ? string.Join(", ", errors) 
                    : "Validation error.";

                TempData["ToastType"] = "danger";
                return View(entity);
            }

            var result = saveAction();

            // Success
            if (result.IsSuccess)
            {
                TempData["ToastMessage"] = result.MessageList[0];
                TempData["ToastType"] = successType;
                return RedirectToAction("Index");
            }

            // Failed
            TempData["ToastMessage"] = string.Join(", ", result.MessageList);
            TempData["ToastType"] = "danger";
            return View(entity);
        }

    }
}