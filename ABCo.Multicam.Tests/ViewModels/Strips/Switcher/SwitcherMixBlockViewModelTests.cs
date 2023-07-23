﻿using ABCo.Multicam.Core;
using ABCo.Multicam.UI.Helpers;
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
    public class SwitcherMixBlockViewModelTests
    {
        [TestMethod]
        public void Ctor_ThrowsWithNoServiceSource() => Assert.ThrowsException<ServiceSourceNotGivenException>(() => new SwitcherMixBlockViewModel(null!, Mock.Of<ISwitcherStripViewModel>()));

        [TestMethod]
        public void Ctor()
        {
            var parent = Mock.Of<ISwitcherStripViewModel>();
            var serviceSource = Mock.Of<IServiceSource>();
            var vm = new SwitcherMixBlockViewModel(serviceSource, parent);

            Assert.AreEqual(parent, vm.Parent);
            Assert.IsNotNull(vm.ProgramBus);
            Assert.IsNotNull(vm.PreviewBus);
            Assert.IsNotNull(vm.CutButton);
            Assert.IsNotNull(vm.AutoButton);

            Assert.AreEqual("Cut", vm.CutButton.Text);
            Assert.AreEqual("Auto", vm.AutoButton.Text);
        }
    }
}