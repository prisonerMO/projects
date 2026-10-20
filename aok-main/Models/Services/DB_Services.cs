using AOK.Models.Services;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AOK.Models.Services
{
    public class DB_Services
    {
        private static string? DATABASE_NAME;
        private static string? DATABASE_ADDRESS;
        private static MongoServerAddress? address;
        private static MongoClientSettings? settings;
        private static MongoClient? client;
        private static IMongoDatabase? database;
        private static IConfiguration? config;
        public static void Initialize(IConfiguration configuration)
        {
            config = configuration;
            var sections = config.GetSection("ConnectionStrings");
            DATABASE_ADDRESS = sections.GetValue<string>("MongoAddress");
            DATABASE_NAME = sections.GetValue<string>("MongoDatabaseName");
            address = new MongoServerAddress(DATABASE_ADDRESS);
            settings = new MongoClientSettings() { Server = address };
            client = new MongoClient(settings);
            database = client.GetDatabase(DATABASE_NAME);
        }

        private static IMongoDatabase GetDB()
        {
            if (database == null)
            {
                throw new Exception("Database not initialized");
            }
            else
            {
                return database;
            }
        }

        public static T Save<T>(T obj) where T : DB_Object
        {
            try
            {
                var collection = GetDB().GetCollection<T>(typeof(T).Name);
                if (obj._id == ObjectId.Empty)
                {
                    collection.InsertOne(obj);
                }
                else
                {
                    collection.ReplaceOne(
                    new BsonDocument("_id", obj._id),
                        obj,
                        new ReplaceOptions
                        {
                            IsUpsert = true
                        });
                }

                return obj;
            }
            catch (Exception ex)
            {
                return obj;
            }
        }

        public static void Delete<T>(ObjectId id) where T : DB_Object
        {
            var collection = GetDB().GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", id);
            try
            {
                collection.DeleteOne(filter);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public static T GetById<T>(ObjectId id) where T : DB_Object
        {
            var collection = GetDB().GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", id);
            return collection.Find(filter).FirstOrDefault();
        }

        public static async Task<T> GetObjectById<T>(ObjectId id) where T : DB_Object
        {
            var collection = GetDB().GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("_id", id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public static List<T> GetAll<T>() where T : DB_Object
        {
            var collection = GetDB().GetCollection<T>(typeof(T).Name);
            return collection.Find(new BsonDocument()).ToList();
        }

        public static async Task<List<T>> GetAllAsync<T>() where T : DB_Object
        {
            var collection = GetDB().GetCollection<T>(typeof(T).Name);
            var cursor = await collection.FindAsync(new BsonDocument());
            return await cursor.ToListAsync();
        }

        public static T GetUserByUsername<T>(string username) where T : DB_Object
        {
            var collection = GetDB().GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Regex("Username", new BsonRegularExpression(username, "i"));
            var result = collection.Find(filter).ToList();
            return result.FirstOrDefault();
        }

        public static List<T> GetAllByName<T>(string name) where T : DB_Object
        {
            var collection = GetDB().GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Regex("name", new BsonRegularExpression(name, "i"));
            return collection.Find(filter).ToList();
        }
        public static List<T> GetAllByUsername<T>(string username) where T : DB_Object
        {
            var collection = GetDB().GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Regex("Username", new BsonRegularExpression(username, "i"));
            return collection.Find(filter).ToList();
        }

        public static T GetResultFrom<T>(string from, string key) where T : DB_Object
        {
            var collection = GetDB().GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq(from, key);
            var result = collection.Find(filter).ToList();
            return result.FirstOrDefault();
        }

        public static List<string> GetAllRoles(User user)
        {
            if (user != null)
            {
                return user.Roles;
            }

            return new List<string>();
        }

        public static List<T> GetCommunityById<T>(ObjectId communityId) where T : DB_Object
        {
            var collection = GetDB().GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("Community", communityId);
            var result = collection.Find(filter).ToList();
            return result;
        }

        public static async Task<ObjectId> GetUserCommunityId(string username)
        {
            var collection = GetDB().GetCollection<User>("User");
            var filter = Builders<User>.Filter.Eq(u => u.Username, username);
            var user = await collection.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {               
                return ObjectId.Empty;
            }
            return user.Community;
        }

        public static List<T> GetOwnCommunityUsersById<T>(ObjectId communityId) where T : DB_Object
        {
            var collection = GetDB().GetCollection<T>(typeof(T).Name);
            var filter = Builders<T>.Filter.Eq("Community", communityId);
            var result = collection.Find(filter).ToList();
            return result;
        }
        public static async Task<List<User>> GetUsersInSameCommunityAsync(string username)
        {
            
            var communityId = await GetUserCommunityId(username);
            if (communityId == ObjectId.Empty)
                return new List<User>();
            return GetOwnCommunityUsersById<User>(communityId);
        }
                    

    }
}


