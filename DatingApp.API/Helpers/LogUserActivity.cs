using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DatingApp.API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            var userId = int.Parse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();
            //var user = await repo.GetUserAsync(userId);
            //user.LastActive = DateTime.Now;

            //Moj experiment da se ne ide u bazu po usera samo da bi se LastActive promijenilo
            var db = resultContext.HttpContext.RequestServices.GetService<DataContext>();

            if (db.Set<User>().Local.Any(a => a.Id == userId))
            {
                var currentUser = db.Set<User>().Local.First(f => f.Id == userId);
                currentUser.LastActive = DateTime.Now;
            }
            else
            {
                var currentUser = new User() { Id = userId, LastActive = DateTime.Now };
                var entiry= db.Users.Attach(currentUser);

                entiry.Property(p => p.LastActive).IsModified=true;
            }



            await repo.SaveAll();
        }
    }
}
