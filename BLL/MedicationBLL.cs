using DAL;
using EL;
using System;
using System.Collections.Generic;
using UL;

namespace BLL
{
    public class MedicationBLL
    {
        private readonly MedicationDAL _dal = new MedicationDAL();

        public List<MedicationEntity> GetAll() => _dal.GetAll();

        public List<MedicationEntity> GetFiltered(string patient, string drug, decimal? dosage, DateTime? modifiedDate)
            => _dal.GetFiltered(patient, drug, dosage, modifiedDate);

        public MedicationEntity GetById(int id) => _dal.GetById(id);

        // Insert
        public MedicationEntity Insert(MedicationEntity entity)
        {
            var result = new MedicationEntity();

            if (!ValidationUtil.IsValidPatient(entity.Patient))
            {
                result.IsSuccess = false;
                result.MessageList.Add(MessageUtil.InvalidPatient);
                return result;
            }

            if (!ValidationUtil.IsValidDrug(entity.Drug))
            {
                result.IsSuccess = false;
                result.MessageList.Add(MessageUtil.InvalidDrug);
                return result;
            }

            if (!ValidationUtil.IsValidDosage(entity.Dosage))
            {
                result.IsSuccess = false;
                result.MessageList.Add(MessageUtil.InvalidDosage);
                return result;
            }

            var success = _dal.Insert(entity);
            result.IsSuccess = success;
            result.MessageList.Add(success ? MessageUtil.RecordCreated : "Failed to save record.");

            return result;
        }

        // Update
        public MedicationEntity Update(MedicationEntity entity)
        {
            var result = new MedicationEntity();

            if (!ValidationUtil.IsValidPatient(entity.Patient))
            {
                result.IsSuccess = false;
                result.MessageList.Add(MessageUtil.InvalidPatient);
                return result;
            }

            if (!ValidationUtil.IsValidDrug(entity.Drug))
            {
                result.IsSuccess = false;
                result.MessageList.Add(MessageUtil.InvalidDrug);
                return result;
            }

            if (entity.Dosage <= 0)
            {
                result.IsSuccess = false;
                result.MessageList.Add(MessageUtil.InvalidDosage);
                return result;
            }

            var success = _dal.Update(entity);
            result.IsSuccess = success;
            result.MessageList.Add(success ? MessageUtil.RecordUpdated : "Failed to update record.");

            return result;
        }

        // Delete
        public MedicationEntity Delete(int id)
        {
            var result = new MedicationEntity();

            var success = _dal.Delete(id);
            result.IsSuccess = success;
            result.MessageList.Add(success ? MessageUtil.RecordDeleted : "Error deleting record.");

            return result;
        }
    }
}
