using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Microsoft.Practices.ServiceLocation;
using Hotwire.Shared.Interfaces;

namespace Hotwire.Shared
{
    [DataContract]
    public class Machine
    {
        private static Hotwire.Shared.Interfaces.IEncryptionService _encryptionService;
        static Machine()
        {
            _encryptionService = ServiceLocator.Current.GetInstance<IEncryptionService>();

            if (_encryptionService == null)
                throw new NullReferenceException("Unable to resolve encryption service");
        }

        /// <summary>
        /// Replaces static encryption service *FOR TESTING ONLY*
        /// </summary>
        /// <param name="encryptionService">New service</param>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void SetEncryptionService(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        [DataMember]
        public string Identifier { get; private set; }

        [DataMember]
        public string MachineName { get; set; }

        [DataMember]
        private string EncryptedUrl { get; set; }
        public string Url
        {
            get
            {
                return _encryptionService.Decrypt(EncryptedUrl);
            }
            set
            {
                EncryptedUrl = _encryptionService.Encrypt(value);
            }
        }

        [DataMember]
        private string EncryptedUserName { get; set; }
        public string Username
        {
            get
            {
                return _encryptionService.Decrypt(EncryptedUserName);
            }
            set
            {
                EncryptedUserName = _encryptionService.Encrypt(value);
            }
        }

        [DataMember]
        private string EncryptedPassword { get; set; }
        public string Password
        {
            get
            {
                return _encryptionService.Decrypt(EncryptedPassword);
            }
            set
            {
                EncryptedPassword = _encryptionService.Encrypt(value);
            }
        }

        [DataMember]
        private string EncryptedDomain { get; set; }
        public string Domain
        {
            get
            {
                return _encryptionService.Decrypt(EncryptedDomain);
            }
            set
            {
                EncryptedDomain = _encryptionService.Encrypt(value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the Machine class.
        /// </summary>
        public Machine()
        {
            Identifier = Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            return this.MachineName;
        }
    }
}
