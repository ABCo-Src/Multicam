using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Client.Enumerations;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Client.Presenters.Features.Switcher
{
	public interface ISwitcherMixBlocksPresenter : IClientService<ISwitcherFeatureVM, Dispatched<ISwitcherFeature>> 
	{
		void Refresh(SpecsSpecificInfo info);
		void SetProgram(int mixBlock, int value);
		void SetPreview(int mixBlock, int value);
		void Cut(int mixBlock);
	}

	public class SwitcherMixBlocksPresenter : ISwitcherMixBlocksPresenter
	{
		readonly IClientInfo _servSource;
		readonly ISwitcherFeatureVM _vm;
		readonly Dispatched<ISwitcherFeature> _feature;
		SwitcherSpecs? _lastSeenSpecs = null;

		public SwitcherMixBlocksPresenter(ISwitcherFeatureVM vm, Dispatched<ISwitcherFeature> feature, IClientInfo servSource)
		{
			_vm = vm;
			_feature = feature;
			_servSource = servSource;
		}

		public void Refresh(SpecsSpecificInfo specs)
		{
			// If the specs have changed from what we remember, recreate everything
			if (_lastSeenSpecs != specs.Specs)
			{
				_lastSeenSpecs = specs.Specs;

				var newMixBlocks = new ISwitcherMixBlockVM[_lastSeenSpecs.MixBlocks.Count];
				for (int i = 0; i < newMixBlocks.Length; i++)
				{
					newMixBlocks[i] = _servSource.Get<ISwitcherMixBlockVM>();
					PopulateMixBlockVM(newMixBlocks[i], _lastSeenSpecs.MixBlocks[i], i);
				}
				_vm.MixBlocks = newMixBlocks;
			}

			// Update the state
			for (int i = 0; i < _vm.MixBlocks.Length; i++)
				UpdateMixBlockState(_vm.MixBlocks[i], specs.State[i]);
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

		public void SetProgram(int mixBlock, int value) => _feature.CallDispatched(f => f.SetProgram(mixBlock, value));
		public void SetPreview(int mixBlock, int value) => _feature.CallDispatched(f => f.SetPreview(mixBlock, value));
		public void Cut(int mixBlock) => _feature.CallDispatched(f => f.Cut(mixBlock));
	}
}
