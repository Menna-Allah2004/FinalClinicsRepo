using Microsoft.AspNetCore.Http;
using MedicalConnect.Database;
using MedicalConnect.Repositories;
using MedicalConnect.ViewModels;
using MedicalConnect.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using MedicalConnect.Controllers;

namespace MedicalConnect.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUnitOfWork unitOfWork,
            ApplicationDbContext context,
            IEmailService emailService,
            INotificationService notificationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Register()
        {
            // صفحة اختيار نوع المستخدم للتسجيل
            return View();
        }

        [HttpGet]
        public IActionResult PatientRegister(string returnUrl = null)
        {
            // صفحة تسجيل خاصة بالمرضى
            var model = new RegisterPatientViewModel
            {
                UserType = "Patient",
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PatientRegister(RegisterPatientViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "هذا البريد الإلكتروني مستخدم مسبقاً");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                UserType = "Patient",
                IsApproved = true,
                City = model.City,
                Gender = model.Gender,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Patient");
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError("", $"خطأ في إضافة الدور: {error.Description}");

                await _userManager.DeleteAsync(user);
                return View(model);
            }

            var patient = new Patient
            {
                UserId = user.Id,
                Email = user.Email, 
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Gender = model.Gender ?? "غير محدد",
                BloodType = model.BloodType ?? "غير محدد",
                DateOfBirth = model.DateOfBirth ?? DateTime.Now.AddYears(-25),
                MedicalHistory = "",
                Age = model.DateOfBirth.HasValue ?
                    DateTime.Now.Year - model.DateOfBirth.Value.Year : 25
            };

            try
            {
                await _unitOfWork.Patients.AddAsync(patient);
                await _unitOfWork.SaveAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);

                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    return Redirect(model.ReturnUrl);

                return RedirectToAction("Profile", "Patient");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"حدث خطأ أثناء حفظ بيانات المريض: {ex.Message}");
                await _userManager.DeleteAsync(user);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult DoctorRegister(string returnUrl = null)
        {
            // صفحة تسجيل خاصة بالأطباء
            var model = new RegisterDoctorViewModel
            {
                UserType = "Doctor",
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoctorRegister(RegisterDoctorViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                UserType = "Doctor",
                City = model.City,
                IsApproved = false // يحتاج موافقة المسؤول
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Doctor");
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError("", $"خطأ في إضافة الدور: {error.Description}");

                await _userManager.DeleteAsync(user);
                return View(model);
            }

            var doctor = new Doctor
            {
                UserId = user.Id,
                Email = user.Email, 
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                City = user.City,
                Specialty = model.Specialty,
                Bio = model.Bio ?? null,
                Experience = model.ExperienceYears ?? 0,
                Rating = 0,
                Education = model.Education,
                ConsultationFee = model.ConsultationFee
            };

            try
            {
                await _unitOfWork.Doctors.AddAsync(doctor);
                await _unitOfWork.SaveAsync();

                var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                foreach (var admin in adminUsers)
                {
                    await _notificationService.CreateNotificationAsync(
                        admin.Id,
                        "طلب تسجيل طبيب جديد",
                        $"طلب {user.FullName} التسجيل كطبيب. يرجى مراجعة الطلب.",
                        "System"
                    );
                }

                TempData["SuccessMessage"] = "تم تسجيل حسابك بنجاح. سيتم مراجعة معلوماتك والرد عليك قريبًا.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"حدث خطأ أثناء حفظ بيانات الطبيب: {ex.Message}");
                await _userManager.DeleteAsync(user);
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
                    return View(model);
                }

                // تحقق من صحة كلمة المرور
                var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!passwordValid)
                {
                    ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
                    return View(model);
                }

                // تحقق من حالة الموافقة للأطباء
                if (user.UserType == "Doctor" && !user.IsApproved)
                {
                    ModelState.AddModelError(string.Empty, "طلبك قيد الانتظار. لم تتم الموافقة على حسابك بعد.");
                    return View(model);
                }

                // تسجيل الدخول الفعلي
                var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                        return RedirectToAction("Dashboard", "Admin");

                    if (await _userManager.IsInRoleAsync(user, "Doctor"))
                        return RedirectToAction("Dashboard", "Doctors");

                    if (await _userManager.IsInRoleAsync(user, "Patient"))
                        return RedirectToAction("Profile", "Patient");

                    return LocalRedirect(returnUrl);
                }
                else if (result.IsLockedOut)
                {
                    return RedirectToAction("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "حدث خطأ أثناء تسجيل الدخول. حاول مرة أخرى.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }
            if (user.UserType == "Patient")
            {
                return RedirectToAction("Profile", "Patient");
            }
            else if (user.UserType == "Doctor")
            {
                return RedirectToAction("Dashboard", "Doctors");
            }
            else if (user.UserType == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                // Admin profile
                return View("AdminProfile", user);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePatientProfile(PatientViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("PatientProfile", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("PatientProfile", model);
            }

            var patient = await _unitOfWork.Patients.GetByIdAsync(model.Id);
            if (patient == null)
            {
                return NotFound();
            }

            patient.DateOfBirth = model.DateOfBirth;
            patient.MedicalHistory = model.MedicalHistory;

            await _unitOfWork.Patients.UpdateAsync(patient);

            TempData["StatusMessage"] = "Your profile has been updated";
            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDoctorProfile(DoctorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("DoctorProfile", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("DoctorProfile", model);
            }

            var doctor = await _unitOfWork.Doctors.GetByIdAsync(model.Id);
            if (doctor == null)
            {
                return NotFound();
            }

            doctor.Specialty = model.Specialty;
            doctor.Experience = model.ExperienceYears;

            // Handle profile image upload
            if (model.ProfileImage != null)
            {
                // Implementation for file upload
                // This is a placeholder - you would need to implement file storage
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                // Save file to wwwroot/images/doctors
                // string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", "doctors", uniqueFileName);
                // using (var fileStream = new FileStream(filePath, FileMode.Create))
                // {
                //     await model.ProfileImage.CopyToAsync(fileStream);ئ
                // }

                doctor.ImageUrl = "/images/doctors/" + uniqueFileName;
            }
            await _unitOfWork.Doctors.UpdateAsync(doctor);

            TempData["StatusMessage"] = "Your profile has been updated";
            return RedirectToAction(nameof(Profile));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}