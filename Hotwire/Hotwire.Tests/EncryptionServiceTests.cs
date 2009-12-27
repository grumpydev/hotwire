using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hotwire.Shared;
using Hotwire.Shared.Interfaces;

namespace Hotwire.Tests
{
    [TestClass]
    public class EncryptionServiceTests
    {
        private ISystemInformationService _realSystemInformation;
        private ISystemInformationService _fakeSystemInformation;

        [TestInitialize]
        public void TestInitialize()
        {
            _realSystemInformation = new Hotwire.Shared.Services.SystemInformationService();

            var fakeSystemInformation = new Moq.Mock<ISystemInformationService>();
            fakeSystemInformation.Setup(o => o.CpuId).Returns("AAAAAAAAAAAAAAAA");
            fakeSystemInformation.Setup(o => o.MacAddress).Returns("AAAAAAAAAAAA");
            fakeSystemInformation.Setup(o => o.DriveSerial).Returns("AAAAAAAA");
            _fakeSystemInformation = fakeSystemInformation.Object;
        }

        [TestMethod]
        public void Encrypt_InputString_ReturnsEncryptedString()
        {
            var service = new Hotwire.Shared.Services.EncryptionService(_realSystemInformation);
            var plainText = "Testing String";

            var encText = service.Encrypt(plainText);

            Assert.AreNotEqual(plainText, encText);
        }

        [TestMethod]
        public void Decrypt_EncryptedStringRealSystemInformation_ReturnsCorrectPlainText()
        {
            var service = new Hotwire.Shared.Services.EncryptionService(_realSystemInformation);
            var plainText = "Testing String";
            var encText = service.Encrypt(plainText);

            var decText = service.Decrypt(encText);

            Assert.AreEqual(plainText, decText);
        }

        [TestMethod]
        public void Decrypt_EncryptedStringFakeSystemInformation_ReturnsInvalidPlainText()
        {
            var realService = new Hotwire.Shared.Services.EncryptionService(_realSystemInformation);
            var fakeService = new Hotwire.Shared.Services.EncryptionService(_fakeSystemInformation);
            var plainText = "Testing String";
            var encText = realService.Encrypt(plainText);

            var decText = fakeService.Decrypt(encText);

            Assert.AreNotEqual(plainText, decText);
        }
    }
}
