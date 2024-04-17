﻿using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RefreshCourseServer.Data;
using RefreshCourseServer.Models;
using RefreshCourseServer.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RefreshCourseServer.Data.Encrypt;

namespace RefreshCourseServer.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _config;

        public AuthController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        // Контроллер обмена ключами
        [HttpPost]
        public async Task<IActionResult> SwapKeys(SwapModel requestData)
        {
            if (requestData != null && requestData.Email != string.Empty && requestData.PublicKey != string.Empty)
            {
                using (var serviceScope = ServiceActivator.GetScope())
                {
                    // Подключение к БД с пользователями
                    var dbContext = serviceScope.ServiceProvider.GetService<AuthDbContext>();

                    if (dbContext != null)
                    {
                        // Поиск пользователя в этой БД
                        AppUser? currentUser = await dbContext.Users
                            .Where(x => x.Email == requestData.Email)
                            .FirstOrDefaultAsync();

                        if (currentUser != null)
                        {
                            // Запись его публичного ключа в БД
                            currentUser.PublicKey = requestData.PublicKey;
                            await dbContext.SaveChangesAsync();

                            // Возврат клиенту публичного ключа сервера
                            return Ok(_config.GetValue<string>("Keys:PublicKey")!);
                        }
                        else
                            return BadRequest("Неверный логин или пароль");
                    }
                }
            }

            return BadRequest();
        }

        // Контроллер для авторизации
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginData)
        {
            if (ModelState.IsValid && loginData != null && loginData.Email != null && loginData.Password != null)
            {
                using (var serviceScope = ServiceActivator.GetScope())
                {
                    // Подключение к БД с пользователями и поиск пользователя по email
                    var dbContext = serviceScope.ServiceProvider.GetService<AuthDbContext>();
                    AppUser? user = _userManager.FindByEmailAsync(loginData.Email).Result;

                    if (user != null && dbContext != null)
                    {
                        // Вычисление общего приватного ключа и расшифровка пароля клиента
                        string serverPrivateKey = _config.GetValue<string>("Keys:PrivateKey")!;
                        string privateKey = VKOGost.GetHash(serverPrivateKey, user.PublicKey!);
                        loginData.Password = CipherEngine.DecryptString(loginData.Password, privateKey);

                        // Попытка авторизации с логином и паролем
                        var result = await _signInManager.PasswordSignInAsync(user.UserName!, loginData.Password, true, lockoutOnFailure: true);

                        if (result.Succeeded)
                        {
                            Response.Cookies.Delete(".AspNetCore.Identity.Application");

                            // Возврат токена для доступа
                            return Ok(JwtToken.GenerateToken(user, _config));
                        }

                        if (result.IsLockedOut)
                            return Unauthorized("Вы превысили максимальное количество попыток входа. Попробуйте позже");

                        return Unauthorized("Неверное имя пользователя или пароль");
                    }
                    else
                    {
                        ModelState.AddModelError("Input.Error", "Нет учетной записи пользователя с таким логином");
                        return Unauthorized("Неверное имя пользователя или пароль");
                    }
                }
            }

            return BadRequest("Произошла ошибка. Попробуйте еще раз");
        }

        // Контроллер для регистрации пользователей (for privileged user only)
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel loginData)
        {
            if (ModelState.IsValid && loginData != null)
            {
                // Создаем учетную запись пользователя и записываем в БД
                var user = new AppUser
                {
                    Email = loginData.Email,
                    UserName = loginData.UserName,
                    Initials = loginData.Initials,
                    PublicKey = string.Empty
                };

                var result = await _userManager.CreateAsync(user, loginData.Password);

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                if (result.Succeeded)
                    return Ok();
            }

            return BadRequest("Произошла ошибка. Попробуйте еще раз");
        }

        // Контроллер выхода из системы
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            string? email = User.FindFirstValue(ClaimTypes.Email);

            if (ModelState.IsValid && email != null)
            {
                using (var serviceScope = ServiceActivator.GetScope())
                {
                    // Подключение к БД с пользователями
                    var dbContext = serviceScope.ServiceProvider.GetService<AuthDbContext>();

                    if (dbContext != null)
                    {
                        // Поиск пользователя в этой БД
                        var currentUser = await dbContext.Users
                            .Where(x => x.Email == email)
                            .FirstOrDefaultAsync();

                        if (currentUser != null)
                        {
                            // Выходим из системы и удаляем публичный ключ пользователя
                            await _signInManager.SignOutAsync();

                            currentUser.PublicKey = string.Empty;
                            await dbContext.SaveChangesAsync();

                            return Ok();
                        }
                    }
                }
            }

            return BadRequest();
        }
    }
}