

namespace UserGuard_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet]
        public async Task<IActionResult> tesr()
        {
            await _mediator.Publish(new UserRegisteredEvent("karamomari20010@gmail.com","sda","sda"));
            return Ok();
        }
    }
}
