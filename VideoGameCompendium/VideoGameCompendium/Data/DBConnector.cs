using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoGameCompendium.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace VideoGameCompendium.Data
{
    public class DBConnector
    {
        static IMongoCollection<BsonDocument> Games;
        static IMongoCollection<BsonDocument> Genres;
        static IMongoCollection<BsonDocument> Platforms;
        static IMongoCollection<BsonDocument> Users;
        static IMongoCollection<BsonDocument> CollectionConnectors;
        static IMongoCollection<BsonDocument> FavoritesConnector;


        public DBConnector()
        {
            ConnectToMongo();
        }

        static void ConnectToMongo()
        {
            var connectionString = "mongodb://localhost:27017";

            var client = new MongoClient(connectionString);

            IMongoDatabase db = client.GetDatabase("VGC");
            Games = db.GetCollection<BsonDocument>("Games");
            Genres = db.GetCollection<BsonDocument>("Genres");
            Platforms = db.GetCollection<BsonDocument>("Platforms");
            Users = db.GetCollection<BsonDocument>("Users");

            Console.WriteLine("Mongo Connection Success!");
        }

        //Passed
        public bool InsertUser(ref User user)
        {
            try
            {
                BsonDocument doc = user.ToBsonDocument();
                doc.Remove("ID");
                Users.InsertOne(doc);
                user.ID = doc["_id"].AsObjectId.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            
            return true;
        }

        //Passed
        public bool EditUser(string id, User user)
        {
            try
            {
                BsonDocument doc = user.ToBsonDocument();
                doc.Remove("ID");
                var queryDoc = new BsonDocument();
                queryDoc["_id"] = ObjectId.Parse(id);
                Users.ReplaceOne(queryDoc, doc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        //Passed
        public bool DeleteUser(string id)
        {
            try
            {
                var queryDoc = new BsonDocument();
                queryDoc["_id"] = ObjectId.Parse(id);
                Users.DeleteOne(queryDoc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        //Passed
        public User GetUserByID(string id)
        {
            User user;
            try
            {
                var queryDoc = new BsonDocument();
                queryDoc["_id"] = ObjectId.Parse(id);
                var doc = Users.Find(queryDoc).ToList().First();
                user = new User(doc["Username"].AsString, doc["Password"].AsString, doc["Bio"].AsString, doc["Image"].AsString, doc["IsAdmin"].AsBoolean);
                user.ID = doc["_id"].AsObjectId.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            return user;
        }

        public bool CheckForUsername(string username)
        {
            try
            {
                var queryDoc = new BsonDocument();
                queryDoc["Username"] = username;
                var result = Users.Find(queryDoc).ToList();
                return result.Count > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        //Passed
        public User CheckLogin(string username, string password)
        {
            try
            {
                var queryDoc = new BsonDocument();
                queryDoc["Username"] = username;
                queryDoc["Password"] = password;
                var result = Users.Find(queryDoc).ToList();
                if (result.Count > 0)
                {
                    var doc = result.First();
                    User user = new User(doc["Username"].AsString, doc["Password"].AsString, doc["Bio"].AsString, doc["Image"].AsString, doc["IsAdmin"].AsBoolean);
                    user.ID = doc["_id"].AsObjectId.ToString();

                    return user;
                }
                else
                {
                    return null;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public List<Game> GetCollection(string userId)
        {
            return null;
        }

        public List<Game> GetFavorites(string id)
        {
            return null;
        }

        public List<Game> BrowseGames(string search, string platform, string genre, string maxEsrb)
        {
            return null;
        }

        public Game GetGameByID(string id)
        {
            return null;
        }

        public Game GetGenreByID(string id)
        {
            return null;
        }

        public Game GetPlatformByID(string id)
        {
            return null;
        }

        public bool AddToCollection(string userId, string gameId)
        {
            return false;
        }

        public bool RemoveFromCollection(string userId, string gameId)
        {
            return false;
        }

        public bool AddToFavorites(string userId, string gameId)
        {
            return false;
        }

        public bool RemoveFromFavorites(string userId, string gameId)
        {
            return false;
        }
    }
}
