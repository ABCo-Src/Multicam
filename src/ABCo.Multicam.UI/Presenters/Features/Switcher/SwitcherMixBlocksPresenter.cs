using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;

namespace ABCo.Multicam.UI.Presenters.Features.Switcher
{
	public interface ISwitcherMixBlocksPresenter : INeedsInitialization<ISwitcherFeatureVM, IFeature> 
	{
		void UpdateSpecs(SwitcherSpecs specs);
		void UpdateState(MixBlockState[] mixBlocks);
		void SetProgram(int mixBlock, int value);
		void SetPreview(int mixBlock, int value);
		void Cut(int mixBlock);
	}

	public class SwitcherMixBlocksPresenter : ISwitcherMixBlocksPresenter
	{
		IServiceSource _servSource;
		ISwitcherFeatureVM _vm = null!;
		IFeature _feature = null!;

		public SwitcherMixBlocksPresenter(IServiceSource servSource) => _servSource = servSource;

		public void FinishConstruction(ISwitcherFeatureVM vm, IFeature feature)
		{
			_vm = vm;
			_feature = feature;
		}

		public void UpdateSpecs(SwitcherSpecs specs)
		{
			// Update mix-blocks
			_vm.MixBlocks = new ISwitcherMixBlockVM[specs.MixBlocks.Count];
			for (int i = 0; i < specs.MixBlocks.Count; i++)
			{
				_vm.MixBlocks[i] = _servSource.Get<ISwitcherMixBlockVM>();
				PopulateMixBlockVM(_vm.MixBlocks[i], specs.MixBlocks[i], i);
			}
		}

		void PopulateMixBlockVM(ISwitcherMixBlockVM vm, SwitcherMixBlock mb, int mixBlockIndex)
		{
			// Initialize the program bus
			vm.ProgramBus = new ISwitcherProgramInputVM[mb.ProgramInputs.Count];
			for (int i = 0; i < vm.ProgramBus.Length; i++)
			{
				vm.ProgramBus[i] = _servSource.Get<ISwitcherProgramInputVM, ISwitcherMixBlocksPresenter, int, int>(this, mixBlockIndex, i);
				PopulateInputVM(vm.ProgramBus[i], mb.ProgramInputs[i]);
			}

			// Initialize the preview bus
			if (mb.PreviewInputs != null)
			{
				vm.PreviewBus = new ISwitcherPreviewInputVM[mb.PreviewInputs.Count];
				for (int i = 0; i < mb.PreviewInputs.Count; i++)
				{
					vm.PreviewBus[i] = _servSource.Get<ISwitcherPreviewInputVM, ISwitcherMixBlocksPresenter, int, int>(this, mixBlockIndex, i);
					PopulateInputVM(vm.PreviewBus[i], mb.PreviewInputs[i]);
				}
			}

			// Initialize the cut button
			vm.CutButton = _servSource.Get<ISwitcherCutButtonVM, ISwitcherMixBlocksPresenter, int>(this, mixBlockIndex);
			vm.CutButton.Text = "Cut";

			// Setup additional info
			vm.MainLabel = mb.NativeType == SwitcherMixBlockType.CutBus ? "Cut Bus" : "Program";
			vm.ShowPreview = mb.NativeType == SwitcherMixBlockType.ProgramPreview;
		}

		void PopulateInputVM(ISwitcherBusInputVM vm, SwitcherBusInput input) => vm.Text = input.Name;

		public void UpdateState(MixBlockState[] mixBlocks)
		{
			for (int i = 0; i < mixBlocks.Length; i++)
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
				vm.ProgramBus[i].Status = enabled ? SwitcherButtonStatus.PreviewActive : SwitcherButtonStatus.NeutralInactive;
			}
		}

		public void SetProgram(int mixBlock, int value) => _feature.PerformAction((int)SwitcherFeatureActionID.SetProgram, mixBlock, value);
		public void SetPreview(int mixBlock, int value) => _feature.PerformAction((int)SwitcherFeatureActionID.SetPreview, mixBlock, value);
		public void Cut(int mixBlock) => _feature.PerformAction((int)SwitcherFeatureActionID.Cut, mixBlock);
	}
}
