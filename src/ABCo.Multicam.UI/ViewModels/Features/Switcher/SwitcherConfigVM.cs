﻿using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Features.Switchers.Types;
using ABCo.Multicam.UI.ViewModels.Features.Switcher.Types;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.ViewModels.Features.Switcher
{
    public interface ISwitcherConfigVM : INeedsInitialization<SwitcherConfig, ISwitcherFeatureVM>, INotifyPropertyChanged
    {
		string[] Items { get; }
        string SelectedItem { get; set; }
		ISpecificSwitcherConfigVM? CurrentConfig { get; }
	}

    public interface ISpecificSwitcherConfigVM
    {

    }

    public partial class SwitcherConfigVM : ViewModelBase, ISwitcherConfigVM
    {
        bool _initialized = false;

        ISpecificSwitcherConfigVMFactory _configVMFactory;
        ISwitcherFeatureVM _parent = null!;
        IServiceSource _servSource;

        [ObservableProperty] string[] _items = new string[]
        {
            "Dummy",
            "ATEM"
        };

        [ObservableProperty] string _selectedItem = "Dummy";
        [ObservableProperty] ISpecificSwitcherConfigVM? _currentConfig;

        public SwitcherConfigVM(IServiceSource servSource)
        {
            _servSource = servSource;
            _configVMFactory = servSource.Get<ISpecificSwitcherConfigVMFactory>();
        }

        public void FinishConstruction(SwitcherConfig config, ISwitcherFeatureVM parent)
        {
            _parent = parent;

            // Update the selected item
            SelectedItem = config.Type switch 
            {
                SwitcherType.ATEM => "ATEM",
                _ => "Dummy"
			};

            CurrentConfig = _configVMFactory.Create(config, parent);
            _initialized = true;
        }

        partial void OnSelectedItemChanged(string value)
        {
            if (!_initialized) return;

            _parent.UpdateConfig(value switch
            {
                "Dummy" => new DummySwitcherConfig(),
                "ATEM" => new DummySwitcherConfig(4, 4),
                _ => throw new Exception("Unsupported combo box item")
            });
        }
    }

    public record class SwitcherConfigComboItemVM(SwitcherType Type, string Text, int Id);

    public interface ISpecificSwitcherConfigVMFactory
    {
        ISpecificSwitcherConfigVM Create(SwitcherConfig config, ISwitcherFeatureVM parent);
    }

    public class SpecificSwitcherConfigVMFactory : ISpecificSwitcherConfigVMFactory
    {
        public ISpecificSwitcherConfigVM Create(SwitcherConfig config, ISwitcherFeatureVM parent)
        {
            return config switch
            {
                DummySwitcherConfig dummyConfig => new DummySwitcherConfigVM(dummyConfig, parent),
                _ => throw new Exception("Unrecognised config")
            };
        }
    }
}
