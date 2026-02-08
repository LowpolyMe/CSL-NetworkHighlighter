using ColossalFramework.UI;
using NetworkHighlightOverlay.Code.ModOptions;
using System;
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
        private const float DragHandleHeight = 18f;

        private static readonly ToggleDefinition[] ToggleDefinitions = new[]
        {
            new ToggleDefinition(
                "Pedestrian paths",
                "SubBarBeautificationPedestrianZoneEssentials",
                new ToggleBinding(
                    () => ModSettings.HighlightPedestrianPaths,
                    v => ModSettings.HighlightPedestrianPaths = v),
                () => ModSettings.PedestrianPathColor),
            new ToggleDefinition(
                "Pink paths",
                "SubBarRoadsMaintenance",
                new ToggleBinding(
                    () => ModSettings.HighlightPinkPaths,
                    v => ModSettings.HighlightPinkPaths = v),
                () => ModSettings.PinkPathColor),
            new ToggleDefinition(
                "Terraforming networks",
                "ToolbarIconLandscaping",
                new ToggleBinding(
                    () => ModSettings.HighlightTerraformingNetworks,
                    v => ModSettings.HighlightTerraformingNetworks = v),
                () => ModSettings.TerraformingNetworksColor),
            new ToggleDefinition(
                "Roads",
                "SubBarRoadsSmall",
                new ToggleBinding(
                    () => ModSettings.HighlightRoads,
                    v => ModSettings.HighlightRoads = v),
                () => ModSettings.RoadsColor),
            new ToggleDefinition(
                "Highways",
                "SubBarRoadsHighway",
                new ToggleBinding(
                    () => ModSettings.HighlightHighways,
                    v => ModSettings.HighlightHighways = v),
                () => ModSettings.HighwaysColor),
            new ToggleDefinition(
                "Train tracks",
                "SubBarPublicTransportTrain",
                new ToggleBinding(
                    () => ModSettings.HighlightTrainTracks,
                    v => ModSettings.HighlightTrainTracks = v),
                () => ModSettings.TrainTracksColor),
            new ToggleDefinition(
                "Metro tracks",
                "SubBarPublicTransportMetro",
                new ToggleBinding(
                    () => ModSettings.HighlightMetroTracks,
                    v => ModSettings.HighlightMetroTracks = v),
                () => ModSettings.MetroTracksColor),
            new ToggleDefinition(
                "Tram tracks",
                "SubBarPublicTransportTram",
                new ToggleBinding(
                    () => ModSettings.HighlightTramTracks,
                    v => ModSettings.HighlightTramTracks = v),
                () => ModSettings.TramTracksColor),
            new ToggleDefinition(
                "Monorail tracks",
                "SubBarPublicTransportMonorail",
                new ToggleBinding(
                    () => ModSettings.HighlightMonorailTracks,
                    v => ModSettings.HighlightMonorailTracks = v),
                () => ModSettings.MonorailTracksColor),
            new ToggleDefinition(
                "Cable cars",
                "SubBarPublicTransportCableCar",
                new ToggleBinding(
                    () => ModSettings.HighlightCableCars,
                    v => ModSettings.HighlightCableCars = v),
                () => ModSettings.CableCarColor),
            new ToggleDefinition(
                "Bridges",
                "SubBarRoadsSmall",
                new ToggleBinding(
                    () => ModSettings.HighlightBridges,
                    v => ModSettings.HighlightBridges = v),
                null),
            new ToggleDefinition(
                "Tunnels",
                "SubBarBeautificationPaths",
                new ToggleBinding(
                    () => ModSettings.HighlightTunnels,
                    v => ModSettings.HighlightTunnels = v),
                null)
        };

        private readonly ToggleButton[] _buttons = new ToggleButton[Columns * Rows];
        private DragHandle _dragHandle;

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
                DragHandleHeight + Padding * 2f + Rows * ButtonSize + (Rows - 1) * Spacing);
            size = panelSize;
        }

        public override void Start()
        {
            base.Start();
            CreateDragHandle();
            CreateButtons();
            ApplySavedPosition();
            ModSettings.SettingsChanged += OnSettingsChanged;
            UpdateButtonStates();
        }

        public override void OnDestroy()
        {
            if (_dragHandle != null)
            {
                _dragHandle.eventMouseUp -= OnDragHandleMouseUp;
                _dragHandle = null;
            }
            ModSettings.SettingsChanged -= OnSettingsChanged;
            base.OnDestroy();
        }

        private void CreateDragHandle()
        {
            _dragHandle = AddUIComponent<DragHandle>();
            _dragHandle.name = "NHO_TogglePanelDragHandle";
            _dragHandle.target = this;
            _dragHandle.relativePosition = Vector3.zero;
            _dragHandle.width = width;
            _dragHandle.height = DragHandleHeight;
            _dragHandle.isInteractive = true;
            _dragHandle.isVisible = true;
            _dragHandle.eventMouseUp += OnDragHandleMouseUp;
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
                        DragHandleHeight + Padding + row * (ButtonSize + Spacing));
                    button.Initialize(definition.SpriteName, definition.Binding, definition.Label, definition.ColorProvider);
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

        private void OnDragHandleMouseUp(UIComponent component, UIMouseEventParameter eventParam)
        {
            SaveCurrentPosition();
        }

        private void SaveCurrentPosition()
        {
            UIView view = UIView.GetAView();
            if (view == null)
                return;

            Vector2 currentPosition = new Vector2(absolutePosition.x, absolutePosition.y);
            Vector2 clamped = ClampToScreen(view, currentPosition, size);

            if (!Mathf.Approximately(currentPosition.x, clamped.x) ||
                !Mathf.Approximately(currentPosition.y, clamped.y))
            {
                absolutePosition = new Vector3(clamped.x, clamped.y);
            }

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
            public readonly Func<Color> ColorProvider;

            public ToggleDefinition(string label, string spriteName, ToggleBinding binding, Func<Color> colorProvider)
            {
                Label = label;
                SpriteName = spriteName;
                Binding = binding;
                ColorProvider = colorProvider;
            }
        }
    }
}
