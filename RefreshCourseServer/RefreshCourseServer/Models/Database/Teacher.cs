﻿namespace RefreshCourseServer.Models.Database
{
    public class Teacher
    {
        public int Id { get; set; }
        public string? Surname { get; set; }
        public string? Name { get; set; }
        public string? Patronymic { get; set; }
        public string? Phone { get; set; }
        public double Experience { get; set; }
    }
}
