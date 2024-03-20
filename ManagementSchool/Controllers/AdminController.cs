using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSchool.Controllers;
[Authorize(Roles = "Admin,Student")]
[Route("api/[controller]")]
[ApiController]
public class AdminController: ControllerBase
{
     [HttpGet("Employees")]
     public IEnumerable<string> Get()
     {
          return new string[] { "Khanh", "Toan", "Nghia" };
     }
}