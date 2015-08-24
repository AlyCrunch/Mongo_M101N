using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestMongo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Doc(args);
            //Poco(args);
            //InsertOneAndMany(args).Wait();
            //Find().Wait();
            //RemoveHomeworkWithTheLowestScore(args).Wait();
            //RemoveHomeworkWithTheLowestScoreOneToMany(args).Wait();
            Console.WriteLine("\nFinish - Press any to exit");
            Console.ReadLine();
        }

        static void Doc(string[] args)
        {

            var doc = new BsonDocument
            {
                { "name" , "Jones" }
            };

            doc.Add("age", 30);

            doc["profession"] = "hacker";

            var nestedArray = new BsonArray();
            nestedArray.Add(new BsonDocument("color", "red"));

            doc.Add("array", nestedArray);

            Console.WriteLine(doc["array"][0]["color"]);


            Console.WriteLine(doc);
        }

        static void Poco(string[] args)
        {
            var conventionPack = new ConventionPack();
            conventionPack.Add(new CamelCaseElementNameConvention());
            ConventionRegistry.Register("camelCase", conventionPack, t => true);

            BsonClassMap.RegisterClassMap<Person>(cm =>
            {
                cm.AutoMap();
                cm.MapMember(x => x.Name).SetElementName("name");
            });

            var person = new Person
            {
                Name = "Jones",
                Age = 30,
                Colors = new List<string> { "red", "blue" },
                Pets = new List<Pet> { new Pet { Name = "Fluffy", Type = "Pig" } },
                ExtraElements = new BsonDocument("anotherName", "anotherValue")
            };

            using (var writer = new JsonWriter(Console.Out))
            {
                BsonSerializer.Serialize(writer, person);
            }
        }

        static async Task InsertOneAndMany(string[] args)
        {
            var client = new MongoClient();
            var db = client.GetDatabase("test");

            var col = db.GetCollection<BsonDocument>("people");
            var peopleCollection = db.GetCollection<Person>("people");

            var doc = new BsonDocument
            {
                { "Name", "Emily" },
                { "Age", 25 },
                { "Profession", "Designer" }
            };

            var doc2 = new BsonDocument
            {
                { "Name", "Georges" },
                { "Age", 28 },
                { "Profession", "Doctor" }
            };

            var person = new Person
            {
                Name = "Martin",
                Age = 49,
                Profession = "Hacker"
            };

            await col.InsertOneAsync(doc2);
            doc2.Remove("_id");
            await col.InsertOneAsync(doc2);

            //await col.InsertManyAsync(new[] { doc, doc2 });
            //await peopleCollection.InsertOneAsync(person);
        }

        //HW 2.2
        static async Task RemoveHomeworkWithTheLowestScore(string[] args)
        {
            var client = new MongoClient();
            var db = client.GetDatabase("students");

            var col = db.GetCollection<Grade>("grades");

            var filters = Builders<Grade>.Filter.Eq(x => x.type, "homework");

            var list = await col.Find(filters)
                .SortBy(g => g.student_id).ThenBy(g => g.score)
                .ToListAsync();
            int idStud = -1;
            foreach (var doc in list)
            {
                if (idStud != doc.student_id)
                {
                    col.DeleteOneAsync(Builders<Grade>.Filter.Eq(x => x._id, doc._id)).Wait();
                }
                idStud = doc.student_id;
            }
        }
        
        //HW 3.1
        static async Task RemoveHomeworkWithTheLowestScoreOneToMany(string[] args)
        {
            var client = new MongoClient();
            var db = client.GetDatabase("school");
            var col = db.GetCollection<Students>("students");
            
            var list = await col.Find(_ => true).ToListAsync();

            foreach (var doc in list)
            {
                var leDoc = doc.Scores.Where(x => x.Type == "homework").OrderBy(x => x.Note);

                //Console.WriteLine(doc._id + " " + doc.Name + " : \"" + leDoc.First().Type + "\" : " + leDoc.First().Note);

                var filter = Builders<Students>.Filter.Eq(x => x._id, doc._id);
                var update = Builders<Students>.Update.Pull(x => x.Scores, leDoc.First());
                col.UpdateOneAsync(filter, update).Wait();                
            }
        }
    }
}
