using MRP.model;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRP.Data
{
    internal class UserRepository
    {
        PostgresDB _postgres;
        public UserRepository(PostgresDB postgres)  
        {
            _postgres = postgres;
        }

        public User FindById(int Id)
        {
            await _postgres.ReadData("","");
            return null;
        }
    }
}
