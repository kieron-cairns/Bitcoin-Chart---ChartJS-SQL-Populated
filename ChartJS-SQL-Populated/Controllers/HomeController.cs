using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChartJS_SQL_Populated.Models;
using ChartJS_SQL_Populated.Repository;


namespace ChartJS_SQL_Populated.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDatabaseRepository repository;

        public HomeController(ILogger<HomeController> logger, IDatabaseRepository repository)
        {
            _logger = logger;
            this.repository = repository;
        }

        public IActionResult Index()
        {
            //below method will will call GetBtcPrices which uses nomics.com API to get prices.
            //method is currently commented out as my DB has results. However, a javascript function
            //will be written to call this method at set time intervals, so each day the DB is populated with new results.

            //repository.GetBtcPrices();

            return View();
        }

        public IActionResult DisplayBtcPrices(int range)
        {
            //This method is the primary method fo retrieving all relevant information 
            //from the SQL database.

            var array = repository.DisplayBtcPrices(range);

            //retrieve results as json ready to be plotted in graph.

            return Json(new { sucess = true, html = array });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public void GetBtcPrices()
        {
            repository.GetBtcPrices();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
