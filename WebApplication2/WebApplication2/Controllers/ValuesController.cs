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
        private readonly IUser _user;
        private readonly IMapper _mapper;
        private readonly MockUser _mockUser;
        public ValuesController( IConfiguration configuration, IUser user,IMapper mapper,Context context,MockUser mockUser)
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
            var result = _user.GetUserById(Id);
            if(result !=null)
            {
                return Ok(_mapper.Map<UserReadDto>(result));

            }
            return NotFound();
        }
        [Authorize]
        [HttpGet]
        public ActionResult <UserReadDto> GetAllUsers()
        {
            var result = _user.GetAllUsers();
            return Ok(_mapper.Map<IEnumerable<UserReadDto>>(result));
        }
        [Authorize(Roles ="Admin")]
        [HttpPost]
        public ActionResult<UserReadDto> Create(UserCreateDto userCreateDto)
        {
            var UserModel = _mapper.Map<User>(userCreateDto);
            _user.Create(UserModel);
            _user.SaveChanges();
            var UserReadDto = _mapper.Map<UserReadDto>(UserModel);
            return CreatedAtRoute(nameof(GetUserById),new { Id=UserReadDto.Id},UserReadDto);
        }
        [Authorize]
        [HttpPut("{id}")]
        public ActionResult Update(Guid id,UserUpdateDto userUpdateDto)
        {
            var UserModelFromRepo = _user.GetUserById(id);
            if (UserModelFromRepo == null)
            {
                return NotFound();
            }
            _mapper.Map(userUpdateDto, UserModelFromRepo);
            _user.Update(UserModelFromRepo);
            _user.SaveChanges();
            return NoContent();
        }
        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(Guid id, JsonPatchDocument<UserUpdateDto> patchDoc)
        {
            var commandModelFromRepo = _user.GetUserById(id);
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

            _user.Update(commandModelFromRepo);

            _user.SaveChanges();

            return NoContent();
        }
        [HttpPost]
        public async Task<ActionResult<User>> Login([FromBody] User model)
        {
            var response = await _mockUser.Authenticate(model);
            if (response != "")
            {
                return Ok(response);
            }
            else return BadRequest();

        }
    }
}
