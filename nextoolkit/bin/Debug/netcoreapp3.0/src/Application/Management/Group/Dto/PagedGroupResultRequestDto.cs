using RecordStatus.cs;
using Abp.Application.Services.Dto;

namespace ConsoleManager.NexGroup.Dto
{
	public class PagedGroupResultRequestDto : PagedResultRequestDto
	{

		public string Keyword { get; set; }
		public RecordStatus? Status { get; set; }

	}
}
