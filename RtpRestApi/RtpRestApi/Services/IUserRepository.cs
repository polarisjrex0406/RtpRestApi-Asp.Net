using RtpRestApi.Entities;

namespace RtpRestApi.Services
{
    public interface IUserRepository
    {
        /*
         * For simplicity purpose, lets just hard code the data
         * and just retrieve a list of users in a hard coded database
         */
        public IEnumerable<User> GetAllUsers();
    }
}
