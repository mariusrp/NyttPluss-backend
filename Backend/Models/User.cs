//modell for en bruker
using System;
using System.ComponentModel.DataAnnotations;
using AspNetCore.Identity.Mongo.Model;

namespace Backend.Models
{
    public class User : MongoUser
    {
        [Key] public string UserId { get; set; } //  GoogleId
        public List<RssItem> FavoriteRssItems { get; set; }= new List<RssItem>();

        
        [MaxLength(200)] public string ProfilePictureUrl { get; set; } // Hentes fra Google

        public DateTime RegisteredOn { get; set; } = DateTime.UtcNow; // Registreringstidspunktet
        
    }
}


