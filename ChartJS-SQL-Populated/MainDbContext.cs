using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ChartJS_SQL_Populated.Models;

namespace ChartJS_SQL_Populated
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
        {

        }

        public DbSet<BtcPriceModel> BtcPriceList { get; set; }


    }
}
