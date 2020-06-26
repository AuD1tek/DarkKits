using System;
using System.Xml.Serialization;

namespace DarkKits
{
    [Serializable]
    public class TemporaryOwner
    {
        [XmlAttribute("key")] public string Key;
        [XmlAttribute("amount")] public uint Amount;

        public TemporaryOwner(string key, uint amount)
        {
            Key = key;
            Amount = amount;
        }
        public TemporaryOwner() { }
    }
}
