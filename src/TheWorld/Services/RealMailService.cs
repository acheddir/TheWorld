namespace TheWorld.Services
{
    public class RealMailService : IMailService
    {
        #region Implementation of IMailService

        public bool SendMail(string to, string @from, string subject, string body)
        {
            return true;
        }

        #endregion
    }
}