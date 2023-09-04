using ABCo.Multicam.Core.Features.Switchers.Types.ATEM.Native;
using BMDSwitcherAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Types.ATEM.Windows
{
	[SupportedOSPlatform("windows")]
	public class WindowsNativeATEMSwitcherDiscovery : INativeATEMSwitcherDiscovery
	{
		public INativeATEMSwitcher Connect(string address)
		{
			var discovery = new CBMDSwitcherDiscovery();
			_BMDSwitcherConnectToFailure failure = 0;

			try
			{
				discovery.ConnectTo(address, out IBMDSwitcher switcher, out failure);
				Marshal.ReleaseComObject(discovery);
				return new WindowsNativeATEMSwitcher(switcher);
			}
			catch (COMException ex)
			{
				throw failure switch
				{
					_BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureNoResponse => new SwitcherErrorException("No response from switcher."),
					_BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureIncompatibleFirmware => new SwitcherErrorException("Requires updated switcher SDK software."),
					_BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureCorruptData => new SwitcherErrorException("Corrupt data transmitted."),
					_BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureDeprecatedAfter_v7_3 => new SwitcherErrorException("Switcher not supported after SDK v7.3."),
					_BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureStateSync => new SwitcherErrorException("State synchronisation failed."),
					_BMDSwitcherConnectToFailure.bmdSwitcherConnectToFailureStateSyncTimedOut => new SwitcherErrorException("State synchronisation timed out."),
					_ => ex
				};
			}
		}
	}

	[SupportedOSPlatform("windows")]
	public class WindowsNativeATEMSwitcher : INativeATEMSwitcher, IBMDSwitcherCallback
	{
		IBMDSwitcher _comSwitcher;
		INativeATEMSwitcherCallbackHandler _callbackHandler = null!;

		public WindowsNativeATEMSwitcher(IBMDSwitcher comInterface) => _comSwitcher = comInterface;

		public INativeATEMBlockIterator CreateMixBlockIterator()
		{
			// Get iterator as IUnknown
			_comSwitcher.CreateIterator(typeof(IBMDSwitcherMixEffectBlockIterator).GUID, out IntPtr iterBase);

			// Cast to real object
			var iter = (IBMDSwitcherMixEffectBlockIterator)Marshal.GetObjectForIUnknown(iterBase);
			Marshal.Release(iterBase);

			return new WindowsNativeATEMBlockIterator(iter);
		}

		public INativeATEMInputIterator CreateInputIterator()
		{
			// Get iterator as IUnknown
			_comSwitcher.CreateIterator(typeof(IBMDSwitcherInputIterator).GUID, out IntPtr iterBase);

			// Cast to real object
			var iter = (IBMDSwitcherInputIterator)Marshal.GetObjectForIUnknown(iterBase);
			Marshal.Release(iterBase);

			return new WindowsNativeATEMInputIterator(iter);
		}

		public void AddCallback(INativeATEMSwitcherCallbackHandler callback)
		{
			_callbackHandler = callback;
			_comSwitcher.AddCallback(this);
		}

		public void ClearCallback() => _comSwitcher.RemoveCallback(this);

		// Native callback:
		public void Notify(_BMDSwitcherEventType eventType, _BMDSwitcherVideoMode coreVideoMode) => _callbackHandler.Notify(eventType, coreVideoMode);

		public void Dispose() => Marshal.ReleaseComObject(_comSwitcher);
	}

	[SupportedOSPlatform("windows")]
	public class WindowsNativeATEMBlockIterator : INativeATEMBlockIterator
	{
		IBMDSwitcherMixEffectBlockIterator _comIterator;
		public WindowsNativeATEMBlockIterator(IBMDSwitcherMixEffectBlockIterator comInterface) => _comIterator = comInterface;

		public bool MoveNext(out INativeATEMMixBlock item)
		{
			_comIterator.Next(out var comItem);
			item = new WindowsNativeATEMBlock(comItem);
			return comItem != null;
		}

		public void Dispose() => Marshal.ReleaseComObject(_comIterator);
	}

	[SupportedOSPlatform("windows")]
	public class WindowsNativeATEMInputIterator : INativeATEMInputIterator
	{
		IBMDSwitcherInputIterator _comIterator;
		public WindowsNativeATEMInputIterator(IBMDSwitcherInputIterator comInterface) => _comIterator = comInterface;

		public bool MoveNext(out INativeATEMInput item)
		{
			_comIterator.Next(out var comItem);
			item = new WindowsNativeATEMInput(comItem);
			return comItem != null;
		}

		public void Dispose() => Marshal.ReleaseComObject(_comIterator);
	}

	[SupportedOSPlatform("windows")]
	public class WindowsNativeATEMInput : INativeATEMInput
	{
		IBMDSwitcherInput _comObject;
		public WindowsNativeATEMInput(IBMDSwitcherInput comInterface) => _comObject = comInterface;

		public long GetID()
		{
			_comObject.GetInputId(out long id);
			return id;
		}

		public string GetShortName()
		{
			_comObject.GetShortName(out string name);
			return name;
		}

		public _BMDSwitcherInputAvailability GetAvailability()
		{
			_comObject.GetInputAvailability(out _BMDSwitcherInputAvailability a);
			return a;
		}

		public void Dispose() => Marshal.ReleaseComObject(_comObject);
	}

	[SupportedOSPlatform("windows")]
	public class WindowsNativeATEMBlock : INativeATEMMixBlock, IBMDSwitcherMixEffectBlockCallback
	{
		IBMDSwitcherMixEffectBlock _comObject;
		INativeATEMBlockCallbackHandler _handler = null!;

		public WindowsNativeATEMBlock(IBMDSwitcherMixEffectBlock comObject) => _comObject = comObject;

		public void AddCallback(INativeATEMBlockCallbackHandler handler)
		{
			_handler = handler;
			_comObject.AddCallback(this);
		}

		public void ClearCallback() => _comObject.RemoveCallback(this);

		public void Dispose() => Marshal.ReleaseComObject(_comObject);

		public long GetPreviewInput()
		{
			_comObject.GetPreviewInput(out long val);
			return val;
		}

		public long GetProgramInput()
		{
			_comObject.GetProgramInput(out long val);
			return val;
		}

		public void SetPreviewInput(long val) => _comObject.SetPreviewInput(val);
		public void SetProgramInput(long val) => _comObject.SetProgramInput(val);
		public void Cut() => _comObject.PerformCut();

		// Native callback:
		public void Notify(_BMDSwitcherMixEffectBlockEventType eventType) => _handler.Notify(eventType);

	}
}
