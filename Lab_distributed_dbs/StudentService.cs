using Lab_distributed_dbs.DAL.Settings;
using Lab_distributed_dbs.DAL;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Lab_distributed_dbs.Services
{
    public class StudentService
    {
        private readonly IMongoCollection<Student> _studentsCollection;
        public StudentService(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient
        mongoClient)
        {
            var mongoDatabase =
            mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _studentsCollection = mongoDatabase.GetCollection<Student>("Students");
        }
        public async Task<List<Student>> GetAsync() =>
        await _studentsCollection.Find(s => true).ToListAsync();
        public async Task<Student> GetByIdAsync(int id) =>
        await _studentsCollection.Find(s => s.StudentId == id).FirstOrDefaultAsync();
        public async Task CreateAsync(Student student) =>
        await _studentsCollection.InsertOneAsync(student);
        public async Task UpdateAsync(int id, Student updatedStudent) =>
        await _studentsCollection.ReplaceOneAsync(s => s.StudentId == id, updatedStudent);
        public async Task RemoveAsync(int id) =>
        await _studentsCollection.DeleteOneAsync(s => s.StudentId == id);
    }

    public class StudentEnrollmentService
    {
        private readonly IMongoCollection<StudentEnrollment> _studentEnrollmentCollection;
        public StudentEnrollmentService(IOptions<MongoDBSettings> mongoDBSettings, IMongoClient
        mongoClient)
        {
            var mongoDatabase =
            mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _studentEnrollmentCollection = mongoDatabase.GetCollection<StudentEnrollment>("StudentEnrollments");
        }
        public async Task<List<StudentEnrollment>> GetAsync() =>
        await _studentEnrollmentCollection.Find(l => true).ToListAsync();
        public async Task<StudentEnrollment> GetByIdAsync(int id) =>
        await _studentEnrollmentCollection.Find(l => l.EnrollmentId == id).FirstOrDefaultAsync();
        public async Task CreateAsync(StudentEnrollment lecturer) =>
        await _studentEnrollmentCollection.InsertOneAsync(lecturer);
        public async Task UpdateAsync(int id, StudentEnrollment updatedEnrollment) =>
        await _studentEnrollmentCollection.ReplaceOneAsync(l => l.EnrollmentId == id, updatedEnrollment);
        public async Task RemoveAsync(int id) =>
        await _studentEnrollmentCollection.DeleteOneAsync(l => l.EnrollmentId == id);
    }

}
