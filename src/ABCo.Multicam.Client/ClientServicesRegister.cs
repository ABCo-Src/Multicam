using ABCo.Multicam.Client.Presenters;
using ABCo.Multicam.Client.Presenters.Features;
using ABCo.Multicam.Client.Presenters.Features.Switcher;
using ABCo.Multicam.Client.Presenters.Features.Switcher.Config;
using ABCo.Multicam.Client.Presenters.Hosting;
using ABCo.Multicam.Client.ViewModels;
using ABCo.Multicam.Client.ViewModels.Features;
using ABCo.Multicam.Client.ViewModels.Features.Switcher;
using ABCo.Multicam.Client.ViewModels.Features.Switcher.Config.ATEM;
using ABCo.Multicam.Client.ViewModels.Features.Switcher.Types;
using ABCo.Multicam.Client.ViewModels.Hosting;
using ABCo.Multicam.Server.Hosting.Clients;

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
			container.AddScoped<IMainUIPresenter, MainUIPresenter>();

			container.AddTransient<IHostingPresenter, IServerTarget>((p1, s) => new HostingPresenter(p1, s));
			container.AddTransient<IProjectFeaturesPresenter, IServerTarget>((p1, s) => new ProjectFeaturesPresenter(p1, s));
            container.AddTransient<IMainFeaturePresenter, IServerTarget> ((p1, s) => new FeaturePresenter(p1, s));
			container.AddTransient<ISwitcherFeaturePresenter, IServerTarget>((p1, s) => new SwitcherFeaturePresenter(p1, s));
			container.AddTransient<ISwitcherConnectionPresenter, IServerTarget>((p1, s) => new SwitcherConnectionPresenter(p1, s));
			container.AddTransient<ISwitcherErrorPresenter, IServerTarget, Action>((p1, p2, s) => new SwitcherErrorPresenter(p1, p2, s));
			container.AddTransient<ISwitcherMixBlocksPresenter, ISwitcherFeatureVM, IServerTarget>((p1, p2, s) => new SwitcherMixBlocksPresenter(p1, p2, s));

			container.AddTransient<ISwitcherConfigPresenter, IServerTarget>((p1, s) => new SwitcherConfigPresenter(p1, s));
			container.AddTransient<ISwitcherDummyConfigPresenter, IServerTarget>((p1, s) => new SwitcherDummyConfigPresenter(p1, s));
			container.AddTransient<ISwitcherATEMConfigPresenter, IServerTarget>((p1, s) => new SwitcherATEMConfgPresenter(p1, s));

			// Register view-models
            container.AddTransient<IMainUIVM, IMainUIPresenter, IProjectFeaturesVM, IServerHostingVM>((p1, p2, p3, s) => new MainUIVM(p1, p2, p3));
            container.AddTransient<IServerHostingVM, IHostingPresenter>((p1, s) => new ServerHostingVM(p1));
            container.AddTransient<IProjectFeaturesVM, IProjectFeaturesPresenter>((p1, s) => new ProjectFeaturesVM(p1));
            container.AddTransient<IProjectFeaturesListItemVM, IProjectFeaturesPresenter, IServerTarget, IFeatureVM>((p1, p2, p3, s) => new ProjectFeaturesListItemVM(p1, p2, p3));
            container.AddTransient<IFeatureVM, IMainFeaturePresenter>((p1, s) => new FeatureVM(p1));
			container.AddTransient<ISwitcherFeatureVM>(s => new SwitcherFeatureVM());
            container.AddTransient<ISwitcherMixBlockVM>(s => new SwitcherMixBlockVM());
            container.AddTransient<ISwitcherCutButtonVM, ISwitcherMixBlocksPresenter, int>((p1, p2, s) => new SwitcherCutButtonVM(p1, p2));
            container.AddTransient<ISwitcherProgramInputVM, ISwitcherMixBlocksPresenter, int, int>((p1, p2, p3, s) => new SwitcherProgramInputVM(p1, p2, p3));
			container.AddTransient<ISwitcherPreviewInputVM, ISwitcherMixBlocksPresenter, int, int>((p1, p2, p3, s) => new SwitcherPreviewInputVM(p1, p2, p3));
			container.AddTransient<ISwitcherConnectionVM, ISwitcherErrorPresenter>((p1, s) => new SwitcherConnectionVM(p1));

			container.AddTransient<ISwitcherConfigVM, ISwitcherConfigPresenter>((p1, s) => new SwitcherConfigVM(p1));
			container.AddTransient<ISwitcherDummyConfigVM, ISwitcherDummyConfigPresenter>((p1, s) => new SwitcherDummyConfigVM(p1));
			container.AddTransient<ISwitcherDummyConfigMixBlockVM, ISwitcherDummyConfigPresenter>((p1, s) => new DummySwitcherConfigMixBlockVM(p1));
			container.AddTransient<ISwitcherATEMConfigVM, ISwitcherATEMConfigPresenter>((p1, s) => new SwitcherATEMConfigVM(p1));
        }
    }
}
