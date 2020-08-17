using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace CSharpExam.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Required]
        [Display(Name = " ")]
        [MinLength(2)]
        public string Name {get;set;}

        [Required]
        [Display(Name = " ")]
        [MinLength(2)]
        public string Alias {get;set;}

        [Required]
        [Display(Name = " ")]
        [EmailAddress]
        public string Email {get;set;}

        [DataType(DataType.Password)]
        [Required]
        [Display(Name = " ")]
        [MinLength(8)]
        public string Password {get;set;}
        
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        // Will not be mapped to your users table!
        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm {get;set;}
        public List<Idea> CreatedIdeas {get;set;}
        public List<Like> Likes {get;set;}
    }

    public class LoginUser
    {
        [Required]
        [EmailAddress]
        [Display(Name = " ")]
        public string LoginEmail {get; set;}
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
        [Display(Name = " ")]
        public string LoginPassword { get; set; }
    }

    public class Idea
    {
        [Key]
        public int IdeaId {get;set;}

        [Required]
        [Display(Name = " ")]
        [MinLength(5, ErrorMessage="Do it right!")]
        public string Content {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public int UserId { get; set; }
        public User Creator {get;set;}
        public List<Like> Likes {get;set;}
        public Idea()
                {
                    Likes = new List<Like>();
                    CreatedAt = DateTime.Now;
                    UpdatedAt = DateTime.Now;
                }
    }
    public class Like
    {
        [Key]
        public int LikeId {get;set;}
        public int UserId {get;set;}
        public int IdeaId {get;set;}
        public User User {get;set;}
        public Idea Idea {get;set;}
    }
}