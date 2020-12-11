using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplication2.Model;
namespace WebApplication2.Data
{
    public abstract class BaseRepository<T, TId> : IBaseRepository<T, TId> where T:User
    {
        private readonly Context _context;
        private readonly IBaseRepository<T,TId> _wrapper;
        private readonly IMapper _mapper;


        public BaseRepository(IBaseRepository<T,TId> wrapper, IMapper mapper,Context context)
        {
            _wrapper = wrapper;
            _mapper = mapper;
            _context = context;
        }
 
        public async Task<string> Authenticate(User form)
        {
            var user = _wrapper.GetUser(form.UserName,form.Password);
            if (user != null)
            {
                var claims = new[]
                {
                    //  new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                    new Claim("Username", form.UserName),
                    new Claim(ClaimTypes.Role, form.Role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddDays(30),
                  notBefore: DateTime.UtcNow, audience: "Audience", issuer: "Issuer",
                  signingCredentials: new SigningCredentials(
                      new SymmetricSecurityKey(
                          Encoding.UTF8.GetBytes("Hlkjds0-324mf34pojf-14r34fwlknef0943")),
                      SecurityAlgorithms.HmacSha256));
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            return "" ;
        }
        public User GetUserById(Guid Id)
        {
            return _context.User.FirstOrDefault(x=>x.Id==Id);
        }
        public User GetUser(string UserName,string Password)
        {
            return _context.User.FirstOrDefault(x => x.UserName == UserName && x.Password==Password);
        }
        public IEnumerable<User> GetAllUsers()
        {
            return _context.User.ToList();
        }
        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public async Task<T> Create(T t)
        {
            await _context.Set<T>().AddAsync(t);
            await _context.SaveChangesAsync();
            return t;
        }

        public List<T> read()
        {
            throw new NotImplementedException();
        }

        public T readById(TId id)
        {
            throw new NotImplementedException();
        }

        public T delete(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<T> create(T entity)
        {
            throw new NotImplementedException();
        }

        public T update(T entity)
        {
            throw new NotImplementedException();
        }

        T IBaseRepository<T, TId>.GetUser(string UserName, string Password)
        {
            throw new NotImplementedException();
        }
    }
}
