using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Qwerty.BLL.DTO;
using Qwerty.DAL.Entities;

namespace Qwerty.Settings
{
    public class MapperSetting : AutoMapper.Profile
    {
        public MapperSetting()
        {
            CreateMap<MessageDTO, Message>().ReverseMap();
            CreateMap<FriendshipRequestDTO, FriendshipRequest>().ReverseMap();
        } 
    }
}
