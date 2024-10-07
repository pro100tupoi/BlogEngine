using Blog_Engine_2.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Blog_Engine_2.Objects;
using System.IO;
using System;
using Azure.Core;

namespace Blog_Engine_2.Controllers
{
    public class HomeController(Context context) : Controller
    {
        [HttpGet]
        public IActionResult Authorization() => View();

        [HttpPost]
        public async Task<IActionResult> Authorization(LoginRequest request)
        {

            var user = context.Users.SingleOrDefault(x => x.Login == request.Login && x.Password == request.Password);
            if (user != null)
            {
                var claims = new Claim[]
                {
                    new(ClaimTypes.Name, user.Login),
                    new(ClaimTypes.Role, user.Role.ToString())
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(identity.AuthenticationType, new(identity));
                return RedirectToAction(nameof(PostCreate));
            }
            else
            {
                return View(request);
            }
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = context.Users.SingleOrDefault(x => x.Login == request.Login);
            if (user != null)
            {
                return View(request);
            }
            else
            {
                context.Users.Add(new User
                {
                    Login = request.Login,
                    Password = request.Password,
                    Role = RoleUser.Viewer
                });
                context.SaveChanges();
                return RedirectToAction(nameof(Authorization));
            }
        }
        [Authorize(Policy = Constants.AuthorPolicy)]
        [HttpGet]
        public IActionResult PostCreate() => View();

        [Authorize(Policy = Constants.AuthorPolicy)]
        [HttpPost]
        public async Task<IActionResult> PostCreate(PostCreateModel request)
        {
            var post = context.Posts.SingleOrDefault(x => x.Header == request.Header);
            if (post != null)
            {
                return View(request);
            }
            var login = User.Claims.First(y => y.Type == ClaimTypes.Name).Value;
            var user = context.Users.SingleOrDefault(x => x.Login == login);

            if (user == null)
            {
                return Unauthorized();
            }
            Picture? picture = null;
            if (request.Photo != null)
            {
                byte[] imageData = null;

                using (var binaryReader = new BinaryReader(request.Photo.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)request.Photo.Length);
                }

                picture = new Picture
                {
                    Photo = imageData,
                    Name = request.Photo.Name
                };

                context.Pictures.Add(picture);
            }

            context.Posts.Add(new Post
            {
                Header = request.Header,
                Content = request.Content,
                Avtor = user,
                UploadTime = DateTime.Now,
                Photo = picture
            });

            context.SaveChanges();
            return RedirectToAction(nameof(Main));
        }

        [Authorize(Policy = Constants.AuthorPolicy)]
        [HttpGet]
        public IActionResult Main()
        {
            var posts = context.Posts.Include(x => x.Photo).OrderByDescending(x => x.UploadTime).ToList();
            return View(posts);
        }

        [HttpGet]
        public IActionResult Pic(Guid id)
        {
            var picture = context.Pictures.Find(id);
            if (picture == null)
            {
                return NotFound();
            }
            return File(picture.Photo, "image/jpeg", picture.Name);
        }

        [HttpGet]
        public IActionResult PostEdit(Guid id)
        {
            var post = context.Posts.Find(id);
            if (post == null)
            {
                return NotFound();
            }
            var request = new PostEditRequest
            {
                Content = post.Content,
                Header = post.Header,
                Id = id
            };
            return View(request);
        }

        [HttpPost]
        public IActionResult PostEdit(PostEditRequest request)
        {
            var post = context.Posts.Include(x => x.Photo).SingleOrDefault(x => x.Id == request.Id);
            post.Content = request.Content;
            post.Header = request.Header;
            if ((request.DeletePhoto || request.Photo != null) && post.Photo != null)
            {
                context.Remove(post.Photo);
            }
            if (request.Photo != null && !request.DeletePhoto)
            {
                byte[] imageData = null;

                using (var binaryReader = new BinaryReader(request.Photo.OpenReadStream()))
                {
                    imageData = binaryReader.ReadBytes((int)request.Photo.Length);
                }

                var picture = new Picture
                {
                    Photo = imageData,
                    Name = request.Photo.Name
                };

                context.Pictures.Add(picture);
                post.Photo = picture;
            }
            context.SaveChanges();
            return RedirectToAction("Main");
        }

        [HttpGet]
        public IActionResult PostPreDelete(Guid id) 
        {
            var post = context.Posts.Include(x => x.Photo).SingleOrDefault(x => x.Id == id);
            if (post == null)
            {
                return NotFound();

            }
            return View(post); 
        }

        [HttpPost]
        public IActionResult PostDelete(Guid id)
        {
            var post = context.Posts.Include(x => x.Photo).SingleOrDefault(x => x.Id == id);
            if (post == null)
            {
                return NotFound();

            }
            if (post.Photo != null)
            {
                context.Pictures.Remove(post.Photo);
            }
            context.Posts.Remove(post);
            context.SaveChanges();
            return RedirectToAction("Main");
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Main");
        }
    }
}
//private readonly ILogger<HomeController> _logger;

//public HomeController(ILogger<HomeController> logger)
//{
//    _logger = logger;
//}

//public IActionResult Index()
//{
//    return View();
//}

//public IActionResult Privacy()
//{
//    return View();
//}

//[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//public IActionResult Error()
//{
//    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//}

