using LeaveManagementSystem.Web.Models.LeaveTypes;

namespace LeaveManagementSystem.Web.Services
{
    public interface ILeaveTypesService
    {
        Task<bool> CheckIfLeaveTypeAlreadyExistsForEdit(LeaveTypeEditVM leaveTypeEdit);
        Task<bool> CheckIfLeaveTypeNameExists(string name);
        Task Create(LeaveTypeCreateVM model);
        Task Edit(LeaveTypeEditVM model);
        Task<T?> Get<T>(int id) where T : class;
        Task<List<LeaveTypeReadOnlyVM>> GetAll();
        bool LeaveTypeExists(int id);
        Task Remove(int id);
    }
}