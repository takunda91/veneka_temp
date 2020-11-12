using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Veneka.Indigo.Integration.Objects
{
    [Serializable]
    [DataContract]
    public class Parameters
    {
        #region Enums
        public enum protocol
        {
            HTTP,
            HTTPS,
            TCPIP,
        }

        public enum authType
        {
            None,
            Basic
        }
        #endregion

        public int IssuerId { get; private set; }

        public string Path { get; private set; }
        public string FileName { get; private set; }

        public bool DeleteFile { get; private set; }
        public int FileEncryption { get; private set; }
        public bool DuplicateFileCheck { get; private set; }

        public string Address { get; private set; }
        public int Port { get; private set; }
        public protocol? Protocol { get; private set; }
        public authType? AuthType { get; private set; }

        public int HeaderLength { get; private set; }
        public string Identification { get; private set; }

        public int? Timeout { get; private set; }
        public int? BufferSize { get; private set; }

        public char HsmDocumentType { get; private set; }

        public string Username { get; private set; }
        public string Password { get; private set; }
        public string PrivateKey { get; private set; }
        public string PublicKey { get; private set; }

        public Parameters() { }
        public Parameters(int issuerId, string path, string fileName, bool deleteFile, int fileEncryption, bool duplicateCheck, string address, int port, protocol? prot, authType? authenticationType,
                            string username, string password,  int headerLength, string identification,
                            int? timeout, int? bufferSize, char hsmDocumentType, string privateKey, string publicKey)
        {
            this.IssuerId = issuerId;
            this.FileName = fileName;

            this.DeleteFile = deleteFile;
            this.FileEncryption = fileEncryption;
            this.DuplicateFileCheck = duplicateCheck;

            this.Path = path;
            this.Address = address;
            this.Port = port;
            this.Protocol = prot;
            this.AuthType = authenticationType;
            this.Username = username;
            this.Password = password;
            this.HeaderLength = headerLength;
            this.Identification = identification;
            this.Timeout = timeout;
            this.BufferSize = bufferSize;
            this.HsmDocumentType = hsmDocumentType;
            this.PrivateKey = privateKey;
            this.PublicKey = publicKey;
        }
        public Parameters( string path, string address, int port, protocol? prot, authType? authenticationType,
                         string username, string password,
                         int? timeout )
        {
           

            this.Path = path;
            this.Address = address;
            this.Port = port;
            this.Protocol = prot;
            this.AuthType = authenticationType;
            this.Username = username;
            this.Password = password;
          
            this.Timeout = timeout;
          
        }
    }
}
