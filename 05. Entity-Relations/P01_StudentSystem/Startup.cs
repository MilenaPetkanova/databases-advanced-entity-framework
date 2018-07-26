namespace P01_StudentSystem
{
    using System;
    using P01_StudentSystem.Data;
    using P01_StudentSystem.Data.Models;

    public class Startup
    {
        public static void Main()
        {
            //using (var context = new StudentSystemContext())
            //{
            //    var student1 = new Student
            //    {
            //        Name = "TestName1",
            //        PhoneNumber = "11111111",
            //        RegisteredOn = DateTime.Now
            //    };

            //    var student2 = new Student
            //    {
            //        Name = "TestName1",
            //        PhoneNumber = "11111111",
            //        RegisteredOn = DateTime.Now
            //    };

            //    context.Students.AddRange(student1, student2);

            //    var course1 = new Course
            //    {
            //        Name = "TestName",
            //        StartDate = DateTime.Now,
            //        EndDate = DateTime.Now,
            //        Price = (decimal)99.9
            //    };

            //    context.Courses.Add(course1);

            //    var resource1 = new Resource
            //    {
            //        Name = "TestName",
            //        Url = "someurl.com",
            //        ResourceType = ResourceType.Presentation,
            //        CourseId = 1
            //    };

            //    var resource2 = new Resource
            //    {
            //        Name = "TestName",
            //        Url = "someurl.com",
            //        ResourceType = ResourceType.Presentation,
            //        CourseId = 1
            //    };

            //    context.Resources.AddRange(resource1, resource2);

            //    var homework = new Homework
            //    {
            //        Content = "Test content",
            //        ContentType = ContentType.Application,
            //        SubmissionTime = DateTime.Now,
            //        StudentId = 1,
            //        CourseId = 1
            //    };

            //    context.Homeworks.Add(homework);

            //    var studentCourse = new StudentCourse
            //    {
            //        StudentId = 1,
            //        CourseId = 1
            //    };

            //    context.StudentsCourses.Add(studentCourse);

            //    context.SaveChanges();
            //}
        }
    }
}
