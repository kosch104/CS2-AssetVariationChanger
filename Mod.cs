using AssetVariationChanger.Systems;
using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Colossal.IO.AssetDatabase;
using Game.Input;

namespace AssetVariationChanger
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(AssetVariationChanger)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        public static Setting m_Setting;
        public static ProxyAction m_PreviousVariationAction;
        public static ProxyAction m_NextVariationAction;
        public static ProxyAction m_ToggleVariationChooserAction;
        public const string kPreviousVariationBindingName = "PreviousVariationBinding";
        public const string kNextVariationBindingName = "NextVariationBinding";
        public const string kToggleVariationChooserBindingName = "ToggleVariationChooserBinding";

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));

            m_Setting.RegisterKeyBindings();

            m_PreviousVariationAction = m_Setting.GetAction(kPreviousVariationBindingName);
            m_NextVariationAction = m_Setting.GetAction(kNextVariationBindingName);
            m_ToggleVariationChooserAction = m_Setting.GetAction(kToggleVariationChooserBindingName);

            m_PreviousVariationAction.shouldBeEnabled = true;
            m_NextVariationAction.shouldBeEnabled = true;
            m_ToggleVariationChooserAction.shouldBeEnabled = true;

            m_PreviousVariationAction.onInteraction += (_, phase) =>
                RandomSeedSystem.Instance.OnPreviousAssetVariation(phase);

            m_NextVariationAction.onInteraction += (_, phase) =>
                RandomSeedSystem.Instance.OnNextAssetVariation(phase);

            m_ToggleVariationChooserAction.onInteraction += (_, _) =>
                m_Setting.EnableVariationChooser = !m_Setting.EnableVariationChooser;

            AssetDatabase.global.LoadSettings(nameof(AssetVariationChanger), m_Setting, new Setting(this));

            updateSystem.UpdateBefore<RandomSeedSystem>(SystemUpdatePhase.Modification1);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}