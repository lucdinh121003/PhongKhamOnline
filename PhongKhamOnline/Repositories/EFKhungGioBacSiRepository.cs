using Microsoft.EntityFrameworkCore;
using PhongKhamOnline.DataAccess;
using PhongKhamOnline.Models;


namespace PhongKhamOnline.Repositories
{
    public class EFKhungGioBacSiRepository : IKhungGioBacSiRepository
    {
        private readonly ApplicationDbContext _context;

        public EFKhungGioBacSiRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<KhungGioBacSi>> GetAllAsync()
        {
            return await _context.KhungGioBacSis.Include(p => p.BacSis).ToListAsync();
        }

        public async Task<KhungGioBacSi> GetByIdAsync(int id)
        {
            return await _context.KhungGioBacSis.Include(p => p.BacSis).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(KhungGioBacSi khungGioBacSi)
        {
            _context.KhungGioBacSis.Add(khungGioBacSi);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(KhungGioBacSi khungGioBacSi)
        {
            _context.KhungGioBacSis.Update(khungGioBacSi);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var khungGioBacSi = await _context.KhungGioBacSis.FindAsync(id);
            _context.KhungGioBacSis.Remove(khungGioBacSi);
            await _context.SaveChangesAsync();
        }
    }

}
