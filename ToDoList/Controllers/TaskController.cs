using Microsoft.AspNetCore.Mvc;
using ToDoList.Domain.Filter.Task;
using ToDoList.Domain.Helpers;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;

namespace ToDoList.Controllers;

public class TaskController : Controller
{
    private readonly ITaskService _taskService;

    public TaskController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    public IActionResult Index()
    {
        return View();
        //
    }

    public async Task<IActionResult> GetCompletedTasks()
    {
        var response = await _taskService.GetCompletedTask();

        return Json(new { data = response.Data });
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask(CreateTaskViewModel model)
    {
        var response = await _taskService.CreateTaskAsync(model);

        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return StatusCode(StatusCodes.Status201Created, new { description = response.Description });
        }
        
        return BadRequest( new { description = response.Description });
    }

    [HttpPost]
    public async Task<IActionResult> TaskHandler(TaskFilter filter)
    {
        var response = await _taskService.GetTasks(filter);
        return Json(new { recordsFiltered = response.Total, recordsTotal = response.Total, data = response.Data });
    }


    [HttpPost]
    public async Task<IActionResult> EndTask(long id)
    {
        var response = await _taskService.EndTask(id);

        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            return Ok(new { description = response.Description });
        }
        
        return BadRequest( new { description = response.Description });
    }

    [HttpPost]
    public async Task<IActionResult> CalculateCompletedTasks()
    {
        var response = await _taskService.CalculateCompletedTask();

        if (response.StatusCode == Domain.Enum.StatusCode.OK)
        {
            var csvService = new CsvBaseService<IEnumerable<TaskViewModel>>();
            var uploadFile = csvService.UploadFiles(response.Data);
            
            return File(uploadFile, "text/csv", $"Статистика за {DateTime.Today.ToLongDateString()}.csv");
        }
        
        return BadRequest( new { description = response.Description });
    }
}