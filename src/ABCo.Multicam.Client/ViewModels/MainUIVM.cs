using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ABCo.Multicam.Client.Presenters
{
	public interface IMainUIVM : INotifyPropertyChanged
	{
		IPageSwitcherVM PageSwitcher { get; }
		IPopOutVM PopOut { get; }
	}

	public partial class MainUIVM : ViewModelBase, IMainUIVM
	{
		[ObservableProperty] IPageSwitcherVM _pageSwitcher;
		[ObservableProperty] IPopOutVM _popOut;

		public MainUIVM(IClientInfo info)
		{
			_pageSwitcher = new PageSwitcherVM(info);
			_popOut = info.Shared.PopOut;
		}
	}
}
