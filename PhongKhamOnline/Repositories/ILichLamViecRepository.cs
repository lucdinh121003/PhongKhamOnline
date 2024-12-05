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
        Task<LichLamViec> GetByBacSiAndDateAsync(int bacSiId, DateTime ngayLamViec);

        Task<IEnumerable<LichLamViec>> getListLichLamViecByBacSiIdAndDate(int bacSiId, DateTime ngayLamViec);

        Task<IEnumerable<LichLamViec>> getListLichLamViecById(int id);

        Task<List<LichLamViec>> GetByBacSiIdAsync(int bacSiId);
    }
}