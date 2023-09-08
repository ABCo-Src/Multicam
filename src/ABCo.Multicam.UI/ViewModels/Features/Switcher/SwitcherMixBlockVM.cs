using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
	public interface ISwitcherMixBlockVM
    {
		ISwitcherProgramInputVM[] ProgramBus { get; set; }
		ISwitcherPreviewInputVM[] PreviewBus { get; set; }
        ISwitcherCutButtonVM CutButton { get; set; }
		//ISwitcherCutButtonVM AutoButton { get; set; }
        bool ShowPreview { get; set; }
        string MainLabel { get; set; }
    }

    public partial class SwitcherMixBlockVM : ViewModelBase, ISwitcherMixBlockVM
    {
        [ObservableProperty] bool _showPreview;
        [ObservableProperty] string _mainLabel;
        [ObservableProperty] ISwitcherProgramInputVM[] _programBus = Array.Empty<ISwitcherProgramInputVM>();
        [ObservableProperty] ISwitcherPreviewInputVM[] _previewBus = Array.Empty<ISwitcherPreviewInputVM>();
        [ObservableProperty] ISwitcherCutButtonVM _cutButton;
	}
}