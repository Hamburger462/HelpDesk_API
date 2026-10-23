using AutoMapper;
using HelpDesk.Data.Dto;
using HelpDesk.Data.Models;

namespace HelpDesk.Mapping
{
    public class MainMappingProfile: Profile
    {
        public MainMappingProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<Ticket, TicketDto>();
            CreateMap<Attachment, AttachmentDto>();
            CreateMap<Message, MessageDto>();
        }
    }
}