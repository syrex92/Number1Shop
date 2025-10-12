using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UsersService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealhController : ControllerBase
    {
        public HealhController()
        {
        }
        /// <summary>
        /// Проверка доступности сервиса
        /// </summary>
        /// <remarks>
        /// Вызывается для проверки доступности сервиса. Если возвращён статус 200 - значит сервис доступен.
        /// Применяется для контроля состояния сервиса в контейнере
        /// </remarks>
        /// <returns>plain string message</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public IActionResult CheckHealth()
        {
            return Ok(new
            {
                Status = "Cart service working",
                Timestamp = DateTime.UtcNow,
                Service = "User Service",
            });
        }
    }
}
