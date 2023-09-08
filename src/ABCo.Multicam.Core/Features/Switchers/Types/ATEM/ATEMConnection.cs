using ABCo.Multicam.Core.Features.Switchers.Types.ATEM.Native;
using BMDSwitcherAPI;
using static ABCo.Multicam.Core.Features.Switchers.Types.ATEM.ATEMSwitcher;

namespace ABCo.Multicam.Core.Features.Switchers.Types.ATEM
{
	public interface IATEMConnection : IDisposable, INeedsInitialization<ISwitcher>
	{
		SwitcherSpecs InvalidateCurrentSpecs();
		long GetProgram(int mixBlock);
		long GetPreview(int mixBlock);
		void SendProgram(int mixBlock, long val);
		void SendPreview(int mixBlock, long val);
		void Cut(int mixBlock);
	}

	public class ATEMConnection : IATEMConnection
	{
		IServiceSource _servSource;
		IATEMCallbackHandler _callbackHandler = null!;

		INativeATEMSwitcher _nativeSwitcher = null!;
		INativeATEMMixBlock[] _nativeBlocks = Array.Empty<INativeATEMMixBlock>();

		public ATEMConnection(IServiceSource servSource)
		{
			_servSource = servSource;
		}

		public void FinishConstruction(ISwitcher eventHandler)
		{
			_nativeSwitcher = _servSource.Get<INativeATEMSwitcherDiscovery>().Connect("");
			_callbackHandler = _servSource.Get<IATEMCallbackHandler, ISwitcher>(eventHandler);
			_callbackHandler.AttachToSwitcher(_nativeSwitcher);
		}

		public long GetProgram(int mixBlock) => _nativeBlocks[mixBlock].GetProgramInput();
		public long GetPreview(int mixBlock) => _nativeBlocks[mixBlock].GetPreviewInput();
		public void SendProgram(int mixBlock, long val) => _nativeBlocks[mixBlock].SetProgramInput(val);
		public void SendPreview(int mixBlock, long val) => _nativeBlocks[mixBlock].SetPreviewInput(val);
		public void Cut(int mixBlock) => _nativeBlocks[mixBlock].Cut();

		public SwitcherSpecs InvalidateCurrentSpecs()
		{
			// Get raw data
			var rawInputs = GetRawInputData();
			var rawMixBlocks = GetRawMixBlocks();

			// Update internal structure
			DisposeNativeBlocks();
			_nativeBlocks = rawMixBlocks.ToArray();			
			_callbackHandler.AttachMixBlocks(_nativeBlocks);

			// Update specs
			return CreateSpecs(rawMixBlocks, rawInputs);
		}

		SwitcherSpecs CreateSpecs(IList<INativeATEMMixBlock> rawMixBlocks, IList<RawInputData> rawInputs)
		{
			SwitcherMixBlock[] mixBlockSpecs = new SwitcherMixBlock[rawMixBlocks.Count];

			for (int i = 0; i < rawMixBlocks.Count; i++)
			{
				List<SwitcherBusInput> inputs = new();
				for (int j = 0; j < rawInputs.Count; j++)
				{
					// Verify this input supports this mix block
					int currentMixBlockMask = 1 << i;
					int mixBlockMaskOnInput = rawInputs[j].MixBlockMask & currentMixBlockMask;
					if (mixBlockMaskOnInput == 0) continue;

					inputs.Add(new((int)rawInputs[j].Id, rawInputs[j].Name));
				}

				mixBlockSpecs[i] = SwitcherMixBlock.NewProgPrevSameInputs(GetFeatures(), inputs.ToArray());
			}

			return new SwitcherSpecs(true, mixBlockSpecs);
		}

		IList<RawInputData> GetRawInputData()
		{
			var res = new List<RawInputData>();

			using var iter = _nativeSwitcher.CreateInputIterator();
			while (iter.MoveNext(out var input))
			{
				// Only count inputs that are actually assigned to a mix block
				var availability = input.GetAvailability();
				if ((availability & _BMDSwitcherInputAvailability.bmdSwitcherInputAvailabilityInputCut) == 0) continue;

				res.Add(new RawInputData(
					input.GetID(),
					input.GetShortName(),
					(byte)(availability & (
						_BMDSwitcherInputAvailability.bmdSwitcherInputAvailabilityMixEffectBlock0 |
						_BMDSwitcherInputAvailability.bmdSwitcherInputAvailabilityMixEffectBlock1 |
						_BMDSwitcherInputAvailability.bmdSwitcherInputAvailabilityMixEffectBlock2 |
						_BMDSwitcherInputAvailability.bmdSwitcherInputAvailabilityMixEffectBlock3
					))
				));

				input.Dispose();
			}

			return res;
		}

		IList<INativeATEMMixBlock> GetRawMixBlocks()
		{
			var res = new List<INativeATEMMixBlock>();

			using var iter = _nativeSwitcher.CreateMixBlockIterator();
			while (iter.MoveNext(out var input))
				res.Add(input);

			return res;
		}

		static SwitcherMixBlockFeatures GetFeatures() => new()
		{
			SupportsDirectProgramModification = true,
			SupportsDirectPreviewAccess = true,
			SupportsCutAction = true,
			SupportsAutoAction = false,
			SupportsCutBusCutMode = false,
			SupportsCutBusAutoMode = false,
			SupportsCutBusModeChanging = false,
			SupportsCutBusSwitching = false
		};

		void DisposeNativeBlocks()
		{
			_callbackHandler.DetachMixBlocks(_nativeBlocks);
			for (int i = 0; i < _nativeBlocks.Length; i++)
				_nativeBlocks[i].Dispose();
		}

		public void Dispose()
		{
			DisposeNativeBlocks();
			_callbackHandler.DetachFromSwitcher(_nativeSwitcher);
			_nativeSwitcher.Dispose();
		}
	}
}
