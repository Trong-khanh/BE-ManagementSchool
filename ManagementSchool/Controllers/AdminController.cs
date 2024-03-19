using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManagementSchool.Controllers;
[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class AdminController: ControllerBase
{
     
}