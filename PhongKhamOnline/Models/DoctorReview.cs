using System.Numerics;

namespace PhongKhamOnline.Models
{
    public class DoctorReview
    {
        public int Id { get; set; }

        public string UserId { get; set; } // ID bệnh nhân
        public string ReviewText { get; set; } // Nội dung đánh giá
        public int Rating { get; set; } // Số sao (1-5)
        public DateTime CreatedAt { get; set; } // Thời gian đánh giá
        public int BacSiId { get; set; } // ID bác sĩ
        public BacSi BacSi { get; set; } // Điều hướng đến bảng Doctor

        public string? Reply { get; set; } // Nội dung phản hồi từ bác sĩ
        public DateTime? RepliedAt { get; set; } // Thời gian bác sĩ phản hồi
    }

}