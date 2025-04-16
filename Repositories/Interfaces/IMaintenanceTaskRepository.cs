namespace UserGuard_API.Repositories.Interfaces
{
    public interface IMaintenanceTaskRepository
    {
         Task AddTaskAsync(MaintenanceTaskDto dto, string EmployeeBarnchId, List<IFormFile>? imageFiles = null);
        Task<List<MaintenanceTaskDtoResponse>> GetTasksByEmployeeIdAsyncDto(string employeeId);
        Task<List<MaintenanceTask>> GetAllTasksAsync();
        Task UpdateTaskAsync(string taskId, MaintenanceTaskToUpdateDto updateDto,string EmployeeBarnchId ,List<IFormFile>? newImages = null);
    }
}
