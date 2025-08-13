using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MedicalConnect.ViewModels;
using MedicalConnect.Database;
using MedicalConnect.Repositories;
using MedicalConnect.Services;
using static MedicalConnect.ViewModels.PatientViewModel;
using static MedicalConnect.ViewModels.DoctorDashboardViewModel;

namespace MedicalConnect.Controllers
{
    public class PatientsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatientsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(
                    p => p.UserId == user.Id,
                    includeProperties: "User,Appointments,Appointments.Doctor,Appointments.Doctor.User,MedicalReports,MedicalReports.Doctor,MedicalReports.Doctor.User"
                );

                if (patient == null)
                {
                    TempData["ErrorMessage"] = "لم يتم العثور على ملفك الطبي";
                    return RedirectToAction("Index", "Home");
                }

                var viewModel = new PatientProfileViewModel
                {
                    PatientId = patient.Id,
                    FullName = patient.User.FullName,
                    Email = patient.User.Email,
                    PhoneNumber = patient.User.PhoneNumber,
                    Gender = patient.User.Gender,
                    City = patient.User.City,
                    Location = patient.User.Location,
                    DateOfBirth = patient.DateOfBirth,
                    Age = CalculateAge(patient.DateOfBirth),
                    BloodType = patient.BloodType,
                    MedicalHistory = patient.MedicalHistory,
                    Height = patient.Height,
                    Weight = patient.Weight,
                    LastVisit = patient.Appointments?.OrderByDescending(a => a.AppointmentDate).FirstOrDefault()?.AppointmentDate,
                    TotalAppointments = patient.Appointments?.Count ?? 0,
                    TotalMedicalReports = patient.MedicalReports?.Count ?? 0,
                    RecentAppointments = patient.Appointments?
                        .OrderByDescending(a => a.AppointmentDate)
                        .Take(5)
                        .Select(a => new AppointmentViewModel
                        {
                            Id = a.Id,
                            AppointmentDate = a.AppointmentDate.Add(a.StartTime),
                            Status = a.Status,
                            Notes = a.Notes,
                            DoctorName = a.Doctor?.User?.FullName ?? "غير محدد",
                            DoctorSpecialization = a.Doctor?.Specialty ?? "غير محدد"
                        }).ToList() ?? new List<AppointmentViewModel>(),
                    RecentMedicalReports = patient.MedicalReports?
                        .OrderByDescending(mr => mr.ReportDate)
                        .Take(5)
                        .Select(mr => new MedicalReportViewModel
                        {
                            Id = mr.Id,
                            Title = mr.Title,
                            Diagnosis = mr.Diagnosis,
                            Treatment = mr.Treatment,
                            CreatedAt = mr.CreatedAt,
                            DoctorName = mr.Doctor?.User?.FullName ?? "غير محدد"
                        }).ToList() ?? new List<MedicalReportViewModel>()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحميل الملف الشخصي: " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> UpdateProfile(PatientProfileViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(
                    p => p.UserId == user.Id,
                    includeProperties: "User"
                );

                if (patient == null)
                {
                    TempData["ErrorMessage"] = "لم يتم العثور على ملفك الطبي";
                    return RedirectToAction("Profile");
                }

                if (!ModelState.IsValid)
                {
                    // إعادة تحميل البيانات المطلوبة للعرض
                    model.PatientId = patient.Id;
                    return View("Profile", model);
                }

                // Check if email is changed and already exists
                if (patient.User.Email != model.Email)
                {
                    var existingUser = await _userManager.FindByEmailAsync(model.Email);
                    if (existingUser != null && existingUser.Id != patient.UserId)
                    {
                        ModelState.AddModelError("Email", "البريد الإلكتروني مستخدم بالفعل");
                        return View("Profile", model);
                    }
                }

                // Update user data
                patient.User.FullName = model.FullName;
                patient.User.Email = model.Email;
                patient.User.UserName = model.Email;
                patient.User.PhoneNumber = model.PhoneNumber;
                patient.User.Gender = model.Gender;
                patient.User.City = model.City;
                patient.User.Location = model.Location;
                patient.User.BirthDate = model.DateOfBirth;
                patient.User.UpdatedAt = DateTime.UtcNow;

                // Update patient data
                patient.DateOfBirth = model.DateOfBirth;
                patient.BloodType = model.BloodType;
                patient.MedicalHistory = model.MedicalHistory;
                patient.Height = model.Height;
                patient.Weight = model.Weight;
                patient.UpdatedAt = DateTime.UtcNow;

                var userResult = await _userManager.UpdateAsync(patient.User);
                if (!userResult.Succeeded)
                {
                    foreach (var error in userResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("Profile", model);
                }

                await _unitOfWork.Patients.UpdateAsync(patient);
                await _unitOfWork.SaveAsync();

                TempData["SuccessMessage"] = "تم تحديث الملف الشخصي بنجاح";
                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحديث الملف الشخصي: " + ex.Message;
                return RedirectToAction("Profile");
            }
        }

        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> Appointments()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(
                    p => p.UserId == user.Id,
                    includeProperties: "User,Appointments,Appointments.Doctor,Appointments.Doctor.User"
                );

                if (patient == null)
                {
                    TempData["ErrorMessage"] = "لم يتم العثور على ملفك الطبي";
                    return RedirectToAction("Index", "Home");
                }

                var now = DateTime.Now;

                var all = patient.Appointments?
                    .Select(a => new AppointmentViewModel
                    {
                        Id = a.Id,
                        DoctorId = a.DoctorId,
                        DoctorName = a.Doctor?.User?.FullName ?? "غير محدد",
                        DoctorSpecialization = a.Doctor?.Specialty ?? "غير محدد",
                        AppointmentDate = a.AppointmentDate.Date.Add(a.StartTime),
                        StartTime = a.StartTime,
                        EndTime = a.EndTime,
                        Title = a.Title,
                        Type = a.Type,
                        Status = a.Status,
                        Notes = a.Notes,
                        IsVirtual = a.IsVirtual,
                        MeetingLink = a.MeetingLink
                    })
                    .OrderByDescending(x => x.AppointmentTime)
                    .ToList() ?? new List<AppointmentViewModel>();

                bool IsCancelled(AppointmentViewModel a) => string.Equals(a.Status, "Cancelled", StringComparison.OrdinalIgnoreCase) || a.Status == "ملغي";
                bool IsCompleted(AppointmentViewModel a) => string.Equals(a.Status, "Completed", StringComparison.OrdinalIgnoreCase) || a.Status == "مكتمل";
                bool IsUpcoming(AppointmentViewModel a) => (string.Equals(a.Status, "Scheduled", StringComparison.OrdinalIgnoreCase) || string.Equals(a.Status, "Pending", StringComparison.OrdinalIgnoreCase) || a.Status == "قادم")
                                                            && a.AppointmentDate >= now && !IsCancelled(a);

                var upcoming = all.Where(IsUpcoming).OrderBy(a => a.AppointmentTime).ToList();
                var completed = all.Where(IsCompleted).ToList();
                var cancelled = all.Where(IsCancelled).ToList();

                var vm = new PatientAppointmentsViewModel
                {
                    PatientId = patient.Id,
                    PatientName = patient.User.FullName,
                    TotalAppointmentsCount = all.Count,
                    CompletedAppointmentsCount = completed.Count,
                    UpcomingAppointmentsCount = upcoming.Count,
                    CancelledAppointmentsCount = cancelled.Count,
                    AllAppointments = all,
                    UpcomingAppointments = upcoming,
                    CompletedAppointments = completed,
                    CancelledAppointments = cancelled
                };

                ViewData["Title"] = "مواعيدي - Medical Connect";
                ViewData["MetaDescription"] = "عرض جميع مواعيد المريض مع الإحصائيات والتبويبات";

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحميل المواعيد: " + ex.Message;
                return RedirectToAction("Profile");
            }
        }

        private int? CalculateAge(DateTime? birthDate)
        {
            var today = DateTime.Today;
            int age = today.Year - birthDate.Value.Year;
            if (birthDate.Value.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}