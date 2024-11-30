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
            return await _context.LichLamViecs.Include(l => l.BacSi).ToListAsync();
        }

        public async Task<LichLamViec> GetByIdAsync(int id)
        {
            return await _context.LichLamViecs.Include(l => l.BacSi).FirstOrDefaultAsync(l => l.Id == id);
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
        public async Task<IEnumerable<LichLamViec>> GetByBacSiAndDateAsync(int bacSiId, DateTime ngayLamViec)
        {
            return await _context.LichLamViecs
                .Where(llv => llv.BacSiId == bacSiId && llv.NgayLamViec.Date == ngayLamViec.Date)
                .ToListAsync();
        }


    }
}
