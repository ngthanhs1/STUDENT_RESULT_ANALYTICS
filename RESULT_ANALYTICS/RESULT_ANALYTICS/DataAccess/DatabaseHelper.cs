
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;

namespace RESULT_ANALYTICS.DataAccess
{
    internal class DatabaseHelper
    {
        // Chuoi ket noi SQL Server
        private static readonly string connectionString =
            ConfigurationManager.ConnectionStrings["StudentDB"]
            .ConnectionString;

        // Tao ket noi
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        // Kiem tra ket noi
        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
