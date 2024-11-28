using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;
using Game.Input;
using Game.UI.Localization;
using UnityEngine.InputSystem;

namespace AssetVariationChanger
{
    [FileLocation($"ModsSettings/{nameof(AssetVariationChanger)}/{nameof(AssetVariationChanger)}")]
    [SettingsUIShowGroupName(kMainGroup, kCompatibilityGroup)]
    public class Setting : ModSetting
    {
        public const string kMainGroup = "Settings";
        public const string kCompatibilityGroup = "Compatibility";

        public Setting(IMod mod) : base(mod)
        {
        }

        [SettingsUISection(kMainGroup)]
        public bool EnableVariationChooser { get; set; } = true;

        [SettingsUISection(kMainGroup)]
        [SettingsUIKeyboardBinding(BindingKeyboard.V, Mod.kToggleVariationChooserBindingName, shift:true)]
        public ProxyBinding ToggleVariationChooser { get; set; }

        [SettingsUISection(kMainGroup)]
        [SettingsUIKeyboardBinding(BindingKeyboard.LeftArrow, Mod.kPreviousVariationBindingName)]
        public ProxyBinding PreviousVariationBinding { get; set; }

        [SettingsUISection(kMainGroup)]
        [SettingsUIKeyboardBinding(BindingKeyboard.RightArrow, Mod.kNextVariationBindingName)]
        public ProxyBinding NextVariationBinding { get; set; }

        [SettingsUISection(kCompatibilityGroup)]
        public bool LineToolCompatibility { get; set; } = true;

        [SettingsUISection(kCompatibilityGroup)]
        public bool TreeControllerCompatibility { get; set; } = true;

        public override void SetDefaults()
        {
            LineToolCompatibility = true;
            TreeControllerCompatibility = true;
        }

        public bool ResetBindings
        {
            set
            {
                Mod.log.Info("Reset key bindings");
                ResetKeyBindings();
            }
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

                { m_Setting.GetOptionGroupLocaleID(Setting.kMainGroup), "Settings" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kCompatibilityGroup), "Compatibility" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.PreviousVariationBinding)), "Previous Variation" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.PreviousVariationBinding)),
                    "This key will be bound to select the previous asset variation"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.NextVariationBinding)), "Next Variation" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.NextVariationBinding)),
                    "This key will be bound to select the next asset variation"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ToggleVariationChooser)), "Toggle Variation Chooser" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.ToggleVariationChooser)),
                    "Use this keybind to toggle the mod without having to open the menu"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableVariationChooser)), "Enable Variation Chooser" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableVariationChooser)),
                    "If enabled, the mod actually works. If disabled, the mod does nothing. This is useful if you want the vanilla random behavior"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "Reset key bindings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), $"" },

                { m_Setting.GetBindingKeyLocaleID(Mod.kPreviousVariationBindingName), "Previous Asset Variation" },
                { m_Setting.GetBindingKeyLocaleID(Mod.kNextVariationBindingName), "Next Asset Variation" },
                { m_Setting.GetBindingKeyLocaleID(Mod.kToggleVariationChooserBindingName), "Toggle Variation Chooser" },

                {m_Setting.GetOptionLabelLocaleID(nameof(Setting.LineToolCompatibility)), "Line Tool Compatibility"},
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.LineToolCompatibility)),
                    $"Deactivates Asset Variation Changer functionality while the line tool is being used (recommended)"
                },

                {m_Setting.GetOptionLabelLocaleID(nameof(Setting.TreeControllerCompatibility)), "Tree Controller Compatibility"},
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.TreeControllerCompatibility)),
                    $"Deactivates Asset Variation Changer functionality for most vegetation when Tree Controller is used (recommended)"
                },
            };
        }

        public void Unload()
        {
        }
    }
}