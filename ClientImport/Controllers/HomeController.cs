using ClientImport.DbContexts;
using ClientImport.DTOs;
using ClientImport.Models;
using ClientImport.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace ClientImport.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClientDbContext _context;
        private readonly ILogger<HomeController> _logger;
        private readonly IPostService _postService;

        public HomeController(ClientDbContext context, ILogger<HomeController> logger, IPostService postService)
        {
            _context = context;
            _logger = logger;
            _postService = postService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public ActionResult ImportClients()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadClients(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    using (var fileStream = file.OpenReadStream())
                    using (var streamReader = new StreamReader(fileStream, System.Text.Encoding.UTF8))
                    {
                        var clientList = JsonConvert.DeserializeObject<ClientDTO[]>(streamReader.ReadToEnd());

                        foreach (var client in clientList)
                        {
                            var existingClient = _context.Clients.Where(c => c.Name.Equals(client.Name))
                                .FirstOrDefault();

                            if (existingClient == null)
                            {
                                var newClient = new Client
                                {
                                    Address = client.Address,
                                    Name = client.Name,
                                    PostCode = string.IsNullOrWhiteSpace(client.PostCode) ? null : client.PostCode
                                };
                                _context.Clients.Add(newClient);
                            }
                            else if (!existingClient.Address.Equals(client.Address))
                            {
                                existingClient.Address = client.Address;
                                existingClient.PostCode = string.IsNullOrWhiteSpace(client.PostCode) ? null : client.PostCode;
                                _context.Clients.Update(existingClient);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                ViewBag.StatusMessage = "Klientai sėkmingai importuoti.";
            }
            catch
            {
                ViewBag.StatusMessage = "Importavimas nepavyko! Pabandykite dar kartą.";
            }
            return View("ImportClients");
        }

        [HttpGet]
        public async Task<IActionResult> UpdatePostIndexes()
        {
            try
            {
                var clientList = _context.Clients.ToList();
                foreach (var client in clientList)
                {
                    client.PostCode = await _postService.GetPostIndexAsync(client.Address);
                }
                await _context.SaveChangesAsync();
                ViewBag.StatusMessage = "Pašto indeksai sėkmingai atnaujinti.";
            }
            catch
            {
                ViewBag.StatusMessage = "Pašto indeksų atnaujinti nepavyko! Pabandykite dar kartą.";
            }
            return View("UpdatePostIndexStatus");
        }

        [HttpGet]
        public IActionResult DisplayClientList()
        {
            var clientList = _context.Clients.ToList();
            return View("ClientList", clientList);
        }
    }
}
