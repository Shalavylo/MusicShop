using MusicStore.Library.data;
using MusicStore.Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Library.Services
{
    public class StoreInitializer
    {
        public static void Initialize(MusicStoreContext db)
        {
            db.Database.EnsureCreated();
            if(!db.AppUsers.Any(u => u.Login == "admin"))
            {
                db.AppUsers.Add(new AppUser
                {
                    Login = "admin",
                    Password = "admin2008",
                    Role = "Admin"
                });
            }

            if (!db.MusicClassess.Any())
            {
                db.MusicClassess.AddRange(
                    new MusicClasses 
                    { 
                        Title = "The Dark Side of the Moon", 
                        Artist = "Pink Floyd", 
                        Publisher = "Harvest Records", 
                        TrackCount = 10, 
                        Genre = "Progressive Rock", 
                        ReleaseYear = 1973, 
                        Cost = 500, 
                        Price = 850, 
                        Quantity = 10 
                    },

                    new MusicClasses 
                    {
                        Title = "Thriller", 
                        Artist = "Michael Jackson", 
                        Publisher = "Epic Records", 
                        TrackCount = 9, 
                        Genre = "Pop", 
                        ReleaseYear = 1982, 
                        Cost = 400, 
                        Price = 750, 
                        Quantity = 15 
                    },

                    new MusicClasses 
                    {
                        Title = "Kind of Blue", 
                        Artist = "Miles Davis", 
                        Publisher = "Columbia Records",
                        TrackCount = 5,
                        Genre = "Jazz", 
                        ReleaseYear = 1959,
                        Cost = 350, 
                        Price = 650, 
                        Quantity = 8 
                    },

                    new MusicClasses 
                    { 
                        Title = "Nevermind", 
                        Artist = "Nirvana", 
                        Publisher = "DGC Records",
                        TrackCount = 12,
                        Genre = "Grunge",
                        ReleaseYear = 1991,
                        Cost = 450, 
                        Price = 800, 
                        Quantity = 12 
                    }
                );
            }

            db.SaveChanges();
        }
    }
}
