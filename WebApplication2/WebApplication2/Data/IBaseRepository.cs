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
    public interface IBaseRepository<T, K>
    {
        List<T> read();

        T readById(K id);
        Task<T> create(T entity);
        T update(T entity);
        T delete(T entity);
        bool SaveChanges();
        T GetUser(string UserName, string Password);
    }
}
