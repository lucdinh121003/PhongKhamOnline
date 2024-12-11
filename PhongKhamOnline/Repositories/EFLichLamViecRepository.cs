using Microsoft.EntityFrameworkCore;
using PhongKhamOnline.DataAccess;
using PhongKhamOnline.Models;

namespace PhongKhamOnline.Repositories
{
    public class EFLichLamViecRepository : ILichLamViecRepository
    {
        private readonly ApplicationDbContext _context;

        public EFLichLamViecRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LichLamViec>> GetAllAsync()
        {
            return await _context.LichLamViecs
                .Include(l => l.BacSi)
                .Include(l => l.KhungThoiGian)
                .Where(l => l.BacSi != null && l.KhungThoiGian != null) // Loại bỏ bản ghi null
                .ToListAsync();
        }


        public async Task<LichLamViec> GetByIdAsync(int id)
        {
            return await _context.LichLamViecs.Include(l => l.BacSi)
                                              .Include(l => l.KhungThoiGian)
                                              .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task AddAsync(LichLamViec lichLamViec)
        {
            _context.LichLamViecs.Add(lichLamViec);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LichLamViec lichLamViec)
        {
            _context.LichLamViecs.Update(lichLamViec);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var lichLamViec = await GetByIdAsync(id);
            if (lichLamViec != null)
            {
                _context.LichLamViecs.Remove(lichLamViec);
                await _context.SaveChangesAsync();
            }
        }
        //lấy lịch làm việc của bác sĩ trong 1 ngày
        public async Task<LichLamViec> GetByBacSiAndDateAsync(int bacSiId, DateTime ngayLamViec)
        {
            return await _context.LichLamViecs.Include(l => l.KhungThoiGian)
                                              .Include(l => l.BacSi)
                                              .FirstOrDefaultAsync(l => l.BacSiId == bacSiId && l.NgayLamViec == ngayLamViec);
        }

        public async Task<IEnumerable<LichLamViec>> getListLichLamViecByBacSiIdAndDate(int bacSiId, DateTime ngayLamViec)
        {
            return await _context.LichLamViecs.Include(l => l.KhungThoiGian)
                                              .Include(l => l.BacSi)
                                              .Where(l => l.BacSiId == bacSiId && l.NgayLamViec.Date == ngayLamViec.Date)
                                              .ToListAsync();

        }

        public async Task<IEnumerable<LichLamViec>> getListLichLamViecById(int id)
        {
            return await _context.LichLamViecs.Include(l => l.KhungThoiGian)
                                              .Include(l => l.BacSi)
                                              .Where(l => l.Id == id)
                                              .ToListAsync();

        }

        public async Task<List<LichLamViec>> GetByBacSiIdAsync(int bacSiId)
        {
            return await _context.LichLamViecs
                .Include(l => l.BacSi)
                .Include(l => l.KhungThoiGian)
                .Where(l => l.BacSiId == bacSiId)
                .ToListAsync();
        }

        public async Task<bool> GetLichDaTonTai(int bacSiId, DateTime ngayLamViec, int khungThoiGianId)
        {
            return await _context.LichLamViecs
                                             .AnyAsync(l => l.BacSiId == bacSiId
                                             && l.NgayLamViec == ngayLamViec
                                             && l.KhungThoiGianId == khungThoiGianId);
        }
    }
}