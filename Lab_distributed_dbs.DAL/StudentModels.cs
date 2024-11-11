using MongoDB.Bson.Serialization.Attributes;

namespace Lab_distributed_dbs.DAL
{

    public class Student
    {
        [BsonId]
        [BsonElement("_id")]
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<StudentCourse> StudentCourses { get; set; }
        public ICollection<StudentEnrollment> StudentEnrollments{ get; set; }
    }

    public class Course
    {
        [BsonId]
        [BsonElement("_id")]
        public int CourseId { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }
        [BsonIgnore]
        public ICollection<StudentCourse> StudentCourses { get; set; }
    }

    public class StudentCourse
    {
        [BsonId]
        [BsonElement("_id")]
        public int StudentId { get; set; }
        [BsonIgnore]
        public Student Student { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }

    public class StudentEnrollment
    {
        [BsonId]
        [BsonElement("_id")]
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string Grade { get; set; }
        [BsonIgnore]
        public Student Student { get; set; }
        [BsonIgnore]
        public Course Course { get; set; }
    }
}
