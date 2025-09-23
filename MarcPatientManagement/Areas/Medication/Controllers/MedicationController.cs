using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BLL;
using EL;
using UL;

// ... existing using statements ...

namespace AL.Areas.Medication.Controllers
{
    public class MedicationController : Controller
    {
        private readonly MedicationBLL _bll = new MedicationBLL();

        public ActionResult Index(int page = 1, int pageSize = 10)
        {
            ViewBag.ActiveTab = "Medication";
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            var allData = _bll.GetAll();
            ViewBag.TotalRecords = allData.Count();

            return View(allData.Skip((page - 1) * pageSize).Take(pageSize).ToList());
        }
        // GET: Create
        public ActionResult Create()
        {
        
            if (Request.QueryString.Count > 0)
            {
                return RedirectToAction("NotFound", "Error", new { area = "" });
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
            if (!id.HasValue)
                return RedirectToAction("NotFound", "Error", new { area = "" });

            var entity = _bll.GetById(id.Value);
            if (entity == null)
                return RedirectToAction("NotFound", "Error", new { area = "" });

            return View(entity);
        }

        // POST: Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MedicationEntity entity)
        {
            // Extra validation: check if the entity exists before updating
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

            if (allRecords.Any(x =>
                    x.Patient == patient &&
                    x.Drug == drug &&
                    x.Dosage == dosage))
            {
                return Json(new { isDuplicate = true, message = MessageUtil.RecordAlreadyExists, isValid = false });
            }

            if (allRecords.Any(x =>
                    x.Patient == patient &&
                    x.Drug == drug))
            {
                return Json(new { isDuplicate = true, message = MessageUtil.DuplicateRecord, isValid = false });
            }

            return Json(new { isDuplicate = false, isValid = true });
        }

        // Helper Handle Submit for Create and Edit 
        private ActionResult HandleSubmit(Func<MedicationEntity> saveAction, MedicationEntity entity, string successType)
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

            TempData["ToastMessage"] = string.Join(", ", result.MessageList);
            TempData["ToastType"] = "danger";
            return View(entity);
        }
    }
}