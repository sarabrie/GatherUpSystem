namespace GatherUp.Core.Interfaces
{
    public interface IMailService
    {
        void Send(string to, string subject, string body);
    }
}
