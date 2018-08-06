using Microsoft.AspNetCore.Mvc;

namespace FabricDemo.ProductService.Controllers
{
    /// <summary>
    /// controller 基类
    /// </summary>
    [Produces("application/json")]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {

    }
}
