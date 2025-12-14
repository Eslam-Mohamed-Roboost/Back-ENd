using System.ComponentModel;
using API.Domain.Enums;

namespace API.EndPoints.Admin.GetBadges;
public class GetBadgesRequest : PageingRequest
{
   public BadgeCategory? category{ get; set; }
 
}