using ABCo.Multicam.Server;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Client.Presenters.Features.Switcher.Config;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Client.Presenters.Features.Switcher
{
	public interface ISwitcherConfigPresenter : IClientService<IDispatchedServerComponent<ISwitcherFeature>>
	{
		void Refresh(SwitcherConfig config, SwitcherPlatformCompatibilityValue compatibility);
		void SelectedChanged();
		ISwitcherConfigVM VM { get; }
	}

	public interface ISwitcherSpecificConfigPresenter
	{
		void Refresh(SwitcherConfig config);
		void OnCompatibility(SwitcherPlatformCompatibilityValue compatibility);
		ISwitcherSpecificConfigVM VM { get; }
	}

	public class SwitcherConfigPresenter : ISwitcherConfigPresenter
	{
		readonly IDispatchedServerComponent<ISwitcherFeature> _feature;
		readonly IClientInfo _info;
		SwitcherType? _oldType;
		ISwitcherSpecificConfigPresenter? _currentConfigPresenter;

		public ISwitcherConfigVM VM { get; }

		public SwitcherConfigPresenter(IDispatchedServerComponent<ISwitcherFeature> feature, IClientInfo info)
		{
			_feature = feature;
			_info = info;
			VM = info.Get<ISwitcherConfigVM, ISwitcherConfigPresenter>(this);
		}

		public void Refresh(SwitcherConfig config, SwitcherPlatformCompatibilityValue compatibility) 
		{
			// If the config's type changed, create a new specific presenter 
			if (config.Type != _oldType)
			{
				_oldType = config.Type;

				// Set the selected item
				VM.SelectedItem = config.Type switch
				{
					SwitcherType.ATEM => "ATEM",
					_ => "Dummy"
				};

				// Update the inner VM
				_currentConfigPresenter = config.Type switch
				{
					SwitcherType.Dummy => _info.Get<ISwitcherDummyConfigPresenter, IDispatchedServerComponent<ISwitcherFeature>>(_feature),
					SwitcherType.ATEM => _info.Get<ISwitcherATEMConfigPresenter, IDispatchedServerComponent<ISwitcherFeature>>(_feature),
					_ => null
				};

				if (_currentConfigPresenter != null)
					VM.CurrentConfig = _currentConfigPresenter.VM;
			}

			// Inform the specific config about everything.
			_currentConfigPresenter?.OnCompatibility(compatibility);
			_currentConfigPresenter?.Refresh(config);
		}

		public void SelectedChanged()
		{
			_feature.CallDispatched(f => f.ChangeConfig(VM.SelectedItem switch
			{
				"Dummy" => new DummySwitcherConfig(4),
				"ATEM" => new ATEMSwitcherConfig(null),
				_ => throw new Exception("Unsupported selected mode given")
			}));
		}
	}
}