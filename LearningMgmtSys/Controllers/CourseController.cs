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
        private readonly ICourseService _courseService;
        private readonly IRabitMQProducer _rabitMQProducer;
        public CourseController(ICourseService courseService,IRabitMQProducer rabitMQProducer)
        {
            _rabitMQProducer = rabitMQProducer;
            _courseService = courseService;
        }

        [Authorize(Roles = "Administrator,User")]
        [HttpGet("getall")]
        public async Task<List<Courses>> GetAllCourses() =>
            await _courseService.GetAsync();

        [Authorize(Roles = "Administrator,User")]
        [HttpGet("info/{technology}")]
        public async Task<ActionResult<List<Courses>>> GetCoursebyTechnology(string technology)
        {
            var courses = await _courseService.GetCoursebyTechnology(technology);

            if (courses is null)
            {
                return NotFound(new { message = "Course Not Found" });
            }

            return courses;
        }
        [Authorize(Roles = "Administrator,User")]
        [HttpGet("get/{technology}/{durationFromRange}/{durationToRange}")]
        public async Task<ActionResult<List<Courses>>> GetCoursebyTechnologyandduration(string technology, decimal durationFromRange, decimal durationToRange)
        {
            var courses = await _courseService.GetCourseAsync(technology, durationFromRange, durationToRange);

            if (courses is null)
            {
                return NotFound(new {message = "Course Not Found" });
            }

            return courses;
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost("add")]
        public async Task<IActionResult> AddCourse([FromBody] Courses course)
        {
            var courseobj = await _courseService.GetCoursebyId(course.Id);
            if(courseobj is null)
            {
                await _courseService.CreateAsync(course);
                //send the inserted product data to the queue and consumer will listening this data from queue
                // _rabitMQProducer.SendProductMessage(courseobj);
                return Ok(new { course = course.CourseName, status = "Course created successfully" });
            }
            
            
            return NotFound(new { message = "There was some error while adding course" });
        }

        [Authorize(Roles ="Administrator")]
        [HttpDelete("delete/{coursename}")]
        public async Task<IActionResult> DeleteCourse(string coursename)
        {
            var book = await _courseService.GetAsync(coursename);

            if (book is null)
            {
                return NotFound(new { message = "Course Not Found" });
            }

            await _courseService.RemoveAsync(coursename);

            return Ok(new { courseName = coursename, status = "Course with "+coursename+" Deleted successfully" });
        }
    }
}
