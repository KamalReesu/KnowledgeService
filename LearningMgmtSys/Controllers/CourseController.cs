using LearningMgmtSys.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningMgmtSys.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/lms/[controller]")]
    public class CourseController : Controller
    {
        private readonly CourseService _courseService;

        public CourseController(CourseService courseService) =>
            _courseService = courseService;

       //[Authorize]
        [HttpGet("getall")]
        public async Task<List<Courses>> Get() =>
            await _courseService.GetAsync();

       // [Authorize]
        [HttpGet("info/{technology}")]
        public async Task<ActionResult<Courses>> Get(string technology)
        {
            var course = await _courseService.GetAsync(technology);

            if (course is null)
            {
                return NotFound();
            }

            return course;
        }
      //  [Authorize]
        [HttpGet("get/{technology}/{durationFromRange}/{durationToRange}")]
        public async Task<ActionResult<Courses>> GetCourse(string technology, decimal durationFromRange, decimal durationToRange)
        {
            var course = await _courseService.GetCourseAsync(technology, durationFromRange, durationToRange);

            if (course is null)
            {
                return NotFound();
            }

            return course;
        }
       // [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> Post([FromBody] Courses course)
        {
            var courseobj = await _courseService.GetAsync(course.Technology);
            if(courseobj is null)
            {
                await _courseService.CreateAsync(course);
            }
            return CreatedAtAction(nameof(Get), new { id = course.Id }, course);
        }

        //[HttpPut("{id:length(24)}")]
        //public async Task<IActionResult> Update(string id, Courses updatedCourse)
        //{
        //    var book = await _courseService.GetAsync(id);

        //    if (book is null)
        //    {
        //        return NotFound();
        //    }

        //    updatedCourse.Id = book.Id;

        //    await _courseService.UpdateAsync(id, updatedCourse);

        //    return NoContent();
        //}
        //[Authorize]
        [HttpDelete("delete/{coursename}")]
        public async Task<IActionResult> Delete(string coursename)
        {
            var book = await _courseService.GetAsync(coursename);

            if (book is null)
            {
                return NotFound();
            }

            await _courseService.RemoveAsync(coursename);

            return NoContent();
        }
    }
}
