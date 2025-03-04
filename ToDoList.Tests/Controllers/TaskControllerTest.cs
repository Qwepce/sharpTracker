using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ToDoList.Controllers;
using ToDoList.Domain.Entity;
using ToDoList.Domain.Enum;
using ToDoList.Domain.Response;
using ToDoList.Domain.ViewModels.Task;
using ToDoList.Service.Interfaces;

namespace ToDoList.Tests.Controllers;

[TestFixture]
[TestOf(typeof(TaskController))]
public class TaskControllerTest
{
    private Mock<ITaskService> _mockTaskService;
    private TaskController _taskController;

    [SetUp]
    public void Setup()
    {
        _mockTaskService = new Mock<ITaskService>();
        _taskController = new TaskController(_mockTaskService.Object);
    }

    [TearDown]
    public void TearDown()
    {
        _mockTaskService = null;
        _taskController.Dispose();
    }

    [Test]
    [Description("Метод должен вернуть список всех выполненных задач")]
    public async Task GetCompletedTasks_ShouldReturnsListOfCompletedTask()
    {
        //arrange
        var allTasks = new List<TaskViewModel>
        {
            new TaskViewModel
            {
                Id = 1, Name = "Zadacha 1", Description = "Opisanie zadachi 1", IsDone = true.ToString(),
                Priority = Priority.Medium.ToString(), Created = DateTime.Now.ToLongDateString()
            },
            new TaskViewModel
            {
                Id = 2, Name = "Zadacha 2", Description = "Opisanie zadachi 2", IsDone = true.ToString(),
                Priority = Priority.Hard.ToString(), Created = DateTime.Now.ToLongDateString()
            },
            new TaskViewModel
            {
                Id = 3, Name = "Zadacha 3", Description = "Opisanie zadachi 3", IsDone = false.ToString(),
                Priority = Priority.Easy.ToString(), Created = DateTime.Now.ToLongDateString()
            }
        };

        _mockTaskService.Setup(service => service.GetCompletedTask())
            .ReturnsAsync(new BaseResponse<IEnumerable<CompletedTaskViewModel>>
            {
                Data = allTasks
                    .Where(task => task.IsDone.ToString() == "True")
                    .Select(x => new CompletedTaskViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                    }).ToList(),
                StatusCode = StatusCode.OK
            });
        //act
        var result = await _taskController.GetCompletedTasks();

        //assert
        var jsonResult = result as JsonResult;
        var dataProperty = jsonResult.Value.GetType().GetProperty("data");
        var dataValue = dataProperty.GetValue(jsonResult.Value) as IEnumerable<CompletedTaskViewModel>;

        Assert.IsInstanceOf<JsonResult>(result);
        Assert.IsNotNull(jsonResult);
        Assert.IsNotNull(dataProperty);
        Assert.IsNotNull(dataValue);
        Assert.AreEqual(2, dataValue.Count());
    }

    [Test]
    public async Task CreateTask_RequestIsValid_ShouldReturnJsonWithTask()
    {
        //arrange
        var newTask = new CreateTaskViewModel()
        {
            Name = "Zadacha 1",
            Description = "Opisanie zadachi 1",
            Priority = Priority.Medium
        };

        var response = new BaseResponse<TaskEntity>
        {
            StatusCode = StatusCode.OK,
            Description = "Задача создалась успешно"
        };
        
        _mockTaskService.Setup(service => service.CreateTaskAsync(newTask))
            .ReturnsAsync(response);
        
        //act
        var result = await _taskController.CreateTask(newTask);

        //assert

        var objectResult = result as ObjectResult;
        var valueType = objectResult.Value.GetType();
        var descriptionProperty = valueType.GetProperty("description");
        var descriptionValue = descriptionProperty.GetValue(objectResult.Value) as string;

        Assert.IsInstanceOf<ObjectResult>(result);
        Assert.AreEqual(StatusCodes.Status201Created, objectResult.StatusCode);
        Assert.IsNotNull(objectResult.Value);
        Assert.IsNotNull(descriptionProperty);
        Assert.AreEqual("Задача создалась успешно", descriptionValue);
    }
}