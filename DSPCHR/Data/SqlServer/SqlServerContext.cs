using DSPCHR.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.Data.SqlServer
{
    public class SqlServerContext : ApplicationDbContext
    {
        private readonly IConfiguration _configuration;

        public SqlServerContext(DbContextOptions<SqlServerContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_configuration.GetConnectionString("SqlServerConnection"));
        }

    }
}
