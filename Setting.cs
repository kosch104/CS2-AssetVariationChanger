﻿using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;
using Game.UI.Localization;

namespace AssetVariationChanger
{
    [FileLocation(nameof(AssetVariationChanger))]
    public class Setting : ModSetting
    {
        public Setting(IMod mod) : base(mod)
        {
        }

        public bool DummyOption;

        [SettingsUIDropdown(typeof(Setting), nameof(GetStringDropdownItems))]
        public string PreviousKeyDropdown { get; set; } = "leftarrow";
        [SettingsUIDropdown(typeof(Setting), nameof(GetStringDropdownItems))]
        public string NextKeyDropdown { get; set; } = "rightarrow";

        public bool EnableVariationChooser { get; set; } = true;

        public DropdownItem<string>[] GetStringDropdownItems()
        {
            var items = new List<DropdownItem<string>>();

            items.Add(CreateItem("delete", "Delete"));
            items.Add(CreateItem("insert", "Insert"));
            items.Add(CreateItem("pageup", "Page Up"));
            items.Add(CreateItem("pagedown", "Page Down"));
            items.Add(CreateItem("uparrow", "Arrow Up"));
            items.Add(CreateItem("downarrow", "Arrow Down"));
            items.Add(CreateItem("leftarrow", "Arrow Left"));
            items.Add(CreateItem("rightarrow", "Arrow Right"));

            return items.ToArray();
        }

        private DropdownItem<string> CreateItem(string value, string displayName)
        {
            return new DropdownItem<string>()
            {
                value = value,
                displayName = LocalizedString.IdWithFallback($"Konsi.AssetVariationChanger.Keybind.{displayName}", displayName)
            };
        }

        public override void SetDefaults()
        {
            PreviousKeyDropdown = "leftarrow";
            NextKeyDropdown = "rightarrow";
            EnableVariationChooser = true;
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Asset Variation Changer" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PreviousKeyDropdown)), "Keybind - Previous Variation" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.PreviousKeyDropdown)),
                    "This key will be bound to select the previous asset variation (Restart required)"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NextKeyDropdown)), "Keybind - Next Variation" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.NextKeyDropdown)),
                    "This key will be bound to select the next asset variation (Restart required)"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVariationChooser)), "Enable Variation Chooser" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVariationChooser)),
                    "If enabled, the mod actually works. If disabled, the mod does nothing. This is useful if you want the vanilla random behavior"
                },
            };
        }

        public void Unload()
        {
        }
    }
}