using System;
using System.Collections.Generic;
using System.Linq;
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
            try
            {
                ViewBag.ActiveTab = "Medication";
                Session["AllowedEditId"] = null;

                var allData = _bll.GetAll()
                                  .OrderByDescending(x => x.ModifiedDate)
                                  .ToList();

                return View(allData);
            }
            catch (Exception)
            {
                TempData["ToastMessage"] = "An error occurred while loading records.";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Error", "Home", new { area = "" });
            }
        }

        // Get Create
        public ActionResult Create()
        {
            try
            {
                if (Request.Url.Segments.Length > 4)
                    return RedirectToAction("Index");

                return View(new MedicationEntity { ModifiedDate = DateTime.Now });
            }
            catch (Exception)
            {
                // log exception here
                TempData["ToastMessage"] = "An error occurred while preparing the create form.";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index");
            }
        }

        // Post Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MedicationEntity entity)
        {
            try
            {
                entity.ModifiedDate = DateTime.Now;
                return HandleSubmit(() => _bll.Insert(entity), entity, "success");
            }
            catch (Exception)
            {
                TempData["ToastMessage"] = "An error occurred while saving the record.";
                TempData["ToastType"] = "danger";
                return View(entity);
            }
        }

        // Get Edit
        public ActionResult Edit(int? id)
        {
            try
            {
                if (!id.HasValue || Request.Url.Segments.Length > 5)
                    return RedirectToAction("Index");

                if (Session["AllowedEditId"] == null)
                {
                    Session["AllowedEditId"] = id;
                }
                else if ((int)Session["AllowedEditId"] != id)
                {
                    return RedirectToAction("Index");
                }

                var entity = _bll.GetById(id.Value);
                if (entity == null)
                    return RedirectToAction("Index");

                return View(entity);
            }
            catch (Exception)
            {
                TempData["ToastMessage"] = "An error occurred while loading the edit form.";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index");
            }
        }

        // Post Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MedicationEntity entity)
        {
            try
            {
                if (Session["AllowedEditId"] == null || (int)Session["AllowedEditId"] != entity.Id)
                    return RedirectToAction("Index");

                var existing = _bll.GetById(entity.Id);
                if (existing == null)
                    return RedirectToAction("Index");

                return HandleSubmit(() => _bll.Update(entity), entity, "warning");
            }
            catch (Exception)
            {
                TempData["ToastMessage"] = "An error occurred while updating the record.";
                TempData["ToastType"] = "danger";
                return View(entity);
            }
        }

        // Post Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var existing = _bll.GetById(id);
                if (existing == null)
                    return RedirectToAction("NotFound", "Error", new { area = "" });

                var result = _bll.Delete(id);

                TempData["ToastMessage"] = string.Join(", ", result.MessageList);
                TempData["ToastType"] = "danger";

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["ToastMessage"] = "An error occurred while deleting the record.";
                TempData["ToastType"] = "danger";
                return RedirectToAction("Index");
            }
        }

        // Check Duplicates
        public JsonResult CheckDuplicate(string patient, string drug, decimal? dosage, int? id = null)
        {
            try
            {
                var allRecords = _bll.GetAll();

                if (id.HasValue)
                    allRecords = allRecords.Where(x => x.Id != id.Value).ToList();

                if (allRecords.Any(x => x.Patient == patient && x.Drug == drug && x.Dosage == dosage))
                {
                    return Json(new { isDuplicate = true, message = MessageUtil.RecordAlreadyExists, isValid = false });
                }

                if (allRecords.Any(x => x.Patient == patient && x.Drug == drug))
                {
                    return Json(new { isDuplicate = true, message = MessageUtil.DuplicateRecord, isValid = false });
                }

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
            catch (Exception)
            {
                return Json(new { isDuplicate = false, isValid = false, message = "An error occurred while checking duplicates." });
            }
        }

        // Helper Method Save Action
        private ActionResult HandleSubmit(Func<MedicationEntity> saveAction, MedicationEntity entity, string successType)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage)
                                            .ToList();

                    TempData["ToastMessage"] = errors.Any() ? string.Join(", ", errors) : "Validation error.";
                    TempData["ToastType"] = "danger";

                    return View(entity);
                }

                var result = saveAction();

                if (result.IsSuccess)
                {
                    TempData["ToastMessage"] = result.MessageList.FirstOrDefault() ?? "Operation successful.";
                    TempData["ToastType"] = successType;
                    return RedirectToAction("Index");
                }

                TempData["ToastMessage"] = string.Join(", ", result.MessageList);
                TempData["ToastType"] = "danger";

                return View(entity);
            }
            catch (Exception)
            {
                TempData["ToastMessage"] = "An unexpected error occurred while saving the record.";
                TempData["ToastType"] = "danger";
                return View(entity);
            }
        }
    }
}
