using PhongKhamOnline.Models;

namespace PhongKhamOnline.Repositories
{
    public interface IKhungGioBacSiRepository
    {
        Task<IEnumerable<KhungGioBacSi>> GetAllAsync();
        Task<KhungGioBacSi> GetByIdAsync(int id);
        Task AddAsync(KhungGioBacSi khungGioBacSi);
        Task UpdateAsync(KhungGioBacSi khungGioBacSi);
        Task DeleteAsync(int id);
    }

}
