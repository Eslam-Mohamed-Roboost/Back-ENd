using API.Domain.Entities;
using API.Domain.Entities.Gamification;
using API.Domain.Entities.Identity;
using API.Domain.Entities.Portfolio;
using API.Domain.Entities.Teacher;
using API.Domain.Entities.Users;
using API.Domain.Enums;
using API.Infrastructure.Persistence.Repositories;
using API.Shared.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClassEntity = API.Domain.Entities.General.Classes;
using SubjectEntity = API.Domain.Entities.General.Subjects;

namespace API.Application.Features.Admin.SeedData.Commands;

public record SeedDatabaseCommand : IRequest<RequestResult<string>>;

public class SeedDatabaseCommandHandler(
    RequestHandlerBaseParameters parameters,
    IRepository<Domain.Entities.User> userRepository,
    IRepository<ClassEntity> classRepository,
    IRepository<SubjectEntity> subjectRepository,
    IRepository<Domain.Entities.Teacher.TeacherSubjects> teacherSubjectsRepository,
    IRepository<Domain.Entities.Users.Badges> badgesRepository,
    IRepository<PortfolioFiles> portfolioRepository,
    IRepository<TeacherFeedback> teacherFeedbackRepository,
    IRepository<CpdModules> cpdModulesRepository,
    IRepository<TeacherCpdProgress> cpdProgressRepository,
    IRepository<StudentBadges> studentBadgesRepository,
    IRepository<Token> tokenRepository,
    IRepository<Domain.Entities.Gamification.Badges> gamificationBadgesRepository)
    : RequestHandlerBase<SeedDatabaseCommand, RequestResult<string>>(parameters)
{
    public override async Task<RequestResult<string>> Handle(SeedDatabaseCommand request, CancellationToken cancellationToken)
    {
        // Clear existing data (optional - comment out if you want to keep existing data)
        // await ClearExistingData(cancellationToken);

        // Create Classes
        var classes = await CreateClasses(cancellationToken);

        // Create Subjects
        var subjects = await CreateSubjects(cancellationToken);

        // Create Users (Students, Teachers, Admins)
        var users = await CreateUsers(classes, cancellationToken);

        // Create TeacherSubjects
        await CreateTeacherSubjects(users, subjects, cancellationToken);

        // Create Gamification Badges
        var gamificationBadges = await CreateGamificationBadges(cancellationToken);

        // Create CPD Modules
        var cpdModules = await CreateCpdModules(gamificationBadges, cancellationToken);

        // Create Portfolio Files
        await CreatePortfolioFiles(users, subjects, cancellationToken);

        // Create Teacher Feedback
        await CreateTeacherFeedback(users, subjects, cancellationToken);

        // Create Student Badges (User Badges)
        await CreateStudentBadges(users, gamificationBadges, cancellationToken);

        // Create CPD Progress
        await CreateCpdProgress(users, cpdModules, cancellationToken);

        // Create Tokens (for last login tracking)
        await CreateTokens(users, cancellationToken);
        
        // Update LastLogin for users
        await UpdateLastLogin(users, cancellationToken);

        return RequestResult<string>.Success("Database seeded successfully with comprehensive test data!");
    }

    private async Task<List<ClassEntity>> CreateClasses(CancellationToken cancellationToken)
    {
        var classes = new List<ClassEntity>
        {
            new() { Name = "Grade 6A", Grade = 6, StudentCount = 25 },
            new() { Name = "Grade 6B", Grade = 6, StudentCount = 25 },
            new() { Name = "Grade 7A", Grade = 7, StudentCount = 28 },
            new() { Name = "Grade 7B", Grade = 7, StudentCount = 27 },
            new() { Name = "Grade 8A", Grade = 8, StudentCount = 30 },
            new() { Name = "Grade 8B", Grade = 8, StudentCount = 29 }
        };

         classRepository.AddRange(classes);
        return classes;
    }

    private async Task<List<SubjectEntity>> CreateSubjects(CancellationToken cancellationToken)
    {
        var subjects = new List<SubjectEntity>
        {
            new() { Name = "English Language Arts", Icon = "📚", Color = "#4CAF50", IsActive = true },
            new() { Name = "ICT", Icon = "💻", Color = "#2196F3", IsActive = true },
            new() { Name = "Mathematics", Icon = "🔢", Color = "#FF9800", IsActive = true },
            new() { Name = "Science", Icon = "🔬", Color = "#9C27B0", IsActive = true },
            new() { Name = "Arabic", Icon = "🌍", Color = "#F44336", IsActive = true },
            new() { Name = "Islamic Studies", Icon = "☪️", Color = "#00BCD4", IsActive = true }
        };

         subjectRepository.AddRange(subjects);
        return subjects;
    }

    private async Task<List<Domain.Entities.User>> CreateUsers(List<ClassEntity> classes, CancellationToken cancellationToken)
    {
        var users = new List<Domain.Entities.User>();

        // // Create Admin
        // users.Add(new Domain.Entities.User
        // {
        //     Name = "Eslam Mohamed",
        //     UserName = "admin14546213423",
        //     Email = "Eslam1@gamil.com",
        //     Password = "hashed_password",
        //     PhoneNumber = "+971501234567",
        //     Role = ApplicationRole.Admin,
        //     IsActive = true
        // });

        // Create Teachers
        var teacherNames = new[] { "Sarah Johnson", "Ahmed Ali", "Fatima Khan", "Michael Smith", "Aisha Hassan", "David Brown" };
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(); // Unique suffix for this seed run
        
        foreach (var name in teacherNames)
        {
            users.Add(new Domain.Entities.User
            {
                Name = name,
                UserName = $"{name.Replace(" ", "").ToLower()}_{timestamp}",
                Email = $"{name.Replace(" ", "").ToLower()}_{timestamp}@school.ae",
                Password = "hashed_password",
                PhoneNumber = $"+97150{Random.Shared.Next(1000000, 9999999)}",
                Role = ApplicationRole.Teacher,
                IsActive = true
            });
        }

        // Create Students (distribute across classes)
        var studentFirstNames = new[] { "Ali", "Sara", "Mohammed", "Mariam", "Omar", "Layla", "Hassan", "Noor", "Khalid", "Amina" };
        var studentLastNames = new[] { "Ahmed", "Ali", "Hassan", "Ibrahim", "Khalil", "Mahmoud", "Saeed", "Yousef", "Zayed", "Baker" };
        
        var studentCounter = 1;
        foreach (var classItem in classes)
        {
            for (int i = 0; i < classItem.StudentCount; i++)
            {
                var firstName = studentFirstNames[Random.Shared.Next(studentFirstNames.Length)];
                var lastName = studentLastNames[Random.Shared.Next(studentLastNames.Length)];
                var name = $"{firstName} {lastName}";

                users.Add(new Domain.Entities.User
                {
                    Name = name,
                    UserName = $"{firstName.ToLower()}.{lastName.ToLower()}.{studentCounter}_{timestamp}",
                    Email = $"{firstName.ToLower()}.{lastName.ToLower()}.{studentCounter}_{timestamp}@student.ae",
                    Password = "hashed_password",
                    PhoneNumber = $"+97150{Random.Shared.Next(1000000, 9999999)}",
                    Role = ApplicationRole.Student,
                    ClassID = classItem.ID,
                    IsActive = true
                });
                
                studentCounter++;
            }
        }

         userRepository.AddRange(users);
        return users;
    }

    private async Task CreateTeacherSubjects(List<Domain.Entities.User> users, List<SubjectEntity> subjects, CancellationToken cancellationToken)
    {
        var teachers = users.Where(u => u.Role == ApplicationRole.Teacher).ToList();
        var teacherSubjects = new List<Domain.Entities.Teacher.TeacherSubjects>();

        foreach (var teacher in teachers)
        {
            // Assign 1-2 subjects to each teacher
            var subjectCount = Random.Shared.Next(1, 3);
            var assignedSubjects = subjects.OrderBy(_ => Random.Shared.Next()).Take(subjectCount);

            foreach (var subject in assignedSubjects)
            {
                teacherSubjects.Add(new Domain.Entities.Teacher.TeacherSubjects
                {
                    TeacherId = teacher.ID,
                    SubjectId = subject.ID,
                    Grade = Random.Shared.Next(6, 9),
                    AssignedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(30, 365))
                });
            }
        }

         teacherSubjectsRepository.AddRange(teacherSubjects);
    }

    private async Task<List<Domain.Entities.Gamification.Badges>> CreateGamificationBadges(CancellationToken cancellationToken)
    {
        var badges = new List<Domain.Entities.Gamification.Badges>
        {
            new() { Name = "Portfolio Master", Description = "Complete 10 portfolio items", Icon = "🏆", Color = "#FFD700", Category = BadgeCategory.Portfolio, TargetRole = BadgeTargetRole.Student, IsActive = true },
            new() { Name = "Early Submitter", Description = "Submit on time for 5 assignments", Icon = "⏰", Color = "#4CAF50", Category = BadgeCategory.Portfolio, TargetRole = BadgeTargetRole.Student, IsActive = true },
            new() { Name = "CPD Champion", Description = "Complete 5 CPD modules", Icon = "📖", Color = "#2196F3", Category = BadgeCategory.Cpd, TargetRole = BadgeTargetRole.Teacher, CpdHours = 10, IsActive = true },
            new() { Name = "Knowledge Seeker", Description = "Complete 10 CPD modules", Icon = "🎓", Color = "#9C27B0", Category = BadgeCategory.Cpd, TargetRole = BadgeTargetRole.Teacher, CpdHours = 20, IsActive = true },
            new() { Name = "Mission Complete", Description = "Complete all missions", Icon = "🎯", Color = "#FF9800", Category = BadgeCategory.Mission, TargetRole = BadgeTargetRole.Student, IsActive = true },
            new() { Name = "Team Player", Description = "Help 5 classmates", Icon = "🤝", Color = "#00BCD4", Category = BadgeCategory.Engagement, TargetRole = BadgeTargetRole.Student, IsActive = true }
        };

         gamificationBadgesRepository.AddRange(badges);
        return badges;
    }

    private async Task<List<CpdModules>> CreateCpdModules(List<Domain.Entities.Gamification.Badges> badges, CancellationToken cancellationToken)
    {
        var cpdModules = new List<CpdModules>
        {
            new() { Title = "Modern Teaching Strategies", Description = "Learn innovative teaching methods", DurationMinutes = 60, Icon = "🎯", Color = "#4CAF50", BackgroundColor = "#E8F5E9", VideoProvider = VideoProvider.YouTube, BadgeId = badges.FirstOrDefault(b => b.Category == BadgeCategory.Cpd)?.ID, Order = 1, IsActive = true },
            new() { Title = "Digital Literacy in Education", Description = "Enhance digital teaching skills", DurationMinutes = 45, Icon = "💻", Color = "#2196F3", BackgroundColor = "#E3F2FD", VideoProvider = VideoProvider.YouTube, BadgeId = badges.FirstOrDefault(b => b.Category == BadgeCategory.Cpd)?.ID, Order = 2, IsActive = true },
            new() { Title = "Student Assessment Techniques", Description = "Master assessment strategies", DurationMinutes = 90, Icon = "📊", Color = "#FF9800", BackgroundColor = "#FFF3E0", VideoProvider = VideoProvider.Vimeo, BadgeId = badges.FirstOrDefault(b => b.Category == BadgeCategory.Cpd)?.ID, Order = 3, IsActive = true },
            new() { Title = "Classroom Management", Description = "Effective classroom control", DurationMinutes = 75, Icon = "👥", Color = "#9C27B0", BackgroundColor = "#F3E5F5", VideoProvider = VideoProvider.YouTube, Order = 4, IsActive = true }
        };

         cpdModulesRepository.AddRange(cpdModules);
        return cpdModules;
    }

    private async Task CreatePortfolioFiles(List<Domain.Entities.User> users, List<SubjectEntity> subjects, CancellationToken cancellationToken)
    {
        var students = users.Where(u => u.Role == ApplicationRole.Student).ToList();
        var portfolioFiles = new List<PortfolioFiles>();

        foreach (var student in students.Take(100)) // Create portfolio for first 100 students
        {
            var fileCount = Random.Shared.Next(1, 8);
            for (int i = 0; i < fileCount; i++)
            {
                var subject = subjects[Random.Shared.Next(subjects.Count)];
                portfolioFiles.Add(new PortfolioFiles
                {
                    StudentId = student.ID,
                    SubjectId = subject.ID,
                    FileName = $"Assignment_{i + 1}_{subject.Name.Replace(" ", "_")}.pdf",
                    FileType = FileType.Pdf,
                    FileSize = Random.Shared.Next(100000, 5000000),
                    StoragePath = $"/storage/portfolio/{student.ID}/{Guid.NewGuid()}.pdf",
                    DownloadUrl = $"https://storage.example.com/{Guid.NewGuid()}",
                    UploadedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 90))
                });
            }
        }

         portfolioRepository.AddRange(portfolioFiles);
    }

    private async Task CreateTeacherFeedback(List<Domain.Entities.User> users, List<SubjectEntity> subjects, CancellationToken cancellationToken)
    {
        var teachers = users.Where(u => u.Role == ApplicationRole.Teacher).ToList();
        var students = users.Where(u => u.Role == ApplicationRole.Student).ToList();
        var feedbacks = new List<TeacherFeedback>();

        foreach (var teacher in teachers)
        {
            var feedbackCount = Random.Shared.Next(10, 50);
            for (int i = 0; i < feedbackCount; i++)
            {
                var student = students[Random.Shared.Next(students.Count)];
                var subject = subjects[Random.Shared.Next(subjects.Count)];

                feedbacks.Add(new TeacherFeedback
                {
                    StudentId = student.ID,
                    TeacherId = teacher.ID,
                    SubjectId = subject.ID,
                    Comment = "Great work! Keep it up.",
                    Type = FeedbackType.Positive
                });
            }
        }

         teacherFeedbackRepository.AddRange(feedbacks);
    }

    private async Task CreateStudentBadges(List<Domain.Entities.User> users, List<Domain.Entities.Gamification.Badges> badges, CancellationToken cancellationToken)
    {
        var students = users.Where(u => u.Role == ApplicationRole.Student).ToList();
        var studentBadges = new List<StudentBadges>();

        foreach (var student in students.Take(80))
        {
            var badgeCount = Random.Shared.Next(0, 4);
            var earnedBadges = badges.Where(b => b.TargetRole == BadgeTargetRole.Student)
                                      .OrderBy(_ => Random.Shared.Next())
                                      .Take(badgeCount);

            foreach (var badge in earnedBadges)
            {
                studentBadges.Add(new StudentBadges
                {
                    StudentId = student.ID,
                    BadgeId = badge.ID,
                    EarnedDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 180)),
                    AutoAwarded = Random.Shared.Next(0, 2) == 0,
                    Status = Random.Shared.Next(0, 10) < 8 ? Status.Approved : (Random.Shared.Next(0, 2) == 0 ? Status.Pinndeing : Status.Rejected)
                });
            }
        }

         studentBadgesRepository.AddRange(studentBadges);
    }

    private async Task CreateCpdProgress(List<Domain.Entities.User> users, List<CpdModules> modules, CancellationToken cancellationToken)
    {
        var teachers = users.Where(u => u.Role == ApplicationRole.Teacher).ToList();
        var cpdProgress = new List<TeacherCpdProgress>();

        foreach (var teacher in teachers)
        {
            var completedCount = Random.Shared.Next(0, modules.Count);
            var completedModules = modules.OrderBy(_ => Random.Shared.Next()).Take(completedCount);

            foreach (var module in completedModules)
            {
                var startDate = DateTime.UtcNow.AddDays(-Random.Shared.Next(30, 180));
                cpdProgress.Add(new TeacherCpdProgress
                {
                    TeacherId = teacher.ID,
                    ModuleId = module.ID,
                    Status = ProgressStatus.Completed,
                    StartedAt = startDate,
                    CompletedAt = startDate.AddDays(Random.Shared.Next(1, 15)),
                    LastAccessedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(0, 30)),
                    HoursEarned = (decimal)module.DurationMinutes / 60
                });
            }
        }

         cpdProgressRepository.AddRange(cpdProgress);
    }

    private async Task CreateTokens(List<Domain.Entities.User> users, CancellationToken cancellationToken)
    {
        var tokens = new List<Token>();

        foreach (var user in users)
        {
            // Create 1-5 login tokens for each user
            var loginCount = Random.Shared.Next(1, 6);
            for (int i = 0; i < loginCount; i++)
            {
                tokens.Add(new Token
                {
                    UserID = user.ID,
                    JwtID = Random.Shared.NextInt64(),
                    LoggedOutAt = i == 0 ? null : DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30)),
                    IsActive = i == 0,
                    CreatedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(i * 10, (i + 1) * 10))
                });
            }
        }

         tokenRepository.AddRange(tokens);
    }

    private async Task UpdateLastLogin(List<Domain.Entities.User> users, CancellationToken cancellationToken)
    {
        var tokens = await tokenRepository.Get().ToListAsync(cancellationToken);
        
        foreach (var user in users)
        {
            var lastToken = tokens.Where(t => t.UserID == user.ID)
                                  .OrderByDescending(t => t.CreatedAt)
                                  .FirstOrDefault();
            
            if (lastToken != null)
            {
                user.LastLogin = lastToken.CreatedAt;
            }
        }

        await userRepository.SaveChangesAsync();
    }
}
