using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Data;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.Core.Hosting.Scoping;
using ABCo.Multicam.UI.Presenters;
using ABCo.Multicam.UI.Presenters.Features;
using ABCo.Multicam.UI.Presenters.Features.Switcher;
using ABCo.Multicam.UI.Presenters.Features.Switcher.Config;
using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.ViewModels.Features;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using ABCo.Multicam.UI.ViewModels.Features.Switcher.Config.ATEM;
using ABCo.Multicam.UI.ViewModels.Features.Switcher.Types;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;

namespace ABCo.Multicam.UI
{
	public static class UIStatics
    {
        //public static string Log { get; set; }

        public static void Initialize(IParameteredServiceCollection container)
        {
			// Logging for debug purposes if you want it:
			//container.Initialize(registration => true,
			//    (f, i) => ServiceSource.N += $"Service requested: {i.GetType().Name}.   Thread: {Thread.CurrentThread.ManagedThreadId}\n"
			//);

			// Register presenters
			container.AddSingleton<IMainUIPresenter, MainUIPresenter>();

			container.AddTransient<IProjectFeaturesPresenter, IMainFeatureCollection, IScopeInfo>((p1, p2, s) => new ProjectFeaturesPresenter(p1, p2, s));
            container.AddTransient<IMainFeaturePresenter, IFeature, IScopeInfo> ((p1, p2, s) => new FeaturePresenter(p1, p2, s));
			container.AddTransient<ISwitcherFeaturePresenter, IFeature, IScopeInfo>((p1, p2, s) => new SwitcherFeaturePresenter(p1, s));
			container.AddTransient<ISwitcherConnectionPresenter, IFeature>((p1, s) => new SwitcherConnectionPresenter(p1, s));
			container.AddTransient<ISwitcherErrorPresenter, IFeature, Action>((p1, p2, s) => new SwitcherErrorPresenter(p1, p2, s));
			container.AddTransient<ISwitcherMixBlocksPresenter, ISwitcherFeatureVM, IFeature>((p1, p2, s) => new SwitcherMixBlocksPresenter(p1, p2, s));

			container.AddTransient<ISwitcherConfigPresenter, IFeature, SwitcherConfigType>((p1, p2, s) => new SwitcherConfigPresenter(p1, p2, s));
			container.AddTransient<ISwitcherDummyConfigPresenter, IFeature>((p1, s) => new SwitcherDummyConfigPresenter(p1, s));
			container.AddTransient<ISwitcherATEMConfgPresenter, IFeature>((p1, s) => new SwitcherATEMConfgPresenter(p1, s));

			// Register view-models
            container.AddTransient<IMainUIVM, IMainUIPresenter>((p1, s) => new MainUIVM(p1));
            container.AddTransient<IProjectFeaturesVM, IProjectFeaturesPresenter>((p1, s) => new ProjectFeaturesVM(p1));
            container.AddTransient<IProjectFeaturesListItemVM, IProjectFeaturesPresenter, IFeature, IFeatureVM>((p1, p2, p3, s) => new ProjectFeaturesListItemVM(p1, p2, p3));
            container.AddTransient<IFeatureVM, IMainFeaturePresenter>((p1, s) => new FeatureVM(p1));
			container.AddTransient<ISwitcherFeatureVM, IFeature>((p1, s) => new SwitcherFeatureVM(p1));
            container.AddTransient<ISwitcherMixBlockVM, SwitcherMixBlockVM>();
            container.AddTransient<ISwitcherCutButtonVM, ISwitcherMixBlocksPresenter, int>((p1, p2, s) => new SwitcherCutButtonVM(p1, p2));
            container.AddTransient<ISwitcherProgramInputVM, ISwitcherMixBlocksPresenter, int, int>((p1, p2, p3, s) => new SwitcherProgramInputVM(p1, p2, p3));
			container.AddTransient<ISwitcherPreviewInputVM, ISwitcherMixBlocksPresenter, int, int>((p1, p2, p3, s) => new SwitcherPreviewInputVM(p1, p2, p3));
			container.AddTransient<ISwitcherConnectionVM, ISwitcherErrorPresenter>((p1, s) => new SwitcherConnectionVM(p1));

			container.AddTransient<ISwitcherConfigVM, ISwitcherConfigPresenter>((p1, s) => new SwitcherConfigVM(p1));
			container.AddTransient<ISwitcherDummyConfigVM, ISwitcherDummyConfigPresenter>((p1, s) => new SwitcherDummyConfigVM(p1));
			container.AddTransient<ISwitcherDummyConfigMixBlockVM, ISwitcherDummyConfigPresenter>((p1, s) => new DummySwitcherConfigMixBlockVM(p1));
			container.AddTransient<ISwitcherATEMConfigVM, ISwitcherATEMConfgPresenter>((p1, s) => new SwitcherATEMConfigVM(p1));

			CoreStatics.Initialize(container);
        }
    }
}
