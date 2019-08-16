using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBScraper
{
    class Program
    {
        static IMongoCollection<BsonDocument> collection;
        static void Main(string[] args)
        {
            ConnectToMongo().Wait();

            Console.Write("Search for:");
            string searchWord = Console.ReadLine();

            for (int i = 0; i < 4; i++)
            {
                GetAPI(i, searchWord).Wait();
                Console.WriteLine("Batch Loaded Success! "+i);
            }

            Console.ReadLine();
        }

        static async Task GetAPI(int offset, string searchWord)
        {
            string myJson = "{'user-key': 'c5f344d6e5861ca256da27246e274fba', 'Accept': 'application/json'}";
            string url = "https://api-v3.igdb.com/games?search="+searchWord+ "&fields=name,id,platforms,genres,age_ratings,first_release_date,cover,summary,artworks&limit=50&offset=" + (offset*50);
            //string url = "https://api-v3.igdb.com/platforms?&fields=id,name,abbreviation&limit=50&offset=" + (offset*50);
            
            var webRequest = System.Net.WebRequest.Create(url);
            if (webRequest != null)
            {
                webRequest.Method = "GET";
                webRequest.Timeout = 12000;
                webRequest.ContentType = "application/json";
                webRequest.Headers.Add("user-key", "c5f344d6e5861ca256da27246e274fba");

                using (System.IO.Stream s = webRequest.GetResponse().GetResponseStream())
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                    {
                        BsonValue dummy;
                        string json = sr.ReadToEnd();
                        json = json.Substring(1, json.Length - 2).Trim();
                        if (json.Length > 0)
                        {
                            Regex rx = new Regex(@"{[^{}]+}");
                            MatchCollection matches = rx.Matches(json);

                            List<BsonDocument> docs = new List<BsonDocument>();
                            foreach (var o in matches)
                            {
                                BsonDocument doc = BsonDocument.Parse(o.ToString());
                                if (doc.TryGetValue("name", out dummy) && doc.TryGetValue("first_release_date", out dummy) && doc.TryGetValue("cover", out dummy) && doc.TryGetValue("summary", out dummy) && doc.TryGetValue("genres", out dummy) && doc.TryGetValue("platforms", out dummy))
                                {
                                    Int32 i = doc["cover"].AsInt32;
                                    doc.Remove("cover");
                                    doc.Add("cover", new BsonString(i.ToString()));

                                    docs.Add(doc);
                                }
                            }
                            await collection.InsertManyAsync(docs);
                        }
                    }
                }
            }
        }

        static async Task ConnectToMongo()
        {
            var connectionString = "mongodb://localhost:27017";

            var client = new MongoClient(connectionString);

            IMongoDatabase db = client.GetDatabase("VGC");
            collection = db.GetCollection<BsonDocument>("Games");

            Console.WriteLine("Mongo Connection Success!");

            //var documents = await collection.Find(_ => true).ToListAsync();
            //
            //documents.ForEach(x => { Console.WriteLine(x); });
        }
    }
}
