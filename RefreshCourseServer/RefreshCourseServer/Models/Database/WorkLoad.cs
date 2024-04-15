using System.ComponentModel.DataAnnotations;

namespace RefreshCourseServer.Models.Database
{
    // Структура таблицы Нагрузки преподавателей
    public class WorkLoad
    {
        public int Id { get; set; }
        public Group Group { get; set; }
        public Subject Subject { get; set; }
        public LessonType LessonType { get; set; }
        public int HoursCount { get; set; }
    }
}
