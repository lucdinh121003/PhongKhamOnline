using Microsoft.EntityFrameworkCore;
using PhongKhamOnline.DataAccess;
using PhongKhamOnline.Models;

namespace PhongKhamOnline.Repositories
{
    public class EFChuyenMonBacSiRepository : IChuyenMonBacSiRepository
    {
        private readonly ApplicationDbContext _context;

        public EFChuyenMonBacSiRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChuyenMonBacSi>> GetAllAsync()
        {
            return await _context.ChuyenMonBacSi.Include(p => p.BacSis).ToListAsync();
        }


        public async Task<ChuyenMonBacSi> GetByIdAsync(int id)
        {
            return await _context.ChuyenMonBacSi.Include(p => p.BacSis).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(ChuyenMonBacSi chuyenMonBacSi)
        {
            _context.ChuyenMonBacSi.Add(chuyenMonBacSi);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ChuyenMonBacSi chuyenMonBacSi)
        {
            _context.ChuyenMonBacSi.Update(chuyenMonBacSi);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var chuyenMonBacSi = await _context.ChuyenMonBacSi.FindAsync(id);
            _context.ChuyenMonBacSi.Remove(chuyenMonBacSi);
            await _context.SaveChangesAsync();
        }
    }
}
