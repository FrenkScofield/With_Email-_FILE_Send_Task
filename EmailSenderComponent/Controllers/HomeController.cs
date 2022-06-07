using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using EmailSenderComponent.Models.DAL;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using EmailSenderComponent.Services;
using Microsoft.Extensions.Configuration;

namespace EmailSenderComponent.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration configuration;
        private readonly ILogger<HomeController> _logger;
        private readonly MyContext _context;
        private IWebHostEnvironment webHostEnvironment;
        private readonly EmailSender _emailsender;

        public HomeController(ILogger<HomeController> logger,
                                            MyContext context,

                                            IWebHostEnvironment _webHostEnvironment,
                                            IConfiguration _configuration)
        {
            webHostEnvironment = _webHostEnvironment;
            _logger = logger;
            _context = context;
            configuration = _configuration;
            _emailsender = new EmailSender(configuration["EmailSettings:Host"],
                                            configuration.GetValue<int>("EmailSettings:Port"),
                                            configuration.GetValue<bool>("EmailSettings:SSL"),
                                            configuration["EmailSettings:Username"],
                                            configuration["EmailSettings:Password"]
                );
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SendMail(string mail, IFormFile[] attachments)
        {
            
            List<string> fileNames = null;
            if (attachments != null && attachments.Length > 0)
            {
                fileNames = new List<string>();
                foreach (IFormFile attachment in attachments)
                {
                    var path = Path.Combine(webHostEnvironment.WebRootPath, "uploads", attachment.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await attachment.CopyToAsync(stream);
                    }
                    fileNames.Add(path);
                }
            }

            await _emailsender.SendEmailAsync(mail, "", "", fileNames);

            return RedirectToAction(nameof(Index));
        }

    }
}