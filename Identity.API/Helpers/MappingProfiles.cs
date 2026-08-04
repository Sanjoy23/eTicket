using AutoMapper;
using Identity.API.DTOs;
using Identity.API.Models;

namespace Identity.API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Address, AddressDto>().ReverseMap();
        }
    }
}
