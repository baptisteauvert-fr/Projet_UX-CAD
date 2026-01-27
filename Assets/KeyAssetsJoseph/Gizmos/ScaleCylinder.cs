using UnityEngine;

public class ScaleCylinder : MonoBehaviour
{
    [SerializeField]
    private GameObject selecCylinder;

    private float initOffset;
    void Start()
    {
        initOffset = transform.localPosition.y;
        
    }

    // Update is called once per frame
    void Update()
    {
        updateCylinderScale();
    }




    public void updateCylinderScale()
    {
        selecCylinder.transform.localScale = new Vector3(selecCylinder.transform.localScale.x, transform.localPosition.y - initOffset+1, selecCylinder.transform.localScale.z);
    }
}
