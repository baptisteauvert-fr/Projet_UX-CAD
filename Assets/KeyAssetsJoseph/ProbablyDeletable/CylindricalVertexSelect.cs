
using UnityEngine;


using Unity.Mathematics;

public class CylindricalVertexSelect : MonoBehaviour
{

    [SerializeField]
    GameObject Source;

    [SerializeField]
    public GameObject planeToSpawm;
    LayerMask layerMask;
    public GameObject SpawnedPlane;

    [SerializeField] 
    private GameObject TestCube;

    [SerializeField]
    float distance;

    private GameObject CartPlane;
    MeshFilter meshFilter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Genereation de l'equation cartesienne du plan
        Mesh mesh = transform.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3 vectorAB = transform.localToWorldMatrix.MultiplyPoint3x4(vertices[0]);
        Vector3 pointA = transform.position;
        Vector3 Pnormal = transform.up;
        float Ax = vectorAB.x;
        float Ay = vectorAB.y;
        float Az = vectorAB.z;
        float planX = Pnormal.x;
        float planY = Pnormal.y;
        float planZ = Pnormal.z;

        float d = -(planX * Ax + planY * Ay + planZ * Az) ;

        //Projection orthogonale
        Vector3 pointT = TestCube.transform.position;

        float distancePoint = (pointT.x * planX + pointT.y * planY + planZ * pointT.z) + d / Mathf.Sqrt(planX * planX + planY * planY + planZ * planZ);
        distance = d;
        Vector3 projection= pointT - distancePoint * Pnormal;
        Vector3 raycastVector = projection - transform.position;



        




    }

    // Update is called once per frame
    void Update()
    {
     



    }

    public void Orthogonal (){ 
    
    
    
    }
    public void GeneratePlanarCheck3er()
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

            SpawnedPlane = Instantiate(planeToSpawm, center, transform.rotation) as GameObject;
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
