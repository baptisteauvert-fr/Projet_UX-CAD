
using UnityEngine;



public class CylIdentifierGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    GameObject raySource;
    [SerializeField]
    GameObject faceLoopSelect;

  
    LayerMask layerCAD;
   
    
  
    private GameObject crossSection;

    private MeshFilter objectCAD;
    [SerializeField]


    public GameObject objectscale;

    void Start()
    {
        layerCAD = LayerMask.GetMask("CADmodel");
        objectscale = GameObject.Find("ObjectScale");
       
     
       
    }

    // Update is called once per frame
    void Update()
    {
        
   
    }
    public void GenerateCrossSectionSelector()
    {
       
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(raySource.transform.position, raySource.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerCAD))

        {
            
            MeshCollider meshCollider = hit.collider as MeshCollider;
            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
            Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
            Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
            Transform hitTransform = hit.collider.transform;



            //Convert to WorldSpace
            p0 = hitTransform.TransformPoint(p0);
            p1 = hitTransform.TransformPoint(p1);
            p2 = hitTransform.TransformPoint(p2);

            float d1= Vector3.Distance(p0, p1);
            float d2 = Vector3.Distance(p0, p2);
            float d3= Vector3.Distance(p1, p2);

            Vector3 d1Middle = p1 + (p0 - p1) / 2;
            Vector3 d2Middle = p2 + (p0 - p2) / 2;
            Vector3 d3Middle = p1 + (p2 - p1) / 2;
            Vector3 Long1;
            Vector3 Long2;
            Vector3 center;

            if (d1 > d2)
            {
                if (d2 < d3)
                {
                    Long1 = d1Middle;
                    Long2 = d3Middle;

                }
                else
                {
                    Long1 = d1Middle;
                    Long2 = d2Middle;

                }
            }
            else if (d1>d3)
            {
                Long1 = d1Middle;
                Long2 = d2Middle;


            }
            else
            {
                Long1 = d2Middle;
                Long2 = d3Middle;

            }
            center= Long1+(Long2 - Long1)/2;

            Vector3 normal = Vector3.Cross(p1 - p0, p2 - p0).normalized;
            
          
            //SpawnedPlane.transform.up = Vector3.Cross(normal,rightEdge);
            crossSection = Instantiate(faceLoopSelect, center, transform.rotation,objectscale.transform) as GameObject;

            crossSection.transform.forward = normal;
      
            //Visual Debug Stuff

            Debug.DrawLine(p0, p1);
            Debug.DrawLine(p1, p2);
            Debug.DrawLine(p2, p0);
            Debug.DrawRay(raySource.transform.position, raySource.transform.TransformDirection(Vector3.forward) * hit.distance, Color.white);

        }
        else
        {   
            Debug.Log("Did not Hit");
        }
    }


}
