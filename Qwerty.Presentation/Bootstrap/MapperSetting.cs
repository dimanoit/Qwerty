using Qwerty.BLL.DTO;
using Qwerty.DAL.Entities;

namespace Qwerty.WebApi.Bootstrap
{
    public class MapperSetting : AutoMapper.Profile
    {
        public MapperSetting()
        {
            CreateMap<MessageDTO, Message>().ReverseMap();
            CreateMap<FriendshipRequestDTO, FriendshipRequest>().ReverseMap();
            CreateMap<FriendDTO, Friend>().ReverseMap();
        }
    }
}