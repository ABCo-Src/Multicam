﻿using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers.Live.Types.ATEM;
using ABCo.Multicam.UI.Presenters.Features.Switcher.Config;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher.Config.ATEM
{
	public interface ISwitcherATEMConfigVM : IParameteredService<ISwitcherATEMConfgPresenter>, ISwitcherSpecificConfigVM, INotifyPropertyChanged 
	{
		string[] ConnectionTypes { get; }
		bool IsIPAddressEditable { get; }
		string SelectedConnectionType { get; set; }
		ATEMPlatformCompatibilityValue CompatibilityMessage { get; set; }
		bool ShowOneProgramMessage { get; }
		string IpAddress { get; set; }
		void OnIPChange();
		void OnSelectedTypeChange();
	}

	public partial class SwitcherATEMConfigVM : ViewModelBase, ISwitcherATEMConfigVM
	{
		readonly ISwitcherATEMConfgPresenter _presenter;

		public string[] ConnectionTypes => new string[]
		{
			"USB",
			"IP"
		};

		public bool ShowOneProgramMessage => SelectedConnectionType == "USB";
		public bool IsIPAddressEditable => SelectedConnectionType == "IP";

		[ObservableProperty] string _ipAddress = "";
		[ObservableProperty] string _selectedConnectionType = "USB";
		[ObservableProperty] ATEMPlatformCompatibilityValue _compatibilityMessage = ATEMPlatformCompatibilityValue.Supported;


		public SwitcherATEMConfigVM(ISwitcherATEMConfgPresenter presenter) => _presenter = presenter;

		public void OnIPChange() => _presenter.OnUIChange();
		public void OnSelectedTypeChange() => _presenter.OnUIChange();
	}
}