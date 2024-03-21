using Microsoft.AspNetCore.Mvc;
using RefreshCourseServer.Data;
using RefreshCourseServer.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RefreshCourseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]/{id}")]
    public class HomeController : Controller
    {
        // Получение нагрузки преподавателя
        [HttpGet, Authorize]
        public async Task<IActionResult> GetWorkLoad(int id)
        {
            //var email = User.FindFirst("sub")?.Value;
            //string t = User.FindFirstValue(ClaimTypes.Name);

            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
                
                if (dbContext != null)
                {
                    var result = await dbContext.Groups
                        //.Include("Speciality")
                        //.Include("Faculty")
                        .Include(x => x.Speciality)
                        .Include(x => x.Faculty)
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