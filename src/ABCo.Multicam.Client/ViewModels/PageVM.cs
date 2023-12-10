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
}
