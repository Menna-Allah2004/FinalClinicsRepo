using MedicalConnect.Database;
using MedicalConnect.Repositories;
using MedicalConnect.Services;
using MedicalConnect.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClinicPatient.Controllers
{
    public class DoctorsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotificationService _notificationService;
        private readonly ApplicationDbContext _context;

        public DoctorsController(
            IUnitOfWork unitOfWork,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            INotificationService notificationService,
            ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
            _notificationService = notificationService;
            _context = context;
        }

        public async Task<IActionResult> Index(string searchQuery = "", string specialty = "", string location = "", string sortBy = "rating", int page = 1, int pageSize = 9)
        {
            try
            {
                // استرجاع الأطباء المعتمدين فقط
                var allDoctors = await _unitOfWork.Doctors.GetAsync(
                    filter: d => d.User.IsApproved,
                    orderBy: null,
                    includeProperties: "User,Ratings");

                var doctorViewModels = new List<DoctorViewModel>();

                foreach (var doctor in allDoctors)
                {
                    if (doctor.User != null)
                    {
                        // حساب التقييم الفعلي
                        var averageRating = doctor.Ratings != null && doctor.Ratings.Any()
                            ? doctor.Ratings.Average(r => r.RatingValue)
                            : 0;

                        var totalRatings = doctor.Ratings?.Count ?? 0;

                        var firstAvailability = doctor.Availabilities?.FirstOrDefault();

                        doctorViewModels.Add(new DoctorViewModel
                        {
                            Id = doctor.Id,
                            UserId = doctor.UserId,
                            FullName = doctor.User.FullName ?? "غير محدد",
                            Email = doctor.User.Email ?? "",
                            PhoneNumber = doctor.User.PhoneNumber ?? "",
                            Specialty = doctor.Specialty ?? "طبيب عام",
                            ExperienceYears = doctor.Experience,
                            Bio = doctor.Bio ?? "",
                            Education = doctor.Education ?? "",
                            ConsultationFee = doctor.ConsultationFee,
                            ImageUrl = string.IsNullOrEmpty(doctor.User.ImageUrl)
                                ? "~/images/doctors/default-user.png"
                                : doctor.User.ImageUrl,
                            Rating = (decimal)Math.Round(averageRating, 1),
                            RatingCount = totalRatings,
                            City = doctor.City ?? "غزة",
                            HasAvailability = firstAvailability?.IsAvailable ?? false,
                            DisplayWaitingTime = firstAvailability != null ? firstAvailability.WaitingTime : "غير متاح"
                        });
                    }
                }

                // تطبيق البحث
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    doctorViewModels = doctorViewModels.Where(d =>
                        d.FullName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                        d.Specialty.Contains(searchQuery, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // تطبيق فلتر التخصص
                if (!string.IsNullOrEmpty(specialty) && specialty != "جميع التخصصات")
                {
                    doctorViewModels = doctorViewModels.Where(d => d.Specialty == specialty).ToList();
                }

                // تطبيق فلتر المدينة
                if (!string.IsNullOrEmpty(location) && location != "جميع المدن")
                {
                    doctorViewModels = doctorViewModels.Where(d => d.City == location).ToList();
                }

                // تطبيق الترتيب
                switch (sortBy)
                {
                    case "experience":
                        doctorViewModels = doctorViewModels.OrderByDescending(d => d.ExperienceYears).ToList();
                        break;
                    case "fee-asc":
                        doctorViewModels = doctorViewModels.OrderBy(d => d.ConsultationFee).ToList();
                        break;
                    case "fee-desc":
                        doctorViewModels = doctorViewModels.OrderByDescending(d => d.ConsultationFee).ToList();
                        break;
                    default: // rating
                        doctorViewModels = doctorViewModels.OrderByDescending(d => d.Rating)
                            .ThenByDescending(d => d.ExperienceYears).ToList();
                        break;
                }

                // تطبيق الترقيم
                var totalCount = doctorViewModels.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var pagedDoctors = doctorViewModels
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // إنشاء قوائم التخصصات والمواقع من البيانات الفعلية
                var specialties = new List<string> { "جميع التخصصات" };
                specialties.AddRange(allDoctors.Where(d => !string.IsNullOrEmpty(d.Specialty))
                    .Select(d => d.Specialty).Distinct().OrderBy(s => s));

                var locations = new List<string>
                {
                    "جميع المدن", "شمال غزة", "غزة", "المغازي", "النصيرات",
                    "دير البلح", "خانيونس", "البريج", "رفح"
                };

                var viewModel = new DoctorSearchViewModel
                {
                    Doctors = pagedDoctors,
                    Specialties = specialties,
                    Locations = locations,
                    SearchQuery = searchQuery ?? "",
                    SelectedSpecialty = specialty ?? "جميع التخصصات",
                    SelectedLocation = location ?? "جميع المدن",
                    SortBy = sortBy ?? "rating",
                    CurrentPage = page,
                    TotalPages = totalPages
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحميل قائمة الأطباء";
                return View(new DoctorSearchViewModel());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(
                d => d.Id == id && d.User.IsApproved,
                includeProperties: "User,Availabilities");

            if (doctor == null)
            {
                return NotFound();
            }

            var appointments = await _unitOfWork.Appointments.GetAppointmentsByDoctorIdAsync(id);
            var patientCount = appointments.Count();


            var viewModel = new DoctorViewModel
            {
                Id = doctor.Id,
                FullName = doctor.User.FullName,
                Email = doctor.User.Email,
                PhoneNumber = doctor.User.PhoneNumber,
                Bio = doctor.Bio,
                Education = doctor.Education,
                Specialty = doctor.Specialty,
                ExperienceYears = doctor.Experience,
                Rating = doctor.Rating ?? 0m,
                RatingCount = doctor.RatingCount,
                ConsultationFee = doctor.ConsultationFee,
                ImageUrl = doctor.ImageUrl ?? "/images/default-doctor.png",
                Availabilities = doctor.Availabilities?.ToList(),

                DoctorsDash = new DoctorDashboardViewModel
                {
                    PatientsCount = patientCount
                }
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> Dashboard()
        {
            // الحصول على معرف المستخدم الحالي
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // الحصول على معلومات الطبيب مع المواعيد والمرضى
            var doctor = await _unitOfWork.Doctors
                .GetFirstOrDefaultAsync(d => d.UserId == user.Id, "Appointments.Patient.User");

            if (doctor == null)
            {
                return NotFound("لم يتم العثور على بيانات الطبيب");
            }

            var currentDate = DateTime.Now;
            var appointments = doctor.Appointments ?? new List<Appointment>();

            // تحميل بيانات المستخدم لكل مريض إذا وجدت مواعيد
            foreach (var appointment in appointments)
            {
                if (appointment.Patient != null && appointment.Patient.UserId != null)
                {
                    appointment.Patient.User = await _userManager.FindByIdAsync(appointment.Patient.UserId);
                }
            }

            // حساب المرضى الداخليين (المرضى الذين لديهم مواعيد مكتملة)
            var inPatientsCount = appointments
                .Where(a => a.Status == "Completed" && a.Type == "منوم")
                .Select(a => a.PatientId)
                .Distinct()
                .Count();

            // حساب الاستشارات عبر الإنترنت المكتملة
            var onlineConsultationsCount = appointments
                .Count(a => a.Status == "Completed" && a.IsVirtual == true);

            // حساب التحاليل المخبرية المكتملة
            var labTestsCount = appointments
                .Count(a => a.Status == "Completed" && a.Type == "تحليل");

            // حساب الإحصائيات للأسبوع الماضي للمقارنة
            var lastWeekStart = currentDate.AddDays(-7);

            var lastWeekInPatients = appointments
                .Where(a => a.AppointmentDate >= lastWeekStart && a.AppointmentDate < currentDate &&
                           a.Status == "Completed" && a.Type == "منوم")
                .Select(a => a.PatientId)
                .Distinct()
                .Count();

            var lastWeekOnlineConsultations = appointments
                .Count(a => a.AppointmentDate >= lastWeekStart &&
                a.AppointmentDate < currentDate &&
                a.Status == "Completed" &&
                a.IsVirtual == true);

            var lastWeekLabTests = appointments
                .Count(a => a.AppointmentDate >= lastWeekStart && a.AppointmentDate < currentDate &&
                           a.Status == "Completed" && a.Type == "تحليل");

            // حساب نسب التغيير
            int inPatientsChange = lastWeekInPatients > 0
                ? (int)Math.Round((inPatientsCount - lastWeekInPatients) * 100.0 / lastWeekInPatients)
                : 0;

            int onlineConsultationsChange = lastWeekOnlineConsultations > 0
                ? (int)Math.Round((onlineConsultationsCount - lastWeekOnlineConsultations) * 100.0 / lastWeekOnlineConsultations)
                : 0;

            int labTestsChange = lastWeekLabTests > 0
                ? (int)Math.Round((labTestsCount - lastWeekLabTests) * 100.0 / lastWeekLabTests)
                : 0;

            // حساب الأحداث المجدولة والمكتملة من قاعدة البيانات فقط
            var scheduledConsultations = appointments.Count(a => a.Type == "استشارة");
            var completedConsultations = appointments.Count(a => a.Type == "استشارة" && a.Status == "Completed");
            var scheduledLabTests = appointments.Count(a => a.Type == "تحليل");
            var completedLabTests = appointments.Count(a => a.Type == "تحليل" && a.Status == "Completed");
            var scheduledMeetings = appointments.Count(a => a.Type == "اجتماع");
            var completedMeetings = appointments.Count(a => a.Type == "اجتماع" && a.Status == "Completed");

            // مواعيد اليوم من قاعدة البيانات فقط
            var todayAppointments = appointments
                .Where(a => a.AppointmentDate.Date == currentDate.Date)
                .OrderBy(a => a.StartTime)
                .Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.Patient?.User?.FullName ?? "غير معروف",
                    AppointmentTime = a.AppointmentDate.Add(a.StartTime),
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = a.Status ?? "معلق",
                    Notes = a.Notes ?? "",
                    Title = a.Title ?? $"موعد مع {(a.Patient?.User?.FullName ?? "مريض")}",
                    Type = a.Type ?? "استشارة",
                    IsVirtual = (bool)a.IsVirtual,
                    MeetingLink = a.MeetingLink ?? "",
                    CreatedAt = a.CreatedAt,
                    FormattedTime = a.StartTime.ToString(@"h\:mm") + (a.StartTime.Hours >= 12 ? " م" : " ص")
                })
                .ToList();

            // إنشاء أيام التقويم
            var calendarDays = new List<CalendarDayViewModel>();
            var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek);
            for (int i = 0; i < 7; i++)
            {
                var day = startOfWeek.AddDays(i);
                calendarDays.Add(new CalendarDayViewModel
                {
                    Day = day.Day,
                    IsToday = day.Date == currentDate.Date,
                    HasAppointments = appointments.Any(a => a.AppointmentDate.Date == day.Date)
                });
            }

            // تجهيز نموذج عرض لوحة التحكم بناءً على البيانات الحقيقية فقط
            var viewModel = new DoctorDashboardViewModel
            {
                Doctor = new DoctorViewModel
                {
                    Id = doctor.Id,
                    FullName = user.FullName ?? "الطبيب",
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    Specialty = doctor.Specialty ?? "طبيب عام",
                    ExperienceYears = doctor.Experience,
                    Bio = doctor.Bio ?? "",
                    Education = doctor.Education ?? "",
                    ConsultationFee = doctor.ConsultationFee,
                    ImageUrl = "/lovable-uploads/a00758a4-fb3d-4765-b29a-a75c150fcb2b.png"
                },

                // الإحصائيات الأساسية من قاعدة البيانات فقط
                PatientsCount = inPatientsCount,
                OnlineConsultationsCount = onlineConsultationsCount,
                LabTestsCount = labTestsCount,

                // نسب التغيير
                PatientsCountChange = inPatientsChange,
                OnlineConsultationsChange = onlineConsultationsChange,
                LabTestsChange = labTestsChange,

                // الأحداث المجدولة من قاعدة البيانات فقط
                ScheduledConsultations = scheduledConsultations,
                CompletedConsultations = completedConsultations,
                ScheduledLabTests = scheduledLabTests,
                CompletedLabTests = completedLabTests,
                ScheduledMeetings = scheduledMeetings,
                CompletedMeetings = completedMeetings,

                // التاريخ الحالي
                CurrentDate = currentDate,
                CurrentCalendarDate = currentDate,

                // مواعيد اليوم من قاعدة البيانات فقط
                TodayAppointments = todayAppointments,

                // أيام التقويم
                CalendarDays = calendarDays,

                // معلومات الطبيب الشخصية (ستأتي من ملف الطبيب أو تكون فارغة)
                DoctorLocation = doctor.Location ?? "",
                DateOfBirth = doctor.DateOfBirth,
                DoctorBloodType = doctor.BloodType ?? "",
                WorkingHours = doctor.WorkingHours ?? ""
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Appointments()
        {
            // الحصول على معرف المستخدم الحالي
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // الحصول على معلومات الطبيب
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);
            //var doctorInfo = doctor.FirstOrDefault();

            if (doctor == null)
            {
                return NotFound("لم يتم العثور على بيانات الطبيب");
            }

            var currentDate = DateTime.Now;

            // استرجاع جميع المواعيد للطبيب
            var allAppointments = await _unitOfWork.Appointments.GetAsync(
                a => a.DoctorId == doctor.Id,
                a => a.OrderBy(x => x.AppointmentDate).ThenBy(x => x.StartTime),
                "Patient.User");

            // تقسيم المواعيد إلى مواعيد قادمة، ومواعيد اليوم، ومواعيد سابقة
            var upcomingAppointments = allAppointments
                .Where(a => a.AppointmentDate > currentDate.Date ||
                           (a.AppointmentDate == currentDate.Date && a.StartTime > currentDate.TimeOfDay))
                .Select(MapAppointmentToViewModel)
                .ToArray();

            var todayAppointments = allAppointments
                .Where(a => a.AppointmentDate.Date == currentDate.Date)
                .Select(MapAppointmentToViewModel)
                .ToArray();

            var pastAppointments = allAppointments
                .Where(a => a.AppointmentDate < currentDate.Date ||
                           (a.AppointmentDate == currentDate.Date && a.StartTime < currentDate.TimeOfDay))
                .Select(MapAppointmentToViewModel)
                .ToArray();

            var cancelledAppointmentsCount = allAppointments.Count(a => a.Status == "Cancelled");

            var viewModel = new AppointmentListViewModel
            {
                UpcomingAppointments = upcomingAppointments,
                TodayAppointments = todayAppointments,
                PastAppointments = pastAppointments,
                TotalAppointmentsCount = allAppointments.Count(),
                UpcomingAppointmentsCount = upcomingAppointments.Length,
                TodayAppointmentsCount = todayAppointments.Length,
                CancelledAppointmentsCount = cancelledAppointmentsCount
            };

            return View(viewModel);
        }

        private AppointmentViewModel MapAppointmentToViewModel(Appointment appointment)
        {
            return new AppointmentViewModel
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorId,
                PatientId = appointment.PatientId,
                PatientName = appointment.Patient?.User?.FullName,
                AppointmentTime = appointment.AppointmentDate.Add(appointment.StartTime),
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Title = appointment.Title,
                Type = appointment.Type,
                Status = appointment.Status,
                Notes = appointment.Notes,
                IsVirtual = (bool)appointment.IsVirtual,
                MeetingLink = appointment.MeetingLink,
                CreatedAt = appointment.CreatedAt
            };
        }

        private string GetGreeting()
        {
            var hour = DateTime.Now.Hour;
            if (hour < 12)
                return "صباح الخير";
            else
                return "مساء الخير";
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAppointmentStatus(int id, string status)
        {
            var user = await _userManager.GetUserAsync(User);
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null)
            {
                return NotFound();
            }

            var appointment = await _unitOfWork.Appointments.GetFirstOrDefaultAsync(
            a => a.Id == id && a.DoctorId == doctor.Id,
            "Patient.User");

            if (appointment == null)
            {
                return NotFound();
            }

            appointment.Status = status;
            appointment.UpdatedAt = DateTime.Now;
            await _unitOfWork.Appointments.UpdateAsync(appointment);

            // Send notification to patient
            string message = status == "Confirmed"
            ? $"Your appointment with Dr. {user.FullName} on {appointment.AppointmentDate.ToShortDateString()} at {appointment.StartTime} has been confirmed."
            : status == "Completed"
            ? $"Your appointment with Dr. {user.FullName} has been completed. You can now view your medical report."
            : $"Your appointment with Dr. {user.FullName} on {appointment.AppointmentDate.ToShortDateString()} has been cancelled.";

            await _notificationService.CreateNotificationAsync(
            appointment.Patient.UserId,
            $"Appointment Status Update: {status}",
            message,
            "Appointment",
            appointment.Id
            );

            TempData["Success"] = $"Appointment status updated to {status}";
            return RedirectToAction(nameof(Dashboard));
        }

        public async Task<IActionResult> Settings()
        {
            // الحصول على معرف المستخدم الحالي
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // الحصول على معلومات الطبيب
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);

            if (doctor == null)
            {
                return NotFound("لم يتم العثور على بيانات الطبيب");
            }

            // تحضير نموذج إعدادات الطبيب
            var viewModel = new DoctorSettingsViewModel
            {
                ProfileSettings = new DoctorViewModel
                {
                    Id = doctor.Id,
                    FullName = user.FullName ?? "",
                    Email = user.Email ?? "",
                    Specialty = doctor.Specialty ?? "",
                    PhoneNumber = user.PhoneNumber ?? ""
                },
                NotificationSettings = new NotificationSettingsViewModel
                {
                    AppointmentNotifications = true, // يمكن استرجاعها من قاعدة البيانات إذا كانت مخزنة
                    TaskReminders = true,
                    EmailNotifications = false
                },
                SecuritySettings = new SecuritySettingsViewModel()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(DoctorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "يرجى التحقق من صحة البيانات المدخلة";
                return RedirectToAction(nameof(Settings));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(d => d.UserId == user.Id);
            if (doctor == null)
            {
                return NotFound("لم يتم العثور على بيانات الطبيب");
            }

            // تحديث بيانات المستخدم
            user.FullName = model.FullName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            // تحديث بيانات الطبيب
            doctor.Specialty = model.Specialty;

            // حفظ التغييرات
            var userResult = await _userManager.UpdateAsync(user);
            if (!userResult.Succeeded)
            {
                foreach (var error in userResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحديث البيانات";
                return RedirectToAction(nameof(Settings));
            }

            await _unitOfWork.SaveAsync();

            TempData["SuccessMessage"] = "تم تحديث بيانات الملف الشخصي بنجاح";
            return RedirectToAction(nameof(Settings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNotifications(NotificationSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "يرجى التحقق من صحة البيانات المدخلة";
                return RedirectToAction(nameof(Settings));
            }

            // هنا يمكن إضافة كود لتحديث إعدادات الإشعارات في قاعدة البيانات
            // يفترض أن هناك جدول لإعدادات الإشعارات مرتبط بالمستخدم

            TempData["SuccessMessage"] = "تم تحديث إعدادات الإشعارات بنجاح";
            return RedirectToAction(nameof(Settings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(SecuritySettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "يرجى التحقق من صحة البيانات المدخلة";
                return RedirectToAction(nameof(Settings));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // التحقق من صحة كلمة المرور الحالية
            var checkPasswordResult = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!checkPasswordResult)
            {
                ModelState.AddModelError(string.Empty, "كلمة المرور الحالية غير صحيحة");
                TempData["ErrorMessage"] = "كلمة المرور الحالية غير صحيحة";
                return RedirectToAction(nameof(Settings));
            }

            // تحديث كلمة المرور
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحديث كلمة المرور";
                return RedirectToAction(nameof(Settings));
            }

            // إعادة تسجيل الدخول بعد تغيير كلمة المرور
            await _signInManager.SignInAsync(user, isPersistent: false);

            TempData["SuccessMessage"] = "تم تحديث كلمة المرور بنجاح";
            return RedirectToAction(nameof(Settings));
        }

        private string GetCurrentDoctorId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age)) age--;
            return age;
        }

        // GET: Doctor/Patients
        public async Task<IActionResult> Patients(int page = 1)
        {
            // Get current user ID
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                return BadRequest("User not found");
            }

            // Get the doctor record based on the current user ID
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == currentUserId);

            if (doctor == null)
            {
                return BadRequest("Doctor not found");
            }

            // Use the doctor's ID (which is int)
            int doctorId = doctor.Id;

            // Items per page
            int pageSize = 5;

            // Get total count of patients for this doctor
            var totalPatients = await _context.Patients
                .Where(p => p.PrimaryDoctorId == doctorId)
                .CountAsync();

            // Calculate total pages
            int totalPages = (int)Math.Ceiling(totalPatients / (double)pageSize);

            // Ensure page is within valid range
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            // Get patients for the current page
            var patients = await _context.Patients
                .Where(p => p.PrimaryDoctorId == doctorId)
                .OrderByDescending(p => p.LastVisit)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var PatientViewModel = patients.Select(p => new PatientViewModel
            {
                Id = p.Id,
                FullName = p.FullName,
                DateOfBirth = (DateTime)p.DateOfBirth,
                Age = CalculateAge(p.DateOfBirth),
                LastVisit = p.LastVisit,
                MedicalHistory = p.MedicalHistory,
            }).ToList();

            // Create and populate the view model
            var viewModel = new DoctorPatientsViewModel
            {
                TotalPatientsCount = totalPatients,
                Patients = PatientViewModel,
                CurrentPage = page,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        private int CalculateAge(DateTime? dateOfBirth)
        {
            throw new NotImplementedException();
        }

        // GET: Doctor/Reports
        public async Task<IActionResult> Reports()
        {
            // Get current user ID
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserId))
            {
                return BadRequest("User not found");
            }

            // Get the doctor record based on the current user ID
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.UserId == currentUserId);

            if (doctor == null)
            {
                return BadRequest("Doctor not found");
            }

            // Use the doctor's ID (which is int)
            int doctorId = doctor.Id;

            // Get total count of visits (reports) for this doctor
            var totalVisits = await _context.MedicalReports
                .Where(r => r.DoctorId == doctorId)
                .CountAsync();

            // Get the most recent reports
            var reports = await _context.MedicalReports
                .Where(r => r.DoctorId == doctorId)
                .OrderByDescending(r => r.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Get current month and year
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            // Get monthly visits count for current month
            var monthlyVisitsCount = await _context.MedicalReports
                .Where(r => r.DoctorId == doctorId &&
                           r.CreatedAt.Month == currentMonth &&
                           r.CreatedAt.Year == currentYear)
                .CountAsync();

            // Get weekly visits count for current week
            var startOfWeek = DateTime.Now.StartOfWeek(DayOfWeek.Sunday);
            var endOfWeek = startOfWeek.AddDays(7);
            var weeklyVisitsCount = await _context.MedicalReports
                .Where(r => r.DoctorId == doctorId &&
                           r.CreatedAt >= startOfWeek &&
                           r.CreatedAt < endOfWeek)
                .CountAsync();

            // Calculate daily average (last 30 days)
            var thirtyDaysAgo = DateTime.Now.AddDays(-30);
            var last30DaysVisits = await _context.MedicalReports
                .Where(r => r.DoctorId == doctorId && r.CreatedAt >= thirtyDaysAgo)
                .CountAsync();
            var dailyAverage = last30DaysVisits / 30.0;

            // Get monthly visit data for the chart (last 6 months)
            var today = DateTime.Today;
            var sixMonthsAgo = today.AddMonths(-6);
            var monthlyVisits = await _context.MedicalReports
                .Where(r => r.DoctorId == doctorId && r.CreatedAt >= sixMonthsAgo)
                .GroupBy(r => new { r.CreatedAt.Year, r.CreatedAt.Month })
                .Select(g => new {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    VisitCount = g.Count()
                })
                .ToListAsync();

            // تنسيق البيانات بعد جلبها من قاعدة البيانات
            var formattedMonthlyVisits = monthlyVisits.Select(m => new MonthlyVisitData
            {
                Month = $"{m.Month}/{m.Year}",
                VisitCount = m.VisitCount
            }).OrderBy(m => m.Month).ToList();

            // Create and populate the view model
            var viewModel = new DoctorReportsViewModel
            {
                TotalVisits = totalVisits,
                MonthlyVisitsCount = monthlyVisitsCount,
                WeeklyVisits = weeklyVisitsCount,
                DailyAverage = dailyAverage,
                RecentReports = reports,
                MonthlyVisits = formattedMonthlyVisits
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DoctorViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create user account
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    UserType = "Doctor"
                };

                // Generate a random password (you might want to send this via email)
                string password = GenerateRandomPassword();
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Doctor");

                    // Create doctor profile
                    var doctor = new Doctor
                    {
                        UserId = user.Id,
                        Specialty = model.Specialty,
                        Experience = model.ExperienceYears,
                        Rating = 0
                    };

                    // Handle profile image upload
                    if (model.ProfileImage != null)
                    {
                        // Implementation for file upload
                        // This is a placeholder - you would need to implement file storage
                        string uniqueFileName = System.Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                        // Save file to wwwroot/images/doctors
                        // string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", "doctors", uniqueFileName);
                        // using (var fileStream = new FileStream(filePath, FileMode.Create))
                        // {
                        //     await model.ProfileImage.CopyToAsync(fileStream);
                        // }

                        doctor.ImageUrl = "/images/doctors/" + uniqueFileName;
                    }

                    await _unitOfWork.Doctors.AddAsync(doctor);

                    // TODO: Send email with login credentials

                    TempData["Success"] = "Doctor created successfully. Password: " + password;
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(
                d => d.Id == id,
                includeProperties: "User");

            if (doctor == null)
            {
                return NotFound();
            }

            var viewModel = new DoctorViewModel
            {
                Id = doctor.Id,
                FullName = doctor.User.FullName,
                Email = doctor.User.Email,
                PhoneNumber = doctor.User.PhoneNumber,
                Specialty = doctor.Specialty,
                ExperienceYears = doctor.Experience,
                Rating = doctor.Rating ?? 0m,
                ImageUrl = doctor.ImageUrl
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        // POST: Doctors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DoctorViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(
                    d => d.Id == id,
                    includeProperties: "User");

                if (doctor == null)
                {
                    return NotFound();
                }

                // Update user information
                doctor.User.FullName = model.FullName;
                doctor.User.PhoneNumber = model.PhoneNumber;

                var userResult = await _userManager.UpdateAsync(doctor.User);
                if (!userResult.Succeeded)
                {
                    foreach (var error in userResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                // Update doctor information
                doctor.Specialty = model.Specialty;
                doctor.Experience = model.ExperienceYears;

                // Handle profile image upload
                if (model.ProfileImage != null)
                {
                    // Implementation for file upload
                    // This is a placeholder - you would need to implement file storage
                    string uniqueFileName = System.Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                    // Save file to wwwroot/images/doctors
                    // string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", "doctors", uniqueFileName);
                    // using (var fileStream = new FileStream(filePath, FileMode.Create))
                    // {
                    //     await model.ProfileImage.CopyToAsync(fileStream);
                    // }

                    doctor.ImageUrl = "/images/doctors/" + uniqueFileName;
                }

                await _unitOfWork.Doctors.UpdateAsync(doctor);

                TempData["Success"] = "Doctor updated successfully";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        // GET: Doctors/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(
                d => d.Id == id,
                includeProperties: "User");

            if (doctor == null)
            {
                return NotFound();
            }

            var viewModel = new DoctorViewModel
            {
                Id = doctor.Id,
                FullName = doctor.User.FullName,
                Email = doctor.User.Email,
                PhoneNumber = doctor.User.PhoneNumber,
                Specialty = doctor.Specialty,
                ExperienceYears = doctor.Experience,
                Rating = doctor.Rating ?? 0m,
                ImageUrl = doctor.ImageUrl
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(
                d => d.Id == id,
                includeProperties: "User");

            if (doctor == null)
            {
                return NotFound();
            }

            // Delete the doctor profile
            await _unitOfWork.Doctors.RemoveAsync(doctor);

            // Delete the user account
            var user = await _userManager.FindByIdAsync(doctor.UserId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }

            TempData["Success"] = "Doctor deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        private string GenerateRandomPassword()
        {
            // Generate a random password that meets the requirements
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            var random = new System.Random();
            var password = new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return password;
        }
    }
}