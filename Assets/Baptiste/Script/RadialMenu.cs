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
    [SerializeField] private InputActionProperty openMenuAction; // B (ton binding local)

    [Header("Hand selection")]
    [Tooltip("Transform qui représente la main (ou un empty devant la main)")]
    [SerializeField] private Transform handPointer;

    [Tooltip("Distance minimum main->centre pour valider une slice (évite les sélections quand on est au centre)")]
    [SerializeField] private float minRadiusMeters = 0.06f;

    [Tooltip("Décalage du menu à l'ouverture (devant la main)")]
    [SerializeField] private float spawnDistance = 0.25f;

    [Tooltip("Le menu regarde la caméra à l'ouverture")]
    [SerializeField] private bool faceCameraOnOpen = true;

    [Header("Target")]
    [SerializeField] private Renderer targetRenderer;

    [Tooltip("Décalage d'angle si tes slices ne sont pas alignées (ex: 0, 90, 180...)")]
    [SerializeField] private float startAngleOffsetDeg = 0f;

    [Tooltip("Debug: dessine une ligne centre->main dans la Scene")]
    [SerializeField] private bool debugDraw = true;


    private readonly List<RadialSliceUI> spawned = new();
    private bool isOpen;
    private int currentIndex = -1;

    // “position fixe” pendant l’ouverture
    private Vector3 openPos;
    private Quaternion openRot;

    private void Awake()
    {
        if (menuCanvas == null)
            menuCanvas = GetComponentInChildren<Canvas>(true);

        BuildMenu();
        SetOpen(false);
    }

    private void OnEnable() => openMenuAction.action?.Enable();
    private void OnDisable() => openMenuAction.action?.Disable();

    private void Update()
    {
        if (openMenuAction.action == null) return;

        bool pressed = openMenuAction.action.IsPressed();

        if (pressed && !isOpen)
        {
            PlaceMenuOnce();
            SetOpen(true);
        }
        else if (!pressed && isOpen)
        {
            ApplySelection();
            SetOpen(false);
        }

        if (!isOpen) return;

        UpdateSelectionFromHand();
        UpdateHighlight();
    }

    private void PlaceMenuOnce()
    {
        // Menu fixe : on pose UNE fois, puis on ne bouge plus tant que c’est ouvert
        if (handPointer != null)
        {
            var cam = Camera.main;
            Vector3 forward = cam != null ? cam.transform.forward : handPointer.forward;

            openPos = handPointer.position + forward.normalized * spawnDistance;

            if (faceCameraOnOpen && cam != null)
            {
                // le menu fait face à la caméra
                Vector3 dir = (openPos - cam.transform.position);
                openRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
            }
            else
            {
                openRot = transform.rotation;
            }
        }
        else
        {
            openPos = transform.position;
            openRot = transform.rotation;
        }

        transform.position = openPos;
        transform.rotation = openRot;
    }

    private void BuildMenu()
    {
        foreach (var s in spawned)
            if (s != null) Destroy(s.gameObject);
        spawned.Clear();

        if (slicePrefab == null || slicesRoot == null || options.Count == 0) return;

        float fill = 1f / options.Count;
        float degreesPer = 360f / options.Count;

        for (int i = 0; i < options.Count; i++)
        {
            var slice = Instantiate(slicePrefab, slicesRoot);
            float rotation = -degreesPer * i;
            slice.Setup(options[i], i, fill, rotation);
            spawned.Add(slice);
        }
    }

    private void SetOpen(bool open)
    {
        isOpen = open;
        if (menuCanvas != null) menuCanvas.enabled = open;

        if (!open)
        {
            currentIndex = -1;
            UpdateHighlight();
        }
    }

    private void OnDrawGizmos()
    {
        if (!debugDraw || menuCanvas == null || handPointer == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(menuCanvas.transform.position, handPointer.position);
        Gizmos.DrawSphere(handPointer.position, 0.01f);
    }


    private void UpdateSelectionFromHand()
    {
        if (handPointer == null || options.Count == 0 || menuCanvas == null)
        {
            currentIndex = -1;
            return;
        }

        // Centre du menu (prends slicesRoot si c’est bien au centre du disque)
        Transform centerT = slicesRoot != null ? slicesRoot : menuCanvas.transform;

        Vector3 center = centerT.position;
        Vector3 delta = handPointer.position - center;

        // Projection sur le plan du menu
        Vector3 right = menuCanvas.transform.right;
        Vector3 up = menuCanvas.transform.up;

        float x = Vector3.Dot(delta, right);
        float y = Vector3.Dot(delta, up);

        Vector2 p = new Vector2(x, y);

        if (p.magnitude < minRadiusMeters)
        {
            currentIndex = -1;
            return;
        }

        float angle = Mathf.Atan2(p.y, p.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        float sliceSize = 360f / options.Count;
        angle = (angle + startAngleOffsetDeg) % 360f;

        int idx = Mathf.FloorToInt((angle + sliceSize * 0.5f) / sliceSize) % options.Count;
        currentIndex = idx;
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

        targetRenderer.material.color = options[currentIndex].color;
    }
}
