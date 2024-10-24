﻿namespace ABCo.Multicam.Server.Features.Switchers
{
    public abstract class SwitcherConfig
    {
        public abstract SwitcherType Type { get; }
    }

    public enum SwitcherType
    {
        Virtual,
        ATEM,
        OBS
    }
}
