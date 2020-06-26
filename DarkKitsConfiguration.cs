using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace DarkKits
{
    public class DarkKitsConfiguration : IRocketPluginConfiguration
    {
        public uint DefaultGiveKitAmount;
        [XmlArrayItem("Kit")] public List<Kit> Kits;
        [XmlArrayItem("Owner")] public List<TemporaryOwner> TemporaryOwners;
        [XmlArrayItem("Cooldown")] public List<CooldownItem> Cooldowns;

        public void LoadDefaults()
        {
            DefaultGiveKitAmount = 1;
            Kits = new List<Kit>()
            {
                new Kit()
                {
                    Name = "start",
                    Cooldown = 30,
                    XP = 50,
                    Vehicle = new ushort?(0),
                    Items = new List<KitItem>()
                    {
                        new KitItem(15, 2, 1, 100)
                    }
                }
            };
            TemporaryOwners = new List<TemporaryOwner>();
            Cooldowns = new List<CooldownItem>();
        }
    }
}
