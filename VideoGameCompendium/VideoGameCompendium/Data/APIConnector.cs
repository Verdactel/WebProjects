using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VideoGameCompendium.Data
{
    public static class APIConnector
    {
        public static string GetCoverById(Int32 id)
        {
            string url = "https://api-v3.igdb.com/covers/"+id+"?fields=url,id";

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
                        string json = sr.ReadToEnd();
                        json = json.Substring(1, json.Length - 2).Trim();
                        if (json.Length > 0)
                        {
                            Regex rx = new Regex(@"{[^{}]+}");
                            MatchCollection matches = rx.Matches(json);

                            BsonDocument doc = BsonDocument.Parse(matches[0].ToString());

                            BsonValue result;
                            doc.TryGetValue("url", out result);

                            return result.AsString;
                        }
                    }
                }
            }

            return null;
        }

        public static string GetESRBById(Int32 id)
        {
            string url = "https://api-v3.igdb.com/age_ratings/" + id + "?fields=rating,rating_cover_url,synopsis,content_descriptions";

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
                        string json = sr.ReadToEnd();
                        json = json.Substring(1, json.Length - 2).Trim();
                        if (json.Length > 0)
                        {
                            Regex rx = new Regex(@"{[^{}]+}");
                            MatchCollection matches = rx.Matches(json);

                            BsonDocument doc = BsonDocument.Parse(matches[0].ToString());

                            BsonValue result;
                            if (doc.TryGetValue("rating", out result))
                            {
                                string rating = "";
                                switch (result.AsInt32)
                                {
                                    case 6: rating = "Rating Pending";
                                        break;
                                    case 7: rating = "Early Childhood";
                                        break;
                                    case 8: rating = "Everyone";
                                        break;
                                    case 9: rating = "Everyone 10+";
                                        break;
                                    case 10: rating = "Teen";
                                        break;
                                    case 11: rating = "Mature 10+";
                                        break;
                                    case 12: rating = "Adults Only 18+";
                                        break;
                                    default: rating = "";
                                        break;
                                }
                                return rating;
                            }

                            return "";
                        }
                    }
                }
            }

            return null;
        }
    }
}
