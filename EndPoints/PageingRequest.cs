using System.ComponentModel;

namespace API.EndPoints;

public class PageingRequest
{
    [DefaultValue(1)]
    public int page { get; set; }
    [DefaultValue(100)]
    public int pageSize { get; set; }
}