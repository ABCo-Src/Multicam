using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using ABCo.Multicam.UI.ViewModels.Features.Switcher.Types;
using ABCo.Multicam.UI.Presenters.Features.Switcher.Config;
using ABCo.Multicam.Server.Features.Switchers.Data.Config;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Features.Switchers.Data;
using ABCo.Multicam.Server.Hosting;

namespace ABCo.Multicam.UI.Presenters.Features.Switcher
{
    public interface ISwitcherConfigPresenter : IClientService<IServerTarget, SwitcherConfigType>
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
		ISwitcherSpecificConfigPresenter? _currentConfigPresenter;

		public ISwitcherConfigVM VM { get; }

		public SwitcherConfigPresenter(IServerTarget feature, SwitcherConfigType type, IClientInfo servSource)
		{
			_feature = feature;
			VM = servSource.Get<ISwitcherConfigVM, ISwitcherConfigPresenter>(this);

			// Set the selected item
			VM.SelectedItem = type.Type switch
			{
				SwitcherType.ATEM => "ATEM",
				_ => "Dummy"
			};

			// Update the inner VM
			_currentConfigPresenter = type.Type switch
			{
				SwitcherType.Dummy => servSource.Get<ISwitcherDummyConfigPresenter, IServerTarget>(feature),
				SwitcherType.ATEM => servSource.Get<ISwitcherATEMConfigPresenter, IServerTarget>(feature),
				_ => null
			};

			if (_currentConfigPresenter != null)
				VM.CurrentConfig = _currentConfigPresenter.VM;
		}

		public void OnConfig(SwitcherConfig config) => _currentConfigPresenter?.OnConfig(config);
		public void OnCompatibility(SwitcherCompatibility compatibility) => _currentConfigPresenter?.OnCompatibility(compatibility);

		public void SelectedChanged()
		{
			_feature.PerformAction(SwitcherActionID.SET_CONFIG_TYPE, new SwitcherConfigType(VM.SelectedItem switch
			{
				"Dummy" => SwitcherType.Dummy,
				"ATEM" => SwitcherType.ATEM,
				_ => throw new Exception("Unsupported selected mode given")
			}));
		}
	}
}