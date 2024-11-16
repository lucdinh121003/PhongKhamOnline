using PhongKhamOnline.Models;

namespace PhongKhamOnline.Repositories
{
    public interface IChuyenMonBacSiRepository
    {
        Task<IEnumerable<ChuyenMonBacSi>> GetAllAsync();
        Task<ChuyenMonBacSi> GetByIdAsync(int id);
        Task AddAsync(ChuyenMonBacSi chuyenMonBacSi);
        Task UpdateAsync(ChuyenMonBacSi chuyenMonBacSi);
        Task DeleteAsync(int id);
    }
}