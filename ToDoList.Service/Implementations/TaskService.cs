using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoList.DAL.Interfaces;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Enum;
using ToDoList.Domain.Extensions;
using ToDoList.Domain.Filter.Task;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;

namespace ToDoList.Service.Implementations;

public class TaskService : ITaskService
{
    private readonly IBaseRepository<TaskEntity> _taskRepository;
    private readonly ILogger<TaskService> _logger;

    public TaskService(IBaseRepository<TaskEntity> taskRepository, ILogger<TaskService> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task<IBaseResponse<TaskEntity>> CreateTaskAsync(CreateTaskViewModel model)
    {
        try
        {
            model.Validate();

            _logger.LogInformation($"Creating new task - {model.Name}");

            var task = await _taskRepository.GetAll()
                .Where(x => x.Created.Date == DateTime.Today)
                .FirstOrDefaultAsync(x => x.Name == model.Name);

            if (task != null)
            {
                return new BaseResponse<TaskEntity>()
                {
                    Description = "Задача с таким названием уже есть",
                    StatusCode = StatusCode.TaskIsHasAlready
                };
            }

            task = new TaskEntity()
            {
                Name = model.Name,
                Description = model.Description,
                Priority = model.Priority,
                Created = DateTime.Now,
                IsDone = false
            };

            await _taskRepository.Create(task);

            _logger.LogInformation($"Created new task : {task.Name} ===> {task.Created}");
            return new BaseResponse<TaskEntity>()
            {
                StatusCode = StatusCode.OK,
                Description = "Задача создалась успешно"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"[TaskService.CreateTaskAsync]: {ex.Message}]");

            return new BaseResponse<TaskEntity>()
            {
                Description = $"{ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<DataTableResult> GetTasks(TaskFilter filter)
    {
        try
        {
            var tasks = await _taskRepository.GetAll()
                .Where(x => x.IsDone == false)
                .WhereIf(!string.IsNullOrWhiteSpace(filter.Name), x => x.Name == filter.Name)
                .WhereIf(filter.Priority.HasValue, x => x.Priority == filter.Priority)
                .Select(x => new TaskViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsDone = x.IsDone == true ? "Готова" : "Не готова",
                    Priority = x.Priority.GetAttribute<DisplayAttribute>().Name,
                    Created = x.Created.ToLongDateString()
                })
                .ToListAsync();

            var count = _taskRepository.GetAll()
                .Where(x => !x.IsDone)
                .WhereIf(!string.IsNullOrWhiteSpace(filter.Name), x => x.Name == filter.Name)
                .WhereIf(filter.Priority.HasValue, x => x.Priority == filter.Priority)
                .Count();

            return new DataTableResult()
            {
                Data = tasks,
                Total = count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"[TaskService.GetTasks]: {ex.Message}]");

            return new DataTableResult()
            {
                Data = null,
                Total = 0
            };
        }
    }

    public async Task<IBaseResponse<bool>> EndTask(long id)
    {
        try
        {
            var task = await _taskRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);

            if (Equals(task, null))
            {
                return new BaseResponse<bool>()
                {
                    StatusCode = StatusCode.NotFound,
                    Description = "Задача не найдена"
                };
            }

            task.IsDone = true;

            await _taskRepository.Update(task);

            return new BaseResponse<bool>()
            {
                StatusCode = StatusCode.OK,
                Description = "Задача завершена"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"[TaskService.EndTask]: {ex.Message}]");

            return new BaseResponse<bool>()
            {
                Description = $"{ex.Message}",
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<CompletedTaskViewModel>>> GetCompletedTask()
    {
        try
        {
            var tasks = await _taskRepository.GetAll()
                .Where(x => x.IsDone == true)
                .Where(x => x.Created.Date == DateTime.Today)
                .Select(x => new CompletedTaskViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                }).ToListAsync();

            return new BaseResponse<IEnumerable<CompletedTaskViewModel>>()
            {
                Data = tasks,
                StatusCode = StatusCode.OK,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"[TaskService.GetCompletedTask]: {ex.Message}]");

            return new BaseResponse<IEnumerable<CompletedTaskViewModel>>()
            {
                StatusCode = StatusCode.InternalServerError
            };
        }
    }

    public async Task<IBaseResponse<IEnumerable<TaskViewModel>>> CalculateCompletedTask()
    {
        try
        {
            var tasks = await _taskRepository.GetAll()
                .Where(x => x.Created.Date == DateTime.Today)
                .Select(x => new TaskViewModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsDone = x.IsDone ? "Выполнена" : "Не выполнена",
                    Description = x.Description,
                    Priority = x.Priority.GetAttribute<DisplayAttribute>().Name,
                    Created = x.Created.ToString(CultureInfo.InvariantCulture)
                }).ToListAsync();

            return new BaseResponse<IEnumerable<TaskViewModel>>()
            {
                Data = tasks,
                StatusCode = StatusCode.OK,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"[TaskService.GetCompletedTask]: {ex.Message}]");

            return new BaseResponse<IEnumerable<TaskViewModel>>()
            {
                StatusCode = StatusCode.InternalServerError
            };
        }
    }
}