using ;
using RecordStatus.cs;
using Abp.Application.Services;
using Abp.AutoMapper;
using Abp.Application.Services.Dto;
using System;

namespace ConsoleManager.NexGroup.Dto
{
	[AutoMapFrom(typeof(Group))]
	public class GroupDto : EntityDto<int>
	{


	}
}
