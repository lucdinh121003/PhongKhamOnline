using PhongKhamOnline.Models;

namespace PhongKhamOnline.Repositories
{
    public interface ILichLamViecRepository
    {
        Task<IEnumerable<LichLamViec>> GetAllAsync();
        Task<LichLamViec> GetByIdAsync(int id);
        Task AddAsync(LichLamViec lichLamViec);
        Task UpdateAsync(LichLamViec lichLamViec);
        Task DeleteAsync(int id);
        Task<IEnumerable<LichLamViec>> GetByBacSiAndDateAsync(int bacSiId, DateTime ngayLamViec);
    }
}
