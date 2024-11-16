namespace PhongKhamOnline.Models
{
    public class BacSiImage
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int BacSiId { get; set; }
        public BacSi? BacSi { get; set; }
    }
}
