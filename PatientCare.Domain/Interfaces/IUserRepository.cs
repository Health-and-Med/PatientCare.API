using PatientCare.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientCare.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByUsernameAsync(string email);
        Task AddUserAsync(User user);
    }
}

