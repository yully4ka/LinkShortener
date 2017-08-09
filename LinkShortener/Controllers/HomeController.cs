using LinkShortener.Data;
using LinkShortener.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LinkShortener.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        //Start
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.ApplicationUser> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<Models.ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var links = await _context.Links.Where(x => x.User == user).ToListAsync();
            ViewBag.BaseUrl = string.Format("{0}://{1}/", Request.Scheme, Request.Host);
            return View(links);
        }

        public async Task<IActionResult> AddLink(string fullLink)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            Link link = new Link();
            if (fullLink.StartsWith("http://") || fullLink.StartsWith("https://") || fullLink.StartsWith("ftp://"))
                link.OriginalLink = fullLink;
            else
                link.OriginalLink = "http://" + fullLink;
            link.User = user;
            link.ShortLink = CreateShortUrl();

            await _context.Links.AddAsync(link);
            await _context.SaveChangesAsync();

            var links = await _context.Links.Where(x => x.User == user).ToListAsync();

            ViewBag.BaseUrl = string.Format("{0}://{1}/", Request.Scheme, Request.Host);

            return View("Index", links);
        }

        public async Task<RedirectResult> Link(string id)
        {
            var link = await _context.Links.FirstOrDefaultAsync(x => x.ShortLink == id);
            if (link != null)
            {
                link.ClickCount++;
                await _context.SaveChangesAsync();
            }

            return Redirect(link.OriginalLink);
        }

        private string CreateShortUrl()
        {
            var shortUrl = RandomString(5);
            if(_context.Links.Where(x => x.ShortLink == shortUrl).Any())
            {
                CreateShortUrl();
            }
            return shortUrl;
        }

        private string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        //End
    }
}
