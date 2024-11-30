using Microsoft.EntityFrameworkCore;
using PhongKhamOnline.DataAccess;
using PhongKhamOnline.Models;


namespace PhongKhamOnline.Repositories
{
    public class EFBacSiRepository : IBacSiRepository
    {
        private readonly ApplicationDbContext _context;

        public EFBacSiRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BacSi>> GetAllAsync()
        {
            return await _context.BacSis.Include(p => p.ChuyenMonBacSi).ToListAsync();  // Đảm bảo có thể truy cập ChuyenMonBacSi

        }

        public async Task<IEnumerable<BacSi>> searchDoctor(string value)
        {
            IQueryable<BacSi> query = _context.BacSis.Include(p => p.ChuyenMonBacSi); // Đảm bảo có thể truy cập ChuyenMonBacSi

            if (!string.IsNullOrEmpty(value))
            {
                query = query.Where(p => p.Ten.Contains(value));
            }

            return await query.ToListAsync();
        }

        public async Task<BacSi> GetByIdAsync(int id)
        {
            return await _context.BacSis.Include(p => p.ChuyenMonBacSi)  // Đảm bảo có thể truy cập ChuyenMonBacSi
                                         .FirstOrDefaultAsync(p => p.Id == id);
        }

        // Thêm phương thức GetBySpecialtyAsync
        public async Task<IEnumerable<BacSi>> GetBySpecialtyAsync(int specialtyId)
        {
            return await _context.BacSis
                                 .Include(p => p.ChuyenMonBacSi)  // Đảm bảo có thể truy cập ChuyenMonBacSi
                                 .Where(p => p.ChuyenMonBacSiId == specialtyId) // Lọc theo chuyên môn
                                 .ToListAsync();
        }

        public async Task AddAsync(BacSi bacSi)
        {
            _context.BacSis.Add(bacSi);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BacSi bacSi)
        {
            _context.BacSis.Update(bacSi);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var bacSi = await _context.BacSis.FindAsync(id);
            if (bacSi != null)
            {
                _context.BacSis.Remove(bacSi);
                await _context.SaveChangesAsync();
            }
        }
    }
}
