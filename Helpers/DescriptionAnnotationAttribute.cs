namespace API.Helpers;

[AttributeUsage(AttributeTargets.Field)]
public class DescriptionAnnotationAttribute : Attribute
{
    public string Code { get; set; }
    public string Description { get; set; }

    public DescriptionAnnotationAttribute(string code, string description)
    {
        Code = code;
        Description = description;
    }
}

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = (DescriptionAnnotationAttribute?)Attribute.GetCustomAttribute(field!, typeof(DescriptionAnnotationAttribute));
        return attribute?.Description ?? value.ToString();
    }
}

