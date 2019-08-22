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
        static IMongoCollection<BsonDocument> Comments;
        static IMongoCollection<BsonDocument> Ratings;
        static IMongoCollection<BsonDocument> Followers;


        public DBConnector()
        {
            ConnectToMongo();
        }

        //Helper
        static void ConnectToMongo()
        {
            var connectionString = "mongodb://localhost:27017";

            var client = new MongoClient(connectionString);

            IMongoDatabase db = client.GetDatabase("VGC");
            Games = db.GetCollection<BsonDocument>("Games");
            Genres = db.GetCollection<BsonDocument>("Genres");
            Platforms = db.GetCollection<BsonDocument>("Platforms");
            Users = db.GetCollection<BsonDocument>("Users");
            CollectionConnectors = db.GetCollection<BsonDocument>("CollectionConnectors");
            FavoritesConnector = db.GetCollection<BsonDocument>("FavoritesConnector");
            Comments = db.GetCollection<BsonDocument>("Comments");
            Ratings = db.GetCollection<BsonDocument>("Ratings");
            Followers = db.GetCollection<BsonDocument>("Followers");

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

        //Passed
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

        //Passed
        public List<Game> GetCollection(string userId)
        {
            List<Game> toReturn = new List<Game>();

            try
            {
                var queryDoc = new BsonDocument();
                queryDoc["userId"] = (userId);

                var result = CollectionConnectors.Find(queryDoc).ToList();
                
                for(int i = 0; i < result.Count; i++)
                { 
                    int id = result[i]["gameId"].AsInt32;
                    
                    Game game = GetGameByID(id);
                    toReturn.Add(game);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return toReturn;
        }

        //Passed
        public List<Game> GetFavorites(string userId)
        {
            List<Game> toReturn = new List<Game>();

            try
            {
                var queryDoc = new BsonDocument();
                queryDoc["userId"] = (userId);

                var result = FavoritesConnector.Find(queryDoc).ToList();

                for (int i = 0; i < result.Count; i++)
                {
                    int _id = result[i]["gameId"].AsInt32;

                    Game game = GetGameByID(_id);
                    toReturn.Add(game);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return toReturn;

        }

        //Passed
        public List<Game> BrowseGames(string search = "", string platform = "", string genre = "", int maxEsrb = 12)
        {
            List<Game> result;
            try
            {
                var allGames = Games.Find(new BsonDocument()).ToList();
                allGames = allGames.Where(x => x["name"].AsString.Contains(search)).ToList();

                if (!string.IsNullOrEmpty(platform))
                {
                    var p = Platforms.Find(new BsonDocument()).ToList();
                    p = p.Where(x => x["name"].AsString == platform || (x.TryGetValue("abbreviation", out var dummy) && x["abbreviation"].AsString == platform)).ToList();
                    Int32 index = p.Select(x => x["id"].AsInt32).FirstOrDefault();
                    BsonInt32 bint = new BsonInt32(index);

                    allGames = allGames.Where(x => x["platforms"].AsBsonArray.Contains(bint)).ToList();
                }

                if (!string.IsNullOrEmpty(genre))
                {
                    var p = Genres.Find(new BsonDocument()).ToList();
                    p = p.Where(x => x["name"].AsString.Contains(genre)).ToList();
                    Int32 index = p.Select(x => x["id"].AsInt32).FirstOrDefault();
                    BsonInt32 bint = new BsonInt32(index);

                    allGames = allGames.Where(x => x["genres"].AsBsonArray.Contains(bint)).ToList();
                }

                var ids = allGames.Select(x => x["id"].AsInt32).ToList();
                result = new List<Game>();
                ids.ForEach(x => result.Add(GetGameByID(x)));

                result = result.Where(x => x.ESRBNumeric <= maxEsrb).ToList();
                if (maxEsrb < 12)
                    result = result.Where(x => x.ESRBNumeric != 6 && x.ESRBNumeric != 0).ToList();

                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            return result;
        }

        //Passed
        public Game GetGameByID(Int32 id)
        {
            Game game;
            try
            {
                var queryDoc = new BsonDocument();
                queryDoc["id"] = id;
                var docs = Games.Find(queryDoc).ToList();
                var doc = docs.First();
                game = new Game(doc["id"].AsInt32, doc["name"].AsString, doc["summary"].AsString, UnixTimeStampToDateTime(doc["first_release_date"].AsInt32), doc["cover"].AsString);
                foreach (var v in doc["platforms"].AsBsonArray) { game.Platforms.Add(GetPlatformByID(v.AsInt32, true)); }
                foreach (var v in doc["genres"].AsBsonArray) { game.Genres.Add(GetGenreByID(v.AsInt32)); }
                if (int.TryParse(game.Image, out int coverID))
                {
                    game.Image = APIConnector.GetCoverById(coverID);
                    doc["cover"] = game.Image;
                    if (doc.TryGetValue("age_ratings", out var dummy))
                    {
                        var ratings = doc["age_ratings"].AsBsonArray;
                        doc.Remove("age_ratings");
                        foreach (var r in ratings)
                        {
                            var result = APIConnector.GetESRBById(r.AsInt32);
                            if (!string.IsNullOrEmpty(result.Item1))
                            {
                                game.ESRB = result.Item1;
                                game.ESRBNumeric = result.Item2;
                                doc["esrb"] = game.ESRB;
                                doc["esrbNumeric"] = game.ESRBNumeric;
                                break;
                            }
                        }
                    }

                    var query = new BsonDocument();
                    query["_id"] = doc["_id"].AsObjectId;
                    Games.ReplaceOne(query, doc);
                }
                else
                if (doc.TryGetValue("esrb", out var dummy2))
                {
                    game.ESRB = doc["esrb"].AsString;
                    game.ESRBNumeric = doc["esrbNumeric"].AsInt32;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            return game;
        }

        //Passed
        public string GetGenreByID(Int32 id)
        {
            try
            {
                var queryDoc = new BsonDocument();
                queryDoc["id"] = id;
                var doc = Genres.Find(queryDoc).ToList().First();
                return doc["name"].AsString;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        //Passed
        public string GetPlatformByID(Int32 id, bool preferShort)
        {
            try
            {
                var queryDoc = new BsonDocument();
                queryDoc["id"] = id;
                var doc = Platforms.Find(queryDoc).ToList().First();
                if (preferShort && doc["abbreviation"] != null)
                    return doc["abbreviation"].AsString;
                return doc["name"].AsString;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        //Passed
        public bool AddToCollection(string userId, string gameId)
        {
            try
            {
                BsonDocument doc = new BsonDocument();
                doc.Add("gameId", new BsonString(gameId));
                doc.Add("userId", new BsonString(userId));
                CollectionConnectors.InsertOne(doc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        //Passed
        public bool RemoveFromCollection(string userId, string gameId)
        {
            try
            {
                var queryDoc = new BsonDocument();
                queryDoc["gameId"] = new BsonString(gameId);
                queryDoc["userId"] = new BsonString(userId);

                CollectionConnectors.DeleteOne(queryDoc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        //Passed
        public bool AddToFavorites(string userId, string gameId)
        {
            try
            {
                BsonDocument doc = new BsonDocument();
                doc.Add("gameId", new BsonString(gameId));
                doc.Add("userId", new BsonString(userId));
                FavoritesConnector.InsertOne(doc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        //Passed
        public bool RemoveFromFavorites(string userId, string gameId)
        {
            try
            {
                var queryDoc = new BsonDocument();
                queryDoc["gameId"] = new BsonString(gameId);
                queryDoc["userId"] = new BsonString(userId);

                FavoritesConnector.DeleteOne(queryDoc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        //Passed
        /// <summary>
        /// Adds a comment to a game post or a user post
        /// </summary>
        /// <param name="text">Text to as the comment</param>
        /// <param name="pageId">The Id of the page reciveing the comment. Can be a number for a game ID or a uNumber for a user Id</param>
        /// <param name="userId">The Id of the user who posted the comment</param>
        /// <returns></returns>
        public bool AddComment(ref Comment comment)
        {
            try
            {
                BsonDocument doc = comment.ToBsonDocument();
                doc.Remove("ID");
                Comments.InsertOne(doc);
                comment.ID = doc["_id"].AsObjectId.ToString();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        //Passed
        public bool RemoveComment(string commentId)
        {
            try
            {
                var queryDoc = new BsonDocument();
                queryDoc["_id"] = ObjectId.Parse(commentId);
                Comments.DeleteOne(queryDoc);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        //Passed
        public List<Comment> GetComments(string recieverId)
        {
            List<Comment> toReturn = new List<Comment>();
            try
            {
                var queryDoc = new BsonDocument();
                queryDoc["RecieverId"] = recieverId;

                var returned = Comments.Find(queryDoc).ToList();
                
                for(int i = 0; i < returned.Count; i++)
                {
                    Comment toAdd = new Comment(returned[i]["Text"].AsString, returned[i]["SenderId"].AsString, returned[i]["RecieverId"].AsString);
                    toAdd.PostTime = returned[i]["PostTime"].ToLocalTime();

                    toReturn.Add(toAdd);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return toReturn;
        }

        //Passed
        public bool EditComment(ref Comment comment)
        {
            try
            {
                BsonDocument doc = comment.ToBsonDocument();
                doc.Remove("ID");
                var queryDoc = new BsonDocument();
                queryDoc["_id"] = ObjectId.Parse(comment.ID);

                var something = Comments.Find(queryDoc).ToList();
                
                Comments.ReplaceOne(queryDoc, doc);

                comment.ID = doc["_id"].AsObjectId.ToString();
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        public bool AddFollower(string followerId, string leaderId)
        {
            try
            {
                BsonDocument doc = new BsonDocument();
                doc.Add("followerId", followerId);
                doc.Add("leaderId", leaderId);
                Followers.InsertOne(doc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public bool Unfollow(string followerId, string leaderId)
        {
            
            try
            {
                BsonDocument doc = new BsonDocument();
                doc.Add("followerId", followerId);
                doc.Add("leaderId", leaderId);
                Followers.DeleteOne(doc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        public bool IsFollowing(string followerId, string leaderId)
        {
            try
            {
                BsonDocument doc = new BsonDocument();
                doc.Add("followerId", followerId);
                doc.Add("leaderId", leaderId);
                return Followers.Find(doc).ToList().Count() > 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public int GetFollowersNum(string leaderId)
        {
            try
            {
                BsonDocument doc = new BsonDocument();
                doc.Add("leaderId", leaderId);
                var f = Followers.Find(doc).ToList();
                return f.Count;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return -1;
            }
        }

        public List<User> GetFollowers(string leaderId)
        {
            try
            {
                BsonDocument doc = new BsonDocument();
                doc.Add("leaderId", leaderId);
                var f = Followers.Find(doc).ToList().Select(x => x["followerId"].AsString).ToList();

                List<User> followers = new List<User>();
                f.ForEach(x => followers.Add(GetUserByID(x)));

                return followers;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        //Passed
        public bool RateGame(int gameId, string userId, int rating)
        {
            try
            {
                BsonDocument doc = new BsonDocument();
                doc.Add("gameId", gameId);
                doc.Add("userId", userId);
                doc.Add("rating", rating);
                Ratings.InsertOne(doc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        //Passed
        public float GetAverageRating(int gameId)
        {
            float result = 0.0f;
            try
            {
                var queryDoc = new BsonDocument();
                queryDoc.Add("gameId", gameId);

                var ratings = Ratings.Find(queryDoc).ToList();

                foreach (var rating in ratings)
                {
                    result += rating["rating"].AsInt32;
                }
                result /= ratings.Count;
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        //Passed
        public bool EditRating(int gameId, string userId, int rating)
        {
            try
            {
                BsonDocument doc = new BsonDocument();
                doc.Add("gameId", gameId);
                doc.Add("userId", userId);
                doc.Add("rating", rating);

                var queryDoc = new BsonDocument();
                queryDoc.Add("gameId", gameId);
                queryDoc.Add("userId", userId);

                Ratings.ReplaceOne(queryDoc, doc);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        //Helper
        public static DateTime UnixTimeStampToDateTime(Int32 unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
    }
}
