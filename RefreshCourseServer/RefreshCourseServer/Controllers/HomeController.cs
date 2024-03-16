using Microsoft.AspNetCore.Mvc;
using RefreshCourseServer.Data;
using RefreshCourseServer.Service;
using Microsoft.EntityFrameworkCore;
using RefreshCourseServer.Models.Database;

namespace RefreshCourseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]/{id}")]
    public class HomeController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> GetWorkLoad(int id)
        {
            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
                
                if (dbContext != null)
                {
                    var result = await dbContext.Groups
                        .Include("Speciality")
                        .Include("Faculty")
                        .Where(x => x.Id == id)
                        .FirstOrDefaultAsync();

                    if (result != null)
                        return Ok(result);
                }
            }

            return BadRequest();
        }
    }
}