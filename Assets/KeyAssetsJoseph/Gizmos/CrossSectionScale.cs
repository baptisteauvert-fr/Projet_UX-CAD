using UnityEngine;

public class CrossSectionScale : MonoBehaviour
{
    [SerializeField]
    private GameObject selecPlanar;
   
    private float initOffset;
    private float initOffsetY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initOffset = transform.localPosition.x;
       

    }

    // Update is called once per frame
    void Update()
    {
        updateCylinderScale();
    }

    public void updateCylinderScale()
    {
        selecPlanar.transform.localScale = new Vector3((transform.localPosition.x - initOffset) * -2 + 1,selecPlanar.transform.localScale.y, (transform.localPosition.x - initOffset) * -2 + 1);
    }
}
