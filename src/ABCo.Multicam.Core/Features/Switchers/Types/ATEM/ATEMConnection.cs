﻿using ABCo.Multicam.Core.General;
using BMDSwitcherAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ABCo.Multicam.Core.Features.Switchers.Types.ATEM.ATEMSwitcher;

namespace ABCo.Multicam.Core.Features.Switchers.Types.ATEM
{
    public interface IATEMConnection : IDisposable, INeedsInitialization<IATEMConnectionEventHandler>
	{
		SwitcherSpecs InvalidateCurrentSpecs();
		void GetProgram(int mixBlock, out long val);
		void GetPreview(int mixBlock, out long val);
		void SendProgram(int mixBlock, long val);
		void SendPreview(int mixBlock, long val);
	}

	public interface IATEMConnectionEventHandler
	{
		void OnATEMDisconnect();
		void OnATEMProgramChange(int mixBlock);
		void OnATEMPreviewChange(int mixBlock);
	}

	public class ATEMConnection : IATEMConnection
	{
		IServiceSource _servSource;

		// Interaction layers:
		IATEMRawAPI _api = null!;
		IATEMCallbackHandler _callbackHandler = null!;

		IBMDSwitcher _nativeSwitcher = null!;
		IBMDSwitcherMixEffectBlock[] _nativeBlocks = Array.Empty<IBMDSwitcherMixEffectBlock>();

		public ATEMConnection(IServiceSource servSource)
		{
			_servSource = servSource;
			_api = servSource.Get<IATEMRawAPI>();
		}

		public void FinishConstruction(IATEMConnectionEventHandler eventHandler)
		{
			_nativeSwitcher = _api.Connect("");
			_callbackHandler = _servSource.Get<IATEMCallbackHandler, IATEMConnectionEventHandler>(eventHandler);
			_callbackHandler.AttachToSwitcher(_nativeSwitcher);
		}

		public void GetProgram(int mixBlock, out long val) => _nativeBlocks[mixBlock].GetProgramInput(out val);
		public void GetPreview(int mixBlock, out long val) => _nativeBlocks[mixBlock].GetPreviewInput(out val);
		public void SendProgram(int mixBlock, long val) => _nativeBlocks[mixBlock].SetProgramInput(val);
		public void SendPreview(int mixBlock, long val) => _nativeBlocks[mixBlock].SetPreviewInput(val);

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

		SwitcherSpecs CreateSpecs(IList<IBMDSwitcherMixEffectBlock> rawMixBlocks, IList<RawInputData> rawInputs)
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

			var iter = _api.CreateInputIterator(_nativeSwitcher!);
			while (_api.MoveNext(iter, out var input))
			{
				// Only count inputs that are actually assigned to a mix block
				var availability = _api.GetAvailability(input);
				if ((availability & _BMDSwitcherInputAvailability.bmdSwitcherInputAvailabilityInputCut) == 0) continue;

				res.Add(new RawInputData(
					_api.GetID(input),
					_api.GetShortName(input),
					(byte)(availability & (
						_BMDSwitcherInputAvailability.bmdSwitcherInputAvailabilityMixEffectBlock0 |
						_BMDSwitcherInputAvailability.bmdSwitcherInputAvailabilityMixEffectBlock1 |
						_BMDSwitcherInputAvailability.bmdSwitcherInputAvailabilityMixEffectBlock2 |
						_BMDSwitcherInputAvailability.bmdSwitcherInputAvailabilityMixEffectBlock3
					))
				));

				_api.Free(input);
			}

			_api.Free(iter);
			return res;
		}

		IList<IBMDSwitcherMixEffectBlock> GetRawMixBlocks()
		{
			var res = new List<IBMDSwitcherMixEffectBlock>();
			var iter = _api.CreateMixBlockIterator(_nativeSwitcher!);

			while (_api.MoveNext(iter, out var input))
				res.Add(input);

			_api.Free(iter);

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
				_api.Free(_nativeBlocks[i]);
		}

		public void Dispose()
		{
			DisposeNativeBlocks();
			_callbackHandler.DetachFromSwitcher(_nativeSwitcher);
			_api.Free(_nativeSwitcher);
		}
	}
}