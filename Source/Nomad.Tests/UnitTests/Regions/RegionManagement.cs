using System;
using System.Collections.Generic;
using System.Windows;
using Moq;
using Nomad.Regions;
using NUnit.Framework;
using TestsShared;

namespace Nomad.Tests.UnitTests.Regions
{
    public class RegionManagement : UnitTestsFixture
    {
        private const string RegionName = "region name";
        private DependencyObject _view;
        private RegionManager _regionManager;
        private Mock<IRegionFactory> _regionFactoryMock;
        private IRegion _region;

        [SetUp]
        public void SetUp()
        {
            _region = new Mock<IRegion>().Object;
            _view = new DependencyObject();

            _regionFactoryMock = new Mock<IRegionFactory>();
            _regionFactoryMock
                .Setup(factory => factory.CreateRegion(_view))
                .Returns(_region);

            _regionManager = new RegionManager(_regionFactoryMock.Object);

            OnSetUp();
        }

        protected virtual void OnSetUp()
        {
        }
    
        /// <summary>
        /// Describe behavior of empty region manager - no regions should be available, and
        /// trying to get one will result in failure
        /// </summary>
        public class EmptyRegionManager : RegionManagement
        {
            [Test]
            public void contains_no_region()
            {
                Assert.IsFalse(_regionManager.ContainsRegion(RegionName));
            }

            [Test]
            public void get_region_throws_exception()
            {
                Assert.Throws<KeyNotFoundException>(() => _regionManager.GetRegion(RegionName));
            }
        }

        /// <summary>
        /// Describe behavior of region manager with region registered.
        /// Client should be able to retrieve the region, validate it's presence.
        /// It should be impossible to add new region with the same name.
        /// </summary>
        public class SingleRegionAdded : RegionManagement
        {
            private IRegion _returnedRegion;

            protected override void OnSetUp()
            {
                _returnedRegion = _regionManager.AttachRegion(RegionName, _view);
            }

            [Test]
            public void region_manager_creates_region()
            {
                Assert.AreSame(_region, _returnedRegion);
            }

            [Test]
            public void can_retrieve_attached_region()
            {
                Assert.AreSame(_region, _regionManager.GetRegion(RegionName));
            }

            [Test]
            public void can_validate_existence_of_attached_region()
            {
                Assert.IsTrue(_regionManager.ContainsRegion(RegionName));
            }

            [Test]
            public void throws_on_adding_new_region_with_same_name()
            {
                Assert.Throws<ArgumentException>(() => _regionManager.AttachRegion(RegionName, _view));
            }

            [Test]
            public void region_is_not_created_on_adding_new_region_with_same_name()
            {
                var view = new DependencyObject();
                _regionFactoryMock
                    .Setup(factory => factory.CreateRegion(view))
                    .Callback(Assert.Fail)
                    .Returns<DependencyObject>(null);
                Assert.Throws<ArgumentException>(() => _regionManager.AttachRegion(RegionName, view));
            }
        }

        /// <summary>
        /// Validates that region manager can store more than one region
        /// </summary>
        public class TwoRegionsAdded : RegionManagement
        {
            private const string SecondRegionName = "second region name";
            private IRegion _secondRegion;
            private DependencyObject _secondView;

            protected override void OnSetUp()
            {
                _secondView = new DependencyObject();
                _secondRegion = new Mock<IRegion>().Object;
                _regionFactoryMock
                    .Setup(factory => factory.CreateRegion(_secondView))
                    .Returns(_secondRegion);

                _regionManager.AttachRegion(RegionName, _view);
                _regionManager.AttachRegion(SecondRegionName, _secondView);
            }
            
            [Test]
            public void can_retrieve_attached_region()
            {
                Assert.AreSame(_region, _regionManager.GetRegion(RegionName));
            }

            [Test]
            public void can_validate_existence_of_attached_region()
            {
                Assert.IsTrue(_regionManager.ContainsRegion(RegionName));
            }
        }
    }
}