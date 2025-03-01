using ToDoList.Domain.Entity;
using ToDoList.Domain.Filter.Task;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;

namespace ToDoList.Service.Interfaces;

public interface ITaskService
{
    Task<IBaseResponse<TaskEntity>> CreateTaskAsync(CreateTaskViewModel model);

    Task<DataTableResult> GetTasks(TaskFilter filter);

    Task<IBaseResponse<bool>> EndTask(long id);
    
    Task<IBaseResponse<IEnumerable<CompletedTaskViewModel>>> GetCompletedTask();
    
    Task<IBaseResponse<IEnumerable<TaskViewModel>>> CalculateCompletedTask();
}