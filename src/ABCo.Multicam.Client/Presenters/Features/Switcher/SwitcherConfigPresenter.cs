using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Client.ViewModels.Features.Switcher.Types;
using ABCo.Multicam.Client.Presenters.Features.Switcher.Config;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Client.ViewModels.Features;

namespace ABCo.Multicam.Client.Presenters.Features.Switcher
{
    public interface ISwitcherConfigPresenter : IClientService<IServerTarget>
	{
		void OnConfig(SwitcherConfig config);
		void OnCompatibility(SwitcherCompatibility compatibility);
		void SelectedChanged();
		ISwitcherConfigVM VM { get; }
	}

	public interface ISwitcherSpecificConfigPresenter
	{
		void OnConfig(SwitcherConfig config);
		void OnCompatibility(SwitcherCompatibility compatibility);
		ISwitcherSpecificConfigVM VM { get; }
	}

	public class SwitcherConfigPresenter : ISwitcherConfigPresenter
	{
		readonly IServerTarget _feature;
		readonly IClientInfo _info;
		SwitcherType? _oldType;
		ISwitcherSpecificConfigPresenter? _currentConfigPresenter;

		public ISwitcherConfigVM VM { get; }

		public SwitcherConfigPresenter(IServerTarget feature, IClientInfo info)
		{
			_feature = feature;
			_info = info;
			VM = info.Get<ISwitcherConfigVM, ISwitcherConfigPresenter>(this);
		}

		public void OnConfig(SwitcherConfig config) 
		{
			// If the type has changed, reinitialize everything
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
					SwitcherType.Dummy => _info.Get<ISwitcherDummyConfigPresenter, IServerTarget>(_feature),
					SwitcherType.ATEM => _info.Get<ISwitcherATEMConfigPresenter, IServerTarget>(_feature),
					_ => null
				};

				if (_currentConfigPresenter != null)
					VM.CurrentConfig = _currentConfigPresenter.VM;
			}

			// Report the config change to the presenter
			_currentConfigPresenter?.OnConfig(config);
		}

		public void OnCompatibility(SwitcherCompatibility compatibility) => _currentConfigPresenter?.OnCompatibility(compatibility);

		public void SelectedChanged()
		{
			_feature.PerformAction(SwitcherLiveFeature.SET_CONFIG, VM.SelectedItem switch
			{
				"Dummy" => new DummySwitcherConfig(4),
				"ATEM" => new ATEMSwitcherConfig(null),
				_ => throw new Exception("Unsupported selected mode given")
			});
		}
	}
}