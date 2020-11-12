using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Veneka.Indigo.Integration.Objects
{
    [Serializable]
    public class TerminalSessionKey
    {
        public string RandomKey { get; set; }
        public string RandomKeyUnderLMK { get; set; }

        public TerminalSessionKey() { }

        public TerminalSessionKey(string randomKey, string randomKeyUnderLMK)
        {
            this.RandomKey = randomKey;
            this.RandomKeyUnderLMK = randomKeyUnderLMK;
        }
    }
}
