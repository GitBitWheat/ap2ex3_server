using Microsoft.AspNetCore.Mvc;
using ap2ex3_server.Services;
using ap2ex3_server.ActionModels;

namespace ap2ex3_server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferController : ControllerBase
    {
        private readonly IService _service;

        public TransferController(IService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> HandleTransfer([FromBody] TransferModel transferModel)
        {
            if (transferModel.Content == null || transferModel.From == null || transferModel.To == null)
                return Forbid();

            if (await _service.ReceiveMessage(transferModel.Content, transferModel.From, transferModel.To))
                return CreatedAtAction(nameof(HandleTransfer), new { Content = transferModel.Content });
            else
                return Forbid();
        }
    }
}