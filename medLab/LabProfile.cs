﻿using Amazon;
using AutoMapper;
using AutoMapperProfile = AutoMapper.Profile;
using medLab.Models;
using Amazon.DynamoDBv2.DataModel;
using medLab.Models.DTOs;

namespace medLab
{
    public class LabProfile : AutoMapperProfile
    {
        public LabProfile()
        {
            // Mapping for Labs <-> LabsDTO
            CreateMap<LabsDTO, Labs>()
                .ForMember(dest => dest.LabId, opt => opt.MapFrom(src => src.LabId))
                .ForMember(dest => dest.LabEmail, opt => opt.MapFrom(src => src.LabEmail))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.LabAddress, opt => opt.MapFrom(src => src.LabAddress))
                .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.LabName))
                .ForMember(dest => dest.Reports, opt => opt.MapFrom(src => src.Reports))
                .ReverseMap();

            // Mapping for Report <-> ReportDTO
            CreateMap<ReportDTO, Report>()
                .ForMember(dest => dest.ReportId, opt => opt.MapFrom(src => src.ReportId))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.DateOfTest, opt => opt.MapFrom(src => src.DateOfTest))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PatientName))
                .ForMember(dest => dest.Tests, opt => opt.MapFrom(src => src.Tests))
                .ReverseMap();

            // Mapping for Test <-> TestDTO
            CreateMap<TestDTO, Test>()
                .ForMember(dest => dest.TestName, opt => opt.MapFrom(src => src.TestName))
                .ForMember(dest => dest.TestValue, opt => opt.MapFrom(src => src.TestValue))
                .ReverseMap();

            // Mapping for Test <-> TestDTO within LabTests model
            CreateMap<LabTestDTO, LabTest>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => src.Unit))
                .ForMember(dest => dest.ReferenceValue, opt => opt.MapFrom(src => src.ReferenceValue))
                .ReverseMap();

            CreateMap<LabTests, LabTestsDTO>()
                .ForMember(dest => dest.LabId, opt => opt.MapFrom(src => src.LabId))
                .ForMember(dest => dest.Tests, opt => opt.MapFrom(src => src.Tests))
                .ReverseMap();


            CreateMap<RegistrationDTO, Labs>()
                .ForMember(dest => dest.LabId, opt => opt.MapFrom(src => src.LabId))
                .ForMember(dest => dest.LabEmail, opt => opt.MapFrom(src => src.LabEmail))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.LabAddress, opt => opt.MapFrom(src => src.LabAddress))
                .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.LabName))
                .ReverseMap();

            // New mapping for PatientDetailsDTO <-> Report (patient details in the report)
            CreateMap<PatientDetailsDTO, Report>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PatientName))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.DateOfTest, opt => opt.MapFrom(src => src.DateOfTest))
                .ReverseMap();
        }
    }
}
