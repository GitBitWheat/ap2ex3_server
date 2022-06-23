using ap2ex3_server.Models;

namespace ap2ex3_server.Services
{
    public interface IService
    {
        Task<List<User>?> GetAllUsers();

        Task<User?> GetUser(string username);

        Task AddUser(User user);

        Task<bool> Login(string username, string password);

        Task<bool> DoesUserExist(string username);

        Task<List<Contact>?> GetContacts(string username);

        Task<Contact?> GetContactOfUsername(string username, string contactUsername);

        Task<bool> UpdateContactOfUsername(string username, string contactUsername, string name, string server);

        Task<bool> AddContact(string username, Contact newContact);

        Task<bool> RemoveContact(string username, string contactUsername);

        Task<bool> IsContactOfUser(string username, string contactUsername);

        Task<List<Message>?> GetMessagesBetweenUserAndContact(string username, string contactUsername);

        Task<Message?> GetMessageOfIdBetweenUserAndContact(string username, string contactUsername, int messageId);

        Task<bool> UpdateMessageOfIdBetweenUserAndContact(string username, string contactUsername, int messageId, string content);

        Task<bool> SendMessage(string messageContent, string sentFromUserUsername, string sendToContactUsername);

        Task<bool> ReceiveMessage(string messageContent, string sentFromContactUsername, string sendToUserUsername);

        Task<bool> RemoveMessage(string userUsername, string contactUsername, int messageId);
    }
}
