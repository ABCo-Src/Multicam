﻿using ABCo.Multicam.Server;
using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.Presenters.Features;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Client.ViewModels.Features
{
	public interface IProjectFeaturesListItemVM : IClientService<IProjectFeaturesPresenter, IServerTarget, IFeatureVM>, INotifyPropertyChanged, ISideMenuEmbeddableVM
	{
		IServerTarget NativeItem { get; }
		IFeatureVM Feature { get; }
		string EditBtnText { get; set; }
		void OpenMobileView();
		void CloseMobileView();
		void ToggleEdit();
		void MoveDown();
		void MoveUp();
		void Delete();
	}

	public partial class ProjectFeaturesListItemVM : ViewModelBase, IProjectFeaturesListItemVM
	{
		public IServerTarget NativeItem { get; }

		readonly IProjectFeaturesPresenter _presenter;
		[ObservableProperty] IFeatureVM _feature;
		[ObservableProperty] string _editBtnText = "";

		public ProjectFeaturesListItemVM(IProjectFeaturesPresenter presenter, IServerTarget nativeItem, IFeatureVM innerVM)
		{
			NativeItem = nativeItem;
			_presenter = presenter;
			_feature = innerVM;
		}

		public void ToggleEdit() => _presenter.ToggleEdit(this);
		public void MoveDown() => _presenter.MoveDown(this);
		public void MoveUp() => _presenter.MoveUp(this);
		public void Delete() => _presenter.Delete(this);
		public void OpenMobileView() => _presenter.OpenMobileMenu(this);
		public void CloseMobileView() => _presenter.CloseMobileMenu();
	}
}