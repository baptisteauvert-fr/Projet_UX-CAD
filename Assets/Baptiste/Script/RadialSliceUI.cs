using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadialSliceUI : MonoBehaviour
{
    [SerializeField] private Image sliceImage;
    [SerializeField] private TextMeshProUGUI label;

    public RadialOption Option { get; private set; }
    public int Index { get; private set; }

    public void Setup(RadialOption option, int index, float fillAmount, float rotationDeg)
    {
        Option = option;
        Index = index;

        if (sliceImage == null) sliceImage = GetComponent<Image>();
        sliceImage.type = Image.Type.Filled;
        sliceImage.fillMethod = Image.FillMethod.Radial360;
        sliceImage.fillAmount = fillAmount;
        sliceImage.color = option.color;

        if (label != null) label.text = option.label;

        transform.localRotation = Quaternion.Euler(0f, 0f, rotationDeg);
    }

    public void SetHighlighted(bool highlighted)
    {
        if (sliceImage == null || Option == null) return;

        var c = Option.color;
        float mul = highlighted ? 1.25f : 1f;
        sliceImage.color = new Color(c.r * mul, c.g * mul, c.b * mul, 1f);
    }
}
