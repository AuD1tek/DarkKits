using System;
using System.Xml.Serialization;

namespace DarkKits
{
    [Serializable]
    public class CooldownItem
    {
        [XmlAttribute("key")] public string Key;
        [XmlAttribute("cooldown")] public int Cooldown;
        [XmlAttribute("date")] public DateTime Date;

        public CooldownItem(string key, int cooldown)
        {
            Key = key;
            Cooldown = cooldown;
            Date = DateTime.Now;
        }
        public CooldownItem() { }
    }
}
