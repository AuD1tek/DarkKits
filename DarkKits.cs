using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DarkKits
{
    public class DarkKits : RocketPlugin<DarkKitsConfiguration>
    {
        public static DarkKits Instance = null;
        internal static uint DefaultGiveKitAmount;


        #region Load/Unload/TranslateList
        protected override void Load()
        {
            Instance = this;
            DefaultGiveKitAmount = Instance.Configuration.Instance.DefaultGiveKitAmount;
        }
        protected override void Unload()
        {

        }
        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "command_invalide", "Команда не правильна" },
            { "command_exception", "Что-то пошло не так. (посмотрите в консоль)" },
            { "command_player_not_found", "Игрок не найден" },

            { "command_kits_kits_separator", " , " },
            { "command_kits_kits_cooldown", "Киты не готовы: {0}" },
            { "command_kits_kits_ready", "Киты готовы: {0}" },
            { "command_kits_not_kits", "У вас нету ни одного доступного кита." },

            { "kits_cooldown_entry", "{0} ({1})" },
            { "kits_cooldown_entry_temporary", "{0} ({1} шт.) ({2})" },
            { "kits_ready_entry", "{0}" },
            { "kits_ready_entry_temporary", "{0} ({1} шт.)" },

            { "command_kit_syntax", "{0}" },
            { "command_kit_successful", "Кит: {0} получен" },
            { "command_kit_successful_temporary", "Кит: {0} получен (осталось {1} шт.)" },
            { "command_kit_not_found", "Кит: {0} не найден." },
            { "command_kit_no_permissions", "У вас не достаточно прав." },
            { "command_kit_cooldown", "Подождите {0} чтобы использовать повторно" },

            { "command_createkit_syntax", "{0}" },
            { "command_createkit_successful", "Кит {0} был успешно сознан (перезарядка: {1}, опыт: {2}, транспорт: {3})" },
            { "command_createkit_name_exist", "Кит с таким именем уже существует" },
            { "command_createkit_failed", "Не удалось создать кит." },

            { "command_kitclearkd_syntax", "{0}" },
            { "command_kitclearkd_no_permissions_other", "У вас не достаточно прав." },
            { "command_kitclearkd_not_found", "Кит: {0} не найден." },
            { "command_kitclearkd_ready", "Кит: {0} готов, убрано ({1})" },
            { "command_kitclearkd_no_cooldown", "У кита {0} нету перезарядки" },
            { "command_kitclearkd_separator", " , " },
            { "command_kitclearkd_cleared", "Киты: {0} - готовы." },
            { "command_kitclearkd_cleared_entry", "{0} ({1})" },

            { "command_givekit_syntax", "{0}" },
            { "command_givekit_no_permissions_other", "У вас не достаточно прав." },
            { "command_givekit_not_found", "Кит: {0} не найден." },
            { "command_givekit_successful_add", "Кит: {0} успешно добавлено {1} шт. игроку {2} ({3} шт.)" },
            { "command_givekit_successful_add_toplayer", "Вам добавили кит(ы): {0} ({1} шт.) от {2}" },
            { "command_givekit_successful_new_toplayer", "Вы получили кит: {0} ({1} шт.) от {2}" },
            { "", "" },
            { "command_givekit_successful_new", "Кит: {0} успешно выдан игроку {1} ({2} шт.)" },
            { "command_givekit_console_name", "Administration" },
            { "command_givekit_invalide_amount", "Количество должно быть больше чем 0" },

            { "command_kithelp_syntax", "{0}" },
            { "command_kithelp_info_entry", "{0}" },
            { "command_kithelp_info_command_kit", "{0} - получить кит" },
            { "command_kithelp_info_command_kits", "/kits - узнать какие киты доступны" },
            { "command_kithelp_info_command_createkit", "{0} - создать кит" },
            { "command_kithelp_info_command_givekit", "{0} - выдать кит определенное кол-во" },
            { "command_kithelp_info_command_kitclearkd", "{0} - убрать перезарядку у кита/ов" },

            { "cooldown_string_time", "{0}{1}{2}{3}" },
            { "cooldown_string_days", "{0} д. " },
            { "cooldown_string_hours", "{0} ч. " },
            { "cooldown_string_minutes", "{0} м. " },
            { "cooldown_string_seconds", "{0} с. " },
        };
        #endregion


        #region SupportMethods

        #region Group1: Get Player kit
        public static bool GivePlayerKit(UnturnedPlayer player, Kit kit) // выдать кит игроку
        {
            bool result = false;
            try
            {
                if (kit.Vehicle != null)
                {
                    try { player.GiveVehicle(kit.Vehicle.Value); }
                    catch (Exception ex) { Rocket.Core.Logging.Logger.LogException(ex, $"Falied giving vehicle {kit.Vehicle.Value} to player"); }
                }
                if (kit.XP != null && kit.XP.HasValue)
                {
                    player.Experience += kit.XP.Value;
                }
                givePlayerKitItems(player, kit.Items);
                result = true;
            }
            catch (Exception ex) { Rocket.Core.Logging.Logger.LogException(ex, "GetPlayerKit > "); }
            return result;
        }


        private static void givePlayerKitItems(UnturnedPlayer player, List<KitItem> items) // Выдать предметы игроку
        {
            if (items != null)
            {
                foreach (KitItem item in items)
                {
                    Item it = new Item(item.ItemId, item.Amount, item.Durability);
                    if (!string.IsNullOrEmpty(item.State)) { it.state = Convert.FromBase64String(item.State); }
                    for (int i = 0; i < item.Count; i++)
                    {
                        if (!player.Inventory.tryAddItem(it, true))
                        {
                            ItemManager.dropItem(it, player.Position, true, true, false);
                        }
                    }
                }
            }
        }
        #endregion


        #region Group2: Get all items from Player inventory
        public static bool GetPlayerKitItems(Player player, List<KitItem> items)
        {
            List<ItemInfo> playerClothes = getClothes(player.clothing);
            Dictionary<byte, List<ItemInfo>> playerItems = getItems(player.inventory);

            foreach (ItemInfo cloth in playerClothes)
            {
                KitItem kitItem = new KitItem(cloth.Id, cloth.Count, cloth.Amount, cloth.Durability);
                if (cloth.State?.Length > 0)
                {
                    kitItem.State = Convert.ToBase64String(cloth.State);
                }
                items.Add(kitItem);
            }

            foreach (KeyValuePair<byte, List<ItemInfo>> pair in playerItems)
            {
                if (pair.Value.Count != 0)
                {
                    foreach (ItemInfo item in pair.Value)
                    {
                        KitItem kitItem = new KitItem(item.Id, item.Count, item.Amount, item.Durability);
                        if (item.State?.Length > 0)
                        {
                            kitItem.State = Convert.ToBase64String(item.State);
                        }
                        items.Add(kitItem);
                    }
                }
            }

            return items != null;
        } // Получить KitItems с игрока


        private static List<ItemInfo> getClothes(PlayerClothing clothing)
        {
            List<ItemInfo> items = new List<ItemInfo>();

            if (clothing.backpack != 0)
            {
                ItemInfo item = new ItemInfo(clothing.backpack, 1, 1, clothing.backpackQuality, clothing.backpackState);
                items.Add(item);
            }
            if (clothing.glasses != 0)
            {
                ItemInfo item = new ItemInfo(clothing.glasses, 1, 1, clothing.glassesQuality, clothing.glassesState);
                items.Add(item);
            }
            if (clothing.hat != 0)
            {
                ItemInfo item = new ItemInfo(clothing.hat, 1, 1, clothing.hatQuality, clothing.hatState);
                items.Add(item);
            }
            if (clothing.mask != 0)
            {
                ItemInfo item = new ItemInfo(clothing.mask, 1, 1, clothing.maskQuality, clothing.maskState);
                items.Add(item);
            }
            if (clothing.pants != 0)
            {
                ItemInfo item = new ItemInfo(clothing.pants, 1, 1, clothing.pantsQuality, clothing.pantsState);
                items.Add(item);
            }
            if (clothing.shirt != 0)
            {
                ItemInfo item = new ItemInfo(clothing.shirt, 1, 1, clothing.shirtQuality, clothing.shirtState);
                items.Add(item);
            }
            if (clothing.vest != 0)
            {
                ItemInfo item = new ItemInfo(clothing.vest, 1, 1, clothing.vestQuality, clothing.vestState);
                items.Add(item);
            }

            return items;
        } // Получить всю одежду

        private static Dictionary<byte, List<ItemInfo>> getItems(PlayerInventory inventory)
        {
            Dictionary<byte, List<ItemInfo>> items = new Dictionary<byte, List<ItemInfo>>();
            items.Add(1, new List<ItemInfo>()); // Для первого и второго слота
            items.Add(2, new List<ItemInfo>());
            items.Add(3, new List<ItemInfo>());
            items.Add(4, new List<ItemInfo>());
            items.Add(5, new List<ItemInfo>());
            items.Add(6, new List<ItemInfo>());
            items.Add(7, new List<ItemInfo>());

            ItemJar item1 = inventory.getItem(0, 0);
            if (item1 != null)
            {
                items[1].Add(new ItemInfo(item1.item.id, 1, item1.item.amount, item1.item.durability, item1.item.metadata));
            }
            ItemJar item2 = inventory.getItem(1, 0);
            if (item2 != null)
            {
                items[1].Add(new ItemInfo(item2.item.id, 1, item2.item.amount, item2.item.durability, item2.item.metadata));
            }

            for (byte page = 2; page < PlayerInventory.PAGES - 1; page++)
            {
                byte pageItemCount = inventory.getItemCount(page);
                byte count = 1;
                if (pageItemCount > 0)
                {
                    for (byte b = 0; b < pageItemCount; b++)
                    {
                        ItemJar item = inventory.getItem(page, b);
                        ItemJar nextItem = null;
                        ushort id = item.item.id;
                        bool canAdd = false;
                        if ((b + 1) < pageItemCount && (nextItem = inventory.getItem(page, (byte)(b + 1))) != null && nextItem.item.id == id && item.item.metadata.SequenceEqual(nextItem.item.metadata))
                        {
                            count++;
                        }
                        else
                        {
                            canAdd = true;
                        }
                        if (canAdd)
                        {
                            items[page].Add(new ItemInfo(item.item.id, count, item.item.amount, item.item.durability, item.item.metadata));
                            item = null;
                            nextItem = null;
                            count = 1;
                        }
                    }
                }
            }


            return items;
        } // Получить все предметы

        #endregion


        #region Group3: Supports
        public static bool TryGetKit(string nameKit, out Kit kit)
        {
            return (kit = Instance.Configuration.Instance.Kits.FirstOrDefault(k => k.Name.ToLower() == nameKit.ToLower())) != null;
        } // Получить кит по имени
        public static bool IsKitExists(string nameKit)
        {
            return TryGetKit(nameKit, out Kit kit);
        } // Существует ли указаный кит

        public static bool IsKitCooldown(string key, out CooldownItem cooldown)
        {
            bool result = false;
            cooldown = Instance.Configuration.Instance.Cooldowns.FirstOrDefault(c => c.Key == key);
            if (cooldown != null)
            {
                if ((DateTime.Now - cooldown.Date).TotalSeconds >= cooldown.Cooldown)
                {
                    Instance.Configuration.Instance.Cooldowns.Remove(cooldown);
                    Instance.Configuration.Save();
                }
                else
                {
                    result = true;
                }
            }
            return result;
        } // Проверить являеться ли 

        public static bool CanUseTemporaryKit(string key, out TemporaryOwner owner) // Проверить есть ли возможность использоваьб кит
        {
            bool result = false;
            owner = Instance.Configuration.Instance.TemporaryOwners.FirstOrDefault(o => o.Key == key);
            if (owner != null)
            {
                if (owner.Amount > 0)
                {
                    result = true;
                }
                else
                {
                    Instance.Configuration.Instance.TemporaryOwners.Remove(owner);
                    Instance.Configuration.Save();
                }
            }
            return result;
        }

        public static void UseTemporaryKit(TemporaryOwner owner)
        {
            if (owner != null && Instance.Configuration.Instance.TemporaryOwners.Contains(owner))
            {
                owner.Amount--;
                Instance.Configuration.Save();
            }
        } // использовать кит

        public static string GetTimeFromCooldown(CooldownItem cooldown)
        {
            if (cooldown == null) { return "0"; }

            int timeSeconds = int.Parse((cooldown.Cooldown - (DateTime.Now - cooldown.Date).TotalSeconds).ToString("0"));
            TimeSpan time = TimeSpan.FromSeconds(timeSeconds);

            string days = time.Days > 0 ? Instance.Translate("cooldown_string_days", time.Days) : string.Empty;
            string hours = time.Hours > 0 ? Instance.Translate("cooldown_string_hours", time.Hours) : string.Empty;
            string minutes = time.Minutes > 0 ? Instance.Translate("cooldown_string_minutes", time.Minutes) : string.Empty;
            string seconds = time.Seconds > 0 ? Instance.Translate("cooldown_string_seconds", time.Seconds) : string.Empty;

            return Instance.Translate("cooldown_string_time", days, hours, minutes, seconds);
        } // Получить время строкой из секунд

        public static bool GetPlayerKits(UnturnedPlayer player, List<string> readyKits, List<string> cooldownKits, List<string> readyKitNames = null, List<string> cooldownKitNames = null) // Получить возможные киты
        {
            foreach (Kit kit in Instance.Configuration.Instance.Kits)
            {
                string key = player.CSteamID.ToString() + kit.Name;
                string entry = string.Empty;
                if (player.HasPermission("kit.*") || player.HasPermission("kit." + kit.Name.ToLower()))
                {
                    if (IsKitCooldown(key, out CooldownItem cooldown))
                    {
                        entry = Instance.Translate("kits_cooldown_entry", kit.Name, GetTimeFromCooldown(cooldown));
                        cooldownKits?.Add(entry);
                        cooldownKitNames?.Add(kit.Name);
                    }
                    else
                    {
                        entry = Instance.Translate("kits_ready_entry", kit.Name);
                        readyKits?.Add(entry);
                        readyKitNames?.Add(kit.Name);
                    }
                }
                else if (CanUseTemporaryKit(key, out TemporaryOwner owner))
                {
                    if (IsKitCooldown(key, out CooldownItem cooldown))
                    {
                        entry = Instance.Translate("kits_cooldown_entry_temporary", kit.Name, owner.Amount, GetTimeFromCooldown(cooldown));
                        cooldownKits?.Add(entry);
                        cooldownKitNames?.Add(kit.Name);
                    }
                    else
                    {
                        entry = Instance.Translate("kits_ready_entry_temporary", kit.Name, owner.Amount);
                        readyKits?.Add(entry);
                        readyKitNames?.Add(kit.Name);
                    }
                }
            }
            return readyKits?.Count > 0 || cooldownKits?.Count > 0;
        }
        #endregion

        #endregion


        #region Additional Classes

        #region Class1: ItemInfo
        public class ItemInfo
        {
            public ushort Id { get; set; }
            public ushort Count { get; set; }
            public byte Amount { get; set; }
            public byte Durability { get; set; }
            public byte[] State { get; set; } = null;

            public ItemInfo(ushort id, ushort count, byte amount, byte durability)
            {
                Id = id;
                Count = count;
                Amount = amount;
                Durability = durability;
            }
            public ItemInfo(ushort id, ushort count, byte amount, byte durability, byte[] state) : this(id, count, amount, durability)
            {
                State = state;
            }
        }
        #endregion

        #endregion
    }
}
