using System.ComponentModel.DataAnnotations;

namespace PhongKhamOnline.Models
{
    public class KhungGioBacSi
    {
        public int Id { get; set; }
        [Required, StringLength(100)]
        public string GioLamViec { get; set; }
        public List<BacSi>? BacSis { get; set; }

    }
}
