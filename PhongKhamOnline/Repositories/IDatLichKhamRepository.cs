using PhongKhamOnline.Models;

namespace PhongKhamOnline.Repositories
{
    public interface IDatLichKhamRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment> GetByIdAsync(int id);
        Task AddAsync(Appointment appointment);
        Task<IEnumerable<KhungThoiGian>> GetAvailableTimeSlotsAsync(int idBacSi, string ngayLamViec);
        Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status); 
        Task UpdateAsync(Appointment appointment); 
        Task<IEnumerable<Appointment>> GetByDoctorAsync(int doctorId);
        Task<IEnumerable<Appointment>> GetByDoctorAndStatusAsync(int doctorId, AppointmentStatus status);
        Task<IEnumerable<Appointment>> GetByPatientAsync(string userId);
        Task<IEnumerable<Appointment>> GetByPatientAndStatusAsync(string userId, AppointmentStatus status);
        Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId); // Lấy lịch khám theo bác sĩ
        Task<IEnumerable<Appointment>> GetByUserIdAsync(string userId);  // Lấy lịch khám theo bệnh nhân

        Task<IEnumerable<Appointment>> GetByDateAsync(DateTime selectedDate);


        Task<IEnumerable<Appointment>> GetByDoctorIdAndDateAsync(int doctorId, DateTime date);
        Task<IEnumerable<Appointment>> GetByUserIdAndDateAsync(string userId, DateTime date);


    }

}
