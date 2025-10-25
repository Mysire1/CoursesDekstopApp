using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;

namespace CoursesDekstopApp.services
{
    public interface IGroupService
    {
        Task<(List<Group> affectedGroups, int studentsAffected)> ApplySurchargeForSmallGroupsAsync(int minSize, decimal surchargePercentage);
    }
}