using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementSystem.DTO.Task;
using TaskManagementSystem.Models;
using TaskStatus = TaskManagementSystem.Models.TaskStatus;



namespace TaskManagementSystem.AutoMapping
{
    internal class MappingProfile:Profile
    {

        public MappingProfile()
        {
            //// CreateMap<UserTask, TaskDto>()
            //   .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.FullName))
            //  .ForMember(dest => dest.AssignedToName, opt => opt.MapFrom(src => src.AssignedTo.FullName));
            /* CreateMap<TaskCreateDto, UserTask>()
            .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatedById))
            .ForMember(dest => dest.AssignedToId, opt => opt.MapFrom(src => src.AssignedToId))
            //.ForMember(dest => dest.Status, opt => opt.MapFrom(src => TaskStatus.NotStarted)) // Default to NotStarted
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => false)); // Default IsCompleted to false
             CreateMap<TaskDto, UserTask>();

             */


            CreateMap<UserTask, TaskDto>()
              .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.FullName))
              .ForMember(dest => dest.AssignedToName, opt => opt.MapFrom(src => src.AssignedTo.FullName));

            // Create DTO to Entity mapping
            CreateMap<TaskCreateDto, UserTask>()
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()) // Will be set manually
                .ForMember(dest => dest.AssignedTo, opt => opt.Ignore()); // Will be set manually
                 // .ForMember(dest => dest.Status, opt => opt.MapFrom(src => TaskStatus.NotStarted));


            CreateMap<TaskUpdateDto, UserTask>()
            .ForMember(dest => dest.AssignedTo, opt => opt.Ignore()) // Manual setting for navigation properties
            .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()); // If you have "CreatedBy" in UserTask// Default status
        }
    }
}
