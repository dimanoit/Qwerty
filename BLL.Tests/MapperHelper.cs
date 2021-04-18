using System;
using AutoMapper;
using Qwerty.BLL.DTO;
using Qwerty.DAL.Entities;

namespace BLL.Tests
{
    [Obsolete]
    public static class MapperHelper
    {
        private static readonly object ThisLock = new object();

        public static void Initialize()
        {
            lock (ThisLock)
            {
                Mapper.Reset();
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Friend, FriendDTO>().ReverseMap();
                    cfg.CreateMap<FriendshipRequest, FriendshipRequestDTO>().ReverseMap();
                });
            }
        }
    }
}