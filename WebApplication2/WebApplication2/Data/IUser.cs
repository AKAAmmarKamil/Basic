using BlazingIdentity.DBModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Model;
using User = WebApplication2.Model.User;

namespace WebApplication2.Data
{
    public interface IUser
    {
        User GetUserById(Guid Id);
        User GetUser(string UserName,string Password);

        IEnumerable<User> GetAllUsers();
        void Create(User user);
        void Update(User user);

        bool SaveChanges();
    }
}
