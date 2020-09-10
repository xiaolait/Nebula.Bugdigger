using AutoMapper;

namespace Nebula.Bugdigger
{
    public class BugdiggerApplicationAutoMapperProfile : Profile
    {
        public BugdiggerApplicationAutoMapperProfile()
        {
            CreateMap<Param, TestDataDto>();
        }
    }
}
