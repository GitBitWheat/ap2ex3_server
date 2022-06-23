using ap2ex3_server.Models;

namespace ap2ex3_server.ActionModels
{
    public class ContactApiModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Server { get; set; }
        public string? Last { get; set; }
        public string? Lastdate { get; set; }

        public ContactApiModel(Contact contact)
        {
            Id = contact.Username;
            Name = contact.Name;
            Server = contact.Server;
            Last = contact.Last;
            Lastdate = contact.Lastdate;
        }
    }
}
