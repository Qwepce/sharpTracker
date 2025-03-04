using System.Text.Json.Serialization;
using ToDoList.Domain.Enum;

namespace ToDoList.Domain.ViewModels.Task;

public class CreateTaskViewModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("priority")]
    public Priority Priority { get; set; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new ArgumentNullException(Name, "Укажите название задачи");
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            throw new ArgumentNullException(Description, "Укажите описание задачи");
        }
    }
}