using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RadialMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform slicesRoot;
    [SerializeField] private RadialSliceUI slicePrefab;
    [SerializeField] private Canvas menuCanvas;

    [Header("Options")]
    [SerializeField] private List<RadialOption> options = new();

    [Header("Input")]
    [SerializeField] private InputActionProperty openMenuAction;    // B
    [SerializeField] private InputActionProperty thumbstickAction;  // Joystick

    [Header("Target")]
    [SerializeField] private Renderer targetRenderer;

    [Header("Selection")]
    [SerializeField] private float deadZone = 0.35f;

    [Header("Debug")]
    [SerializeField] private bool debugLogs = true;
    [SerializeField] private float logEverySecondsWhilePressed = 0.5f;

    private readonly List<RadialSliceUI> spawned = new();
    private bool isOpen;
    private int currentIndex = -1;

    private float _nextSpamLogTime;

    private void Awake()
    {
        if (menuCanvas == null)
            menuCanvas = GetComponentInChildren<Canvas>(true);

        if (debugLogs)
        {
            Debug.Log($"[RadialMenu] Awake on {name}", this);
            Debug.Log($"[RadialMenu] openMenuAction has action? {(openMenuAction.action != null)}", this);
            Debug.Log($"[RadialMenu] thumbstickAction has action? {(thumbstickAction.action != null)}", this);
        }

        BuildMenu();
        SetOpen(false);
    }

    private void OnEnable()
    {
        if (openMenuAction.action != null) openMenuAction.action.Enable();
        if (thumbstickAction.action != null) thumbstickAction.action.Enable();

        if (debugLogs)
        {
            Debug.Log($"[RadialMenu] OnEnable | open enabled={openMenuAction.action?.enabled} | thumb enabled={thumbstickAction.action?.enabled}", this);
            Debug.Log($"[RadialMenu] Open action name={openMenuAction.action?.name} | bindings={openMenuAction.action?.bindings.Count}", this);
            Debug.Log($"[RadialMenu] Thumb action name={thumbstickAction.action?.name} | bindings={thumbstickAction.action?.bindings.Count}", this);
        }

        // BONUS: logs événementiels (super utile)
        if (openMenuAction.action != null)
        {
            openMenuAction.action.performed += OnOpenPerformed;
            openMenuAction.action.canceled += OnOpenCanceled;
        }
    }

    private void OnDisable()
    {
        if (openMenuAction.action != null)
        {
            openMenuAction.action.performed -= OnOpenPerformed;
            openMenuAction.action.canceled -= OnOpenCanceled;
        }

        if (openMenuAction.action != null) openMenuAction.action.Disable();
        if (thumbstickAction.action != null) thumbstickAction.action.Disable();
    }

    private void OnOpenPerformed(InputAction.CallbackContext ctx)
    {
        if (!debugLogs) return;
        Debug.Log($"[RadialMenu] OPEN performed! phase={ctx.phase} time={Time.time:F2}", this);
    }

    private void OnOpenCanceled(InputAction.CallbackContext ctx)
    {
        if (!debugLogs) return;
        Debug.Log($"[RadialMenu] OPEN canceled! phase={ctx.phase} time={Time.time:F2}", this);
    }

    private void Update()
    {
        if (openMenuAction.action == null)
        {
            if (debugLogs) Debug.LogWarning("[RadialMenu] openMenuAction.action is NULL (pas assigné).", this);
            enabled = false;
            return;
        }

        bool pressed = openMenuAction.action.IsPressed();

        // Log "spam" léger pour confirmer que IsPressed change bien
        if (debugLogs && pressed && Time.time >= _nextSpamLogTime)
        {
            _nextSpamLogTime = Time.time + logEverySecondsWhilePressed;
            Debug.Log($"[RadialMenu] IsPressed = TRUE (B maintenu) time={Time.time:F2}", this);
        }

        if (pressed && !isOpen)
        {
            if (debugLogs) Debug.Log("[RadialMenu] -> SetOpen(TRUE)", this);
            SetOpen(true);
        }
        else if (!pressed && isOpen)
        {
            if (debugLogs) Debug.Log("[RadialMenu] -> ApplySelection + SetOpen(FALSE)", this);
            ApplySelection();
            SetOpen(false);
        }

        if (!isOpen) return;

        UpdateSelectionFromThumbstick();
        UpdateHighlight();
    }

    private void BuildMenu()
    {
        foreach (var s in spawned)
            if (s != null) Destroy(s.gameObject);
        spawned.Clear();

        if (slicePrefab == null || slicesRoot == null || options.Count == 0)
        {
            if (debugLogs)
                Debug.LogWarning($"[RadialMenu] BuildMenu skipped. slicePrefab? {slicePrefab != null}, slicesRoot? {slicesRoot != null}, options={options.Count}", this);
            return;
        }

        float fill = 1f / options.Count;
        float degreesPer = 360f / options.Count;

        for (int i = 0; i < options.Count; i++)
        {
            var slice = Instantiate(slicePrefab, slicesRoot);
            float rotation = -degreesPer * i;
            slice.Setup(options[i], i, fill, rotation);
            spawned.Add(slice);
        }

        if (debugLogs) Debug.Log($"[RadialMenu] BuildMenu OK. spawned={spawned.Count}", this);
    }

    private void SetOpen(bool open)
    {
        isOpen = open;

        if (menuCanvas != null)
            menuCanvas.enabled = open;

        if (debugLogs)
            Debug.Log($"[RadialMenu] Canvas enabled = {menuCanvas != null && menuCanvas.enabled}", this);

        if (!open) currentIndex = -1;
    }

    private void UpdateSelectionFromThumbstick()
    {
        if (thumbstickAction.action == null) return;

        Vector2 v = thumbstickAction.action.ReadValue<Vector2>();
        if (debugLogs)
            Debug.Log($"[RadialMenu] Thumbstick = {v}", this);

        if (v.magnitude < deadZone) { currentIndex = -1; return; }

        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        float sliceSize = 360f / options.Count;
        currentIndex = Mathf.Clamp(Mathf.FloorToInt(angle / sliceSize), 0, options.Count - 1);
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < spawned.Count; i++)
            spawned[i].SetHighlighted(isOpen && i == currentIndex);
    }

    private void ApplySelection()
    {
        if (currentIndex < 0 || currentIndex >= options.Count) return;
        if (targetRenderer == null) return;

        var c = options[currentIndex].color;
        targetRenderer.material.color = c;

        if (debugLogs)
            Debug.Log($"[RadialMenu] ApplySelection index={currentIndex} color={c}", this);
    }
}
