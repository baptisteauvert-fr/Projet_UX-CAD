using UnityEngine;

public class TranslateCylinder : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private GameObject cylinder;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateCylinderPosition();
    }

    public void updateCylinderPosition()
    {
        cylinder.transform.localPosition = new Vector3(cylinder.transform.localPosition.x,transform.localPosition.y,cylinder.transform.localPosition.z);
    }
}
