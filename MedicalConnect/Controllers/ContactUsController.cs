using MedicalConnect.Database;
using MedicalConnect.Repositories;
using MedicalConnect.Services;
using Microsoft.AspNetCore.Identity;
using MedicalConnect.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace MedicalConnect.Controllers
{
    public class ContactUsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;

        public ContactUsController(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            UserManager<ApplicationUser> userManager,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactUsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var message = new ContactUsMessage
                {
                    Name = model.Name,
                    Email = model.Email,
                    Subject = model.Subject,
                    Message = model.Message,
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.ContactUsMessages.AddAsync(message);

                // TODO: Send email notification to admin
                // Send email to admin
                try
                {
                    var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                    foreach (var admin in adminUsers)
                    {
                        await _emailService.SendEmailAsync(
                        admin.Email,
                        $"رسالة تواصل جديدة من  {model.Name}: {model.Subject}",
                        $"<h3>رسالة تواصل جديدة من الموقع</h3>" +
                        $"<p><strong>الاسم:</strong> {model.Name}</p>" +
                        $"<p><strong>البريد الالكتروني:</strong> {model.Email}</p>" +
                        $"<p><strong>الموضوع:</strong> {model.Subject}</p>" +
                        $"<p><strong>الرسالة:</strong></p>" +
                        $"<p>{model.Message}</p>"
                        );

                        // Send notification to admin
                        await _notificationService.CreateNotificationAsync(
                        admin.Id,
                        "رسالة تواصل جديدة",
                        $"رسالة جديدة من {model.Name}: {model.Subject}",
                        "النظام",
                        message.Id
                        );
                    }
                }
                catch (Exception)
                {
                    // تسجيل الخطأ ولكن لا يتم عرضه للمستخدم
                }
                TempData["Success"] = "تم إرسال رسالتك بنجاح. سوف نعاود الاتصال بك قريبًا.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Messages()
        {
            var messages = await _unitOfWork.ContactUsMessages.GetAllAsync();
            return View(messages.OrderByDescending(m => m.CreatedAt));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var message = await _unitOfWork.ContactUsMessages.GetByIdAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _unitOfWork.ContactUsMessages.RemoveAsync(id);
            TempData["Success"] = "Message deleted successfully";
            return RedirectToAction(nameof(Messages));
        }
    }
}
