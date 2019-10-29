using Abp.Application.Services;
using ConsoleManager.NexGroup.Dto;

namespace ConsoleManager.NexGroup
{
	public interface IGroupAppService : IAsyncCrudAppService<GroupDto, int, PagedGroupResultRequestDto, CreateGroupDto, UpdateGroupDto>
	{

		//Put something if applicable

	}
}
