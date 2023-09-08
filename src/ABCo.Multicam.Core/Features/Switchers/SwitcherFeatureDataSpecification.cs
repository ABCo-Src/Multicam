namespace ABCo.Multicam.Core.Features.Switchers
{
	public class SwitcherFeatureDataSpecification : FeatureDataSpecification
	{
		public override FeatureDataValue[] Fragments => new FeatureDataValue[] 
		{
			new FeatureDataValue((int)SwitcherFeatureFragmentID.SwitcherConfig, typeof(SwitcherConfig)),
			new FeatureDataValue((int)SwitcherFeatureFragmentID.SwitcherSpecs, typeof(SwitcherSpecs)),
			new FeatureDataValue((int)SwitcherFeatureFragmentID.SwitcherState, typeof(MixBlockState[])),
		};

		public override FeatureAction[] ParameterlessActions => Array.Empty<FeatureAction>();

		public override FeatureActionParam[] ParamedActions => new FeatureActionParam[]
		{
			new FeatureActionParam((int)SwitcherFeatureActionID.SetProgram, (o, ob) => 
			{
				var info = (BusChangeInfo)ob;
				((SwitcherLiveFeature)o).SendProgram(info.MB, info.Val);
			}),
			new FeatureActionParam((int)SwitcherFeatureActionID.SetPreview, (o, ob) =>
			{
				var info = (BusChangeInfo)ob;
				((SwitcherLiveFeature)o).SendPreview(info.MB, info.Val);
			}),
			new FeatureActionParam((int)SwitcherFeatureActionID.Cut, (o, ob) => ((SwitcherLiveFeature)o).Cut((int)ob)),
			new FeatureActionParam((int)SwitcherFeatureActionID.ChangeConfig, (o, ob) => ((SwitcherLiveFeature)o).ChangeSwitcher((SwitcherConfig)ob))
		};
	}

	public record class BusChangeInfo(int MB, int Val);

	public enum SwitcherFeatureFragmentID
	{
		SwitcherConfig,
		SwitcherSpecs,
		SwitcherState
	}

	public enum SwitcherFeatureActionID
	{
		SetProgram,
		SetPreview,
		ChangeConfig,
		Cut
	}
}
