﻿using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Switchers;
using ABCo.Multicam.Core.Switchers.Types;
using ABCo.Multicam.UI.Enumerations;
using ABCo.Multicam.UI.ViewModels.Strips;
using ABCo.Multicam.UI.ViewModels.Strips.Switcher;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.ViewModels.Strips.Switcher
{
    [TestClass]
    public class SwitcherStripViewModelTests
    {
        class Dummy : SwitcherStripViewModel
        {
            public IServiceSource Source => _serviceSource;
            public Dummy(ISwitcherRunningStrip strip, IServiceSource serviceSource, IProjectStripsViewModel parent) : base(strip, serviceSource, parent) { }
        }

        [TestMethod]
        public void CtorAndRunningStrip()
        {
            var parent = Mock.Of<IProjectStripsViewModel>();
            var serviceSource = Mock.Of<IServiceSource>();
            var model = Mock.Of<ISwitcherRunningStrip>();
            var vm = new Dummy(model, serviceSource, parent);

            Assert.AreEqual(parent, vm.Parent);
            Assert.AreEqual(serviceSource, vm.Source);
            Assert.AreEqual(model, vm.BaseStrip);
            Assert.IsNotNull(vm.MixBlocks);
        }

        [TestMethod]
        public void ContentView()
        {
            var vm = new SwitcherStripViewModel(Mock.Of<ISwitcherRunningStrip>(), Mock.Of<IServiceSource>(), Mock.Of<IProjectStripsViewModel>());
            Assert.AreEqual(StripViewType.Switcher, vm.ContentView);
        }
    }
}