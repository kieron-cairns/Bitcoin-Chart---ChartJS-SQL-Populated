using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartJS_SQL_Populated.Models;
using System.Net;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.IO;

namespace ChartJS_SQL_Populated.Repository
{
    public class DatabaseRepository : IDatabaseRepository
    {
        //initialise main database context and database connection string variables

        private readonly MainDbContext mainDbContext;
        string connectionString = "";
        //initialsie random database name variable.



        public DatabaseRepository(MainDbContext mainDbContext, IConfiguration configuration)
        {

            //Class constructor to assign initilaied variables.

            //Assign mainDbContext and connection string that is stored within the app settings file. 

            this.mainDbContext = mainDbContext;
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public string BtcUrlConfig(string url)
        {
            //This method will create URI from the Transport API call that is used in the following method.
            //This is to parse the JSON data.

            Uri uri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Get;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            string output = reader.ReadToEnd();
            response.Close();

            return output;

        }


        public void GetBtcPrices()
        {
            //Get btc prices and add them to SQL database

            string commandText = @"INSERT INTO BTC_Prices (Date, Price) VALUES (@date, @price)";

            string apiKey = "Key goes here"; //your nomics API key here

            //This string contains the API call URL with the relevant optional paramaters present in the URL.

            string getJson = BtcUrlConfig("https://api.nomics.com/v1/exchange-rates/history?key=" + apiKey + "&currency=BTC&start=2018-04-14T00%3A00%3A00Z&end=2020-11-14T00%3A00%3A00Z%22");

            //Create dynamic arrray to store deserialized json attributes.
            dynamic btcPriceArray = JsonConvert.DeserializeObject(getJson);

            //cycle thorugh each json result
            foreach (var price in btcPriceArray)
            {
                //Establish SQL connection.

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();

                        SqlDependency.Start(connectionString);

                        SqlCommand cmd = new SqlCommand(commandText, conn);

                        //insert the json paramaters to the SQL table

                        string timestamp = price["timestamp"];
                        timestamp.Substring(timestamp.Length - 8);

                        cmd.Parameters.AddWithValue("@date", timestamp);
                        cmd.Parameters.AddWithValue("@price", price["rate"].ToString());

                        //Console.WriteLine(price["timestamp"]);
                        //Console.WriteLine(price["rate"]);

                        //Execute the query
                        cmd.ExecuteNonQuery();
                    }
                    finally
                    {
                        //Close the connection when all results have been gathered.
                        conn.Close();
                    }
                }
            }

        }

        public List<BtcPriceModel> DisplayBtcPrices()
        {
            //This method will display the BTC prices, for a given time range selected on the index page.

            //initialise list for results using the BtcPriceModel class to store each class object.

            List<BtcPriceModel> results = new List<BtcPriceModel>();

            //currently the method will display results for the last 7 days. This is going to be worked on so that the user can specify the amount of time. 

            string commandText = "SELECT * FROM BTC_Prices WHERE DATE BETWEEN DATEADD(DD, -7, GETDATE()) AND GETDATE()";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //Establish SQL connection.

                conn.Open();

                SqlDependency.Start(connectionString);


                //SqlDependency.Start(connectionString);

                SqlCommand cmd = new SqlCommand(commandText, conn);

                //Execute SQL comand and add results to BtcPriceModel List

                var reader = cmd.ExecuteReader();
                try
                {
                    //Read all results until null

                    while (reader.Read())
                    {
                        var tempResult = new BtcPriceModel
                        {

                            //Assign class objects with the fields in SQL table.

                            

                            Date = reader["Date"].ToString(),
                            Price = reader["Price"].ToString(),
                           
                        };

                        results.Add(tempResult);
                    }
                }
                finally
                {
                    reader.Close();
                }


                }

                return results;
        }

    }
}
