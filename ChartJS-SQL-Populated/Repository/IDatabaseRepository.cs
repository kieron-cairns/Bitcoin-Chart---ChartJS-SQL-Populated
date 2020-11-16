using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartJS_SQL_Populated.Models;

namespace ChartJS_SQL_Populated.Repository
{
    public interface IDatabaseRepository
    {
        public void GetBtcPrices();

        List<BtcPriceModel> DisplayBtcPrices(int range);
    }
}
