namespace PhongKhamOnline.Models
{
    public class LichLamViec
    {
        public int Id { get; set; }
        public int BacSiId { get; set; }
        public BacSi BacSi { get; set; } // Liên kết đến bảng bác sĩ
        public DateTime NgayLamViec { get; set; }
        public string ThoiGian { get; set; } // Ví dụ: "9:00-9:30"
        public int SoLuongToiDa { get; set; } // Số bệnh nhân tối đa
        
    }
}
