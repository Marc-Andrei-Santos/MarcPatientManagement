using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EL;

namespace DAL
{
    public class MedicationDAL : DbConnectionDAL
    {

        // Get All
        public List<MedicationEntity> GetAll()
        {
            var list = new List<MedicationEntity>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("spMedicationGet", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new MedicationEntity
                        {
                            Id = (int)reader["Id"],
                            Patient = reader["Patient"].ToString(),
                            Dosage = Convert.ToDecimal(reader["Dosage"]),
                            Drug = reader["Drug"].ToString(),
                            ModifiedDate = Convert.ToDateTime(reader["ModifiedDate"])
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Database error in GetAll.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unexpected error in GetAll.", ex);
            }
            return list;
        }


        // Insert
        public bool Insert(MedicationEntity entity)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("spMedicationAdd", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Patient", entity.Patient);
                        cmd.Parameters.AddWithValue("@Drug", entity.Drug);
                        cmd.Parameters.AddWithValue("@Dosage", entity.Dosage);
                        cmd.Parameters.AddWithValue("@ModifiedDate", entity.ModifiedDate);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true; 
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An unexpected error occurred during insertion.", ex);
            }
        }

        // Update
        public bool Update(MedicationEntity entity)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("spMedicationEdit", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", entity.Id);
                        cmd.Parameters.AddWithValue("@Patient", entity.Patient);
                        cmd.Parameters.AddWithValue("@Drug", entity.Drug);
                        cmd.Parameters.AddWithValue("@Dosage", entity.Dosage);

                        conn.Open();
                        cmd.ExecuteNonQuery(); 
                        return true; 
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An unexpected error occurred during update.", ex);
            }
        }

        // Delete
        public bool Delete(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("spMedicationDelete", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        return true; 
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An unexpected error occurred during deletion.", ex);
            }
        }

        // Get by Id
        public MedicationEntity GetById(int id)
        {
            MedicationEntity entity = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("spMedicationGetById", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            entity = new MedicationEntity
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Patient = reader["Patient"].ToString(),
                                Dosage = Convert.ToDecimal(reader["Dosage"]),
                                Drug = reader["Drug"].ToString(),
                                ModifiedDate = Convert.ToDateTime(reader["ModifiedDate"])
                            };
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Database error in GetById.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unexpected error in GetById.", ex);
            }
            return entity;
        }
    }
}