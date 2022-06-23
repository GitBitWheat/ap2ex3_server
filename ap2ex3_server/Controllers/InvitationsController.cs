using Microsoft.AspNetCore.Mvc;
using ap2ex3_server.Models;
using ap2ex3_server.Services;
using ap2ex3_server.ActionModels;

namespace ap2ex3_server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvitationsController : ControllerBase
    {
        private readonly IService _userService;

        public InvitationsController(IService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> HandleInvitation([FromBody] InvitationModel invitationModel)
        {
            if (invitationModel.To == null)
                return NotFound();

            if (await _userService.DoesUserExist(invitationModel.To))
            {
                Contact newContact = new Contact()
                {
                    Username = invitationModel.From,
                    Name = invitationModel.From,
                    Server = invitationModel.Server
                };
                if (await _userService.AddContact(invitationModel.To, newContact))
                    return CreatedAtAction(nameof(HandleInvitation), new { Id = invitationModel.From });
                else
                    return Forbid();
            }
            else
                return NotFound();
        }
    }
}