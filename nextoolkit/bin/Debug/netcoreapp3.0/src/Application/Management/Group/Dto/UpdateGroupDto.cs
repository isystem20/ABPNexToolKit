using ;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;
using System;
using Abp.AutoMapper;

namespace ConsoleManager.NexGroup.Dto
{
	[AutoMapTo(typeof(Group))]
	public class UpdateGroupDto : CreateGroupDto, IEntityDto
	{

		public int Id { get; set; }

	}
}
