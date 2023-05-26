using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Generic
{
    [Internationalization]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ControllerBaseClass : Controller
    {
        //The task of this Controller is only to inherit the Controller and also over write Internationalization
        //Other wise we have to call [Internationalization] on every control
    }
}
