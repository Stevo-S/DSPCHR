using DSPCHR.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSPCHR.Data.PostgreSql
{
    public class PostgreSqlContext : ApplicationDbContext
    {
        private readonly IConfiguration _configuration;

        public PostgreSqlContext(DbContextOptions<PostgreSqlContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(_configuration.GetConnectionString("PostgreSqlConnection"));
        }

    }
}
