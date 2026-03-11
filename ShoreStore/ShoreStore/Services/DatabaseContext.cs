using Npgsql;
using System;

namespace ShoeStore.Services
{
    public class DatabaseContext
    {
        // ИЗМЕНИТЕ ПАРОЛЬ НА СВОЙ!
        private string connectionString = "Host=localhost;Port=5432;Database=Practika;Username=postgres";

        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connectionString);
        }

        public bool TestConnection()
        {
            try
            {
                using (var conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}