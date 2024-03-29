﻿namespace ABCo.Multicam.Client.ViewModels.Paging
{
    public enum AppPages
    {
        Unknown,
        Home,
        Switchers,
        ScriptButtons,
        ScriptConsole,
        Tally,
        CutRecorder,
        Hosting
    }

    public interface IPageVM
    {
        AppPages Page { get; }
    }
}
