using System.Data.SqlClient;

namespace DAL
{
    public class DbConnectionDAL
    {
        private readonly string _connectionString;

        public DbConnectionDAL()
        {
            _connectionString = "Data Source=DESKTOP-724R7KN\\SQLEXPRESS;Initial Catalog=MarcPatientManagementDB;User ID=santos;Password=andrei;TrustServerCertificate=True";
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
