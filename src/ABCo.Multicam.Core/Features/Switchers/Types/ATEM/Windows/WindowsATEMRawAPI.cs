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
    public class WindowsATEMRawAPI : IATEMRawAPI
	{
		public IBMDSwitcher Connect(string address)
		{
			var discovery = new CBMDSwitcherDiscovery();
			_BMDSwitcherConnectToFailure failure = 0;

			try
			{
				discovery.ConnectTo(address, out IBMDSwitcher switcher, out failure);
				Free(discovery);
				return switcher;
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

		public IBMDSwitcherMixEffectBlockIterator CreateMixBlockIterator(IBMDSwitcher switcher)
        {
            // Get iterator as IUnknown
            switcher!.CreateIterator(typeof(IBMDSwitcherMixEffectBlockIterator).GUID, out IntPtr iterBase);

            // Cast to real object
            var iter = (IBMDSwitcherMixEffectBlockIterator)Marshal.GetObjectForIUnknown(iterBase);
            Marshal.Release(iterBase);

            return iter;
        }

        public IBMDSwitcherInputIterator CreateInputIterator(IBMDSwitcher switcher)
        {
            // Get iterator as IUnknown
            switcher!.CreateIterator(typeof(IBMDSwitcherInputIterator).GUID, out IntPtr iterBase);

            // Cast to real object
            var iter = (IBMDSwitcherInputIterator)Marshal.GetObjectForIUnknown(iterBase);
            Marshal.Release(iterBase);

            return iter;
        }

        public bool MoveNext(IBMDSwitcherMixEffectBlockIterator iter, out IBMDSwitcherMixEffectBlock item)
        {
            iter.Next(out item);
            return item != null;
        }

        public bool MoveNext(IBMDSwitcherInputIterator iter, out IBMDSwitcherInput item)
        {
            iter.Next(out item);
            return item != null;
        }

        public long GetID(IBMDSwitcherInput input)
        {
            input.GetInputId(out long id);
            return id;
        }

        public string GetShortName(IBMDSwitcherInput input)
        {
            input.GetShortName(out string name);
            return name;
        }

        public _BMDSwitcherInputAvailability GetAvailability(IBMDSwitcherInput input)
        {
            input.GetInputAvailability(out _BMDSwitcherInputAvailability a);
            return a;
        }

        public void Free(object obj) => Marshal.ReleaseComObject(obj);
    }
}
