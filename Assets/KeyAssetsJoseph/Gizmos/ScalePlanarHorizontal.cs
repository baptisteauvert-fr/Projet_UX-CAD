using UnityEngine;

public class ScalePlanarHorizontal : MonoBehaviour
{
    [SerializeField]
    private GameObject selecPlanar;
    [SerializeField]
    private GameObject verticalPlane;

    private float initOffset;
    private float initOffsetY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initOffset = transform.localPosition.x;
        initOffsetY = verticalPlane.transform.localPosition.y;

    }

    // Update is called once per frame
    void Update()
    {
        updateCylinderScale();
    }

    public void updateCylinderScale()
    {
        selecPlanar.transform.localScale = new Vector3((transform.localPosition.x - initOffset )*-2+1, (verticalPlane.transform.localPosition.y - initOffsetY) * 20 + 1, (transform.localPosition.x - initOffset  )*-2+1);


    }
}
