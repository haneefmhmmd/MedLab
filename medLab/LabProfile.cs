using Amazon;
using AutoMapper;
using AutoMapperProfile = AutoMapper.Profile;
using medLab.Models;
using Amazon.DynamoDBv2.DataModel;

namespace medLab
{
    public class LabProfile : AutoMapperProfile
    {
        public LabProfile()
        {
            // Mapping from LabsDTO to Labs
            CreateMap<LabsDTO, Labs>()
                .ForMember(dest => dest.LabId, opt => opt.MapFrom(src => src.LabId))
                .ForMember(dest => dest.LabAddress, opt => opt.MapFrom(src => src.LabAddress))
                .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.LabName))
                .ForMember(dest => dest.Reports, opt => opt.MapFrom(src => src.Reports));

            // Mapping from Labs to LabsDTO (if needed)
            CreateMap<Labs, LabsDTO>()
                .ForMember(dest => dest.LabId, opt => opt.MapFrom(src => src.LabId))
                .ForMember(dest => dest.LabAddress, opt => opt.MapFrom(src => src.LabAddress))
                .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.LabName))
                .ForMember(dest => dest.Reports, opt => opt.MapFrom(src => src.Reports));

            // Mapping for the nested Report
            CreateMap<ReportDTO, Report>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.DateOfTest, opt => opt.MapFrom(src => src.DateOfTest))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PatientName))
                .ForMember(dest => dest.Tests, opt => opt.MapFrom(src => src.Tests));

            CreateMap<Report, ReportDTO>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.DateOfTest, opt => opt.MapFrom(src => src.DateOfTest))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PatientName))
                .ForMember(dest => dest.Tests, opt => opt.MapFrom(src => src.Tests));

            // Mapping for the nested Test
            CreateMap<TestDTO, Test>()
                .ForMember(dest => dest.TestName, opt => opt.MapFrom(src => src.TestName))
                .ForMember(dest => dest.TestValue, opt => opt.MapFrom(src => src.TestValue));

            CreateMap<Test, TestDTO>()
                .ForMember(dest => dest.TestName, opt => opt.MapFrom(src => src.TestName))
                .ForMember(dest => dest.TestValue, opt => opt.MapFrom(src => src.TestValue));
        }
    }
}
