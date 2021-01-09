using KW_App_Angular.Dall.Entities;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KW_App_Angular.Dall.Context
{
    public class DataProtKeyContext : DbContext, IDataProtectionKeyContext
    {
        public DataProtKeyContext(DbContextOptions<DataProtKeyContext> options)
                     : base(options) { }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

      
    }
}
