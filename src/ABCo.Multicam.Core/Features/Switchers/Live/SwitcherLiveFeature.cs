using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Features.Switchers.Data;
using ABCo.Multicam.Core.Features.Switchers.Data.Config;
using ABCo.Multicam.Core.Features.Switchers.Interaction;
using ABCo.Multicam.Core.Features.Switchers.Types;

namespace ABCo.Multicam.Core.Features.Switchers
{
	public interface ISwitcherEventHandler
    {
        void OnProgramValueChange(SwitcherProgramChangeInfo info);
        void OnPreviewValueChange(SwitcherPreviewChangeInfo info);
        void OnSpecsChange(SwitcherSpecs newSpecs);
        void OnConnectionStateChange(bool isConnected);
        void OnFailure(SwitcherError error);
    }

    public interface ISwitcherLiveFeature : ILiveFeature, IParameteredService<IInstantRetrievalDataSource> { }
    public interface ISwitcherFeaturePresenter : IFeaturePresenter { }

	public class SwitcherLiveFeature : ISwitcherLiveFeature, ISwitcherEventHandler
    {
		// TODO: Add slamming protection
		// TODO: Add error handling

		// The buffer that sits between the switcher and adds preview emulation, caching and more to all the switcher interactions.
		// A new interaction buffer is created anytime the specs change, and the swapper facilitates for us.
		readonly IHotSwappableSwitcherInteractionBuffer _buffer;        
		readonly IInstantRetrievalDataSource _dataCollection;

		public static ISwitcherLiveFeature New(IInstantRetrievalDataSource fragmentCollection, IServiceSource serviceSource) => new SwitcherLiveFeature(fragmentCollection, serviceSource);
		public SwitcherLiveFeature(IInstantRetrievalDataSource fragmentCollection, IServiceSource serviceSource)
        {
			_dataCollection = fragmentCollection;
            _buffer = serviceSource.Get<IHotSwappableSwitcherInteractionBuffer, SwitcherConfig>(fragmentCollection.GetData<SwitcherConfig>());
			_buffer.SetEventHandler(this);

			OnConnectionStateChange(_buffer.CurrentBuffer.IsConnected);
			OnSpecsChange(_buffer.CurrentBuffer.Specs);
			OnMixBlockStateChange();
		}

		// Methods:
		public void PerformAction(int id)
		{
			switch (id)
			{
				case SwitcherActionID.ACKNOWLEDGE_ERROR:
					_dataCollection.SetData(new SwitcherError(null));
					break;

				case SwitcherActionID.CONNECT:
					_buffer.CurrentBuffer.Connect();
					break;

				case SwitcherActionID.DISCONNECT:
					_buffer.CurrentBuffer.Disconnect();
					break;

			}
		}

		public void PerformAction(int id, object param)
        {
            switch (id)
            {
				case SwitcherActionID.SET_GENERALINFO:
					_dataCollection.SetData((FeatureGeneralInfo)param);
					break;

				case SwitcherActionID.SET_PROGRAM:
                    var setProgStruct = (BusChangeInfo)param;
					_buffer.CurrentBuffer.SendProgram(setProgStruct.MB, setProgStruct.Val);
					break;

				case SwitcherActionID.SET_PREVIEW:
					var setPrevStruct = (BusChangeInfo)param;
					_buffer.CurrentBuffer.SendPreview(setPrevStruct.MB, setPrevStruct.Val);
					break;

				case SwitcherActionID.CUT:
					_buffer.CurrentBuffer.Cut((int)param);
					break;

				case SwitcherActionID.SET_CONFIG_TYPE:
					var newConfigType = (SwitcherConfigType)param;

					SwitcherConfig newDefaultConfig = newConfigType.Type switch
					{
						SwitcherType.Dummy => new DummySwitcherConfig(4),
						SwitcherType.ATEM => new ATEMSwitcherConfig(null),
						_ => throw new Exception("Currently unsupported switcher type!")
					};

					_buffer.ChangeSwitcher(newDefaultConfig);
					_dataCollection.SetData(newConfigType);
					_dataCollection.SetData(newDefaultConfig);
					break;

				case SwitcherActionID.SET_CONFIG:
					var newConfig = (SwitcherConfig)param;
					_buffer.ChangeSwitcher(newConfig);
					_dataCollection.SetData(newConfig);
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

            _dataCollection.SetData(new SwitcherState(res));
        }

        public void OnProgramValueChange(SwitcherProgramChangeInfo info) => OnMixBlockStateChange();
		public void OnPreviewValueChange(SwitcherPreviewChangeInfo info) => OnMixBlockStateChange();

		public void OnSpecsChange(SwitcherSpecs newSpecs)
		{
			_dataCollection.SetData(newSpecs);
			OnMixBlockStateChange();
		}

		public void OnConnectionStateChange(bool newState) => _dataCollection.SetData(new SwitcherConnection(newState));

		public void OnFailure(SwitcherError error)
		{
            // Create a new buffer
            _buffer.ChangeSwitcher(_dataCollection.GetData<SwitcherConfig>());
			_dataCollection.SetData(error);
		}

		// Dispose:
		public void Dispose() => _buffer.Dispose();
	}
}