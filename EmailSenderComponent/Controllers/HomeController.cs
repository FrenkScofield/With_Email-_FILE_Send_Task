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

        //The action of sending a data link to Gmail. START
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMail(string Mail, string GridHtml, string GridHtmlPDF)
        {
            if (ModelState.IsValid)
            {

                if (Mail != null)
                {
                    if (GridHtml != null)
                    {
                        //Converting html grid type, to byte type.
                        byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(GridHtml);
                        var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);

                        //appending the byte type to the corresponding field in the table
                        FilesExPdf exPdf = new FilesExPdf()
                        {
                            FileToken = codeEncoded
                        };

                        // add url to database
                        await _context.FilesExPdfs.AddAsync(exPdf);

                        // save to database
                        await _context.SaveChangesAsync();

                        //email send
                        await _emailSender.SendEmailAsync(Mail, "Değerli Müşterimiz, yeni bir Belgeniz var",

                             $"Lütfen EXCEL belgenizi acmak icin tiklayiniz  " +
                            $"<a href='{HtmlEncoder.Default.Encode($"https://localhost:44394/Home/ExportExcel?token={exPdf.Id}")}'>" +
                            "LINK." +
                            $"</a>");
                    }

                    if (GridHtmlPDF != null)
                    {
                        //Converting html grid type, to byte type.
                        byte[] tokenGeneratedBytes = Encoding.UTF8.GetBytes(GridHtmlPDF);
                        var codeEncoded = WebEncoders.Base64UrlEncode(tokenGeneratedBytes);

                        //appending the byte type to the corresponding field in the table
                        FilesExPdf exPdf = new FilesExPdf()
                        {
                            FileToken = codeEncoded
                        };

                        // add url to database
                        await _context.FilesExPdfs.AddAsync(exPdf);

                        // save to database
                        await _context.SaveChangesAsync();

                        //email send
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
        //END

        // The action of converting the url from Gmail to Excel. START
        [HttpGet]
        public IActionResult ExportExcel(int token)
        {
            foreach (var iteam in _context.FilesExPdfs)
            {
                if (iteam.Id == token)
                {
                    var cod = iteam.FileToken;
                    var codeDecodedBytes = WebEncoders.Base64UrlDecode(cod);
                    var codeDecoded = Encoding.UTF8.GetString(codeDecodedBytes);

                    return File(System.Text.Encoding.ASCII.GetBytes(codeDecoded), "application/vnd.ms-excel", "IGC_EXCEL.xls");
                }
            }
            return View();
        }
        //END

        //The action of converting the url from Gmail to Pdf.  START
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
                        return File(stream.ToArray(), "application/pdf", "IGC_PDF.pdf");
                    }
                }
            }
            return View();


        }
    }
    //END
}
