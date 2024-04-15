namespace RefreshCourseServer.Models.Database
{
    // Структура таблицы Типы занятий
    public class LessonType
    {
        public int Id { get; set; }
        public string? LessonName { get; set; }
        public double PaymentCoeff { get; set; }
    }
}
