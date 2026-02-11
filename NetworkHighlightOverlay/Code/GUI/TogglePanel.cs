using ColossalFramework.UI;
using NetworkHighlightOverlay.Code.Core;
using NetworkHighlightOverlay.Code.ModOptions;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public class TogglePanel : UIPanel
    {
        #region Constants
        private const int Columns = 5;
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
                    v => ModSettings.HighlightPedestrianPaths = v,
                    () => ModSettings.PedestrianPathColor,
                    () => ModSettings.PedestrianPathsHue,
                    v => ModSettings.PedestrianPathsHue = v)),
            new ToggleDefinition(
                "Pink paths",
                "SubBarRoadsMaintenance",
                new ToggleBinding(
                    () => ModSettings.HighlightPinkPaths,
                    v => ModSettings.HighlightPinkPaths = v,
                    () => ModSettings.PinkPathColor,
                    () => ModSettings.PinkPathsHue,
                    v => ModSettings.PinkPathsHue = v)),
            new ToggleDefinition(
                "Terraforming networks",
                "ToolbarIconLandscaping",
                new ToggleBinding(
                    () => ModSettings.HighlightTerraformingNetworks,
                    v => ModSettings.HighlightTerraformingNetworks = v,
                    () => ModSettings.TerraformingNetworksColor,
                    () => ModSettings.TerraformingNetworksHue,
                    v => ModSettings.TerraformingNetworksHue = v)),
            new ToggleDefinition(
                "Roads",
                "SubBarRoadsSmall",
                new ToggleBinding(
                    () => ModSettings.HighlightRoads,
                    v => ModSettings.HighlightRoads = v,
                    () => ModSettings.RoadsColor,
                    () => ModSettings.RoadsHue,
                    v => ModSettings.RoadsHue = v)),
            new ToggleDefinition(
                "Highways",
                "SubBarRoadsHighway",
                new ToggleBinding(
                    () => ModSettings.HighlightHighways,
                    v => ModSettings.HighlightHighways = v,
                    () => ModSettings.HighwaysColor,
                    () => ModSettings.HighwaysHue,
                    v => ModSettings.HighwaysHue = v)),
            new ToggleDefinition(
                "Train tracks",
                "SubBarPublicTransportTrain",
                new ToggleBinding(
                    () => ModSettings.HighlightTrainTracks,
                    v => ModSettings.HighlightTrainTracks = v,
                    () => ModSettings.TrainTracksColor,
                    () => ModSettings.TrainTracksHue,
                    v => ModSettings.TrainTracksHue = v)),
            new ToggleDefinition(
                "Metro tracks",
                "SubBarPublicTransportMetro",
                new ToggleBinding(
                    () => ModSettings.HighlightMetroTracks,
                    v => ModSettings.HighlightMetroTracks = v,
                    () => ModSettings.MetroTracksColor,
                    () => ModSettings.MetroTracksHue,
                    v => ModSettings.MetroTracksHue = v)),
            new ToggleDefinition(
                "Tram tracks",
                "SubBarPublicTransportTram",
                new ToggleBinding(
                    () => ModSettings.HighlightTramTracks,
                    v => ModSettings.HighlightTramTracks = v,
                    () => ModSettings.TramTracksColor,
                    () => ModSettings.TramTracksHue,
                    v => ModSettings.TramTracksHue = v)),
            new ToggleDefinition(
                "Monorail tracks",
                "SubBarPublicTransportMonorail",
                new ToggleBinding(
                    () => ModSettings.HighlightMonorailTracks,
                    v => ModSettings.HighlightMonorailTracks = v,
                    () => ModSettings.MonorailTracksColor,
                    () => ModSettings.MonorailTracksHue,
                    v => ModSettings.MonorailTracksHue = v)),
            new ToggleDefinition(
                "Cable cars",
                "SubBarPublicTransportCableCar",
                new ToggleBinding(
                    () => ModSettings.HighlightCableCars,
                    v => ModSettings.HighlightCableCars = v,
                    () => ModSettings.CableCarColor,
                    () => ModSettings.CableCarsHue,
                    v => ModSettings.CableCarsHue = v))
        };
        #endregion

        #region Fields
        private static TogglePanel _instance;

        private UIView _view;
        private DragHandle _dragHandle;
        private HuePopover _huePopover;
        #endregion

        public static void Create()
        {
            if (_instance != null)
                return;

            UIView view = UIView.GetAView();
            if (view == null)
                return;

            TogglePanel panel = view.AddUIComponent(typeof(TogglePanel)) as TogglePanel;
            if (panel == null)
                return;

            Manager.Instance.IsEnabledChanged -= OnManagerIsEnabledChanged;
            Manager.Instance.IsEnabledChanged += OnManagerIsEnabledChanged;

            panel.isVisible = false;
            _instance = panel;
            OnManagerIsEnabledChanged(Manager.Instance.IsEnabled);
        }

        public static void Destroy()
        {
            Manager.Instance.IsEnabledChanged -= OnManagerIsEnabledChanged;

            if (_instance == null)
                return;

            UnityEngine.Object.Destroy(_instance.gameObject);
            _instance = null;
        }

        public override void Awake()
        {
            base.Awake();
            name = "NHO_TogglePanel";
            backgroundSprite = "GenericPanel";
            color = new Color32(35, 35, 35, 230);
            clipChildren = true;
            isVisible = false;

            int rowCount = GetRowCount(ToggleDefinitions.Length);
            Vector2 panelSize = new Vector2(
                Padding * 2f + Columns * ButtonSize + (Columns - 1) * Spacing,
                DragHandleHeight + Padding * 2f + rowCount * ButtonSize + Mathf.Max(0, rowCount - 1) * Spacing);
            size = panelSize;
        }

        public override void Start()
        {
            base.Start();
            _view = UIView.GetAView();
            CreateDragHandle();
            CreateHuePopover();
            CreateButtons();
            ApplySavedPosition();
        }

        public override void Update()
        {
            base.Update();

            if (_huePopover != null && _huePopover.ShouldCloseForCurrentMouseClick())
            {
                _huePopover.Close();
            }
        }

        public override void OnDestroy()
        {
            Manager.Instance.IsEnabledChanged -= OnManagerIsEnabledChanged;

            if (_instance == this)
            {
                _instance = null;
            }

            UnsubscribeFromDragHandleEvents();
            _dragHandle = null;

            DestroyHuePopover();
            base.OnDestroy();
        }

        public void CloseHuePopover()
        {
            if (_huePopover != null)
            {
                _huePopover.Close();
            }
        }

        private static void OnManagerIsEnabledChanged(bool isEnabled)
        {
            if (_instance == null)
                return;

            if (!isEnabled)
            {
                _instance.CloseHuePopover();
            }

            _instance.isVisible = isEnabled;
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

            SubscribeToDragHandleEvents();
        }

        private void SubscribeToDragHandleEvents()
        {
            if (_dragHandle == null)
                return;

            _dragHandle.eventMouseDown -= OnDragHandleMouseDown;
            _dragHandle.eventMouseDown += OnDragHandleMouseDown;
            _dragHandle.eventMouseUp -= OnDragHandleMouseUp;
            _dragHandle.eventMouseUp += OnDragHandleMouseUp;
        }

        private void UnsubscribeFromDragHandleEvents()
        {
            if (_dragHandle == null)
                return;

            _dragHandle.eventMouseDown -= OnDragHandleMouseDown;
            _dragHandle.eventMouseUp -= OnDragHandleMouseUp;
        }

        private void CreateHuePopover()
        {
            if (_view == null)
                return;

            _huePopover = _view.AddUIComponent(typeof(HuePopover)) as HuePopover;
        }

        private void DestroyHuePopover()
        {
            if (_huePopover == null)
                return;

            _huePopover.Close();
            UnityEngine.Object.Destroy(_huePopover.gameObject);
            _huePopover = null;
        }

        private void CreateButtons()
        {
            int toggleCount = ToggleDefinitions.Length;
            for (int index = 0; index < toggleCount; index++)
            {
                int row = index / Columns;
                int column = index % Columns;
                ToggleDefinition definition = ToggleDefinitions[index];
                ToggleButton button = AddUIComponent<ToggleButton>();
                button.width = ButtonSize;
                button.height = ButtonSize;
                button.relativePosition = new Vector3(
                    Padding + column * (ButtonSize + Spacing),
                    DragHandleHeight + Padding + row * (ButtonSize + Spacing));
                button.Initialize(definition.SpriteName, definition.Binding, definition.Label, OnButtonHueEditRequested);
            }
        }

        private void OnButtonHueEditRequested(ToggleButton button, ToggleBinding binding)
        {
            if (_huePopover == null || button == null || binding == null)
                return;

            if (_huePopover.IsAnchoredTo(button))
            {
                _huePopover.Close();
                return;
            }

            _huePopover.Open(button, binding);
        }

        private void ApplySavedPosition()
        {
            if (_view == null)
                return;

            Vector2 panelSize = size;
            Vector2 target;
            if (ModSettings.PanelX >= 0f && ModSettings.PanelY >= 0f)
            {
                target = new Vector2(ModSettings.PanelX, ModSettings.PanelY);
            }
            else
            {
                target = CenterPosition(_view, panelSize);
            }

            Vector2 clamped = ClampToScreen(_view, target, panelSize);
            absolutePosition = new Vector3(clamped.x, clamped.y);
            ModSettings.PanelX = clamped.x;
            ModSettings.PanelY = clamped.y;
        }

        private void OnDragHandleMouseDown(UIComponent component, UIMouseEventParameter eventParam)
        {
            if (!DragHandle.IsCtrlDown)
                return;

            CloseHuePopover();
        }

        private void OnDragHandleMouseUp(UIComponent component, UIMouseEventParameter eventParam)
        {
            SaveCurrentPosition();
        }

        private void SaveCurrentPosition()
        {
            if (_view == null)
                return;

            Vector2 currentPosition = new Vector2(absolutePosition.x, absolutePosition.y);
            Vector2 clamped = ClampToScreen(_view, currentPosition, size);

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

        private static int GetRowCount(int toggleCount)
        {
            if (toggleCount <= 0)
                return 0;

            return (toggleCount + Columns - 1) / Columns;
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
