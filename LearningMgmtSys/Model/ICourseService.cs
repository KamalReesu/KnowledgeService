using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningMgmtSys.Model
{
    public interface ICourseService
    {
        public Task<List<Courses>> GetAsync();
        public Task<Courses?> GetAsync(string name);
        public Task<List<Courses>> GetCoursebyTechnology(string name);

        public Task<Courses> GetCoursebyId(string id);
        public Task<List<Courses>> GetCourseAsync(string name, decimal FromRange, decimal toRange);

        public Task CreateAsync(Courses newBook);

        public Task RemoveAsync(string coursename);

    }
}
