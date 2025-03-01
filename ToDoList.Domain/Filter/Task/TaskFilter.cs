using ToDoList.Domain.Enum;
using ToDoList.Domain.Filter.Page;

namespace ToDoList.Domain.Filter.Task;

public class TaskFilter : PagingFilter
{
    public string Name { get; set; }
    
    public Priority? Priority { get; set; }
}