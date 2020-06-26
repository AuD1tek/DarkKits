using System;
using System.Xml.Serialization;

namespace DarkKits
{
    [Serializable]
    public class KitItem
    {
        [XmlAttribute("id")] public ushort ItemId;
        [XmlAttribute("count")] public ushort Count;
        [XmlAttribute("amount")] public byte Amount;
        [XmlAttribute("durability")] public byte Durability;
        [XmlAttribute("state")] public string State;


        public KitItem(ushort itemId, ushort count, byte amount, byte durability)
        {
            ItemId = itemId;
            Count = count;
            Amount = amount;
            Durability = durability;
        }
        public KitItem(ushort itemId, ushort count, byte amount, byte durability, string state) : this(itemId, count, amount, durability)
        {
            if (!string.IsNullOrEmpty(state)) { State = state; }
        }
        public KitItem() { }
    }
}
