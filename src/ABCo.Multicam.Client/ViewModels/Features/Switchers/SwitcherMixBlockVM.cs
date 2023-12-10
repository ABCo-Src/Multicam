using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Hosting.Clients;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace ABCo.Multicam.Client.ViewModels.Features.Switcher
{
	public interface ISwitcherMixBlockVM : INotifyPropertyChanged
    {
		ISwitcherProgramInputVM[] ProgramBus { get; set; }
		ISwitcherPreviewInputVM[] PreviewBus { get; set; }
        ISwitcherCutButtonVM CutButton { get; set; }
		//ISwitcherCutButtonVM AutoButton { get; set; }
        bool ShowPreview { get; set; }
        string MainLabel { get; set; }
		void UpdateState(MixBlockState state);
	}

    public partial class SwitcherMixBlockVM : ViewModelBase, ISwitcherMixBlockVM
    {
		readonly Dispatched<ISwitcher> _switcher;

        [ObservableProperty] bool _showPreview;
        [ObservableProperty] string _mainLabel;
        [ObservableProperty] ISwitcherProgramInputVM[] _programBus = Array.Empty<ISwitcherProgramInputVM>();
        [ObservableProperty] ISwitcherPreviewInputVM[] _previewBus = Array.Empty<ISwitcherPreviewInputVM>();
        [ObservableProperty] ISwitcherCutButtonVM _cutButton;

        public SwitcherMixBlockVM(Dispatched<ISwitcher> switcher, SwitcherMixBlock mb, int mixBlockIndex)
        {
			// Initialize the program bus
			ProgramBus = new ISwitcherProgramInputVM[mb.ProgramInputs.Count];
			for (int i = 0; i < mb.ProgramInputs.Count; i++)
				ProgramBus[i] = new SwitcherProgramInputVM(switcher, mixBlockIndex, mb.ProgramInputs[i]);

			// Initialize the preview bus
			PreviewBus = new ISwitcherPreviewInputVM[mb.PreviewInputs.Count];
			for (int i = 0; i < mb.PreviewInputs.Count; i++)
				PreviewBus[i] = new SwitcherPreviewInputVM(switcher, mixBlockIndex, mb.PreviewInputs[i]);

			// Initialize the cut button
			CutButton = new SwitcherCutButtonVM(switcher, mixBlockIndex);

			// Setup additional info
			MainLabel = mb.NativeType == SwitcherMixBlockType.CutBus ? "Cut Bus" : "Program";
			ShowPreview = mb.NativeType == SwitcherMixBlockType.ProgramPreview;
		}

		public void UpdateState(MixBlockState state)
        {
			for (int i = 0; i < ProgramBus.Length; i++)
				ProgramBus[i].UpdateState(state);
			for (int i = 0; i < PreviewBus.Length; i++)
				PreviewBus[i].UpdateState(state);
		}
	}
}