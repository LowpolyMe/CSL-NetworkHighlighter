using ColossalFramework.UI;
using NetworkHighlightOverlay.Code.Core;
using NetworkHighlightOverlay.Code.ModOptions;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace NetworkHighlightOverlay.Code.GUI
{
    public class TogglePanel : UIPanel
    {
        private const int Columns = 5;
        private const float ButtonSize = 40f;
        private const float Spacing = 4f;
        private const float Padding = 2f;
        private const float DragHandleHeight = 18f;
        private const string HeaderText = "Network Highlighter";

        private ModSettings _settings;
        private ActivationHandler _activationHandler;
        private ToggleButtonAtlas _toggleButtonAtlas;
        private Action<bool> _activationChangedHandler;
        private Action _settingsChangedHandler;
        private UIView _view;
        private DragHandle _dragHandle;
        private HuePopover _huePopover;
        private readonly List<ToggleButton> _buttons = new List<ToggleButton>();

        public void Initialize(ModSettings settings, ActivationHandler activationHandler, ToggleButtonAtlas toggleButtonAtlas)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            if (activationHandler == null)
                throw new ArgumentNullException("activationHandler");

            if (toggleButtonAtlas == null)
                throw new ArgumentNullException("toggleButtonAtlas");

            _settings = settings;
            _activationHandler = activationHandler;
            _toggleButtonAtlas = toggleButtonAtlas;

            SubscribeToActivationChanged();
            SubscribeToSettingsChanged();
            isVisible = _activationHandler.IsActive;
        }

        public override void Awake()
        {
            base.Awake();
            name = "NHO_TogglePanel";
            backgroundSprite = "GenericPanel";
            color = new Color32(35, 35, 35, 230);
            clipChildren = true;
            isVisible = false;

            int rowCount = GetRowCount(HighlightCategoryCatalog.All.Length);
            Vector2 panelSize = new Vector2(
                Padding * 2f + Columns * ButtonSize + (Columns - 1) * Spacing,
                DragHandleHeight + Padding * 2f + rowCount * ButtonSize + Mathf.Max(0, rowCount - 1) * Spacing);
            size = panelSize;
        }

        public override void Start()
        {
            base.Start();
            EnsureInitialized();
            _view = UIView.GetAView();
            if (_view == null)
                throw new InvalidOperationException("TogglePanel requires an active UIView.");
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
            UnsubscribeFromActivationChanged();
            UnsubscribeFromSettingsChanged();

            UnsubscribeFromDragHandleEvents();
            _dragHandle = null;
            _buttons.Clear();

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

        private void OnEnabledStateChanged(bool isEnabled)
        {
            if (!isEnabled)
            {
                CloseHuePopover();
            }

            isVisible = isEnabled;
        }

        private void SubscribeToActivationChanged()
        {
            if (_activationChangedHandler != null)
                throw new InvalidOperationException("TogglePanel has already subscribed to ActivationChanged.");

            _activationChangedHandler = OnEnabledStateChanged;
            _activationHandler.ActivationChanged += _activationChangedHandler;
        }

        private void UnsubscribeFromActivationChanged()
        {
            if (_activationHandler == null || _activationChangedHandler == null)
                return;

            _activationHandler.ActivationChanged -= _activationChangedHandler;
            _activationChangedHandler = null;
        }

        private void SubscribeToSettingsChanged()
        {
            if (_settingsChangedHandler != null)
                throw new InvalidOperationException("TogglePanel has already subscribed to SettingsChanged.");

            _settingsChangedHandler = OnSettingsChanged;
            _settings.SettingsChanged += _settingsChangedHandler;
        }

        private void UnsubscribeFromSettingsChanged()
        {
            if (_settings == null || _settingsChangedHandler == null)
                return;

            _settings.SettingsChanged -= _settingsChangedHandler;
            _settingsChangedHandler = null;
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

            CreateHeaderLabel();
            SubscribeToDragHandleEvents();
        }

        private void CreateHeaderLabel()
        {
            UILabel headerLabel = _dragHandle.AddUIComponent<UILabel>();
            headerLabel.name = "NHO_TogglePanelHeaderLabel";
            headerLabel.anchor = UIAnchorStyle.CenterHorizontal | UIAnchorStyle.CenterVertical;
            headerLabel.width = _dragHandle.width;
            headerLabel.height = _dragHandle.height;
            headerLabel.relativePosition = Vector3.zero;
            headerLabel.text = HeaderText;
            headerLabel.autoSize = false;
            headerLabel.textAlignment = UIHorizontalAlignment.Center;
            headerLabel.verticalAlignment = UIVerticalAlignment.Middle;
            headerLabel.textScale = 0.8f;
            headerLabel.isInteractive = false;
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
            _huePopover = _view.AddUIComponent(typeof(HuePopover)) as HuePopover;
            if (_huePopover == null)
                throw new InvalidOperationException("TogglePanel failed to create the hue popover.");
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
            HighlightCategoryDefinition[] categoryDefinitions = HighlightCategoryCatalog.All;
            int categoryCount = categoryDefinitions.Length;
            for (int index = 0; index < categoryCount; index++)
            {
                int row = index / Columns;
                int column = index % Columns;

                HighlightCategoryDefinition categoryDefinition = categoryDefinitions[index];
                ToggleButton button = AddUIComponent<ToggleButton>();
                button.width = ButtonSize;
                button.height = ButtonSize;
                button.relativePosition = new Vector3(
                    Padding + column * (ButtonSize + Spacing),
                    DragHandleHeight + Padding + row * (ButtonSize + Spacing));
                button.Initialize(
                    categoryDefinition,
                    _settings,
                    _toggleButtonAtlas,
                    OnButtonHueEditRequested);
                _buttons.Add(button);
            }
        }

        private void OnButtonHueEditRequested(ToggleButton button, HighlightCategoryId categoryId)
        {
            if (button == null)
                throw new ArgumentNullException("button");

            if (_huePopover.IsAnchoredTo(button))
            {
                _huePopover.Close();
                return;
            }

            _huePopover.Open(button, _settings, categoryId);
        }

        private void OnSettingsChanged()
        {
            RefreshButtonsFromSettings();
            RefreshHuePopoverFromSettings();
        }

        private void RefreshButtonsFromSettings()
        {
            int buttonCount = _buttons.Count;
            for (int i = 0; i < buttonCount; i++)
            {
                ToggleButton button = _buttons[i];
                button.RefreshFromSettings();
            }
        }

        private void RefreshHuePopoverFromSettings()
        {
            if (_huePopover == null || !_huePopover.IsOpen)
                return;

            _huePopover.RefreshFromSettings();
        }

        private void ApplySavedPosition()
        {
            Vector2 panelSize = size;
            Vector2 target;
            if (_settings.PanelX >= 0f && _settings.PanelY >= 0f)
            {
                target = new Vector2(_settings.PanelX, _settings.PanelY);
            }
            else
            {
                target = CenterPosition(_view, panelSize);
            }

            Vector2 clamped = ClampToScreen(_view, target, panelSize);
            absolutePosition = new Vector3(clamped.x, clamped.y);
            _settings.PanelX = clamped.x;
            _settings.PanelY = clamped.y;
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
            Vector2 currentPosition = new Vector2(absolutePosition.x, absolutePosition.y);
            Vector2 clamped = ClampToScreen(_view, currentPosition, size);

            if (!Mathf.Approximately(currentPosition.x, clamped.x) ||
                !Mathf.Approximately(currentPosition.y, clamped.y))
            {
                absolutePosition = new Vector3(clamped.x, clamped.y);
            }

            _settings.PanelX = clamped.x;
            _settings.PanelY = clamped.y;
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

        private void EnsureInitialized()
        {
            if (_settings == null || _activationHandler == null || _toggleButtonAtlas == null)
                throw new InvalidOperationException("TogglePanel must be initialized before Start.");
        }
    }
}
