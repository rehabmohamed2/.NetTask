using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentCoursesSystem
{
    // Entity Classes
    public class Student
    {
        public int StudentId { get; set; }
        public string Name { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }

    public class Teacher
    {
        public int TeacherId { get; set; }
        public string Name { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
    }

    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public List<Student> Students { get; set; } = new List<Student>();
    }

    public class StudentCourseService
    {
        private List<Student> students;
        private List<Course> courses;
        private List<Teacher> teachers;

        public StudentCourseService(List<Student> students, List<Course> courses, List<Teacher> teachers)
        {
            this.students = students;
            this.courses = courses;
            this.teachers = teachers;
        }

        // Requirement 2: Get the most enrolled course
        public Course GetMostEnrolledCourse()
        {
            return courses
                .OrderByDescending(c => c.Students.Count)
                .FirstOrDefault();
        }

        // Requirement 3: Return each course with number of students enrolled
        public IEnumerable<object> GetCoursesWithStudentCountAnonymous()
        {
            return courses
                .Select(c => new
                {
                    CourseName = c.CourseName,
                    StudentCount = c.Students.Count
                });
        }

        // Requirement 4: Verify if a teacher has more than one unique course
      
        public bool HasTeacherMoreThanOneCourse(Teacher teacher)
        {
            return teacher.Courses
                .Select(c => c.CourseId) 
                .Distinct()
                .Count() > 1;
        }

        // Requirement 5: Method that takes teacher as parameter and returns number of students
        public int GetStudentCountByTeacher(Teacher teacher)
        {
            return courses
                .Where(c => c.TeacherId == teacher.TeacherId)
                .SelectMany(c => c.Students)
                .Distinct()
                .Count();
        }

       

        // Requirement 6: Get idle teachers (teachers with no courses)
        public List<Teacher> GetIdleTeachers()
        {
            return teachers
                .Where(t => !courses.Any(c => c.TeacherId == t.TeacherId))
                .ToList();
        }

       
    }
   
    class Program
    {
        static void Main(string[] args)
        {
            var teachers = new List<Teacher>
            {
                new Teacher { TeacherId = 1, Name = "Dr. Smith" },
                new Teacher { TeacherId = 2, Name = "Prof. Johnson" },
                new Teacher { TeacherId = 3, Name = "Dr. Williams" },
                new Teacher { TeacherId = 4, Name = "Prof. Brown" }
            };

            var students = new List<Student>
            {
                new Student { StudentId = 1, Name = "Alice" },
                new Student { StudentId = 2, Name = "Bob" },
                new Student { StudentId = 3, Name = "Charlie" },
                new Student { StudentId = 4, Name = "David" },
                new Student { StudentId = 5, Name = "Eve" }
            };

            var courses = new List<Course>
            {
                new Course { CourseId = 1, CourseName = "Mathematics", TeacherId = 1, Teacher = teachers[0] },
                new Course { CourseId = 2, CourseName = "Physics", TeacherId = 1, Teacher = teachers[0] },
                new Course { CourseId = 3, CourseName = "Chemistry", TeacherId = 2, Teacher = teachers[1] },
                new Course { CourseId = 4, CourseName = "Biology", TeacherId = 3, Teacher = teachers[2] }
            };

            courses[0].Students.AddRange(new[] { students[0], students[1], students[2], students[3] });
            students[0].Courses.Add(courses[0]);
            students[1].Courses.Add(courses[0]);
            students[2].Courses.Add(courses[0]);
            students[3].Courses.Add(courses[0]);

            courses[1].Students.AddRange(new[] { students[0], students[1] });
            students[0].Courses.Add(courses[1]);
            students[1].Courses.Add(courses[1]);

            courses[2].Students.AddRange(new[] { students[2], students[3], students[4] });
            students[2].Courses.Add(courses[2]);
            students[3].Courses.Add(courses[2]);
            students[4].Courses.Add(courses[2]);

            courses[3].Students.Add(students[4]);
            students[4].Courses.Add(courses[3]);

            teachers[0].Courses.AddRange(new[] { courses[0], courses[1] });
            teachers[1].Courses.Add(courses[2]);
            teachers[2].Courses.Add(courses[3]);

            var service = new StudentCourseService(students, courses, teachers);

            Console.WriteLine("===== Student-Course-Teacher System =====\n");

            Console.WriteLine("1. Most Enrolled Course:");
            var mostEnrolled = service.GetMostEnrolledCourse();
            Console.WriteLine($"   {mostEnrolled.CourseName} with {mostEnrolled.Students.Count} students\n");

            Console.WriteLine("2. All Courses with Student Count:");
            var coursesWithCount = service.GetCoursesWithStudentCountAnonymous();
            foreach (var item in coursesWithCount)
            {
                var course = item as dynamic;
                Console.WriteLine($"   {course.CourseName}: {course.StudentCount} students");
            }
            Console.WriteLine();

            Console.WriteLine("3. Teachers with More Than One Course:");
            foreach (var teacher in teachers)
            {
                bool hasMultipleCourses = service.HasTeacherMoreThanOneCourse(teacher);
                Console.WriteLine($"   {teacher.Name}: {(hasMultipleCourses ? "Yes" : "No")}");
            }
            Console.WriteLine();

            Console.WriteLine("4. Number of Students per Teacher:");
            foreach (var teacher in teachers)
            {
                int studentCount = service.GetStudentCountByTeacher(teacher);
                Console.WriteLine($"   {teacher.Name}: {studentCount} students");
            }
            Console.WriteLine();

            Console.WriteLine("5. Idle Teachers:");
            var idleTeachers = service.GetIdleTeachers();
            if (idleTeachers.Any())
            {
                foreach (var teacher in idleTeachers)
                {
                    Console.WriteLine($"   {teacher.Name}");
                }
            }
            else
            {
                Console.WriteLine("   No idle teachers found.");
            }

            Console.WriteLine("\n===== End of Report =====");
        }
    }
}
