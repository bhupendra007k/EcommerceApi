﻿using AutoMapper;
using EcommerceApi.DAL.Entities.UserData;
using EcommerceApi.DAL.Repositories.IRepositories;
using EcommerceApi.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Net;

namespace EcommerceApi.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserAddressController : ControllerBase
    {
        private readonly ILogger<UserAddressController> logger;
        private readonly IUserAddressRepository repository;
        private readonly IMapper mapper;

        public UserAddressController(ILogger<UserAddressController> logger,
            IUserAddressRepository repository,
            IMapper mapper)
        {
            this.logger = logger;
            this.repository = repository;
            this.mapper = mapper;
        }

        //Admin: get all address
        //[Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IEnumerable<UserAddressViewModel>> GetAsync() =>
            mapper.Map<IEnumerable<UserAddress>, IEnumerable<UserAddressViewModel>>(await repository.GetAllAsync());

        //[Authorize(Roles = "customer")]
        [HttpGet("user")]
        public async Task<IActionResult> GetForUserAsync()
        {
            //Get the userid
            var userId = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.NameIdentifier)).Value;
            if (userId == null)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, new { message = "no userid found" });
            }
            var entityToModel = mapper.Map<IEnumerable<UserAddress>,
                IEnumerable<UserAddressViewModel>> (await repository.GetAllForUserAsync(userId));
            return Ok(entityToModel);
        }

        [HttpGet("{id}")]
        public async Task<UserAddressViewModel> GetAsync([FromRoute] Guid id) =>
            mapper.Map<UserAddress, UserAddressViewModel>(await repository.GetByIdAsync(id));
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody] UserAddressViewModel userAddressViewModel)
        {
            if (ModelState.IsValid)
            {
                var username = User.Claims.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Name)).Value;
                if (username != null)
                {
                    var modelToEntity = mapper.Map<UserAddressViewModel, UserAddress>(userAddressViewModel);

                    await repository.InsertAsync(modelToEntity, username);
                    await repository.SaveAsync();
                    return Ok();
                }
            }
            return BadRequest();
        }

        [HttpPut("{id}")]
        public async Task PutAsync([FromRoute] Guid id, [FromBody] UserAddressViewModel userAddressViewModel)
        {
            var userAddress = mapper.Map<UserAddressViewModel, UserAddress>(userAddressViewModel);
            userAddress.Id = id;
            repository.Update(userAddress);
            await repository.SaveAsync();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id)
        {
            var isPresent = await repository.DeleteAsync(id);
            if (isPresent == false)
            {
                return BadRequest(new { message = "id is not present" });
            }
            await repository.SaveAsync();
            return Ok();
        }
    }
}
