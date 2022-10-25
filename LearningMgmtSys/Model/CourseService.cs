using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningMgmtSys.Model
{
public class CourseService
    {
        private readonly IMongoCollection<Courses> _coursesCollection;

        public CourseService(
            IOptions<CourseStoreDatabaseSettings> courseStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                courseStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                courseStoreDatabaseSettings.Value.DatabaseName);

            _coursesCollection = mongoDatabase.GetCollection<Courses>(
                courseStoreDatabaseSettings.Value.CoursesCollectionName);
        }

        public async Task<List<Courses>> GetAsync() =>
            await _coursesCollection.Find(_ => true).ToListAsync();

        public async Task<Courses?> GetAsync(string name) =>
            await _coursesCollection.Find(x => x.Technology == name).FirstOrDefaultAsync();

        public async Task<Courses?> GetCourseAsync(string name,decimal FromRange, decimal toRange) =>
          await _coursesCollection.Find(x => x.Technology == name && (x.Duration <=toRange 
          && x.Duration >= FromRange)).FirstOrDefaultAsync();

        public async Task CreateAsync(Courses newBook) =>
            await _coursesCollection.InsertOneAsync(newBook);

        public async Task UpdateAsync(string id, Courses updatedBook) =>
            await _coursesCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

        public async Task RemoveAsync(string coursename) =>
            await _coursesCollection.DeleteOneAsync(x => x.CourseName == coursename);
    }
    }
