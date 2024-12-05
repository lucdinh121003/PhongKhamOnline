using Microsoft.EntityFrameworkCore;
using PhongKhamOnline.DataAccess;
using PhongKhamOnline.Models;

namespace PhongKhamOnline.Repositories
{
    public class EFKhungThoiGianRepository : IKhungThoiGianRepository
    {
        private readonly ApplicationDbContext _context;

        public EFKhungThoiGianRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<KhungThoiGian>> GetAllAsync()
        {
            return await _context.KhungThoiGians.ToListAsync();
        }
        public async Task<KhungThoiGian> GetById(int id)
        {
            var khungThoiGians = await _context.KhungThoiGians.FindAsync(id);
            return khungThoiGians;
        }
    }
}