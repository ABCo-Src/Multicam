using ABCo.Multicam.Server.Features.Data;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.Features.Switchers.Interaction;
using ABCo.Multicam.Server.Features.Switchers.Types;
using ABCo.Multicam.Server.Features.Switchers.Data;
using System.Data.Common;

namespace ABCo.Multicam.Server.Features.Switchers
{
	public interface ISwitcherEventHandler
    {
        void OnProgramValueChange(SwitcherProgramChangeInfo info);
        void OnPreviewValueChange(SwitcherPreviewChangeInfo info);
        void OnSpecsChange(SwitcherSpecs newSpecs);
        void OnConnectionStateChange(bool isConnected);
        void OnFailure(SwitcherError error);
    }

    public interface ISwitcherLiveFeature : ILiveFeature, IServerService<IFeatureDataStore> { }

	public record class BusChangeInfo(int MB, int Val);

	public class SwitcherLiveFeature : ISwitcherLiveFeature, ISwitcherEventHandler
    {
		public const int SET_GENERALINFO = 0;
		public const int ACKNOWLEDGE_ERROR = 1;
		public const int CONNECT = 2;
		public const int DISCONNECT = 3;
		public const int SET_CONFIG = 4;
		public const int SET_PROGRAM = 5;
		public const int SET_PREVIEW = 6;
		public const int CUT = 7;

		// TODO: Add slamming protection
		// TODO: Add error handling

		// The buffer that sits between the switcher and adds preview emulation, caching and more to all the switcher interactions.
		// A new interaction buffer is created anytime the specs change, and the swapper facilitates for us.
		readonly IHotSwappableSwitcherInteractionBuffer _buffer;        
		readonly IFeatureDataStore _dataCollection;

		public SwitcherLiveFeature(IFeatureDataStore fragmentCollection, IServerInfo serviceSource)
        {
			var defaultConfig = new DummySwitcherConfig(4);

			_dataCollection = fragmentCollection;
            _buffer = serviceSource.Get<IHotSwappableSwitcherInteractionBuffer, SwitcherConfig>(defaultConfig);
			_buffer.SetEventHandler(this);

			// Setup default data values
			fragmentCollection.SetData<FeatureGeneralInfo>(new FeatureGeneralInfo(FeatureTypes.Switcher, "New Switcher"));
			fragmentCollection.SetData<SwitcherCompatibility>(new SwitcherCompatibility(SwitcherPlatformCompatibilityValue.Supported));
			fragmentCollection.SetData<SwitcherConfig>(defaultConfig);
			fragmentCollection.SetData<SwitcherConnection>(new SwitcherConnection(_buffer.CurrentBuffer.IsConnected));
			fragmentCollection.SetData<SwitcherSpecs>(_buffer.CurrentBuffer.Specs);
			fragmentCollection.SetData<SwitcherError>(new SwitcherError(null));
			OnMixBlockStateChange();
		}

		// Methods:
		public void PerformAction(int id)
		{
			switch (id)
			{
				case ACKNOWLEDGE_ERROR:
					_dataCollection.SetData<SwitcherError>(new SwitcherError(null));
					break;

				case CONNECT:
					_buffer.CurrentBuffer.Connect();
					break;

				case DISCONNECT:
					_buffer.CurrentBuffer.Disconnect();
					break;

			}
		}

		public void PerformAction(int id, object param)
        {
            switch (id)
            {
				case SET_GENERALINFO:
					_dataCollection.SetData<FeatureGeneralInfo>((FeatureGeneralInfo)param);
					break;

				case SET_PROGRAM:
                    var setProgStruct = (BusChangeInfo)param;
					_buffer.CurrentBuffer.SendProgram(setProgStruct.MB, setProgStruct.Val);
					break;

				case SET_PREVIEW:
					var setPrevStruct = (BusChangeInfo)param;
					_buffer.CurrentBuffer.SendPreview(setPrevStruct.MB, setPrevStruct.Val);
					break;

				case CUT:
					_buffer.CurrentBuffer.Cut((int)param);
					break;

				case SET_CONFIG:
					var newConfig = (SwitcherConfig)param;

					_buffer.ChangeSwitcher(newConfig);
					_dataCollection.SetData<SwitcherCompatibility>(_buffer.CurrentBuffer.GetPlatformCompatibility());;
					_dataCollection.SetData<SwitcherConfig>(newConfig);
					break;
			}
        }

		// Events:
		void OnMixBlockStateChange()
        {
            var specs = _buffer.CurrentBuffer.Specs;

            var res = new MixBlockState[specs.MixBlocks.Count];
            for (int i = 0; i < specs.MixBlocks.Count; i++)
                res[i] = new MixBlockState(_buffer.CurrentBuffer.GetProgram(i), _buffer.CurrentBuffer.GetPreview(i));

            _dataCollection.SetData<SwitcherState>(new SwitcherState(res));
        }

        public void OnProgramValueChange(SwitcherProgramChangeInfo info) => OnMixBlockStateChange();
		public void OnPreviewValueChange(SwitcherPreviewChangeInfo info) => OnMixBlockStateChange();

		public void OnSpecsChange(SwitcherSpecs newSpecs)
		{
			_dataCollection.SetData<SwitcherSpecs>(newSpecs);
			OnMixBlockStateChange();
		}

		public void OnConnectionStateChange(bool newState) => _dataCollection.SetData<SwitcherConnection>(new SwitcherConnection(newState));

		public void OnFailure(SwitcherError error)
		{
            // Create a new buffer
            _buffer.ChangeSwitcher(_dataCollection.GetData<SwitcherConfig>());
			_dataCollection.SetData<SwitcherError>(error);
		}

		// Dispose:
		public void Dispose() => _buffer.Dispose();
	}
}