using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is create
   
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            transform.GetComponent<PlanarCheckGeneration>().GeneratePlanarCheck();
        }
        if (Input.GetMouseButtonDown(1))
        {
            transform.GetComponent<PlanarScan>().SelectVertices();
            transform.GetComponent<PlanarScan>().DistanceMeasuring();
            
          
        }
       
     
        if (Input.GetMouseButtonDown(2))
        {
            transform.GetComponent<PlanarScan>().ColorVerts();

        }


    }
}
