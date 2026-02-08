using ColossalFramework.UI;
using NetworkHighlightOverlay.Code.ModOptions;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public class TogglePanel : UIPanel
    {
        private const int Columns = 6;
        private const int Rows = 2;
        private const float ButtonSize = 40f;
        private const float Spacing = 4f;
        private const float Padding = 2f;

        private static readonly ToggleDefinition[] ToggleDefinitions = new[]
        {
            new ToggleDefinition(
                "Pedestrian paths",
                "SubBarBeautificationPaths",
                new ToggleBinding(
                    () => ModSettings.HighlightPedestrianPaths,
                    v => ModSettings.HighlightPedestrianPaths = v)),
            new ToggleDefinition(
                "Pink paths",
                "SubBarBeautificationPaths",
                new ToggleBinding(
                    () => ModSettings.HighlightPinkPaths,
                    v => ModSettings.HighlightPinkPaths = v)),
            new ToggleDefinition(
                "Terraforming networks",
                "SubBarBeautificationPaths",
                new ToggleBinding(
                    () => ModSettings.HighlightTerraformingNetworks,
                    v => ModSettings.HighlightTerraformingNetworks = v)),
            new ToggleDefinition(
                "Roads",
                "SubBarRoadsSmall",
                new ToggleBinding(
                    () => ModSettings.HighlightRoads,
                    v => ModSettings.HighlightRoads = v)),
            new ToggleDefinition(
                "Highways",
                "SubBarRoadsHighway",
                new ToggleBinding(
                    () => ModSettings.HighlightHighways,
                    v => ModSettings.HighlightHighways = v)),
            new ToggleDefinition(
                "Train tracks",
                "SubBarPublicTransportTrain",
                new ToggleBinding(
                    () => ModSettings.HighlightTrainTracks,
                    v => ModSettings.HighlightTrainTracks = v)),
            new ToggleDefinition(
                "Metro tracks",
                "SubBarPublicTransportMetro",
                new ToggleBinding(
                    () => ModSettings.HighlightMetroTracks,
                    v => ModSettings.HighlightMetroTracks = v)),
            new ToggleDefinition(
                "Tram tracks",
                "SubBarPublicTransportTram",
                new ToggleBinding(
                    () => ModSettings.HighlightTramTracks,
                    v => ModSettings.HighlightTramTracks = v)),
            new ToggleDefinition(
                "Monorail tracks",
                "SubBarPublicTransportMetro",
                new ToggleBinding(
                    () => ModSettings.HighlightMonorailTracks,
                    v => ModSettings.HighlightMonorailTracks = v)),
            new ToggleDefinition(
                "Cable cars",
                "SubBarPublicTransportTrain",
                new ToggleBinding(
                    () => ModSettings.HighlightCableCars,
                    v => ModSettings.HighlightCableCars = v)),
            new ToggleDefinition(
                "Bridges",
                "SubBarRoadsSmall",
                new ToggleBinding(
                    () => ModSettings.HighlightBridges,
                    v => ModSettings.HighlightBridges = v)),
            new ToggleDefinition(
                "Tunnels",
                "SubBarBeautificationPaths",
                new ToggleBinding(
                    () => ModSettings.HighlightTunnels,
                    v => ModSettings.HighlightTunnels = v))
        };

        private readonly ToggleButton[] _buttons = new ToggleButton[Columns * Rows];

        public override void Awake()
        {
            base.Awake();
            name = "NHO_TogglePanel";
            backgroundSprite = "GenericPanel";
            color = new Color32(35, 35, 35, 230);
            clipChildren = true;
            isVisible = false;
            Vector2 panelSize = new Vector2(
                Padding * 2f + Columns * ButtonSize + (Columns - 1) * Spacing,
                Padding * 2f + Rows * ButtonSize + (Rows - 1) * Spacing);
            size = panelSize;
        }

        public override void Start()
        {
            base.Start();
            CreateButtons();
            ApplySavedPosition();
            ModSettings.SettingsChanged += OnSettingsChanged;
            UpdateButtonStates();
        }

        public override void OnDestroy()
        {
            ModSettings.SettingsChanged -= OnSettingsChanged;
            base.OnDestroy();
        }

        private void CreateButtons()
        {
            if (ToggleDefinitions.Length != Columns * Rows)
                return;

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    int index = row * Columns + column;
                    ToggleDefinition definition = ToggleDefinitions[index];
                    ToggleButton button = AddUIComponent<ToggleButton>();
                    button.width = ButtonSize;
                    button.height = ButtonSize;
                    button.relativePosition = new Vector3(
                        Padding + column * (ButtonSize + Spacing),
                        Padding + row * (ButtonSize + Spacing));
                    button.Initialize(definition.SpriteName, definition.Binding, definition.Label);
                    _buttons[index] = button;
                }
            }
        }

        private void ApplySavedPosition()
        {
            UIView view = UIView.GetAView();
            if (view == null)
                return;

            Vector2 panelSize = size;
            Vector2 target;
            if (ModSettings.PanelX >= 0f && ModSettings.PanelY >= 0f)
            {
                target = new Vector2(ModSettings.PanelX, ModSettings.PanelY);
            }
            else
            {
                target = CenterPosition(view, panelSize);
            }

            Vector2 clamped = ClampToScreen(view, target, panelSize);
            absolutePosition = new Vector3(clamped.x, clamped.y);
            ModSettings.PanelX = clamped.x;
            ModSettings.PanelY = clamped.y;
        }

        private static Vector2 CenterPosition(UIView view, Vector2 panelSize)
        {
            Vector2 screen = view.GetScreenResolution();
            float x = (screen.x - panelSize.x) * 0.5f;
            float y = (screen.y - panelSize.y) * 0.5f;
            return new Vector2(Mathf.Max(0f, x), Mathf.Max(0f, y));
        }

        private static Vector2 ClampToScreen(UIView view, Vector2 desired, Vector2 panelSize)
        {
            Vector2 screen = view.GetScreenResolution();
            float x = Mathf.Clamp(desired.x, 0f, Mathf.Max(0f, screen.x - panelSize.x));
            float y = Mathf.Clamp(desired.y, 0f, Mathf.Max(0f, screen.y - panelSize.y));
            return new Vector2(x, y);
        }

        private void OnSettingsChanged(Config config)
        {
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttons[i]?.Refresh();
            }
        }

        private readonly struct ToggleDefinition
        {
            public readonly string Label;
            public readonly string SpriteName;
            public readonly ToggleBinding Binding;

            public ToggleDefinition(string label, string spriteName, ToggleBinding binding)
            {
                Label = label;
                SpriteName = spriteName;
                Binding = binding;
            }
        }
    }
}
