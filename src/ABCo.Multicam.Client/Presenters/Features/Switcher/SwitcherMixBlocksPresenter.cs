using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Client.Enumerations;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Client.Presenters.Features.Switcher
{
    public interface ISwitcherMixBlocksPresenter : IClientService<ISwitcherFeatureVM, IServerTarget> 
	{
		void OnSpecced(SwitcherSpecs specs);
		void OnState(MixBlockState[] mixBlocks);
		void SetProgram(int mixBlock, int value);
		void SetPreview(int mixBlock, int value);
		void Cut(int mixBlock);
	}

	public class SwitcherMixBlocksPresenter : ISwitcherMixBlocksPresenter
	{
		readonly IClientInfo _servSource;
		readonly ISwitcherFeatureVM _vm;
		readonly IServerTarget _feature;

		public SwitcherMixBlocksPresenter(ISwitcherFeatureVM vm, IServerTarget feature, IClientInfo servSource)
		{
			_vm = vm;
			_feature = feature;
			_servSource = servSource;
		}

		public void OnSpecced(SwitcherSpecs specs)
		{
			// Update mix-blocks
			var newMixBlocks = new ISwitcherMixBlockVM[specs.MixBlocks.Count];
			for (int i = 0; i < newMixBlocks.Length; i++)
			{
				newMixBlocks[i] = _servSource.Get<ISwitcherMixBlockVM>();
				PopulateMixBlockVM(newMixBlocks[i], specs.MixBlocks[i], i);
			}
			_vm.MixBlocks = newMixBlocks;

			// Update mix block states
			OnState(_feature.DataStore.GetData<SwitcherState>().Data);
		}

		void PopulateMixBlockVM(ISwitcherMixBlockVM vm, SwitcherMixBlock mb, int mixBlockIndex)
		{
			// Initialize the program bus
			vm.ProgramBus = new ISwitcherProgramInputVM[mb.ProgramInputs.Count];
			for (int i = 0; i < mb.ProgramInputs.Count; i++)
			{
				vm.ProgramBus[i] = _servSource.Get<ISwitcherProgramInputVM, ISwitcherMixBlocksPresenter, int, int>(this, mixBlockIndex, mb.ProgramInputs[i].Id);
				PopulateInputVM(vm.ProgramBus[i], mb.ProgramInputs[i]);
			}

			// Initialize the preview bus
			vm.PreviewBus = new ISwitcherPreviewInputVM[mb.PreviewInputs.Count];
			for (int i = 0; i < mb.PreviewInputs.Count; i++)
			{
				vm.PreviewBus[i] = _servSource.Get<ISwitcherPreviewInputVM, ISwitcherMixBlocksPresenter, int, int>(this, mixBlockIndex, mb.PreviewInputs[i].Id);
				PopulateInputVM(vm.PreviewBus[i], mb.PreviewInputs[i]);
			}

			// Initialize the cut button
			vm.CutButton = _servSource.Get<ISwitcherCutButtonVM, ISwitcherMixBlocksPresenter, int>(this, mixBlockIndex);
			vm.CutButton.Text = "Cut";

			// Setup additional info
			vm.MainLabel = mb.NativeType == SwitcherMixBlockType.CutBus ? "Cut Bus" : "Program";
			vm.ShowPreview = mb.NativeType == SwitcherMixBlockType.ProgramPreview;
		}

		void PopulateInputVM(ISwitcherBusInputVM vm, SwitcherBusInput input) => vm.Text = input.Name;

		public void OnState(MixBlockState[] mixBlocks)
		{
			// Update as far as our current cache allows
			int min = Math.Min(mixBlocks.Length, _vm.MixBlocks.Length);
			for (int i = 0; i < min; i++)
				UpdateMixBlockState(_vm.MixBlocks[i], mixBlocks[i]);
		}

		void UpdateMixBlockState(ISwitcherMixBlockVM vm, MixBlockState newState)
		{
			for (int i = 0; i < vm.ProgramBus.Length; i++)
			{
				bool enabled = newState.Prog == vm.ProgramBus[i].BusId;
				vm.ProgramBus[i].Status = enabled ? SwitcherButtonStatus.ProgramActive : SwitcherButtonStatus.NeutralInactive;
			}

			for (int i = 0; i < vm.PreviewBus.Length; i++)
			{
				bool enabled = newState.Prev == vm.PreviewBus[i].BusId;
				vm.PreviewBus[i].Status = enabled ? SwitcherButtonStatus.PreviewActive : SwitcherButtonStatus.NeutralInactive;
			}
		}

		public void SetProgram(int mixBlock, int value) => _feature.PerformAction(SwitcherLiveFeature.SET_PROGRAM, new BusChangeInfo(mixBlock, value));
		public void SetPreview(int mixBlock, int value) => _feature.PerformAction(SwitcherLiveFeature.SET_PREVIEW, new BusChangeInfo(mixBlock, value));
		public void Cut(int mixBlock) => _feature.PerformAction(SwitcherLiveFeature.CUT, mixBlock);
	}
}
