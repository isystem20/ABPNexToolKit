using ;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using ConsoleManager.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Extensions;
using ConsoleManager.NexGroup.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Linq.Extensions;

namespace ConsoleManager.NexGroup
{
	//[AbpAuthorize("AdminGroup")]
	public class GroupAppService : AsyncCrudAppService<Group, GroupDto, int, PagedGroupResultRequestDto, CreateGroupDto, UpdateGroupDto>, IGroupAppService
	{

		private readonly IRepository<Group> _rGroup;

		public GroupAppService(IRepository<Group> rGroup) : base(rGroup)
		{
			_rGroup = rGroup;
		}

		[AbpAuthorize("AdminGroup.Read")]
		protected override IQueryable<Group> CreateFilteredQuery(PagedGroupResultRequestDto input)
		{
			return Repository.GetAll()
				.WhereIf(!input.Keyword.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Keyword))
				.WhereIf(input.Status.HasValue, x => x.Status == input.Status);
		}

		[AbpAuthorize("AdminGroup.Create")]
		public override async Task<GroupDto> Create(CreateGroupDto input)
		{
			var entity = MapToEntity(input);

			await Repository.InsertAsync(entity);
			await CurrentUnitOfWork.SaveChangesAsync();

			return MapToEntityDto(entity);
		}

		[AbpAuthorize("AdminGroup.Update")]
		public override async Task<GroupDto> Update(UpdateGroupDto input)
		{
			var entity = await GetEntityByIdAsync(input.Id);

			MapToEntity(input, entity);
			await CurrentUnitOfWork.SaveChangesAsync();

			return MapToEntityDto(entity);
		}
		[AbpAuthorize("AdminGroup.Delete")]
		public override async Task Delete(EntityDto<int> input)
		{
			var record = await _rGroup.FirstOrDefaultAsync(input.Id);

			await _rGroup.DeleteAsync(record);
		}
		[AbpAuthorize("AdminGroup.Read")]
		protected override async Task<Group> GetEntityByIdAsync(int id)
		{
			return await Repository.GetAsync(id);
		}

	}
}
