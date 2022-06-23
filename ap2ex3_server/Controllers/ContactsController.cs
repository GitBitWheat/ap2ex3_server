using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Net.Http.Headers;
using ap2ex3_server.Models;
using ap2ex3_server.Services;
using ap2ex3_server.ActionModels;

namespace ap2ex3_server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IService _service;

        public ContactsController(IService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<ContactApiModel>> GetAllContacts()
        {
            string loggedUserId = GetLoggedUserId();
            List<Contact>? contactList = await _service.GetContacts(loggedUserId);

            if (contactList == null)
                contactList = new List<Contact>();

            return Enumerable.Select(contactList, c => new ContactApiModel(c)).ToArray();
        }

        [HttpPost]
        public async Task<IActionResult> AddContact([FromBody] NewContactModel newContact)
        {
            string loggedUserId = GetLoggedUserId();

            if (!await _service.AddContact(loggedUserId, newContact.ConvertToContact()))
                return Forbid();

            return CreatedAtAction(nameof(AddContact), new { Id = newContact.Id });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactWithId(string id)
        {
            string loggedUserId = GetLoggedUserId();
            Contact? contact = await _service.GetContactOfUsername(loggedUserId, id);

            if (contact == null)
                return NotFound();

            return Ok(new ContactApiModel(contact));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ChangeContactDetails([FromBody] ContactDetailsModel contactDetails, string id)
        {
            string loggedUserId = GetLoggedUserId();

            if (contactDetails.Name == null || contactDetails.Server == null)
                return NotFound();

            if (await _service.UpdateContactOfUsername(loggedUserId, id, contactDetails.Name, contactDetails.Server))
                return NoContent();
            else
                return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveContact(string id)
        {
            string loggedUserId = GetLoggedUserId();

            if (await _service.RemoveContact(loggedUserId, id))
                return NoContent();
            else
                return NotFound();
        }

        [HttpGet("{id}/messages")]
        public async Task<IActionResult> GetMessagesWithContactOfId(string id)
        {
            string loggedUserId = GetLoggedUserId();
            List<Message>? msgList = await _service.GetMessagesBetweenUserAndContact(loggedUserId, id);

            if (msgList == null)
                return NotFound();

            return Ok(Enumerable.Select(msgList, message => new MessageApiModel(message)).ToArray());
        }

        [HttpPost("{id}/messages")]
        public async Task<IActionResult> SendMessage([FromBody] MessageContentModel messageContent, string id)
        {
            string loggedUserId = GetLoggedUserId();

            if (messageContent.Content == null)
                return Forbid();

            if (await _service.SendMessage(messageContent.Content, loggedUserId, id))
                return CreatedAtAction(nameof(SendMessage), new { Content = messageContent.Content });
            else
                return Forbid();
        }

        [HttpGet("{id}/messages/{id2}")]
        public async Task<IActionResult> GetMessageOfIdWithContactOfId(string id, int id2)
        {
            string loggedUserId = GetLoggedUserId();
            Message? message = await _service.GetMessageOfIdBetweenUserAndContact(loggedUserId, id, id2);

            if (message == null)
                return NotFound();

            return Ok(new MessageApiModel(message));
        }

        [HttpPut("{id}/messages/{id2}")]
        public async Task<IActionResult> ChangeMessageDetails([FromBody] MessageContentModel messageContent, string id, int id2)
        {
            string loggedUserId = GetLoggedUserId();

            if (messageContent.Content == null)
                return NotFound();

            if (await _service.UpdateMessageOfIdBetweenUserAndContact(loggedUserId, id, id2, messageContent.Content))
                return NoContent();
            else
                return NotFound();
        }

        [HttpDelete("{id}/messages/{id2}")]
        public async Task<IActionResult> RemoveMessage(string id, int id2)
        {
            string loggedUserId = GetLoggedUserId();

            if (await _service.RemoveMessage(loggedUserId, id, id2))
                return NoContent();
            else
                return NotFound();
        }

        private string GetLoggedUserId()
        {
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(_bearer_token);
            var tokenS = jsonToken as JwtSecurityToken;
            return tokenS.Claims.First(claim => claim.Type == "UserId").Value;
        }
    }
}