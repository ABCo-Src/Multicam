using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using ABCo.Multicam.UI.ViewModels.Features.Switcher.Types;
using ABCo.Multicam.UI.Presenters.Features.Switcher.Config;
using ABCo.Multicam.Core.Features.Switchers.Data.Config;
using ABCo.Multicam.Core.Features.Switchers.Data;

namespace ABCo.Multicam.UI.Presenters.Features.Switcher
{
	public interface ISwitcherConfigPresenter : IParameteredService<IFeature, SwitcherConfigType>
	{
		void OnConfig(SwitcherConfig config);
		void SelectedChanged();
		ISwitcherConfigVM VM { get; }
	}

	public interface ISwitcherSpecificConfigPresenter
	{
		void OnConfig(SwitcherConfig config);
		ISwitcherSpecificConfigVM VM { get; }
	}

	public class SwitcherConfigPresenter : ISwitcherConfigPresenter
	{
		readonly IFeature _feature;
		ISwitcherSpecificConfigPresenter? _currentConfigPresenter;

		public ISwitcherConfigVM VM { get; }

		public SwitcherConfigPresenter(IFeature feature, SwitcherConfigType type, IServiceSource servSource)
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
				SwitcherType.Dummy => servSource.Get<ISwitcherDummyConfigPresenter, IFeature>(feature),
				SwitcherType.ATEM => servSource.Get<ISwitcherATEMConfgPresenter, IFeature>(feature),
				_ => null
			};

			if (_currentConfigPresenter != null)
				VM.CurrentConfig = _currentConfigPresenter.VM;
		}

		public void OnConfig(SwitcherConfig config) => _currentConfigPresenter?.OnConfig(config);

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