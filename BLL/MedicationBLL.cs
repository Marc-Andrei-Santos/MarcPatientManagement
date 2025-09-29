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


        // Insert
        public MedicationEntity Insert(MedicationEntity entity)
        {
            var result = new MedicationEntity();
            try
            {
                var success = _dal.Insert(entity);
                result.IsSuccess = success;
                result.MessageList.Add(success ? MessageUtil.RecordCreated : MessageUtil.SaveFailed);
            }
           
            catch (ApplicationException ex)
            {
                result.IsSuccess = false;
                result.MessageList.Add(ex.Message);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.MessageList.Add("Unexpected error while saving: " + ex.Message);
            }
            return result;
        }

        // Update
        public MedicationEntity Update(MedicationEntity entity)
        {
            var result = new MedicationEntity();
            try
            {
                var currentRecord = GetById(entity.Id);

                var validation = ValidateEntity(entity, true);
                if (validation != null) return validation;

                // lahat ng processing muna, saka current record check sa pinakababa
                if (currentRecord.Patient == entity.Patient &&
                    currentRecord.Drug == entity.Drug &&
                    currentRecord.Dosage == entity.Dosage)
                {
                    result.IsSuccess = false;
                    result.MessageList.Add(MessageUtil.NoChanges);
                    return result;
                }

                var success = _dal.Update(entity);
                result.IsSuccess = success;
                result.MessageList.Add(success ? MessageUtil.RecordUpdated : MessageUtil.UpdateFailed);
            }
            catch (ApplicationException ex)
            {
                result.IsSuccess = false;
                result.MessageList.Add(ex.Message);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.MessageList.Add("Unexpected error while updating: " + ex.Message);
            }
            return result;
        }


        // Delete
        public MedicationEntity Delete(int id)
        {
            var result = new MedicationEntity();
            try
            {
                var isDeleted = _dal.Delete(id);
                if (isDeleted)
                {
                    result.IsSuccess = true;
                    result.MessageList.Add(MessageUtil.RecordDeleted);
                }
                else
                {
                    result.IsSuccess = false;
                    result.MessageList.Add(MessageUtil.DeleteFailed);
                }
            }
            catch (ApplicationException ex)
            {
                result.IsSuccess = false;
                result.MessageList.Add(ex.Message);
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