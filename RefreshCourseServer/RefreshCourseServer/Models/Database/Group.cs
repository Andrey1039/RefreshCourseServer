namespace RefreshCourseServer.Models.Database
{
    public class Group
    {
        public int Id { get; set; }
        public Speciality Speciality { get; set; }
        public Faculty Faculty { get; set; }
        public int StudentsCount { get; set; }
    }
}
