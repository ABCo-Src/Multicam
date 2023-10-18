﻿using ABCo.Multicam.Server.Hosting.Clients;

namespace ABCo.Multicam.Server.Features.Switchers.Data
{
	public class SwitcherState : ServerData
	{
		public MixBlockState[] Data { get; }
		public SwitcherState(MixBlockState[] data) => Data = data;
	}

	public record struct MixBlockState(int Prog, int Prev);
}