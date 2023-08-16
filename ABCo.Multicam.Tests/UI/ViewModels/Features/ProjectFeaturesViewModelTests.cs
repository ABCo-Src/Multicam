﻿using ABCo.Multicam.Core;
using ABCo.Multicam.Core.Features;
using ABCo.Multicam.Core.Features.Switchers;
using ABCo.Multicam.UI.Bindings;
using ABCo.Multicam.UI.Bindings.Features;
using ABCo.Multicam.UI.Helpers;
using ABCo.Multicam.UI.Services;
using ABCo.Multicam.UI.ViewModels;
using ABCo.Multicam.UI.ViewModels.Features;
using ABCo.Multicam.UI.ViewModels.Features.Switcher;
using Moq;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ABCo.Multicam.Tests.UI.ViewModels.Features
{
    [TestClass]
    public class ProjectFeaturesViewModelTests
    {
        public record struct Mocks(
            Mock<IFeatureManager> Manager,
            Mock<IServiceSource> ServiceSource,
            Mock<IUIDialogHandler> DialogHandler,
            Mock<IVMBinder<IVMForFeatureBinder>>[] RunningFeatures,
            Mock<IFeatureViewModel>[] FeatureVMs
        );

        Action<FeatureTypes> _dialogHandlerCallback = d => { };
        IVMBinder<IVMForFeatureBinder>[] _modelFeatures = Array.Empty<IVMBinder<IVMForFeatureBinder>>();
        Mocks _mocks = new();

        [TestInitialize]
        public void InitMocks()
        {
            _mocks.Manager = new();
            
            _mocks.DialogHandler = new();
            _mocks.DialogHandler
                .Setup(a => a.OpenContextMenu(It.IsAny<ContextMenuDetails<FeatureTypes>>()))
                .Callback<ContextMenuDetails<FeatureTypes>>((details) => _dialogHandlerCallback = details.OnSelect);

            _mocks.RunningFeatures = new Mock<IVMBinder<IVMForFeatureBinder>>[] { new(), new(), new() };
            _mocks.FeatureVMs = new Mock<IFeatureViewModel>[] { new(), new(), new() };
            _modelFeatures = new IVMBinder<IVMForFeatureBinder>[] { _mocks.RunningFeatures[0].Object, _mocks.RunningFeatures[1].Object, _mocks.RunningFeatures[2].Object };

            for (int i = 0; i < 3; i++)
            {
                _mocks.FeatureVMs[i].SetupGet(m => m.Binder).Returns(_mocks.RunningFeatures[i].Object);
                _mocks.RunningFeatures[i].Setup(m => m.GetVM<IFeatureViewModel>(It.IsAny<object>())).Returns(_mocks.FeatureVMs[i].Object);
            }

            _mocks.ServiceSource = new();
            _mocks.ServiceSource.Setup(m => m.Get<IUIDialogHandler>()).Returns(() => _mocks.DialogHandler.Object);
        }

        ProjectFeaturesViewModel Create() => new(_mocks.ServiceSource.Object)
        {
            RawManager = _mocks.Manager.Object
        };

        [TestMethod]
        public void Ctor_InitializesLocal()
        {
            var vm = Create();
            Assert.IsNull(vm.CurrentlyEditing);
        }

        [TestMethod]
        public void CreateFeature_OpensDialog()
        {
            Create().CreateFeature();

            _mocks.DialogHandler.Verify(a => a.OpenContextMenu(It.Is<ContextMenuDetails<FeatureTypes>>(d =>
                d.Title == "Choose Type" &&
                d.OnSelect != null &&
                d.OnCancel == null &&
                d.Items.SequenceEqual(new ContextMenuItem<FeatureTypes>[]
                    {
                        new("Switcher", FeatureTypes.Switcher),
                        new("Tally", FeatureTypes.Tally)
                    })
                ))
            );
        }

        [TestMethod]
        [DataRow(FeatureTypes.Switcher)]
        [DataRow(FeatureTypes.Tally)]
        public void CreateFeature_OnChoose(FeatureTypes type)
        {
            Create().CreateFeature();
            _dialogHandlerCallback(type);
            _mocks.Manager.Verify(m => m.CreateFeature(type), Times.Once);
        }

        [TestMethod]
        public void RawFeatures_UpdatesFeatures()
        {
            var vm = Create();

            vm.RawFeatures = _modelFeatures.ToArray();

            var features = vm.Items!;
            Assert.AreEqual(3, features.Length);

            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(_mocks.FeatureVMs[i].Object, features[i]);
                _mocks.RunningFeatures[i].Verify(m => m.GetVM<IFeatureViewModel>(vm), Times.Once);
            }
        }

        [TestMethod]
        public void CurrentlyEditing_RemoveNonEditing()
        {
            var vm = Create();
            vm.RawFeatures = _modelFeatures;
            vm.CurrentlyEditing = vm.Items!.First();
            vm.RawFeatures = new IVMBinder<IVMForFeatureBinder>[] { _mocks.RunningFeatures[0].Object, _mocks.RunningFeatures[2].Object };
            Assert.IsNotNull(vm.CurrentlyEditing);
        }

        [TestMethod]
        public void CurrentlyEditing_RemoveEditing()
        {
            var vm = Create();
            vm.RawFeatures = _modelFeatures;
            vm.CurrentlyEditing = vm.Items!.ToArray()[1];
            vm.RawFeatures = new IVMBinder<IVMForFeatureBinder>[] { _mocks.RunningFeatures[0].Object, _mocks.RunningFeatures[2].Object };
            Assert.IsNull(vm.CurrentlyEditing);
        }

        [TestMethod]
        public void CurrentlyEditing_NoPreviousItem()
        {
            var vm = Create();
            vm.RawFeatures = _modelFeatures;
            vm.CurrentlyEditing = vm.Items!.First();
            _mocks.FeatureVMs[0].VerifySet(m => m.IsEditing = true);
        }

        [TestMethod]
        public void CurrentlyEditing_RemoveItem()
        {
            var vm = Create();
            vm.RawFeatures = _modelFeatures;

            vm.CurrentlyEditing = vm.Items!.First();
            vm.CurrentlyEditing = null;

            _mocks.FeatureVMs[0].VerifySet(m => m.IsEditing = false);
        }

        [TestMethod]
        public void CurrentlyEditing_ReplaceItem()
        {
            var vm = Create();
            vm.RawFeatures = _modelFeatures;

            var items = vm.Items!.ToArray();
            vm.CurrentlyEditing = items[0];
            vm.CurrentlyEditing = items[1];

            _mocks.FeatureVMs[0].VerifySet(m => m.IsEditing = false);
            _mocks.FeatureVMs[1].VerifySet(m => m.IsEditing = true);
        }

        [TestMethod]
        public void ShowEditingPanel_NotEditing()
        {
            Assert.IsFalse(Create().ShowEditingPanel);
        }

        [TestMethod]
        public void ShowEditingPanel_Editing()
        {
            var vm = Create();
            vm.RawFeatures = _modelFeatures;
            vm.CurrentlyEditing = vm.Items!.First();
            Assert.IsTrue(vm.ShowEditingPanel);
        }
    }
}
