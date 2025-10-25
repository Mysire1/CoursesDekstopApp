using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;

namespace CoursesDekstopApp.services
{
    public interface IStudentService
    {
        Task<(List<Student> updatedStudents, int count)> ApplyDiscountForFrequentStudentsAsync(decimal discountPercentage);
    }
}