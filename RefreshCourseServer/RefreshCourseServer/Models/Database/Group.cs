namespace RefreshCourseServer.Models.Database
{
    // Структура таблицы Группы
    public class Group
    {
        public int Id { get; set; }
        public Speciality Speciality { get; set; }
        public Faculty Faculty { get; set; }
        public int StudentsCount { get; set; }
        public int Course { get; set; }
    }
}
