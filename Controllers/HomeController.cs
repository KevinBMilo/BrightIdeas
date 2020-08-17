using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CSharpExam.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace CSharpExam.Controllers
{
    public class HomeController : Controller
    {
        private int? UserSessionData
        {
            get { return HttpContext.Session.GetInt32("UserId");}
            set { HttpContext.Session.SetInt32("UserId", (int)value);}
        }
        private MyContext dbContext;
        public HomeController(MyContext context)
        {
            dbContext = context;
        }


        [HttpGet("")]
        public IActionResult Index()
        {
            return View("Index");
        }



        [HttpGet("bright_ideas")]
        public IActionResult GetAll()
        {
            if(UserSessionData != null)
            {
                var SessionId = (int) UserSessionData;
                ViewModel IdeaGroup = new ViewModel()
                {
                    AllIdeas = dbContext.Ideas.ToList(),
                    AllUsers = dbContext.Users.ToList(),
                    AllLikes = dbContext.Likes.ToList(),
                    SessionData = SessionId
                };
                User oneUser = dbContext.Users.FirstOrDefault(User => User.UserId == (int) UserSessionData);
                ViewBag.UserId = oneUser;
                return View("Dashboard", IdeaGroup);
            }
            ModelState.AddModelError("Name", "You are not logged in.");
            return View("Index");
        }


        [HttpGet("bright_ideas/{IdeaId}")]
        public IActionResult ShowIdea(int IdeaId)
        {
            if(UserSessionData != null)
            {
                var ideasWithFansAndLikes = dbContext.Ideas
                    .Include(idea => idea.Likes)
                    .ThenInclude(lik => lik.User)
                    .Include(idea => idea.Creator)
                    .FirstOrDefault(idea => idea.IdeaId == IdeaId);
                return View("Show", ideasWithFansAndLikes);
            }
            ModelState.AddModelError("Name", "You are not logged in.");
            return View("Index");
        }


        [HttpGet("users/{UserId}")]
        public IActionResult ShowUser(int UserId)
        {
            if(UserSessionData != null)
            {
                User oneUser = dbContext.Users.FirstOrDefault(User => User.UserId == UserId);
                return View("Showuser", oneUser);
            }
            ModelState.AddModelError("Name", "You are not logged in.");
            return View("Index");
        }


        [HttpPost("think")]
        public IActionResult Think(Idea newIdea)
        {
            if(ModelState.IsValid)
            {
                Idea submittedIdea = newIdea;
                newIdea.UserId = (int) UserSessionData;
                dbContext.Add(newIdea);
                dbContext.SaveChanges();
                return RedirectToAction("GetAll");
            }
            var SessionId = (int) UserSessionData;
            ViewModel IdeaGroup = new ViewModel()
                {
                    AllIdeas = dbContext.Ideas.ToList(),
                    AllUsers = dbContext.Users.ToList(),
                    AllLikes = dbContext.Likes.ToList(),
                    SessionData = SessionId
                };
            User oneUser = dbContext.Users.FirstOrDefault(User => User.UserId == (int) UserSessionData);
            ViewBag.UserId = oneUser;
            return View("Dashboard", IdeaGroup);
        }

        [HttpGet("agree/{IdeaId}")]
        public IActionResult Agree(int IdeaId, Like newLike)
        {
            Like subLike = newLike;
            newLike.UserId = (int) UserSessionData;
            newLike.IdeaId = (int) IdeaId;
            dbContext.Add(newLike);
            dbContext.SaveChanges();
            return RedirectToAction("GetAll");
        }


        [HttpGet("delete/{IdeaId}")]
        public IActionResult Delete(int IdeaId)
        {
            Idea RetrievedIdea = dbContext.Ideas.FirstOrDefault(Idea => Idea.IdeaId == IdeaId);
            dbContext.Ideas.Remove(RetrievedIdea);
            dbContext.SaveChanges();
            return RedirectToAction ("GetAll");
        }



        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            User submittedUser = newUser;
            if(dbContext.Users.Any(u => u.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "That email has been taken.");
                return View("Index");
            }
            if(ModelState.IsValid)
            {
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.Add(newUser);
                dbContext.SaveChanges();
                UserSessionData = newUser.UserId;
                return RedirectToAction("GetAll");
            }
            return View("Index");
        }

        [HttpPost("verify")]
        public IActionResult Verify(LoginUser userSubmission)
        {
            LoginUser submittedUser = userSubmission;
            if(ModelState.IsValid)
            {
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.LoginEmail);
                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email.");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.LoginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LoginPassword", "Your current information does not match anything in our database.");
                    return View("Index");
                }
                UserSessionData = userInDb.UserId;
                return RedirectToAction("GetAll");
            }
            return View("Index");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
