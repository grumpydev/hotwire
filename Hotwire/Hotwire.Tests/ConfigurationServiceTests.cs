using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hotwire.Shared;
using Hotwire.Shared.Services;
using Microsoft.Practices.ServiceLocation;
using Moq;
using Hotwire.Shared.Interfaces;

namespace Hotwire.Tests
{
    [TestClass]
    public class ConfigurationServiceTests
    {

        private void SetupRealEncryption()
        {
            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(e => e.GetInstance<IEncryptionService>()).Returns(new EncryptionService(new SystemInformationService()));
            IServiceLocator serviceLocator = serviceLocatorMock.Object;

            ServiceLocator.SetLocatorProvider(new ServiceLocatorProvider(() => serviceLocator));

            Machine.SetEncryptionService(new EncryptionService(new SystemInformationService()));
        }

        private void SetupFakeEncryption()
        {
            var fakeSystemInformation = new Moq.Mock<ISystemInformationService>();
            fakeSystemInformation.Setup(o => o.CpuId).Returns("AAAAAAAAAAAAAAAA");
            fakeSystemInformation.Setup(o => o.MacAddress).Returns("AAAAAAAAAAAA");
            fakeSystemInformation.Setup(o => o.DriveSerial).Returns("AAAAAAAA");

            var serviceLocatorMock = new Mock<IServiceLocator>();
            serviceLocatorMock.Setup(e => e.GetInstance<IEncryptionService>()).Returns(new EncryptionService(fakeSystemInformation.Object));
            IServiceLocator serviceLocator = serviceLocatorMock.Object;

            ServiceLocator.SetLocatorProvider(new ServiceLocatorProvider(() => serviceLocator));

            Machine.SetEncryptionService(new EncryptionService(fakeSystemInformation.Object));
        }

        [TestMethod]
        public void Configuration_MachineSavedThenLoaded_LoadsMachineCorrectly()
        {
            SetupRealEncryption();

            var machines = new List<Machine>();
            machines.Add(new Machine() { MachineName = "Test Machine", Password = "password", Url = "http://www.test.com/", Username = "username" });
            var configService = new ConfigurationService();

            configService.SaveConfiguration(machines);
            var savedMachines = configService.LoadConfiguration();

            Assert.AreEqual(machines.FirstOrDefault().MachineName, savedMachines.FirstOrDefault().MachineName);
            Assert.AreEqual(machines.FirstOrDefault().Password, savedMachines.FirstOrDefault().Password);
            Assert.AreEqual(machines.FirstOrDefault().Url, savedMachines.FirstOrDefault().Url);
            Assert.AreEqual(machines.FirstOrDefault().Username, savedMachines.FirstOrDefault().Username);
        }

        [TestMethod]
        public void Configuration_MachineSavedThenLoadedWithDifferentHardware_LoadsIncorrectMachine()
        {
            SetupRealEncryption();
            var machines = new List<Machine>();
            string testMachineName = "Test Machine";
            string testPassword = "password";
            string testUrl = "http://www.test.com/";
            string testUsername = "username";
            machines.Add(new Machine() { MachineName = testMachineName, Password = testPassword, Url = testUrl, Username = testUsername });
            var configService = new ConfigurationService();
            configService.SaveConfiguration(machines);
            SetupFakeEncryption();

            var savedMachines = configService.LoadConfiguration();

            Assert.AreEqual(testMachineName, savedMachines.FirstOrDefault().MachineName);
            Assert.AreNotEqual(testPassword, savedMachines.FirstOrDefault().Password);
            Assert.AreNotEqual(testUrl, savedMachines.FirstOrDefault().Url);
            Assert.AreNotEqual(testUsername, savedMachines.FirstOrDefault().Username);
        }

        [TestMethod]
        public void ConfigurationAppearsValid_ValidMachines_ReturnsTrue()
        {
            SetupRealEncryption();
            var machines = new List<Machine>();
            machines.Add(new Machine() { MachineName = "Test Machine", Password = "password", Url = "http://www.test.com/", Username = "username" });
            machines.Add(new Machine() { MachineName = "Test Machine2", Password = "password", Url = "http://www.test.com/", Username = "username" });
            var configService = new ConfigurationService();

            var result = configService.ConfigurationAppearsValid(machines);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ConfigurationAppearsValid_InvalidMachine_ReturnsFalse()
        {
            SetupRealEncryption();
            var machines = new List<Machine>();
            machines.Add(new Machine() { MachineName = "Test Machine", Password = "password", Url = "http://www.test.com/", Username = "username" });
            machines.Add(new Machine() { MachineName = "Test Machine2", Password = "password", Url = "dummytext", Username = "username" });
            var configService = new ConfigurationService();

            var result = configService.ConfigurationAppearsValid(machines);

            Assert.IsFalse(result);
        }
    }
}
