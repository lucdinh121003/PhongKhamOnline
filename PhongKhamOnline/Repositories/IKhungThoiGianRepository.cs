﻿using PhongKhamOnline.Models;

namespace PhongKhamOnline.Repositories
{
    public interface IKhungThoiGianRepository
    {
        Task<IEnumerable<KhungThoiGian>> GetAllAsync();

    }

}

