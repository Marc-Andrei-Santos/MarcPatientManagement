using DAL;
using EL;
using System;
using System.Collections.Generic;
using System.Linq;
using UL;

namespace BLL
{
    public class MedicationBLL
    {
        private readonly MedicationDAL _dal = new MedicationDAL();

        public List<MedicationEntity> GetAll() => _dal.GetAll();

        public MedicationEntity GetById(int id) => _dal.GetById(id);

        public MedicationEntity Insert(MedicationEntity entity)
        {
            var result = new MedicationEntity();
            try
            {
                var validation = ValidateEntity(entity);
                if (validation != null) return validation;

                var success = _dal.Insert(entity);
                result.IsSuccess = success;
                result.MessageList.Add(success ? MessageUtil.RecordCreated : MessageUtil.SaveFailed);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.MessageList.Add("Unexpected error while saving: " + ex.Message);
            }

            return result;
        }

        public MedicationEntity Update(MedicationEntity entity)
        {
            var result = new MedicationEntity();
            try
            {
                var currentRecord = GetById(entity.Id);
                if (currentRecord.Patient == entity.Patient &&
                    currentRecord.Drug == entity.Drug &&
                    currentRecord.Dosage == entity.Dosage)
                {
                    result.IsSuccess = false;
                    result.MessageList.Add(MessageUtil.NoChanges);
                    return result;
                }

                var validation = ValidateEntity(entity, true);
                if (validation != null) return validation;

                var success = _dal.Update(entity);
                result.IsSuccess = success;
                result.MessageList.Add(success ? MessageUtil.RecordUpdated : MessageUtil.UpdateFailed);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.MessageList.Add("Unexpected error while updating: " + ex.Message);
            }

            return result;
        }

        public MedicationEntity Delete(int id)
        {
            var result = new MedicationEntity();
            try
            {
                var success = _dal.Delete(id);
                result.IsSuccess = success;
                result.MessageList.Add(success ? MessageUtil.RecordDeleted : MessageUtil.DeleteFailed);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.MessageList.Add("Unexpected error while deleting: " + ex.Message);
            }

            return result;
        }

        // Helper Validate Entity
        private MedicationEntity ValidateEntity(MedicationEntity entity, bool isUpdate = false)
        {
            var result = new MedicationEntity();

            if (!ValidationUtil.IsValidPatient(entity.Patient))
                result.MessageList.Add(MessageUtil.InvalidPatient);

            if (!ValidationUtil.IsValidDrug(entity.Drug))
                result.MessageList.Add(MessageUtil.InvalidDrug);

            if (!ValidationUtil.IsValidDosage(entity.Dosage))
                result.MessageList.Add(MessageUtil.InvalidDosage);

            if (result.MessageList.Any())
            {
                result.IsSuccess = false;
                return result;
            }

            return null;
        }
    }
}