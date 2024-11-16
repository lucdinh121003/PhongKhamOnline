namespace PhongKhamOnline.Models
{
    public class DoctorViewModel
    {
        // Danh sách bác sĩ
        public IEnumerable<BacSi> BacSis { get; set; }

        // Danh sách chuyên môn
        public IEnumerable<ChuyenMonBacSi> ChuyenMons { get; set; }

        // Chuyên môn đã chọn
        public int SelectedSpecialtyId { get; set; }
    }
}