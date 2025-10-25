using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoursesDekstopApp.data;
using DefaultNamespace;
using Microsoft.EntityFrameworkCore;

namespace CoursesDekstopApp.services.impl
{
    public class GroupServiceImpl : IGroupService
    {
        private readonly ApplicationDbContext _context;

        public GroupServiceImpl(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<(List<Group> affectedGroups, int studentsAffected)> ApplySurchargeForSmallGroupsAsync(int minSize, decimal surchargePercentage)
        {
            var smallGroups = await _context.Groups
                .Include(g => g.Enrollments) 
                .Include(g => g.Level)       
                .Where(g => g.Enrollments.Count < minSize)
                .ToListAsync();

            int studentsAffectedCount = 0;
            
            foreach (var group in smallGroups)
            {
                foreach (var enrollment in group.Enrollments)
                {
                    decimal baseCost = group.Level.Cost;
                    decimal targetCost = baseCost * (1 + (surchargePercentage / 100));

                    if (enrollment.FinalCost == baseCost) 
                    {
                        enrollment.FinalCost = targetCost;
                        _context.Enrollments.Update(enrollment); 
                        studentsAffectedCount++;
                    }
                }
            }

            if (studentsAffectedCount > 0)
            {
                await _context.SaveChangesAsync();
            }
            
            return (smallGroups, studentsAffectedCount);
        }
        
        public async Task<(List<Group> affectedGroups, int studentsAffected)> ApplyDiscountForLargeGroupsAsync(int maxSize, decimal discountPercentage)
        {
            var largeGroups = await _context.Groups
                .Include(g => g.Enrollments)
                .Include(g => g.Level)
                .Where(g => g.Enrollments.Count == maxSize)
                .ToListAsync();

            int studentsAffectedCount = 0;
            
            foreach (var group in largeGroups)
            {
                foreach (var enrollment in group.Enrollments)
                {
                    decimal baseCost = group.Level.Cost;
                    
                    decimal targetCost = baseCost * (1 - (discountPercentage / 100)); 
                    
                    if (enrollment.FinalCost == baseCost)
                    {
                        enrollment.FinalCost = targetCost;
                        _context.Enrollments.Update(enrollment);
                        studentsAffectedCount++;
                    }
                }
            }
            
            if (studentsAffectedCount > 0)
            {
                await _context.SaveChangesAsync();
            }
            
            return (largeGroups, studentsAffectedCount);
        }
    }
}