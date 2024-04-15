namespace RefreshCourseServer.Models.Database
{
    // Структура таблицы Предметы
    public class Subject
    {
        public int Id { get; set; }
        public Teacher Teacher { get; set; }
        public string? SubjectName { get; set; }
        public int SubjectPayment { get; set; }
    }
}
