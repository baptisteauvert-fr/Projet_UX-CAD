using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.UI.Image;

public class PlanarCheckGeneration : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    GameObject Source;
  
    [SerializeField]
    public GameObject planeToSpawm;
    LayerMask layerMask;
    public GameObject SpawnedPlane;
    private GameObject objectscale;
    void Start()
    {
        layerMask = LayerMask.GetMask("CADmodel");
       
        objectscale = GameObject.Find("ObjectScale");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        

    }

    public void GeneratePlanarCheck()
    {
        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(Source.transform.position, Source.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))

        {
            Destroy(SpawnedPlane);
            MeshCollider meshCollider = hit.collider as MeshCollider;
            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
            Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
            Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
            Transform hitTransform = hit.collider.transform;

                                            
                

            p0 = hitTransform.TransformPoint(p0);
            p1 = hitTransform.TransformPoint(p1);
            p2 = hitTransform.TransformPoint(p2);

            Vector3 center = (p0 + p1 + p2) / 3;
            Vector3 normal = Vector3.Cross(p1 - p0, p2 - p0).normalized;

            SpawnedPlane = Instantiate(planeToSpawm, center, transform.rotation,objectscale.transform) as GameObject;
            SpawnedPlane.transform.up = normal;


            //Visual Debug Stuff

            Debug.DrawLine(p0, p1);
            Debug.DrawLine(p1, p2);
            Debug.DrawLine(p2, p0);
            Debug.DrawRay(Source.transform.position, Source.transform.TransformDirection(Vector3.forward) * hit.distance, Color.white);

        }
        else
        {
            Debug.DrawRay(Source.transform.position, Source.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
    }
}
