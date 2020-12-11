using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WebApplication2.Data;
using WebApplication2.Dtos;
using WebApplication2.Model;
using User = WebApplication2.Model.User;

namespace WebApplication2.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private IConfiguration _Configuration;
       private readonly Context _dbContext;
        private readonly IBaseRepository<User,Guid> _user;
        private readonly IMapper _mapper;
        private readonly BaseRepository<User, Guid> _mockUser;
        public ValuesController( IConfiguration configuration, IBaseRepository<User, Guid> user,IMapper mapper,Context context,BaseRepository<User, Guid> mockUser)
        {
            _Configuration = configuration;
            _dbContext = context;
            _user = user;
            _mapper = mapper;
            _mockUser = mockUser;
        }
        [HttpGet("{id}",Name = "GetUserById")]
        public ActionResult<UserReadDto> GetUserById(Guid Id)
        {
            var result = _user.readById(Id);
            if(result !=null)
            {
                return Ok(_mapper.Map<UserReadDto>(result));

            }
            return NotFound();
        }
        //[Authorize]
        [HttpGet]
        public ActionResult <UserReadDto> GetAllUsers()
        {
            var result = _user.read();
            return Ok(_mapper.Map<IEnumerable<UserReadDto>>(result));
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public ActionResult<UserReadDto> Create(UserCreateDto userCreateDto)
        {
            var UserModel = _mapper.Map<User>(userCreateDto);
            _user.create(UserModel);
            _user.SaveChanges();
            var UserReadDto = _mapper.Map<UserReadDto>(UserModel);
            return CreatedAtRoute(nameof(GetUserById),new { Id=UserReadDto.Id},UserReadDto);
        }
        [Authorize]
        [HttpPut("{id}")]
        public ActionResult Update(Guid id,UserUpdateDto userUpdateDto)
        {
            var UserModelFromRepo = _user.readById(id);
            if (UserModelFromRepo == null)
            {
                return NotFound();
            }
            _mapper.Map(userUpdateDto, UserModelFromRepo);
            _user.update(UserModelFromRepo);
            _user.SaveChanges();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(Guid id, JsonPatchDocument<UserUpdateDto> patchDoc)
        {
            var commandModelFromRepo = _user.readById(id);
            if (commandModelFromRepo == null)
            {
                return NotFound();
            }

            var commandToPatch = _mapper.Map<UserUpdateDto>(commandModelFromRepo);
            patchDoc.ApplyTo(commandToPatch, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);

            if (!TryValidateModel(commandToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(commandToPatch, commandModelFromRepo);

            _user.update(commandModelFromRepo);

            _user.SaveChanges();

            return NoContent();
        }
        [HttpPost]
        public async Task<ActionResult<User>> Login([FromBody] User form)
        {
            var user = await _dbContext.User.FirstOrDefaultAsync(x => x.UserName == form.UserName && x.Password == form.Password);
            if (user != null)
            {
                var claims = new[]
                {
                   new Claim("Username", user.UserName),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("Role", user.Role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                var token = new JwtSecurityToken(claims: claims, expires: DateTime.UtcNow.AddDays(30),
                  notBefore: DateTime.UtcNow, audience: "Audience", issuer: "Issuer",
                  signingCredentials: new SigningCredentials(
                      new SymmetricSecurityKey(
                          Encoding.UTF8.GetBytes("Hlkjds0-324mf34pojf-14r34fwlknef0943")),
                      SecurityAlgorithms.HmacSha256));
                var Token = new JwtSecurityTokenHandler().WriteToken(token);
                var expire = DateTime.UtcNow.AddDays(30);
                return Ok(new { Token = Token, Expire = expire });

            }
            else return BadRequest();

        }
    }
}
