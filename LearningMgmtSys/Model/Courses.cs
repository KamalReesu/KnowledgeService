using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LearningMgmtSys.Model
{
    public class Courses
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [JsonPropertyName("Name")]
        [Required]
        [MinLength(20)]
        public string CourseName { get; set; } = null!;

        [Required]
        public decimal Duration { get; set; }

        [Required]
        [MinLength(100)]
        public string Description { get; set; } = null!;

        [Required]
        public string Technology { get; set; } = null!;

        [Required]
        public string LaunchURL { get; set; }
    }
}
