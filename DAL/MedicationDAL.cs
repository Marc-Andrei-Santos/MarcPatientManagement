using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using EL;

namespace DAL
{
    public class MedicationDAL
    {
        private readonly string _connectionString = "Data Source=DESKTOP-724R7KN\\SQLEXPRESS;Initial Catalog=MarcPatientManagementDB;User ID=santos;Password=andrei;TrustServerCertificate=True";

        public List<MedicationEntity> GetAll()
        {
            var list = new List<MedicationEntity>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT Id, Patient, Dosage, Drug, ModifiedDate FROM MedicationRecords", conn);
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
            return list;
        }

        public List<MedicationEntity> GetFiltered(string patient, string drug, decimal? dosage, DateTime? modifiedDate)
        {
            var result = new List<MedicationEntity>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand();
                cmd.Connection = conn;

                var query = "SELECT Id, Patient, Dosage, Drug, ModifiedDate FROM MedicationRecords WHERE 1=1";

                if (!string.IsNullOrEmpty(patient))
                {
                    query += " AND Patient LIKE @Patient";
                    cmd.Parameters.AddWithValue("@Patient", "%" + patient + "%");
                }

                if (!string.IsNullOrEmpty(drug))
                {
                    query += " AND Drug LIKE @Drug";
                    cmd.Parameters.AddWithValue("@Drug", "%" + drug + "%");
                }

                if (dosage.HasValue)
                {
                    query += " AND Dosage = @Dosage";
                    cmd.Parameters.AddWithValue("@Dosage", dosage.Value);
                }

                if (modifiedDate.HasValue)
                {
                   
                    query += " AND CAST(ModifiedDate AS DATE) = @ModifiedDate";
                    cmd.Parameters.AddWithValue("@ModifiedDate", modifiedDate.Value.Date);
                }

                cmd.CommandText = query;

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new MedicationEntity
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

            return result;
        }


        // Insert record
        public bool Insert(MedicationEntity entity)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var query = @"INSERT INTO MedicationRecords (Patient, Drug, Dosage, ModifiedDate)
                      VALUES (@Patient, @Drug, @Dosage, @ModifiedDate)";

                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Patient", entity.Patient);
                    cmd.Parameters.AddWithValue("@Drug", entity.Drug);
                    cmd.Parameters.AddWithValue("@Dosage", entity.Dosage);
                    cmd.Parameters.AddWithValue("@ModifiedDate", entity.ModifiedDate); 


                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }


        // Update record
        public bool Update(MedicationEntity entity)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand(@"UPDATE MedicationRecords 
                           SET Patient=@Patient, Dosage=@Dosage, Drug=@Drug, ModifiedDate=@ModifiedDate
                           WHERE Id=@Id", conn);

                cmd.Parameters.AddWithValue("@Patient", entity.Patient);
                cmd.Parameters.AddWithValue("@Dosage", entity.Dosage);
                cmd.Parameters.AddWithValue("@Drug", entity.Drug);
                cmd.Parameters.AddWithValue("@ModifiedDate", entity.ModifiedDate);
                cmd.Parameters.AddWithValue("@Id", entity.Id);


                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Delete record
        public bool Delete(int id)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("DELETE FROM MedicationRecords WHERE Id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Get by Id
        public MedicationEntity GetById(int id)
        {
            MedicationEntity entity = null;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT Id, Patient, Dosage, Drug, ModifiedDate FROM MedicationRecords WHERE Id=@Id", conn);
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

            return entity;
        }
    }
}
