using BMDSwitcherAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers.Types.ATEM.Native
{
    public interface INativeATEMSwitcherDiscovery
    {
        INativeATEMSwitcher Connect(string address);
	}

    public interface INativeATEMSwitcher : IDisposable
    {
        INativeATEMBlockIterator CreateMixBlockIterator();
		INativeATEMInputIterator CreateInputIterator();
        void AddCallback(INativeATEMSwitcherCallbackHandler callback);
        void ClearCallback();
    }

    public interface INativeATEMBlockIterator : IDisposable
	{
        bool MoveNext(out INativeATEMMixBlock item);
    }

    public interface INativeATEMInputIterator : IDisposable
    {
		bool MoveNext(out INativeATEMInput item);
	}

    public interface INativeATEMMixBlock : IDisposable
	{
		void AddCallback(INativeATEMBlockCallbackHandler handler);
		void ClearCallback();
        long GetProgramInput();
        long GetPreviewInput();
        void SetProgramInput(long val);
        void SetPreviewInput(long val);
        void Cut();
	}

    public interface INativeATEMInput : IDisposable
	{
		long GetID();
		string GetShortName();
		_BMDSwitcherInputAvailability GetAvailability();
	}

    public interface INativeATEMSwitcherCallbackHandler
    {
        void Notify(_BMDSwitcherEventType type, _BMDSwitcherVideoMode videoMode);
	}

	public interface INativeATEMBlockCallbackHandler
	{
        void Notify(_BMDSwitcherMixEffectBlockEventType eventType);
	}
}