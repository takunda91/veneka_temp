using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Veneka.Indigo.UX.NativeAppAPI
{
    public enum PINBlockFormats
    {
        ISO_0,
        ISO_1,
    }

    public enum CardInterfaces
    {
        MagStripe,
        Chip_EMV
    }

    [DataContract]
    public class CardData
    {
        private int? _productId;
        private int? _branchId;
        private CardInterfaces? _cardInterface;
        private string _track2;
        private bool? _isTrack2Encrypted;
        private string _pan;
        private bool? _isPanEncrypted;
        private PINBlockFormats? _pinBlockFormat;
        private string _pinBlock;

        public void CheckNullSet<T>(T field)
        {
            if (EqualityComparer<T>.Default.Equals(field, default(T)))
            {
                return;
            }

            throw new Exception("Property already set.");
        }

        [DataMember]
        public int? ProductId
        {
            get { return _productId; }
            set
            {
                CheckNullSet<int?>(_productId);
                _productId = value;
            }
        }

        [DataMember]
        public int? BranchId
        {
            get { return _branchId; }
            set
            {
                CheckNullSet<int?>(_branchId);
                _branchId = value;
            }
        }

        [DataMember()]
        public CardInterfaces CardInterface
        {
            get { return _cardInterface ?? CardInterfaces.MagStripe; }
            set
            {
                CheckNullSet<CardInterfaces?>(_cardInterface);
                _cardInterface = value;
            }
        }

        [DataMember]
        public string Track2
        {
            get { return _track2; }
            set
            {
                CheckNullSet<string>(_track2);
                _track2 = value;
            }
        }

        [DataMember(IsRequired = true)]
        public bool IsTrack2Encrypted
        {
            get { return _isTrack2Encrypted ?? false; }
            set
            {
                CheckNullSet<bool?>(_isTrack2Encrypted);
                _isTrack2Encrypted = value;
            }
        }

        [DataMember]
        public string PAN
        {
            get { return _pan; }
            set
            {
                CheckNullSet<string>(_pan);
                _pan = value;
            }
        }

        [DataMember(IsRequired = true)]
        public bool IsPANEncrypted
        {
            get { return _isPanEncrypted ?? false; }
            set
            {
                CheckNullSet<bool?>(_isPanEncrypted);
                _isPanEncrypted = value;
            }
        }

        [DataMember(IsRequired = true)]
        public PINBlockFormats PINBlockFormat
        {
            get { return _pinBlockFormat ?? PINBlockFormats.ISO_0; }
            set
            {
                CheckNullSet<PINBlockFormats?>(_pinBlockFormat);
                _pinBlockFormat = value;
            }
        }

        [DataMember]
        public string PINBlock
        {
            get { return _pinBlock; }
            set
            {
                CheckNullSet<string>(_pinBlock);
                _pinBlock = value;
            }
        }
    }
}
