using Microsoft.EntityFrameworkCore;
using PhongKhamOnline.DataAccess;
using PhongKhamOnline.Models;

namespace PhongKhamOnline.Repositories
{
    public class EFDatLichKhamRepository : IDatLichKhamRepository
    {
        private readonly ApplicationDbContext _context;

        public EFDatLichKhamRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<Appointment> GetByIdAsync(int id)
        {
            return await _context.Appointments
               .Include(a => a.KhungThoiGian)
               .Include(a => a.BacSi)
               .FirstOrDefaultAsync(a => a.AppointmentId == id);
        }

        public async Task AddAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<KhungThoiGian>> GetAvailableTimeSlotsAsync(int idBacSi, string ngayLamViec)
        {
            var appointments = await _context.Appointments
                .Where(a => a.BacSiId == idBacSi && a.NgayKham.ToString("yyyy-MM-dd") == ngayLamViec)
                .Select(a => a.KhungThoiGianId)
                .ToListAsync();

            return await _context.KhungThoiGians
                .Where(k => !appointments.Contains(k.Id))
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status)
        {
            return await _context.Appointments
                .Where(a => a.Status == status)
                .Include(a => a.BacSi) // Bao gồm thông tin bác sĩ nếu cần
                .Include(a => a.KhungThoiGian)
                .ToListAsync();
        }

        public async Task UpdateAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorAsync(int doctorId)
        {
            return await _context.Appointments
                .Where(a => a.BacSiId == doctorId)
                .Include(a => a.BacSi)
                .Include(a => a.KhungThoiGian)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorAndStatusAsync(int doctorId, AppointmentStatus status)
        {
            return await _context.Appointments
                .Where(a => a.BacSiId == doctorId && a.Status == status)
                .Include(a => a.BacSi)
                .Include(a => a.KhungThoiGian)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments
                 .Include(a => a.BacSi)
                 .Include(a => a.KhungThoiGian)
                 .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByPatientAsync(string userId)
        {
            return await _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.BacSi)
                .Include(a => a.KhungThoiGian)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByPatientAndStatusAsync(string userId, AppointmentStatus status)
        {
            return await _context.Appointments
                .Where(a => a.UserId == userId && a.Status == status)
                .Include(a => a.BacSi)
                .Include(a => a.KhungThoiGian)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.Appointments
                .Include(a => a.BacSi)
                .Include(a => a.KhungThoiGian)
                .Where(a => a.BacSiId == doctorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByUserIdAsync(string userId)
        {
            return await _context.Appointments
                .Include(a => a.BacSi)
                .Include(a => a.KhungThoiGian)
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDateAsync(DateTime selectedDate)
        {
            return await _context.Appointments
                .Include(a => a.KhungThoiGian)
                .Where(a => a.NgayKham.Date == selectedDate.Date) // Lọc theo ngày
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDoctorIdAndDateAsync(int doctorId, DateTime date)
        {
            return await _context.Appointments
                .Include(a => a.KhungThoiGian)
                .Where(a =>  a.NgayKham.Date == date.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByUserIdAndDateAsync(string userId, DateTime date)
        {
            return await _context.Appointments
                .Include(a => a.KhungThoiGian)
                .Where(a => a.NgayKham.Date == date.Date)
                .ToListAsync();
        }

    }
}
