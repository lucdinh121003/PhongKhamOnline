using System.ComponentModel.DataAnnotations;

namespace PhongKhamOnline.Models
{
    public class ChuyenMonBacSi
    {
        public int Id { get; set; }
        [Required, StringLength(100)]

        public string TenChuyenMon { get; set; }
        public List<BacSi>? BacSis { get; set; }
    }
}
