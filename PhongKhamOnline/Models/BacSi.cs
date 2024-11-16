using System.ComponentModel.DataAnnotations;

namespace PhongKhamOnline.Models
{
    public class BacSi
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string Ten { get; set; }
        public string SoDienThoai { get; set; }
        public int ChuyenMonBacSiId { get; set; }
        public ChuyenMonBacSi? ChuyenMonBacSi { get; set; }
        public string DiaChiPhongKham { get; set; }
        public string DiaChiThuongTru { get; set; }
        public string Email { get; set; }
        public string SoDienThoaiPhongKham { get; set; }
        public string? MoTa { get; set; }
        public int? SoHenToiDaTrongNgay { get; set; }
        public string? AnhDaiDien { get; set; }
        public List<BacSiImage>? AnhDaiDiens { get; set; }
        public string SoTaiKhoan { get; set; }
        public int KhungGioBacSiId { get; set; }
        public ApplicationUser User { get; set; }
        public KhungGioBacSi? KhungGioBacSi { get; set; }

    }
}
