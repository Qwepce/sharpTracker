using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ToDoList.Domain.Extensions
{
    public static class EnumExtension
    {
        public static TAttribute GetAttribute<TAttribute>(this System.Enum enumValue) 
            where TAttribute : Attribute
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<TAttribute>();
        }
    }
}