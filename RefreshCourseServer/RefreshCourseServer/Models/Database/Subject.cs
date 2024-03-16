namespace RefreshCourseServer.Models.Database
{
    public class Subject
    {
        public int Id { get; set; }
        public Teacher Teacher { get; set; }
        public string? SubjectName { get; set; }
        public int SubjectPayment { get; set; }
    }
}
