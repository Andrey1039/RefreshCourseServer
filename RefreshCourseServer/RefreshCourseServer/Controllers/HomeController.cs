using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RefreshCourseServer.Data;
using RefreshCourseServer.Service;
using Microsoft.EntityFrameworkCore;
using RefreshCourseServer.Data.Encrypt;
using Microsoft.AspNetCore.Authorization;

namespace RefreshCourseServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HomeController : Controller
    {
        private IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        // Получение нагрузки преподавателя из базы данных
        [HttpGet]
        public async Task<IActionResult> GetWorkLoad()
        {
            string? email = User.FindFirstValue(ClaimTypes.Email);

            using (var serviceScope = ServiceActivator.GetScope())
            {
                // Подключения к базам данных с нагрузкой и пользователями
                var dbContext = serviceScope.ServiceProvider.GetService<AppDbContext>();
                var authContext = serviceScope.ServiceProvider.GetService<AuthDbContext>();

                if (dbContext != null && authContext != null)
                {
                    // Поиск преподавателя в базе данных нагрузки по email
                    var user = await dbContext.Teachers
                        .Where(u => u.Email == email)
                        .FirstOrDefaultAsync();

                    // Поиск публичного ключа авторизованного пользователя
                    var userPublicKey = await authContext.Users
                        .Where(u => u.Email == email)
                        .Select(u => u.PublicKey)
                        .FirstOrDefaultAsync();

                    if (user != null)
                    {
                        // Вычисляем общий приватный ключ для шифрования
                        var privateKey = VKOGost.GetHash(_config.GetValue<string>("Keys:PrivateKey")!, userPublicKey!);

                        // Выборка записей из БД нагрузки для конкретного преподавателя
                        var result = await dbContext.WorkLoads
                            .Include(x => x.Group)
                            .Include(x => x.Subject)
                            .Include(x => x.LessonType)
                            .Where(x => x.Subject.Teacher.Id == user.Id)
                            .Select(x => new
                            {
                                Id = x.Id,
                                GroupName = $"{x.Group.Faculty.ShortName} {x.Group.Speciality.ShortName}-{x.Group.Course}",
                                SubjectName = x.Subject.SubjectName,
                                LessonType = x.LessonType.LessonName,
                                HoursCount = x.HoursCount,
                                
                                // Вычисление оплаты за час и суммы оплаты за предмет
                                PayHour = x.Subject.SubjectPayment * x.LessonType.PaymentCoeff,
                                Money = x.Subject.SubjectPayment * x.LessonType.PaymentCoeff * x.HoursCount
                            })
                            .OrderBy(u => u.Id)
                            .ToArrayAsync();

                        var jsonData = JsonConvert.SerializeObject(result);

                        // Отправляем зашифрованный json с результатом выборки
                        if (result != null)
                            return Ok(CipherEngine.EncryptString(jsonData.ToString()!, privateKey));
                    }
                }
            }

            return BadRequest("Отсутствуют данные по запросу");
        }
    }
}