namespace UserGuard_API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Employee, EmployeeDetailsDto>()
           .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<ApplicationUser, UserDto>()
                        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));

            CreateMap<Claim, ClaimDto>();
        }
    }
}
