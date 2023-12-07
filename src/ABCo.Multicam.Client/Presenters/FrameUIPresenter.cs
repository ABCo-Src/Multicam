using ABCo.Multicam.Client.Presenters.Features;
using ABCo.Multicam.Client.ViewModels.Frames;
using ABCo.Multicam.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Client.Presenters
{
	public interface IFrameUIPresenter
	{
		void Select(IFrameMenuTabVM vm);
		IFrameVM VM { get; }
	}

	public class FrameUIPresenter : IFrameUIPresenter
	{
		public IFrameVM VM { get; }

		public FrameUIPresenter(IClientInfo info)
		{
			var switcherPage = info.ServerConnection.GetFeatures().ClientNotifier.GetOrAddClientEndpoint<IMainFeatureCollectionPresenter>(info).VM;

			var vm = new FrameVM(null, 
				new IFrameMenuTabVM[]
				{
					new FrameMenuTabVM("Welcome", this, null)
				},
				new IFrameMenuTabVM[]
				{
					new FrameMenuTabVM("Switchers / Video Devices", this, switcherPage),
					new FrameMenuTabVM("Digital Tally", this, null),
					new FrameMenuTabVM("Cut Recording", this, null)
				},
				new IFrameMenuTabVM[]
				{
					new FrameMenuTabVM("Automation", this, null),
					new FrameMenuTabVM("Sync Devices", this, null)
				});

			VM = vm;

			// Start with the very first thing selected.
			Select(VM.TopTabs[0]);
		}

		public void Select(IFrameMenuTabVM vm)
		{
			// Deselect everything else
			foreach (IFrameMenuTabVM a in VM.TopTabs)
				a.IsSelected = false;
			foreach (IFrameMenuTabVM b in VM.MiddleTabs)
				b.IsSelected = false;
			foreach (IFrameMenuTabVM c in VM.BottomTabs)
				c.IsSelected = false;

			// Select this
			vm.IsSelected = true;
			VM.SelectedPage = vm.AssociatedPage;
		}
	}
}
