using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMongo
{

    class Person : BsonDocument
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string Profession { get; set; }

        public List<string> Colors { get; set; }

        public List<Pet> Pets { get; set; }

        public BsonDocument ExtraElements { get; set; }
    }

    class Pet
    {
        public string Name { get; set; }

        public string Type { get; set; }
    }

    class Grade
    {
        public ObjectId _id { get; set; }
        public int student_id { get; set; }
        public string type { get; set; }
        public double score { get; set; }
    }

    class Students
    {
        public int _id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("scores")]
        public List<Score> Scores { get; set; }
    }

    class Score
    {
        [BsonElement("type")]
        public string Type { get; set; }

        [BsonElement("score")]
        public double Note { get; set; }
    }
}
