namespace PhongKhamOnline.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public string PatientName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTimeSlot { get; set; }
        public string UserId { get; set; } // Liên kết với tài khoản người dùng
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending; // Mặc định là Pending

        // Liên kết với bác sĩ

        public int BacSiId { get; set; }
        public BacSi BacSi { get; set; }
    }

    public enum AppointmentStatus
    {
        Pending, // Chờ xử lý
        Confirmed, // Đã xác nhận
        Cancelled // Đã hủy
    }
 
}
