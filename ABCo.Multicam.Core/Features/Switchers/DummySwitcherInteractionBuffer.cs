using ABCo.Multicam.Core.Features.Switchers.Fading;
using ABCo.Multicam.Core.Features.Switchers.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Core.Features.Switchers
{
    public class DummySwitcherInteractionBuffer : ISwitcherInteractionBuffer
    {
        IDummySwitcher _switcher;
        Action<RetrospectiveFadeInfo?>? _onBusChangeCallback;

        public DummySwitcherInteractionBuffer(IDummySwitcher switcher)
        {
            _switcher = switcher;
            switcher.SetOnBusChangeFinishCall(OnBusChange);
        }

        public bool IsConnected => true;
        public SwitcherSpecs Specs => _switcher.ReceiveSpecs();
        public int GetValue(int mixBlock, int bus) => _switcher.ReceiveValue(mixBlock, bus);
        public void PostValue(int mixBlock, int bus, int value) => _switcher.PostValue(mixBlock, bus, value);
        public void Dispose() => _switcher.Dispose();

        void OnBusChange(SwitcherBusChangeInfo info) => _onBusChangeCallback?.Invoke(info.FadeInfo);

        public void SetOnBusChangeFinishCall(Action<RetrospectiveFadeInfo?>? callback) => _onBusChangeCallback = callback;

        public void Cut(int mixBlock) => _switcher.Cut(mixBlock);
    }
}
