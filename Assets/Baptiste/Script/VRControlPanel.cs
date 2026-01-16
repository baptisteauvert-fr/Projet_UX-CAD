using UnityEngine;
using UnityEngine.UI;

public class VRControlPanel : MonoBehaviour
{
    [Header("Target to test")]
    [SerializeField] private Renderer targetCubeRenderer;

    [Header("UI")]
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private Button button4;

    [SerializeField] private Slider slider; // 1..10
    [SerializeField] private TMPro.TMP_Text sliderValueText; // optional

    [Header("Colors")]
    [SerializeField] private Color color1 = Color.red;
    [SerializeField] private Color color2 = Color.green;
    [SerializeField] private Color color3 = Color.blue;
    [SerializeField] private Color color4 = Color.yellow;

    private Material _runtimeMat;

    private void Awake()
    {
        // Sécurité
        if (targetCubeRenderer != null)
        {
            // Instancier un matériau runtime pour ne pas modifier le material asset
            _runtimeMat = targetCubeRenderer.material;
        }

        // Hook boutons
        if (button1) button1.onClick.AddListener(() => SetCubeColor(color1));
        if (button2) button2.onClick.AddListener(() => SetCubeColor(color2));
        if (button3) button3.onClick.AddListener(() => SetCubeColor(color3));
        if (button4) button4.onClick.AddListener(() => SetCubeColor(color4));

        // Slider 1..10
        if (slider)
        {
            slider.wholeNumbers = true;
            slider.minValue = 1;
            slider.maxValue = 10;
            slider.onValueChanged.AddListener(OnSliderChanged);

            // Init affichage
            OnSliderChanged(slider.value);
        }
    }

    private void OnDestroy()
    {
        // Nettoyage listeners (propre)
        if (button1) button1.onClick.RemoveAllListeners();
        if (button2) button2.onClick.RemoveAllListeners();
        if (button3) button3.onClick.RemoveAllListeners();
        if (button4) button4.onClick.RemoveAllListeners();

        if (slider) slider.onValueChanged.RemoveAllListeners();
    }

    private void SetCubeColor(Color c)
    {
        if (_runtimeMat == null) return;
        _runtimeMat.color = c;
    }

    private void OnSliderChanged(float v)
    {
        int valueInt = Mathf.RoundToInt(v);

        if (sliderValueText != null)
            sliderValueText.text = valueInt.ToString();

        // Exemple de test: la valeur 1..10 agit sur l’intensité (brightness)
        if (_runtimeMat != null)
        {
            Color baseC = _runtimeMat.color;
            float t = Mathf.InverseLerp(1f, 10f, valueInt); // 0..1
            float mul = Mathf.Lerp(0.3f, 1.5f, t);
            _runtimeMat.color = baseC * mul;
        }
    }
}
