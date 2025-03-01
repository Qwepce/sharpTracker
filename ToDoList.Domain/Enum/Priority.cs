using System.ComponentModel.DataAnnotations;

namespace ToDoList.Domain.Enum;

public enum Priority
{
    [Display(Name = "Простая")] Easy = 0,
    [Display(Name = "Средняя")] Medium = 1,
    [Display(Name = "Сложная")] Hard = 2
}