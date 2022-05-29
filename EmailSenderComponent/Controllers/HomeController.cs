using EmailSenderComponent.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using EmailSenderComponent.Models.DAL;
using EmailSenderComponent.Models.BLL;
using System;
using System.IO;
using iText.Html2pdf;

namespace EmailSenderComponent.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyContext _context;
        private readonly Microsoft.AspNetCore.Identity.UI.Services.IEmailSender _emailSender;

        public HomeController(ILogger<HomeController> logger,
                                            MyContext context, 
                                            Microsoft.AspNetCore.Identity.UI.Services.IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMail(string Mail, string GridHtml, string GridHtmlPDF)
        {
            if (ModelState.IsValid)
            {
                //emailSendVM.FileUrl = GridHtml;

                if (Mail != null)
                {
                    if(GridHtml != null)
                    {
                    //email send
                    byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(GridHtml);
                    var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);

                    FilesExPdf exPdf = new FilesExPdf()
                    {
                        FileToken = codeEncoded
                    };
               
                    await _context.FilesExPdfs.AddAsync(exPdf);

                    await _context.SaveChangesAsync();

                    await _emailSender.SendEmailAsync(Mail, "Değerli Müşterimiz, yeni bir Belgeniz var", 

                         $"Lütfen EXCEL belgenizi acmak icin tiklayiniz  " +
                        $"<a href='{HtmlEncoder.Default.Encode($"https://localhost:44394/Home/ExportExcel?token={exPdf.Id}")}'>" +
                        "LINK."+
                        $"</a>");
                    }
                    if (GridHtmlPDF != null)
                    {
                        //email send
                        byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(GridHtmlPDF);
                        var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);

                        FilesExPdf exPdf = new FilesExPdf()
                        {
                            FileToken = codeEncoded
                        };

                        await _context.FilesExPdfs.AddAsync(exPdf);

                        await _context.SaveChangesAsync();

                        await _emailSender.SendEmailAsync(Mail, "Değerli Müşterimiz, yeni bir Belgeniz var",

                             $" Lütfen PDF belgenizi acmak icin tiklayiniz  " +
                            $"<a href='{HtmlEncoder.Default.Encode($"https://localhost:44394/Home/ExportPdf?token={exPdf.Id}")}'>" +
                            "LINK." +
                            $"</a>");
                    }


                }

            }
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult ExportExcel( int token)
        {
            foreach( var iteam in _context.FilesExPdfs)
            {
               if(iteam.Id == token)
                {
                    var cod = iteam.FileToken;
                    var codeDecodedBytes = WebEncoders.Base64UrlDecode(cod);
                    var codeDecoded = Encoding.UTF8.GetString(codeDecodedBytes);
                    var date =  DateTime.Now;

                    return File(System.Text.Encoding.ASCII.GetBytes(codeDecoded), "application/vnd.ms-excel", "IGC.xls");
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult ExportPdf(int token)
        {

            foreach (var iteam in _context.FilesExPdfs)
            {
                if (iteam.Id == token)
                {
                    var cod = iteam.FileToken;
                    var codeDecodedBytes = WebEncoders.Base64UrlDecode(cod);
                    var codeDecoded = Encoding.UTF8.GetString(codeDecodedBytes);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        HtmlConverter.ConvertToPdf(codeDecoded, stream);
                        return File(stream.ToArray(), "application/pdf", "IGC.pdf");
                    }

                }
            }
            return View();

            
        }
    }
}
