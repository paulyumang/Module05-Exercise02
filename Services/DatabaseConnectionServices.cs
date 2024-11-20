using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module07DataAccess.Services
{
    public class DatabaseConnectionServices
    {
        // Set Up all of this 
        private readonly string _connectionString;

        public DatabaseConnectionServices()
        {
            _connectionString = "Server=localhost;Database=CompanyDB;User ID=paul;Password=09283788349";
        }

        //Up to here
        public string GetConnectionString()
        {
            return _connectionString;
        }
    }
}
