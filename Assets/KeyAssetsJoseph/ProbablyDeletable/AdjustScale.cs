using UnityEngine;

public class AdjustScale : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject objectscale;
    private float scaling;

    void Start()
    {
        objectscale= GameObject.Find("ObjectScale");
        scaling = objectscale.transform.localScale.x;
        transform.localScale = new Vector3(transform.localScale.x*scaling,transform.localScale.y*scaling,transform.localScale.z*scaling);


    }

    void Update()
    {
                
    }
}
