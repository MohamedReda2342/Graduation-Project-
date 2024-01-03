namespace WebApi.Helpers
{
    using AutoMapper;
    using WebApi.Entities;
    using WebApi.Models.Users;
    using WebApi.Models;
    using static WebApi.Models.PatientUpdateRequest;

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //---------------------------------------------- User ----------------------------------------------

            // User -> AuthenticateResponse
            CreateMap<User, AuthenticateResponse>();

            // RegisterRequest -> User
            CreateMap<RegisterRequest, User>();

            // UpdateRequest -> User
            CreateMap<UpdateRequest, User>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        return true;
                    }
                ));
           // CreateMap<Patient, PatientResponse>()
           //.ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => $"elderpeopleband.azurewebsites.net/patients/{src.PatientId}/{src.Photo}"));

            //---------------------------------------------- Patient ----------------------------------------------

            // PatientAddRequest -> Patient
            CreateMap<PatientAddRequest, Patient>();
            // PatientUpdateRequest -> Patient
            CreateMap<PatientUpdateRequest, Patient>();

            CreateMap<Patient, PatientResponse>();
            //---------------------------------------------- Health & Location ----------------------------------------------


            CreateMap<BandData, Patient>();


            //---------------------------------------------- Medicine ----------------------------------------------

            // MedicineAddRequest -> Medicine
            CreateMap<MedicineAddRequest, Medicine>();
            // MedicineUpdateRequest -> Medicine
            CreateMap<MedicineUpdateRequest, Medicine>();

            CreateMap<Medicine, MedicineResponse>();



        }
    }
}
