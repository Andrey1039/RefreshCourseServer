namespace RefreshCourseServer.Models.Database
{
    // Структура таблицы Преподаватели
    public class Teacher
    {
        public int Id { get; set; }
        public string? Surname { get; set; }
        public string? Name { get; set; }
        public string? Patronymic { get; set; }
        public string? Email { get; set; }
    }
}
