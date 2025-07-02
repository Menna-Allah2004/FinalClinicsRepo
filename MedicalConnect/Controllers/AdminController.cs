using MedicalConnect.Database;
using MedicalConnect.Repositories;
using MedicalConnect.Services;
using MedicalConnect.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicPatient.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;

        public AdminController(IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailService = emailService;
            _notificationService = notificationService;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var totalDoctors = (await _unitOfWork.Doctors.GetAllAsync()).Count();
            var totalPatients = (await _unitOfWork.Patients.GetAllAsync()).Count();
            var totalAppointments = (await _unitOfWork.Appointments.GetAllAsync()).Count();
            var pendingDoctors = await _unitOfWork.Doctors.GetAsync(d => !d.User.IsApproved, null, "User");
            var pendingDoctorApprovals = pendingDoctors.Count();
            var unreadMessages = (await _unitOfWork.ContactUsMessages.GetAsync(m => !m.IsRead)).Count();
            var pendingDoctorViewModels = pendingDoctors.Take(5).Select(d => new DoctorApprovalViewModel
            {
                UserId = d.UserId,
                DoctorId = d.Id,
                FullName = d.User.FullName,
                Email = d.User.Email,
                PhoneNumber = d.User.PhoneNumber,
                Specialization = d.Specialty,
                ImageUrl = d.User.ImageUrl ?? "~/images/doctors/default-user.png",
                ExperienceYears = d.Experience
            })
            .ToList();

            var recentMessages = (await _unitOfWork.ContactUsMessages.GetAsync(null, q => q.OrderByDescending(m => m.CreatedAt))).Take(5).Select(m => new ContactMessageViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Subject = m.Subject,
                Message = m.Message,
                IsRead = m.IsRead,
                CreatedAt = m.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            })
            .ToList();

            var viewModel = new AdminDashboardViewModel
            {
                TotalDoctors = totalDoctors,
                TotalPatients = totalPatients,
                TotalAppointments = totalAppointments,
                PendingDoctorApprovals = pendingDoctorApprovals,
                UnreadMessages = unreadMessages,
                PendingDoctors = pendingDoctorViewModels,
                RecentMessages = recentMessages
            };

            return View(viewModel);
        }

        // GET: Admin/DoctorApprovals
        public async Task<IActionResult> DoctorApprovals()
        {
            var pendingDoctors = await _unitOfWork.Doctors.GetAsync(d => !d.User.IsApproved, null, "User");

            var viewModels = pendingDoctors.Select(d => new DoctorApprovalViewModel
            {
                UserId = d.UserId,
                DoctorId = d.Id,
                FullName = d.User.FullName,
                Email = d.User.Email,
                PhoneNumber = d.User.PhoneNumber,
                Specialization = d.Specialty,
                ImageUrl = d.User.ImageUrl ?? "~/images/doctors/default-user.png",
                ExperienceYears = d.Experience
            }).ToList();

            return View(viewModels);
        }

        // POST: Admin/ApproveDoctorRegistration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveDoctorRegistration(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.UserType != "Doctor")
            {
                return NotFound();
            }

            user.IsApproved = true;
            user.UpdatedAt = DateTime.Now;
            await _userManager.UpdateAsync(user);

            // إرسال إشعار للطبيب
            await _notificationService.CreateNotificationAsync(userId, "تمت الموافقة على الحساب",
            "تمت الموافقة على حسابك كطبيب. يمكنك الآن تسجيل الدخول واستخدام النظام.",
            "النظام");

            // إرسال بريد إلكتروني للطبيب
            try
            {
                await _emailService.SendEmailAsync(
                user.Email,
                "تمت الموافقة على الحساب",
                $"<h3>مرحبا {user.FullName},</h3>" +
                "<p>يسعدنا أن نعلمك بأنه تم الموافقة على حسابك كطبيب في نظام إدارة العيادات لدينا.</p>" +
                "<p>يمكنك الآن تسجيل الدخول واستخدام النظام.</p>" +
                "<p>شكرًا لانضمامك إلينا!</p>"
                );
            }
            catch (Exception)
            {
                // تسجيل الخطأ ولكن عدم إظهاره للمستخدم
            }

            TempData["Success"] = $"تمت الموافقة بنجاح على حساب الطبيب {user.FullName}.";
            return RedirectToAction(nameof(DoctorApprovals));
        }

        // POST: Admin/RejectDoctorRegistration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectDoctorRegistration(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.UserType != "Doctor")
            {
                return NotFound();
            }

            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor != null)
            {
                await _unitOfWork.Doctors.RemoveAsync(doctor);
            }

            await _userManager.DeleteAsync(user);

            TempData["Success"] = $"تم رفض حساب الطبيب {user.FullName} وإزالته من النظام.";
            return RedirectToAction(nameof(DoctorApprovals));
        }

        // GET: Admin/Messages
        public async Task<IActionResult> Messages()
        {
            var messages = await _unitOfWork.ContactUsMessages.GetAsync(
            null,
            q => q.OrderByDescending(m => m.CreatedAt));

            var viewModels = messages.Select(m => new ContactMessageViewModel
            {
                Id = m.Id,
                Name = m.Name,
                Email = m.Email,
                Subject = m.Subject,
                Message = m.Message,
                IsRead = m.IsRead,
                CreatedAt = m.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            }).ToList();

            return View(viewModels);
        }

        // GET: Admin/MessageDetails/5
        public async Task<IActionResult> MessageDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _unitOfWork.ContactUsMessages.GetByIdAsync(id.Value);
            if (message == null)
            {
                return NotFound();
            }

            // Update read status
            if (!message.IsRead)
            {
                message.IsRead = true;
                await _unitOfWork.ContactUsMessages.UpdateAsync(message);
            }

            var viewModel = new ContactMessageViewModel
            {
                Id = message.Id,
                Name = message.Name,
                Email = message.Email,
                Subject = message.Subject,
                Message = message.Message,
                IsRead = message.IsRead,
                CreatedAt = message.CreatedAt.ToString("yyyy-MM-dd HH:mm")
            };

            return View(viewModel);
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users(string userType)
        {
            var users = await _userManager.Users.ToListAsync();

            if (!string.IsNullOrEmpty(userType))
            {
                users = users.Where(u => u.UserType == userType).ToList();
            }

            ViewBag.UserType = userType;
            return View(users);
        }

        // GET: Admin/UserDetails/userId
        public async Task<IActionResult> UserDetails(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            if (user.UserType == "Doctor")
            {
                var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == id);
                if (doctor != null)
                {
                    ViewBag.Doctor = doctor;
                }
            }
            else if (user.UserType == "Patient")
            {
                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == id);
                if (patient != null)
                {
                    ViewBag.Patient = patient;
                }
            }

            return View(user);
        }

        // POST: Admin/DeleteUser/userId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Delete user
            await _userManager.DeleteAsync(user);

            TempData["Success"] = $"تم حذف المستخدم {user.FullName} بنجاح.";
            return RedirectToAction(nameof(Users));
        }

        // GET: Admin/Appointments
        public async Task<IActionResult> Appointments(string status)
        {
            var appointments = await _unitOfWork.Appointments.GetAsync(status != null ? a => a.Status == status : null,
            q => q.OrderByDescending(a => a.AppointmentDate).ThenBy(a => a.StartTime), "Doctor.User,Patient.User");

            var viewModels = appointments.Select(a => new AppointmentViewModel
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.User.FullName,
                PatientId = a.PatientId,
                PatientName = a.Patient.User.FullName,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status,
                Notes = a.Notes,
                CreatedAt = a.CreatedAt
            }).ToList();

            ViewBag.Status = status;
            return View(viewModels);
        }

        // GET: Admin/MedicalReports
        public async Task<IActionResult> MedicalReports()
        {
            var reports = await _unitOfWork.MedicalReports.GetAsync(null,
            q => q.OrderByDescending(m => m.CreatedAt), "Doctor.User,Patient.User");

            var viewModels = reports.Select(m => new MedicalReportViewModel
            {
                Id = m.Id,
                DoctorId = m.DoctorId,
                DoctorName = m.Doctor.User.FullName,
                PatientId = m.PatientId,
                PatientName = m.Patient.User.FullName,
                AppointmentId = m.AppointmentId,
                Diagnosis = m.Diagnosis,
                CreatedAt = m.CreatedAt
            }).ToList();

            return View(viewModels);
        }

        public IActionResult ManageDoctors()
        {
            return RedirectToAction("Index", "Doctors");
        }

        public IActionResult ManagePatients()
        {
            return RedirectToAction("Index", "Patients");
        }

        public IActionResult ManageAppointments()
        {
            return RedirectToAction("Index", "Appointments");
        }

        public IActionResult ContactMessages()
        {
            return RedirectToAction("Messages", "Contact");
        }

        [HttpGet]
        public IActionResult UpdateAboutUs()
        {
            // In a real application, you would retrieve the content from a database
            // For this example, we'll use a simple string
            ViewBag.AboutUsContent = "Our clinic is dedicated to providing high-quality healthcare services...";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAboutUs(string content)
        {
            // In a real application, you would save the content to a database
            // For this example, we'll just show a success message
            TempData["Success"] = "About Us content updated successfully";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}