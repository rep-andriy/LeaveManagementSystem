using AutoMapper;
using AutoMapper.Features;
using LeaveManagementSystem.Web.Models.LeaveAllocations;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Composition;
using System.Runtime.CompilerServices;

namespace LeaveManagementSystem.Web.Services.LeaveAllocations
{
    public class LeaveAllocationsService(ApplicationDbContext _context,
        IHttpContextAccessor _httpContextAccessor, UserManager<ApplicationUser> _userManager,
        IMapper _mapper) : ILeaveAllocationsService
    {
        public async Task AllocateLeave(string employeeId)
        {
            // get all the leave types 
            var leaveTypes = await _context.LeaveTypes
                .Where(q => !q.LeaveAllocations.Any(x => x.EmployeeId == employeeId))
                .ToListAsync();

            // get the current period based on the year
            var currentDate = DateTime.Now;
            var period = await _context.Periods.SingleAsync(q => q.EndDate.Year == currentDate.Year);
            var monthsRemaining = period.EndDate.Month - currentDate.Month;

            // calculate leave based on number of months left in the period
            // foreach leave type, create an allocation entry
            foreach (var leaveType in leaveTypes)
            {
                // Works, but not the best practice
                /*
                var allocationExists = await AllocationExists(employeeId, period.Id, leaveType.Id);
                if (allocationExists)
                {
                    continue;
                }
                */
                var accuralRate = decimal.Divide(leaveType.NumberOfDays, 12);
                var leaveAllocation = new LeaveAllocation
                {
                    EmployeeId = employeeId,
                    LeaveTypeId = leaveType.Id,
                    PeriodId = period.Id,
                    Days = (int)Math.Ceiling(accuralRate * monthsRemaining)
                };

                _context.Add(leaveAllocation);
            }

            await _context.SaveChangesAsync();

        }

        public async Task<EmployeeAllocationVM> GetEmployeeAllocations(string? userId)
        {
            var user = string.IsNullOrEmpty(userId) 
                ? await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User) 
                : await _userManager.FindByIdAsync(userId);
            
            var allocations = await GetAllocations(user.Id);
            var allocationVMList = _mapper.Map<List<LeaveAllocation>, List<LeaveAllocationVM>>(allocations);
            var leaveTypesCount = await _context.LeaveTypes.CountAsync();


            var employeeVm = new EmployeeAllocationVM
            {
                DateOfBirth = user.DateOfBirth,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                LeaveAllocations = allocationVMList,
                IsCompletedAllocation = leaveTypesCount == allocations.Count()
            };

            return employeeVm;
        }

        public async Task<List<EmployeeListVM>> GetEmployees()
        {
            var users = await _userManager.GetUsersInRoleAsync(Roles.Employee);
            var employees = _mapper.Map<List<ApplicationUser>, List<EmployeeListVM>>(users.ToList());

            return employees;
        }

		public async Task<LeaveAllocationEditVM> GetEmployeeAllocation(int allocationId)
		{
			var allocation = await _context.LeaveAllocations
				.Include(q => q.LeaveType)
				.Include(q => q.Employee)
				.FirstOrDefaultAsync(q => q.Id == allocationId);

			var model = _mapper.Map<LeaveAllocationEditVM>(allocation);

			return model;
		}

		public async Task EditAllocation(LeaveAllocationEditVM allocationEditVm)
		{
            // var leaveAllocation = await GetEmployeeAllocation(allocationEditVm.Id) ??
            // if(leaveAllocation == null)
            // {
            //    throw new Exception("Leave allocation record does not exist");
            // }
            // leaveAllocation.Days = allocationEditVm.Days;
            // option 1 _context.Update(leaveAllocation);
            // option 2 _context.Entry(leaveAllocation).State = EntityState.Modified;
            // await _context.SaveChangesAsync();

            await _context.LeaveAllocations
                .Where(q => q.Id == allocationEditVm.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(e => e.Days, allocationEditVm.Days));
		}

		private async Task<List<LeaveAllocation>> GetAllocations(string? userId)
		{
			var currentDate = DateTime.Now;
			/*
            var period = _context.Periods.SingleAsync(q => q.EndDate.Year == currentDate.Year);
             * var leaveAllocations = await _context.LeaveAllocations
                .Include(q => q.LeaveType)
                .Include(q => q.Period)
                .Where(q => q.EmployeeId == user.Id && q.PeriodId == period.Id)
                .ToListAsync();
            */

			var leaveAllocations = await _context.LeaveAllocations
				.Include(q => q.LeaveType)
				.Include(q => q.Period)
				.Where(q => q.EmployeeId == userId && q.Period.EndDate.Year == currentDate.Year)
				.ToListAsync();

			return leaveAllocations;
		}

        private async Task<bool> AllocationExists(string userId, int periodId, int leaveTypeId)
        {
            var exists = await _context.LeaveAllocations.AnyAsync(q =>
            q.EmployeeId == userId
            && q.LeaveTypeId == leaveTypeId
            && q.PeriodId == periodId
            );

            return exists;
        }
	}
}
