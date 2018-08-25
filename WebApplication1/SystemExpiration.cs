using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using WebApplication1.Models;

namespace WebApplication1
{
    public class SystemExpiration
    {
        //private readonly ApplicationDbContext _context;


        public DateTime Expiration { set; get; }
        private static SystemExpiration instance;
        private SystemExpiration() { }

        public static SystemExpiration Instance(DateTime? expiration, IConfiguration configuration)
        {
            if (instance == null)
            {
                instance = new SystemExpiration();
                if (expiration != null)
                {
                    instance.Expiration = GetExpiration(configuration);

                }

            }
            return instance;
        }
        private static DateTime GetExpiration(IConfiguration configuration)
        {
            SystemExpirationTable systemExpirationTable = null;
            using (var connection = new SqlConnection(configuration.GetConnectionString("DefaultConnection")))
            {
                var command = new SqlCommand("select * from SystemExpirationTable", connection);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        systemExpirationTable = new SystemExpirationTable
                        {
                            Id = reader.GetInt32(0),
                            Expiration = reader.GetDateTime(1)
                        };
                    }
                }


            }


            if (systemExpirationTable != null)
            {
                return systemExpirationTable.Expiration;
            }
            return DateTime.Now;
        }
        public bool IsExpired(DateTime currentTime)
        {
            if (currentTime >= Expiration)
            {
                return true;
            }
            return false;
        }


    }

}
