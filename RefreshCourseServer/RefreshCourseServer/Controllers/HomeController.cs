﻿using Microsoft.AspNetCore.Mvc;
using RefreshCourseServer.Data;
using RefreshCourseServer.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace RefreshCourseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HomeController : Controller
    {
        // Получение нагрузки преподавателя
        [HttpGet, Authorize]
        public async Task<IActionResult> GetWorkLoad()
        {
            //var email = User.FindFirst("sub")?.Value;
            string userName = User.FindFirstValue(ClaimTypes.Name);

            using (var serviceScope = ServiceActivator.GetScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();

                if (dbContext != null)
                {
                    // Поиск пользователя в БД с курсами по логину
                    var user = await dbContext.Teachers
                        .Where(u => u.Email == userName)
                        .Select(u => u.Id)
                        .FirstOrDefaultAsync();

                    if (user != null)
                    {
                        //var result = await dbContext.WorkLoads
                        //    .Include(x => x.Group)
                        //    .Include(x => x.Subject)
                        //    .Include(x => x.LessonType)
                        //    .Where(x => x.Id == user)
                        //    .OrderBy(u => u.Id)
                        //    .ToArrayAsync();

                        var result = await dbContext.WorkLoads
                            .Include(x => x.Group)
                            .Include(x => x.Subject)
                            .Include(x => x.LessonType)
                            .Where(x => x.Subject.Teacher.Id == user)
                            //.Select(x => new {x.Id, x.Subject.SubjectName, x.LessonType.LessonName, x.HoursCount })
                            .Join(dbContext.Groups, x => x.Id, y => y.Id, (x,y) => new
                            {
                                Id = x.Id,
                                GroupId = y.Id,
                                SubjectName = x.Subject.SubjectName,
                                LessonType = x.LessonType.LessonName,
                                HoursCount = x.HoursCount,
                                PayHour = x.Subject.SubjectPayment * x.LessonType.PaymentCoeff,
                                Money = x.Subject.SubjectPayment * x.LessonType.PaymentCoeff * x.HoursCount
                            })
                            .OrderBy(u => u.Id)
                            .ToArrayAsync();

                        if (result != null)
                            return Ok(result);
                    }
                }
            }

            return BadRequest();
        }
    }
}