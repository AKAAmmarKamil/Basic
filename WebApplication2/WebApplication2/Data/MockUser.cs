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
    public class MockUser : IUser
    {
        private readonly Context _context;
        private readonly IUser _wrapper;
        private readonly IMapper _mapper;


        public MockUser(IUser wrapper, IMapper mapper,Context context)
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
        public void Create(User user)
        {
            if (user == null)
            {
                throw new ArgumentException(nameof(user));
            }
            _context.User.Add(user);
        }
        public void Update(User user)
        {
        }
        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
