﻿using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.General;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.UI.Presenters.Features;
using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.ViewModels.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.UI.Presenters
{
	public interface IMainUIPresenter : IFeatureSideMenuPresenter
	{
		void Init(IMainThreadDispatcher dispatcher);
		IMainUIVM VM { get; }
	}

	public interface IFeatureSideMenuPresenter
	{
		void OpenMenu(ISideMenuEmbeddableVM vm, string title, Action onClose);
		void CloseMenu();
	}

	public interface ISideMenuEmbeddableVM : INotifyPropertyChanged { }

	public class MainUIPresenter : IMainUIPresenter, IFeatureSideMenuPresenter
	{
		Action? _onClose;
		IServiceSource _servSource;

		public IMainUIVM VM { get; }

        public MainUIPresenter(IServiceSource servSource)
        {
			_servSource = servSource;
            VM = servSource.Get<IMainUIVM, IMainUIPresenter>(this);
        }

        public void Init(IMainThreadDispatcher dispatcher)
		{
            var mainFeaturesCollection = _servSource.Get<IMainFeatureCollection>();
			var scope = _servSource.Get<IScopedConnectionManager>().CreateScope(dispatcher);

			VM.ContentVM = mainFeaturesCollection.UIPresenters.GetPresenter<IProjectFeaturesPresenter>(scope).VM;
        }

		public void OpenMenu(ISideMenuEmbeddableVM vm, string title, Action onClose)
		{
			if (_onClose != null) CloseMenu();

			_onClose = onClose;
			VM.MenuTitle = title;
			VM.MenuVM = vm;
		}

		public void CloseMenu()
		{
			VM.MenuTitle = "";

			if (_onClose != null)
			{
				_onClose();
				VM.MenuVM = null;
			}
		}
	}
}
