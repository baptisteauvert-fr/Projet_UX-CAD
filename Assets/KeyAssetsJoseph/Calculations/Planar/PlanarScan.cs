using System.Collections.Generic;

using UnityEngine;



public class PlanarScan : MonoBehaviour
{
    [SerializeField]
    GameObject TestCube;
    [SerializeField]
    public MeshFilter printedObject;
    
    public Collider selectionCollider;

    private GameObject SpawnedPlane;

    LayerMask selectionLayer;
    LayerMask distanceLayer;
    [SerializeField]
    private float Tolerance;

    // Projection Orthogonale et Equation Cartesienne
    float d;
    float planX;
    float planY;
    float planZ;
    Vector3 pNormal;


    //Valeur Pour L'utilisateur
 
    public float maxDistance = -700;
    public float greenValueMin;
    public float greenValueMax;
    public float yellowValueMin;
    public float yellowValueMax;
    public float redValueMin;
    public float redValueMax;

    private GameObject manager;

    List<VertexDat> selectedVerts= new List<VertexDat>();

    void Start()
    {
        selectionLayer = LayerMask.GetMask("Selection");
        distanceLayer = LayerMask.GetMask("Distance");
        manager = GameObject.Find("Manager");
        manager.GetComponent<CalculationManager>().addObjectToList(manager.GetComponent<CalculationManager>().calculatorsPlanar, transform.gameObject);
        GenerateEquationCartesienne();

    }

    void Update()
    {
       
    }

    public void ColorVerts()
    {
        float thirdMaxError = (maxDistance - Tolerance) / 3;
        greenValueMin = Tolerance;
        greenValueMax = Tolerance + thirdMaxError;
        yellowValueMin = Tolerance + thirdMaxError;
        yellowValueMax = Tolerance + thirdMaxError * 2;
        redValueMin = Tolerance + thirdMaxError * 2;
        redValueMax = Tolerance + thirdMaxError * 3;
        //:::COLORVERTEX:::///
        Color finalColor;

        Mesh mesh = printedObject.mesh;
        Vector3[] vertices = mesh.vertices;
        Color[] colors;
        if (mesh.colors.Length > 0)
            colors = mesh.colors;
        else
            colors = new Color[vertices.Length];

        for (int i = 0; selectedVerts.Count > i; i++)
        {
            if (selectedVerts[i].distance > Tolerance) // La valeur est plus grande que celle toleree
            {
                finalColor = DetermineColor(thirdMaxError, i);
                //FIND AND COLOR VERTEX
                colors[selectedVerts[i].vertNum] = finalColor;
            }
          
        }
        mesh.colors = colors;

    }

    private Color DetermineColor(float thirdMaxError, int i)
    {
        Color finalColor;
        float gradient;



        if (selectedVerts[i].distance > Tolerance)
        {

            if (selectedVerts[i].distance - Tolerance < thirdMaxError)
            {

                gradient = Map(Mathf.Abs(selectedVerts[i].distance), Tolerance, Tolerance + thirdMaxError, 0, 1);
                finalColor = new Color(0, gradient, 0, 1);
            }
            else if (selectedVerts[i].distance - Tolerance < thirdMaxError * 2)
            {
                gradient = Map(Mathf.Abs(selectedVerts[i].distance), Tolerance + thirdMaxError, Tolerance + thirdMaxError * 2, 0, 1);
                finalColor = new Color(gradient, 1, 0, 1);
            }
            else
            {
                gradient = Map(Mathf.Abs(selectedVerts[i].distance), Tolerance + thirdMaxError * 2, Tolerance + thirdMaxError * 3, 1, 0);
                finalColor = new Color(1, gradient, 0, 1);
            }

        }
        else
        {
            finalColor = new Color(0, 0, 1, 1);

        }
        return finalColor;
    }
    public void DistanceSorting()
    {
        float biggestdistance = -5000;
        float newDist;
        for (int i = 0; selectedVerts.Count > i; i++)
        {
            if (biggestdistance < selectedVerts[i].distance)
                biggestdistance = selectedVerts[i].distance;
        }

        for (int i = 0; selectedVerts.Count > i; i++)
        {
            newDist = biggestdistance - selectedVerts[i].distance;
            if (newDist > maxDistance)
                maxDistance = newDist;

            
            selectedVerts[i] = new VertexDat(selectedVerts[i].vertNum, selectedVerts[i].vertPos, newDist);
        }

    }
    public void DistanceMeasuring()
    {
        distanceLayer = LayerMask.GetMask("Distance");

    
        for (int i = 0; selectedVerts.Count > i; i++)
        {
            Plane plane = new Plane(pNormal, transform.position);
            float signedDistance = plane.GetDistanceToPoint(selectedVerts[i].vertPos);


            selectedVerts[i] = new VertexDat(selectedVerts[i].vertNum, selectedVerts[i].vertPos, signedDistance);

        }
    }

    public void SelectVertices()
    {
       
        printedObject = GameObject.Find("Printed").GetComponent<MeshFilter>();
        
        Mesh mesh = printedObject.mesh;
        Transform meshTransform = printedObject.transform;
        Vector3[] verts = mesh.vertices;

        for (int i = 0; verts.Length > i; i++)
        {
            Vector3 worldPos = meshTransform.TransformPoint(verts[i]);
            if (PointInsideCollider(worldPos))
            {
                VertexDat NewVert = new VertexDat(i, worldPos, 0f);
                selectedVerts.Add(NewVert);
                Debug.DrawRay(worldPos, Vector3.up * 0.02f, Color.green, 1f);
            }

        }
        for (int i = 0; selectedVerts.Count > i; i++)    //DEBUG DELETE THIS
        {
            Debug.DrawRay(selectedVerts[i].vertPos, Vector3.up * 0.02f, Color.green, 1f);
        }
    }

        bool PointInsideCollider(Vector3 point)
    {
       selectionCollider= transform.GetChild(0).GetChild(0).GetComponent<BoxCollider>();
        Vector3 closest = selectionCollider.ClosestPoint(point);
        return closest == point;
    }
    public void GenerateEquationCartesienne()
    {
        Vector3 A = transform.position;
        pNormal = transform.up;
        planX = transform.up.x;
        planY = transform.up.y;
        planZ = transform.up.z;

        float Ax = A.x;
        float Ay = A.y;
        float Az = A.z;
        d = -(planX * Ax + planY * Ay + planZ * Az);
    }
    public Vector3 ProjectionOrthogonalePoint(Vector3 pointProj,float distance)
    {

        float distancePoint = (pointProj.x * planX + pointProj.y * planY + pointProj.z * planZ + d  ) / Mathf.Sqrt(planX * planX + planY * planY + planZ * planZ);
        distance = distancePoint;
        Vector3 projection = pointProj - distancePoint * pNormal;
        //Vector3 rayVector = transform.position - projection;
        return projection;
    }
    public float ProjectionOrthogonaleDistance(Vector3 pointProj)
    {

        float distancePoint = (pointProj.x * planX + pointProj.y * planY + pointProj.z * planZ + d) / Mathf.Sqrt(planX * planX + planY * planY + planZ * planZ);
     
        return distancePoint;
    }


    public static float Map(float input, float min1, float max1, float min2, float max2)
    {
        return min2 + (input - min1) * (max2 - min2) / (max1 - min1);
    }



    public void Calculation()
    {
        GenerateEquationCartesienne();
        SelectVertices();
        DistanceMeasuring();
        DistanceSorting();
        ColorVerts();
    }
}
