using MySql.Data.MySqlClient;

namespace StadiumWeb.Data
{
    public class DbHelper
    {
        private readonly string _connectionString;

        public DbHelper(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? "server=localhost;database=stadium_demo;uid=root;pwd=;";
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
