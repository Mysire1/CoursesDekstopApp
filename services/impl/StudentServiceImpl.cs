using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoursesDekstopApp.data;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore;

namespace CoursesDekstopApp.services.impl
{
    public class StudentServiceImpl : IStudentService
    {
        private readonly ApplicationDbContext _context;
        
        public StudentServiceImpl(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<(List<Student> updatedStudents, int count)> ApplyDiscountForFrequentStudentsAsync(decimal discountPercentage)
        {
            var studentIdsWithManyLevels = await _context.ExamResults
                .Where(er => er.Grade >= 50) 
                .GroupBy(er => er.StudentId) 
                .Select(g => new {
                    StudentId = g.Key,
                    CompletedLevels = g.Count() 
                })
                .Where(s => s.CompletedLevels > 3) 
                .Select(s => s.StudentId) 
                .ToListAsync();
            
            var studentsToUpdate = await _context.Students
                .Where(s => studentIdsWithManyLevels.Contains(s.StudentId) && s.HasDiscount == false)
                .ToListAsync();
            
            foreach (var student in studentsToUpdate)
            {
                student.HasDiscount = true;
                student.DiscountPercentage = discountPercentage;
            }
            
            int affectedRows = 0;
            if (studentsToUpdate.Count > 0)
            {
                affectedRows = await _context.SaveChangesAsync();
            }
            
            return (studentsToUpdate, affectedRows);
        }
    }
}