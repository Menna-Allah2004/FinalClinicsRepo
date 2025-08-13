using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MedicalConnect.Database;
using MedicalConnect.ViewModels;
using MedicalConnect.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;


namespace MedicalConnect.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppointmentsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // GET: /Appointment/BookAppointment/{doctorId}
        [HttpGet]
        public async Task<IActionResult> BookAppointment(int doctorId)
        {
            var doctor = await _unitOfWork.Doctors.GetFirstOrDefaultAsync(
                d => d.Id == doctorId,
                includeProperties: "User"
            );
            if (doctor == null)
            {
                TempData["Error"] = "الطبيب غير موجود";
                return RedirectToAction("Index", "Doctors");
            }

            var user = await _userManager.GetUserAsync(User);
            var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);

            if (patient == null)
            {
                TempData["Error"] = "يجب أن تكون مريضاً مسجلاً لحجز موعد";
                return RedirectToAction("Register", "Account");
            }

            var viewModel = new BookAppointmentViewModel
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.User.FullName,
                DoctorSpecialization = doctor.User.Specialty,
                DoctorImageUrl = doctor.User.ImageUrl ?? "/images/doctors/default-user.png",
                DoctorLocation = doctor.User.Location,
                ConsultationFee = doctor.ConsultationFee,
                DoctorRating = doctor.Rating,
                DoctorReviews = doctor.RatingCount,
                PatientName = patient.User.FullName,
                PatientPhone = patient.User.PhoneNumber,
                PatientEmail = patient.User.Email,
                AvailableDates = await GetAvailableDatesForDoctor(doctorId)
            };

            return View(viewModel);
        }

        // POST: /Appointment/GetAvailableTimeSlots
        [HttpPost]
        public async Task<IActionResult> GetAvailableTimeSlots([FromBody] AvailableTimesRequest request)
        {
            try
            {
                var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.DoctorId);
                if (doctor == null)
                {
                    return Json(new { success = false, message = "الطبيب غير موجود" });
                }

                var dayOfWeek = request.Date.DayOfWeek.ToString();

                // جلب مواعيد الطبيب المتاحة لهذا اليوم
                var availability = await _unitOfWork.DoctorAvailabilities
                    .GetFirstOrDefaultAsync(a => a.DoctorId == request.DoctorId &&
                                                 a.DayOfWeek.ToString() == dayOfWeek &&
                                                 a.IsAvailable);

                if (availability == null)
                {
                    return Json(new { success = true, timeSlots = new List<object>() });
                }

                // جلب المواعيد المحجوزة لهذا التاريخ
                var bookedAppointments = await _unitOfWork.Appointments
                    .GetAsync(a => a.DoctorId == request.DoctorId &&
                                   a.AppointmentDate.Date == request.Date.Date &&
                                   a.Status != "Cancelled");

                var bookedTimes = bookedAppointments.Select(a => a.AppointmentTime).ToList();

                // إنشاء قائمة بالأوقات المتاحة
                var timeSlots = new List<object>();
                var currentTime = availability.StartTime;
                var endTime = availability.EndTime;

                while (currentTime < endTime)
                {
                    // تحقق من فترة الاستراحة
                    if (availability.BreakStartTime.HasValue && availability.BreakEndTime.HasValue)
                    {
                        if (currentTime >= availability.BreakStartTime && currentTime < availability.BreakEndTime)
                        {
                            currentTime = currentTime.Add(TimeSpan.FromMinutes(availability.SlotDuration));
                            continue;
                        }
                    }

                    // تحقق من عدم وجود حجز في هذا الوقت
                    if (!bookedTimes.Contains(currentTime))
                    {
                        timeSlots.Add(new
                        {
                            startTime = new
                            {
                                hours = currentTime.Hours,
                                minutes = currentTime.Minutes
                            },
                            displayTime = currentTime.ToString(@"hh\:mm"),
                            period = currentTime.Hours < 12 ? "صباحاً" : "مساءً"
                        });
                    }

                    currentTime = currentTime.Add(TimeSpan.FromMinutes(availability.SlotDuration));
                }

                return Json(new { success = true, timeSlots });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ في جلب الأوقات المتاحة" });
            }
        }

        // POST: /Appointment/ConfirmBooking
        [HttpPost]
        public async Task<IActionResult> ConfirmBooking([FromBody] BookAppointmentViewModel model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);

                if (patient == null)
                {
                    return Json(new { success = false, message = "المريض غير موجود" });
                }

                var doctor = await _unitOfWork.Doctors.GetByIdAsync(model.DoctorId);
                if (doctor == null)
                {
                    return Json(new { success = false, message = "الطبيب غير موجود" });
                }

                // تحقق من توفر الوقت المحدد
                var existingAppointment = await _unitOfWork.Appointments
                    .GetAsync(a => a.DoctorId == model.DoctorId &&
                                   a.AppointmentDate.Date == model.SelectedDate.Date &&
                                   a.AppointmentTime == model.SelectedTime &&
                                   a.Status != "Cancelled");

                if (existingAppointment.Any())
                {
                    return Json(new { success = false, message = "هذا الوقت محجوز بالفعل" });
                }

                // إنشاء الموعد الجديد
                var appointment = new Appointment
                {
                    PatientId = patient.Id,
                    DoctorId = model.DoctorId,
                    AppointmentDate = model.SelectedDate,
                    AppointmentTime = model.SelectedTime,
                    Duration = doctor.ConsultationDuration,
                    Status = "Scheduled",
                    Type = model.AppointmentType == "inPerson" ? "InPerson" : "Online",
                    Reason = model.Reason,
                    Notes = model.Notes,
                    Fee = doctor.ConsultationFee,
                    PaymentStatus = model.PaymentMethod == "creditCard" ? "Paid" : "Pending",
                    Title = $"موعد مع د. {doctor.User.FullName}",
                    StartTime = model.SelectedTime,
                    EndTime = model.SelectedTime.Add(TimeSpan.FromMinutes(doctor.ConsultationDuration)),
                    IsVirtual = model.AppointmentType == "online",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Appointments.AddAsync(appointment);
                await _unitOfWork.SaveAsync();

                // إنشاء رقم الحجز
                var bookingNumber = $"BK-{appointment.Id:D6}";

                return Json(new
                {
                    success = true,
                    bookingNumber = bookingNumber,
                    message = "تم تأكيد الحجز بنجاح"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء تأكيد الحجز" });
            }
        }

        // GET: /Appointment/MyAppointments
        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            var user = await _userManager.GetUserAsync(User);
            var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);

            if (patient == null)
            {
                TempData["Error"] = "يجب أن تكون مريضاً مسجلاً لعرض المواعيد";
                return RedirectToAction("Register", "Account");
            }

            var appointments = await _unitOfWork.Appointments
                .GetAsync(a => a.PatientId == patient.Id, includeProperties: "Doctor,Doctor.User");

            var appointmentViewModels = appointments.OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentViewModel
                {
                    Id = a.Id,
                    DoctorName = a.Doctor.User.FullName,
                    DoctorSpecialization = a.Doctor.User.Specialty,
                    DoctorImageUrl = a.Doctor.User.ImageUrl ?? "/images/doctors/default-user.png",
                    AppointmentDate = a.AppointmentDate,
                    AppointmentTime = a.AppointmentTime,
                    Status = a.Status,
                    Type = a.Type,
                    Reason = a.Reason,
                    Fee = a.Fee,
                    PaymentStatus = a.PaymentStatus,
                    IsVirtual = a.IsVirtual,
                    MeetingLink = a.MeetingLink
                }).ToList();

            return View(appointmentViewModels);
        }

        // GET: /Appointment/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);

            if (patient == null)
            {
                TempData["Error"] = "يجب أن تكون مريضاً مسجلاً لعرض تفاصيل الموعد";
                return RedirectToAction("Register", "Account");
            }

            var appointment = await _unitOfWork.Appointments.GetFirstOrDefaultAsync(
                a => a.Id == id,
                includeProperties: "Doctor,Doctor.User");

            if (appointment == null || appointment.PatientId != patient.Id)
            {
                TempData["Error"] = "الموعد غير موجود أو ليس لديك صلاحية لعرضه";
                return RedirectToAction("MyAppointments");
            }

            var viewModel = new AppointmentViewModel
            {
                Id = appointment.Id,
                DoctorName = appointment.Doctor.User.FullName,
                DoctorSpecialization = appointment.Doctor.User.Specialty,
                DoctorImageUrl = appointment.Doctor.User.ImageUrl ?? "/images/default-doctor.jpg",
                DoctorLocation = appointment.Doctor.User.Location,
                AppointmentDate = appointment.AppointmentDate,
                AppointmentTime = appointment.AppointmentTime,
                Status = appointment.Status,
                Type = appointment.Type,
                Reason = appointment.Reason,
                Notes = appointment.Notes,
                Fee = appointment.Fee,
                PaymentStatus = appointment.PaymentStatus,
                IsVirtual = appointment.IsVirtual,
                MeetingLink = appointment.MeetingLink,
                CanCancel = appointment.Status == "Scheduled" && appointment.AppointmentDate > DateTime.Today,
                CanReschedule = appointment.Status == "Scheduled" && appointment.AppointmentDate > DateTime.Today.AddDays(1)
            };

            return View(viewModel);
        }

        // POST: /Appointment/Cancel/{id}
        [HttpPost]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelAppointmentRequest request)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);

                var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);

                if (appointment == null || appointment.PatientId != patient.Id)
                {
                    return Json(new { success = false, message = "الموعد غير موجود" });
                }

                if (appointment.Status == "Cancelled")
                {
                    return Json(new { success = false, message = "الموعد ملغي بالفعل" });
                }

                // حذف الموعد نهائياً من قاعدة البيانات
                await _unitOfWork.Appointments.RemoveAsync(appointment);
                await _unitOfWork.SaveAsync();

                return Json(new { success = true, message = "تم إلغاء وحذف الموعد بنجاح" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء إلغاء الموعد" });
            }
        }

        // POST: /Appointment/Reschedule/{id}
        [HttpPost]
        public async Task<IActionResult> Reschedule(int id, [FromBody] RescheduleAppointmentRequest request)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var patient = await _unitOfWork.Patients.GetFirstOrDefaultAsync(p => p.UserId == user.Id);

                var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);

                if (appointment == null || appointment.PatientId != patient.Id)
                {
                    return Json(new { success = false, message = "الموعد غير موجود" });
                }

                if (appointment.Status != "Scheduled")
                {
                    return Json(new { success = false, message = "لا يمكن إعادة جدولة هذا الموعد" });
                }

                // تحقق من توفر الوقت الجديد
                var existingAppointment = await _unitOfWork.Appointments
                    .GetAsync(a => a.DoctorId == appointment.DoctorId &&
                                   a.AppointmentDate.Date == request.NewDate.Date &&
                                   a.AppointmentTime == request.NewTime &&
                                   a.Status != "Cancelled" &&
                                   a.Id != id);

                if (existingAppointment.Any())
                {
                    return Json(new { success = false, message = "الوقت الجديد محجوز بالفعل" });
                }

                // تحديث الموعد
                appointment.AppointmentDate = request.NewDate;
                appointment.AppointmentTime = request.NewTime;
                appointment.StartTime = request.NewTime;
                appointment.EndTime = request.NewTime.Add(TimeSpan.FromMinutes(appointment.Duration));
                appointment.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.Appointments.UpdateAsync(appointment);
                await _unitOfWork.SaveAsync();

                return Json(new
                {
                    success = true,
                    message = "تم إعادة جدولة الموعد بنجاح",
                    newDate = request.NewDate.ToString("dd/MM/yyyy"),
                    newTime = request.NewTime.ToString(@"hh\:mm")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ أثناء إعادة جدولة الموعد" });
            }
        }

        // POST: /Appointment/GetAvailableTimesForReschedule
        [HttpPost]
        public async Task<IActionResult> GetAvailableTimesForReschedule([FromBody] AvailableTimesRequest request)
        {
            try
            {
                var doctor = await _unitOfWork.Doctors.GetByIdAsync(request.DoctorId);
                if (doctor == null)
                {
                    return Json(new { success = false, message = "الطبيب غير موجود" });
                }

                var dayOfWeek = request.Date.DayOfWeek.ToString();

                // جلب مواعيد الطبيب المتاحة لهذا اليوم
                var availability = await _unitOfWork.DoctorAvailabilities
                    .GetFirstOrDefaultAsync(a => a.DoctorId == request.DoctorId &&
                                                 a.DayOfWeek.ToString() == dayOfWeek &&
                                                 a.IsAvailable);

                if (availability == null)
                {
                    return Json(new { success = true, timeSlots = new List<object>() });
                }

                // جلب المواعيد المحجوزة لهذا التاريخ
                var bookedAppointments = await _unitOfWork.Appointments
                    .GetAsync(a => a.DoctorId == request.DoctorId &&
                                   a.AppointmentDate.Date == request.Date.Date &&
                                   a.Status != "Cancelled");

                var bookedTimes = bookedAppointments.Select(a => a.AppointmentTime).ToList();

                // إنشاء قائمة بالأوقات المتاحة
                var timeSlots = new List<object>();
                var currentTime = availability.StartTime;
                var endTime = availability.EndTime;

                while (currentTime < endTime)
                {
                    // تحقق من فترة الاستراحة
                    if (availability.BreakStartTime.HasValue && availability.BreakEndTime.HasValue)
                    {
                        if (currentTime >= availability.BreakStartTime && currentTime < availability.BreakEndTime)
                        {
                            currentTime = currentTime.Add(TimeSpan.FromMinutes(availability.SlotDuration));
                            continue;
                        }
                    }

                    // تحقق من عدم وجود حجز في هذا الوقت
                    if (!bookedTimes.Contains(currentTime))
                    {
                        timeSlots.Add(new
                        {
                            time = currentTime.ToString(@"hh\:mm"),
                            display = currentTime.ToString(@"hh\:mm") + (currentTime.Hours < 12 ? " صباحاً" : " مساءً")
                        });
                    }

                    currentTime = currentTime.Add(TimeSpan.FromMinutes(availability.SlotDuration));
                }

                return Json(new { success = true, timeSlots });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "حدث خطأ في جلب الأوقات المتاحة" });
            }
        }

        private async Task<List<DateTime>> GetAvailableDatesForDoctor(int doctorId)
        {
            var availableDates = new List<DateTime>();
            var today = DateTime.Today;

            // جلب مواعيد الطبيب المتاحة
            var availabilities = await _unitOfWork.DoctorAvailabilities
                .GetAsync(a => a.DoctorId == doctorId && a.IsAvailable);

            // إنشاء قائمة بالتواريخ المتاحة للشهر القادم
            for (int i = 1; i <= 30; i++)
            {
                var date = today.AddDays(i);
                var dayOfWeek = date.DayOfWeek.ToString();

                if (availabilities.Any(a => a.DayOfWeek.ToString() == dayOfWeek))
                {
                    availableDates.Add(date);
                }
            }

            return availableDates;
        }
    }
}