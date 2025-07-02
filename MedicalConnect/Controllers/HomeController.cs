using System.Diagnostics;
using MedicalConnect.Database;
using MedicalConnect.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicalConnect.Repositories;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalConnect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _context = context;
            _unitOfWork = unitOfWork;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            // Get top rated doctors
            var topDoctors = await _unitOfWork.Doctors.GetAsync(
                orderBy: q => q.OrderByDescending(d => d.Rating),
                includeProperties: "User");

            var topDoctorsViewModel = topDoctors.Take(4).Select(d => new DoctorViewModel
            {
                Id = d.Id,
                FullName = d.User.FullName,
                Email = d.User.Email,
                PhoneNumber = d.User.PhoneNumber,
                Specialty = d.Specialty,
                ExperienceYears = d.Experience,
                Rating = d.Rating ?? 0m,
                ImageUrl = d.ImageUrl ?? "/images/default-doctor.png"
            }).ToList();

            ViewBag.TopDoctors = topDoctorsViewModel;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult faq()
        {
            return View();
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            var doctors = await _unitOfWork.Doctors.SearchDoctorsAsync(searchTerm);
            var viewModels = doctors.Select(d => new DoctorViewModel
            {
                Id = d.Id,
                FullName = d.User.FullName,
                Email = d.User.Email,
                PhoneNumber = d.User.PhoneNumber,
                Specialty = d.Specialty,
                ExperienceYears = d.Experience,
                Rating = d.Rating ?? 0m,
                ImageUrl = d.ImageUrl
            }).ToList();

            ViewData["SearchTerm"] = searchTerm;
            return View(viewModels);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public class ErrorViewModel
        {
            public string RequestId { get; set; }
            public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        }
    }
}
