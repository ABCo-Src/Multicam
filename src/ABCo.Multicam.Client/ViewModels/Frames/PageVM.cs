using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.ViewModels.Frames
{
	public enum AppPages
	{
		Unknown,
		Switchers,
		Tally,
		CutRecorder,
		Hosting,
	}

	public interface IPageVM
	{
		AppPages Page { get; }
	}

	public class UnimplementedPageVM : IPageVM
	{
		public AppPages Page => AppPages.Unknown;
	}
}
