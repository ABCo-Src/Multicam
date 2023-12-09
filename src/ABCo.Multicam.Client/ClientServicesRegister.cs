using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.Presenters.Features;
using ABCo.Multicam.Client.Presenters.Features.Switchers;
using ABCo.Multicam.Client.Presenters.Features.Switchers.Config;
using ABCo.Multicam.Client.Presenters.Hosting;
using ABCo.Multicam.Client.ViewModels;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Client.ViewModels.Features.Switcher.Config.ATEM;
using ABCo.Multicam.Client.ViewModels.Features.Switcher.Types;
using ABCo.Multicam.Client.ViewModels.Hosting;
using ABCo.Multicam.Server.Features;
using ABCo.Multicam.Server.Features.Switchers;
using ABCo.Multicam.Server.Hosting.Clients;
using ABCo.Multicam.Server.Hosting.Management;

namespace ABCo.Multicam.Client
{
    public static class ClientServicesRegister
    {
        //public static string Log { get; set; }

        public static void AddServices(ClientServicesBuilder container)
        {
			// Logging for debug purposes if you want it:
			//container.Initialize(registration => true,
			//    (f, i) => ServiceSource.N += $"Service requested: {i.GetType().Name}.   Thread: {Thread.CurrentThread.ManagedThreadId}\n"
			//);

			// Register presenters
			container.AddScoped<IMainUIVM, MainUIVM>();

			container.AddTransient<IHostingPresenter, Dispatched<IHostingManager>>((p1, s) => new HostingPresenter(p1, s));
			container.AddTransient<ISwitcherListVM, Dispatched<IMainFeatureCollection>>((p1, s) => new SwitcherListVM(p1, s));
            container.AddTransient<IFeaturePresenter, Dispatched<IFeature>>((p1, s) => new FeaturePresenter(p1, s));
			container.AddTransient<ISwitcherFeaturePresenter, Dispatched<IFeature>>((p1, s) => new SwitcherFeaturePresenter(p1, s));
			container.AddTransient<ISwitcherMixBlocksPresenter, ISwitcherFeatureVM, Dispatched<ISwitcherFeature>>((p1, p2, s) => new SwitcherMixBlocksPresenter(p1, p2, s));

			container.AddTransient<ISwitcherConfigPresenter, Dispatched<ISwitcherFeature>>((p1, s) => new SwitcherConfigPresenter(p1, s));
			container.AddTransient<ISwitcherDummyConfigPresenter, Dispatched<ISwitcherFeature>>((p1, s) => new SwitcherDummyConfigPresenter(p1, s));
			container.AddTransient<ISwitcherATEMConfigPresenter, Dispatched<ISwitcherFeature>>((p1, s) => new SwitcherATEMConfigPresenter(p1, s));

			// Register view-models
            container.AddTransient<IServerHostingVM, IHostingPresenter, IHostnameConfigVM, IHostingExecutionVM>((p1, p2, p3, s) => new ServerHostingVM(p1, p2, p3));
            container.AddTransient<IHostnameConfigVM, IHostingPresenter>((p1, s) => new HostnameConfigVM(p1));
            container.AddTransient<IHostingExecutionVM, IHostingPresenter>((p1, s) => new HostingExecutionVM(p1));
            container.AddTransient<ISwitcherListItemVM, ISwitcherListVM, IFeature, IFeatureVM>((p1, p2, p3, s) => new SwitcherListItemVM(p1, p2, p3));
            container.AddTransient<IFeatureVM, IFeaturePresenter>((p1, s) => new FeatureVM(p1));
			container.AddTransient<ISwitcherFeatureVM>(s => new SwitcherFeatureVM());
            container.AddTransient<ISwitcherMixBlockVM>(s => new SwitcherMixBlockVM());
            container.AddTransient<ISwitcherCutButtonVM, ISwitcherMixBlocksPresenter, int>((p1, p2, s) => new SwitcherCutButtonVM(p1, p2));
            container.AddTransient<ISwitcherProgramInputVM, ISwitcherMixBlocksPresenter, int, int>((p1, p2, p3, s) => new SwitcherProgramInputVM(p1, p2, p3));
			container.AddTransient<ISwitcherPreviewInputVM, ISwitcherMixBlocksPresenter, int, int>((p1, p2, p3, s) => new SwitcherPreviewInputVM(p1, p2, p3));
			container.AddTransient<ISwitcherConnectionVM, SwitcherConnectionPresenter>((p1, s) => new SwitcherConnectionVM(p1));

			container.AddTransient<ISwitcherConfigVM, ISwitcherConfigPresenter>((p1, s) => new SwitcherConfigVM(p1));
			container.AddTransient<ISwitcherDummyConfigVM, ISwitcherDummyConfigPresenter>((p1, s) => new SwitcherDummyConfigVM(p1));
			container.AddTransient<ISwitcherDummyConfigMixBlockVM, ISwitcherDummyConfigPresenter>((p1, s) => new DummySwitcherConfigMixBlockVM(p1));
			container.AddTransient<ISwitcherATEMConfigVM, ISwitcherATEMConfigPresenter>((p1, s) => new SwitcherATEMConfigVM(p1));
        }
    }
}
