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

            CreateMap<Branch, BranchDetailsDTO>()
            .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager.User.FirstName)); // ربط المدير
            CreateMap<Employee, EmployeeGenralDto>();
            CreateMap<Course, CourseDto>();

            CreateMap<Branch, BranchListDto>()
             .ForMember(dest => dest.ManagerName,
                opt => opt.MapFrom(src => src.Manager != null && src.Manager.User != null
                ? src.Manager.User.FirstName
                 : null));

            CreateMap<UpdateBranchDto, Branch>()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CreateBranchDto, Branch>();

        }
    }
}
