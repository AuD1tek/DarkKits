using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DarkKits
{
    [Serializable]
    public class Kit
    {
        public string Name;
        public int Cooldown;
        public uint? XP;
        public ushort? Vehicle;
        [XmlArrayItem("Item")] public List<KitItem> Items;
    }
}
