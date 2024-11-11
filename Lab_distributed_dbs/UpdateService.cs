using Lab_distributed_dbs.DAL;
using Lab_distributed_dbs.DAL.Settings;
using Lab_distributed_dbs.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Transactions;

namespace Lab_distributed_dbs
{
    public class UpdateService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly string _databaseName;
        private readonly IMongoClient _mongoClient;
        public UpdateService(IServiceScopeFactory serviceScopeFactory,
            IOptions<MongoDBSettings> mongoDBSettings,
            IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
            _serviceScopeFactory = serviceScopeFactory;
            _databaseName = mongoDBSettings.Value.DatabaseName;
        }
        public async Task StartAsync(CancellationToken cancellationToken) => await Sync();
        public async Task Sync()
        {
            await _mongoClient.DropDatabaseAsync(_databaseName);
            await using (var scope = _serviceScopeFactory.CreateAsyncScope())
            using (var dbContext = scope.ServiceProvider.GetRequiredService<LabDbContext>())
            using (var transaction = new
            TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var studentService =
            scope.ServiceProvider.GetRequiredService<StudentService>();
                var studentEnrollmentService =
            scope.ServiceProvider.GetRequiredService<StudentEnrollmentService>();
                try
                {
                    var students = dbContext.Students
                        .Include(s => s.StudentEnrollments)
                        .Include(s => s.StudentCourses)
                        .ThenInclude(sc => sc.Course)
                        .ToList();
                    foreach (var student in students)
                    {
                        await studentService.CreateAsync(student);
                    }
                    var enrollments = dbContext.StudentEnrollments.ToList();
                    foreach (var enrollment in enrollments)
                    {
                        await studentEnrollmentService.CreateAsync(enrollment);
                    }
                    transaction.Complete();
                }
                catch (Exception)
                {
                    transaction.Dispose();
                    throw;
                }
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
