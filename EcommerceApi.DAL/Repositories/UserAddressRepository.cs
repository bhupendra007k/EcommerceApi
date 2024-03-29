﻿using EcommerceApi.DAL.DataContext;
using EcommerceApi.DAL.Entities.UserData;
using EcommerceApi.DAL.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceApi.DAL.Repositories
{
    public class UserAddressRepository : GenericRepository<UserAddress>, IUserAddressRepository
    {
        private readonly UserManager<User> userManager;

        public UserAddressRepository(UserManager<User> userManager, EcommerceContext context)
            : base(context)
        {
            this.userManager = userManager;
        }
        public async Task<IEnumerable<UserAddress>> GetAllForUserAsync(string userId) => 
            await table.Where(x => x.User.Id == userId).ToListAsync();
        public async Task InsertAsync(UserAddress userAddress, string username)
        {
            //Find parent entity
            var user = await userManager.FindByNameAsync(username);
            
            if (user != null)
            {
                //Add child to parent
                if (user.UserAddresses == null)
                {
                    user.UserAddresses = new List<UserAddress> { userAddress };
                }
                else
                {
                    user.UserAddresses.Add(userAddress);
                }
                //Add child
                await InsertAsync(userAddress);
            }
        }
    }
}
