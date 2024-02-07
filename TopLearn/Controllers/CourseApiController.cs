using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TopLearn.DataLayer.Context;

namespace TopLearn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseApiController : ControllerBase
    {
        TopLearnContext _context;

        public CourseApiController(TopLearnContext context)
        {
            _context = context;
        }
        [HttpGet("search")]
        public async Task<IActionResult> search()
        {
            try
            {
                string term = HttpContext.Request.Query["term"].ToString();
                var courseTitle = _context.Courses.Where(x => x.CourseTitle.Contains(term)).Select(x => x.CourseTitle).ToList();
                return Ok(courseTitle);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
