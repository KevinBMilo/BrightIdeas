using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using CSharpExam.Models;

namespace CSharpExam.Models
{
    public class ViewModel
    {
        public User NewUser {get;set;}
        public User Alias {get;set;}
        public Idea NewIdea {get;set;}
        public Idea Content {get;set;}
        public List<User> AllUsers {get;set;}
        public List<Idea> AllIdeas {get;set;}
        public List<Like> AllLikes {get;set;}
        public int UserId { get; set; }
        public User Creator {get;set;}
        public int SessionData {get;set;}
        public User User {get;set;}
    }
}