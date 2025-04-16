using Microsoft.AspNetCore.Http.HttpResults;
using System;
using UserGuard_API.Service;

namespace UserGuard_API.Repositories.Implementations
{
    public class MaintenanceTaskRepository : IMaintenanceTaskRepository
    {
        private readonly AcademyAPI _context;
        private readonly IEmployeeRepository employeeRepository;
        private readonly IImageService imageService;
        public MaintenanceTaskRepository(AcademyAPI context,IEmployeeRepository employeeRepository,IImageService imageService)
        {
            _context = context;
            this.employeeRepository = employeeRepository;
            this.imageService = imageService;
        }

        public async Task AddTaskAsync(MaintenanceTaskDto dto, string EmployeeBarnchId, List<IFormFile>? imageFiles = null)
        {
            var employee = await employeeRepository.GetEmployeeByEmployeeIdAsync(dto.EmployeeId);
            if (employee == null)
                throw new Exception("Employee not found");

           
            var task = new MaintenanceTask
            {
                TaskDescription = dto.TaskDescription,
                TaskDate = dto.TaskDate,
                TaskEnd = dto.TaskEnd,
                Notes = dto.Notes,
                BranchId=EmployeeBarnchId,
                EmployeeId = dto.EmployeeId
            };

            if (imageFiles != null && imageFiles.Any())
            {
                foreach (var image in imageFiles.Take(3)) 
                {
                    var url = await imageService.SaveImageAsync(image, "maintenance");
                    task.Images.Add(new MaintenanceTaskImage { ImageUrl = url });
                }
            }

            _context.MaintenanceTasks.Add(task);
            await _context.SaveChangesAsync();
        }



        public async Task<List<MaintenanceTaskDtoResponse>> GetTasksByEmployeeIdAsyncDto(string employeeId)
        {
            var tasks = await _context.MaintenanceTasks
                .Include(t => t.Images)
                .Where(t => t.EmployeeId == employeeId)
                .ToListAsync();

            var  result = tasks.Select(t => new MaintenanceTaskDtoResponse
            {
                TaskDescription = t.TaskDescription,
                TaskDate = t.TaskDate,
                TaskEnd = t.TaskEnd,
                Notes = t.Notes,
                ImageUrls = t.Images.Select(img => img.ImageUrl).ToList()
            }).ToList();

            return result;
        }
        public async Task<List<MaintenanceTask>> GetAllTasksAsync()
        {
            return await _context.MaintenanceTasks.ToListAsync();
        }


        public async Task UpdateTaskAsync(string taskId, MaintenanceTaskToUpdateDto updateDto,string EmployeeBarnchId, List<IFormFile>? newImages = null)
        {
            taskId = taskId.Trim();
            var task = await _context.MaintenanceTasks
                .Include(t => t.Images).Where(t => t.Id == taskId)
                .FirstOrDefaultAsync();

            if (task == null)
                throw new Exception("Task not found");

            if (EmployeeBarnchId != task.BranchId)
                throw new Exception("Unauthorized This Task in Anthouer Branch");

            task.TaskDescription = updateDto.TaskDescription ?? task.TaskDescription;
            task.TaskDate = updateDto.TaskDate != default ? updateDto.TaskDate : task.TaskDate;
            task.TaskEnd = updateDto.TaskEnd ?? task.TaskEnd;
            task.Notes = updateDto.Notes ?? task.Notes;
            task.Status = !string.IsNullOrEmpty(updateDto.Status) ? updateDto.Status : task.Status;

            if (newImages != null && newImages.Any())
            {
                foreach (var image in newImages.Take(3 - task.Images.Count))
                {
                    var url = await imageService.SaveImageAsync(image, "maintenance");
                    task.Images.Add(new MaintenanceTaskImage { ImageUrl = url });
                }
            }

            await _context.SaveChangesAsync();
        }

    }

}
