using ap2ex3_server.Data;
using ap2ex3_server.Models;
using Microsoft.EntityFrameworkCore;

namespace ap2ex3_server.Services
{
    public class Service : IService
    {
        private readonly ap2ex3_serverContext _context;

        public Service(ap2ex3_serverContext context)
        {
            _context = context;
        }

        public async Task<List<User>?> GetAllUsers()
        {
            return _context.User != null ?
                        await _context.User.ToListAsync() :
                        null;
        }

        private async Task<List<Contact>?> GetAllContacts()
        {
            return _context.Contact != null ?
                        await _context.Contact.Include(c => c.User).ToListAsync() :
                        null;
        }

        private async Task<List<Message>?> GetAllMessages()
        {
            return _context.Message != null ?
                        await _context.Message.Include(m => m.Contact).ToListAsync() :
                        null;
        }

        public async Task<User?> GetUser(string username)
        {
            if (_context.User == null)
            {
                return null;
            }

            User? foundUser = await _context.User.FirstOrDefaultAsync(m => m.Username != null ? m.Username.Equals(username) : false);
            if (foundUser != null)
            {
                return foundUser;
            }
            else
            {
                return null;
            }
        }

        public async Task AddUser(User user)
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Login(string username, string password)
        {
            User? user = await GetUser(username);
            if (user == null)
                return false;

            return user.Password == password;
        }

        public async Task<bool> DoesUserExist(string username)
        {
            List<User>? users = await GetAllUsers();
            if (users == null)
                return false;
            else
                return users.Exists(u => u.Username != null ? u.Username.Equals(username) : false);
        }

        public async Task<List<Contact>?> GetContacts(string username)
        {
            List<Contact>? contacts = await GetAllContacts();
            if (contacts == null)
                return null;
            else
                return contacts.FindAll(c => (c.User != null && c.User.Username != null) ? c.User.Username.Equals(username) : false);
        }

        public async Task<Contact?> GetContactOfUsername(string username, string contactUsername)
        {
            List<Contact>? contactList = await GetContacts(username);
            if (contactList == null)
            {
                return null;
            }

            return contactList.Find(c => c.Username != null ? c.Username.Equals(contactUsername) : false);
        }

        public async Task<bool> UpdateContactOfUsername(string username, string contactUsername, string name, string server)
        {
            Contact? contact = await GetContactOfUsername(username, contactUsername);
            if (contact == null)
                return false;

            contact.Name = name;
            contact.Server = server;
            _context.Update(contact);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddContact(string username, Contact newContact)
        {
            User? user = await GetUser(username);
            List<Contact>? contactList = await GetContacts(username);
            if (user == null || contactList == null)
                return false;

            if (contactList.Exists(c => c.Username != null ? c.Username.Equals(newContact.Username) : false))
                return false;
            else
            {
                user.Contacts.Add(newContact);
                _context.Update(user);
                _context.Add(newContact);
                await _context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> RemoveContact(string username, string contactUsername)
        {
            User? user = await GetUser(username);
            List<Contact>? contactList = await GetContacts(username);
            if (user == null || contactList == null || _context.Contact == null)
                return false;

            foreach (Contact contact in contactList)
            {
                if (contact.Username != null && contact.Username.Equals(contactUsername))
                {
                    List<Message>? msgList = await GetAllMessages();
                    if (msgList != null && _context.Message != null)
                    {
                        msgList = msgList.FindAll(m => (m.Contact != null && m.Contact.Username != null) ? m.Contact.Username.Equals(contactUsername) : false);
                        if (msgList != null)
                        {
                            foreach (Message message in msgList)
                                _context.Message.Remove(message);
                        }
                    }
                    _context.Contact.Remove(contact);
                }
            }

            bool wereAnyRemoved = contactList.RemoveAll(c => c.Username != null ? c.Username.Equals(contactUsername) : false) > 0;
            user.Contacts = contactList;

            _context.Update(user);
            await _context.SaveChangesAsync();

            return wereAnyRemoved;
        }

        public async Task<bool> IsContactOfUser(string username, string contactUsername)
        {
            User? user = await GetUser(username);
            if (user == null)
                return false;
            else
                return user.Contacts.Exists(c => c.Username != null ? c.Username.Equals(contactUsername) : false);
        }



        public async Task<List<Message>?> GetMessagesBetweenUserAndContact(string username, string contactUsername)
        {
            List<Message>? msgList = await GetAllMessages();
            Contact? contact = await GetContactOfUsername(username, contactUsername);
            if (msgList == null)
                return null;

            msgList = msgList.FindAll(m => (m.Contact != null && m.Contact.Username != null) ? m.Contact.Username.Equals(contactUsername) : false);
            return msgList.Count > 0 ? msgList : null;
        }

        public async Task<Message?> GetMessageOfIdBetweenUserAndContact(string username, string contactUsername, int messageId)
        {
            List<Message>? msgList = await GetMessagesBetweenUserAndContact(username, contactUsername);
            if (msgList == null)
                return null;

            return msgList.Find(m => m.Id.Equals(messageId));
        }

        public async Task<bool> UpdateMessageOfIdBetweenUserAndContact(string username, string contactUsername, int messageId, string content)
        {
            Message? message = await GetMessageOfIdBetweenUserAndContact(username, contactUsername, messageId);

            if (message == null)
                return false;

            message.Content = content;
            _context.Update(message);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<bool> SendMessage(string messageContent, string sentFromUserUsername, string sendToContactUsername)
        {
            Contact? contact = await GetContactOfUsername(sentFromUserUsername, sendToContactUsername);
            if (contact == null)
                return false;

            DateTime creationTime = DateTime.Now;
            Message message = new Message()
            {
                Content = messageContent,
                Created = creationTime,
                Sent = true
            };

            contact.Messages.Add(message);
            contact.Last = messageContent;
            contact.Lastdate = creationTime.ToString("s");

            _context.Update(contact);
            _context.Add(message);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ReceiveMessage(string messageContent, string sentFromContactUsername, string sendToUserUsername)
        {
            Contact? contact = await GetContactOfUsername(sendToUserUsername, sentFromContactUsername);
            if (contact == null)
                return false;

            DateTime creationTime = DateTime.Now;
            Message message = new Message()
            {
                Content = messageContent,
                Created = creationTime,
                Sent = false
            };

            contact.Messages.Add(message);
            contact.Last = messageContent;
            contact.Lastdate = creationTime.ToString("s");

            _context.Update(contact);
            _context.Add(message);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveMessage(string userUsername, string contactUsername, int messageId)
        {
            Contact? contact = await GetContactOfUsername(userUsername, contactUsername);
            List<Message>? msgList = await GetMessagesBetweenUserAndContact(userUsername, contactUsername);
            if (contact == null || msgList == null || _context.Message == null)
                return false;

            foreach (Message message in msgList)
            {
                if (message.Id.Equals(messageId))
                    _context.Message.Remove(message);
            }

            bool wereAnyRemoved = msgList.RemoveAll(m => m.Id.Equals(messageId)) > 0;
            contact.Messages = msgList;

            _context.Update(contact);
            await _context.SaveChangesAsync();

            return wereAnyRemoved;
        }
    }
}
