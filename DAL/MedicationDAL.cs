using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using EL;

namespace DAL
{
    public class MedicationDAL : DbConnectionDAL
    {
        public List<MedicationEntity> GetAll()
        {
            var list = new List<MedicationEntity>();

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("Medication_GetAll", conn);
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

        public List<MedicationEntity> GetFiltered(string patient, string drug, decimal? dosage, DateTime? modifiedDate)
        {
            var results = new List<MedicationEntity>();

            using (var conn = GetConnection())
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand("Medication_GetFiltered", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Patient", (object)patient ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Drug", (object)drug ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Dosage", (object)dosage ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ModifiedDate", (object)modifiedDate?.Date ?? DBNull.Value);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(new MedicationEntity
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Patient = reader["Patient"].ToString(),
                                Drug = reader["Drug"].ToString(),
                                Dosage = Convert.ToDecimal(reader["Dosage"]),
                                ModifiedDate = Convert.ToDateTime(reader["ModifiedDate"])
                            });
                        }
                    }
                }
            }

            return results;
        }

        public bool Insert(MedicationEntity entity)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand("Medication_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Patient", entity.Patient);
                        cmd.Parameters.AddWithValue("@Drug", entity.Drug);
                        cmd.Parameters.AddWithValue("@Dosage", entity.Dosage);
                        cmd.Parameters.AddWithValue("@ModifiedDate", entity.ModifiedDate);

                        conn.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Database error in Insert.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unexpected error in Insert.", ex);
            }
        }

        public bool Update(MedicationEntity entity)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("Medication_Update", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Id", entity.Id);
                    cmd.Parameters.AddWithValue("@Patient", entity.Patient);
                    cmd.Parameters.AddWithValue("@Dosage", entity.Dosage);
                    cmd.Parameters.AddWithValue("@Drug", entity.Drug);
                    cmd.Parameters.AddWithValue("@ModifiedDate", entity.ModifiedDate);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Database error in Update.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unexpected error in Update.", ex);
            }
        }

        public bool Delete(int id)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("Medication_Delete", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", id);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new ApplicationException("Database error in Delete.", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Unexpected error in Delete.", ex);
            }
        }

        public MedicationEntity GetById(int id)
        {
            MedicationEntity entity = null;
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    var cmd = new SqlCommand("Medication_GetById", conn);
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