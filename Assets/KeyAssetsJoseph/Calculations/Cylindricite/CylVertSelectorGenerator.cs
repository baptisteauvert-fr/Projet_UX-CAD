using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CylVertSelectorGenerator : MonoBehaviour
{
    List<Vector3> cylinderNormals = new List<Vector3>();
    List<Vector3> selectedVerts = new List<Vector3>();
    private MeshFilter objectCAD;
    [SerializeField]
    private GameObject selectionCylinder;
    private GameObject spawnedCylinder;

    // Projection Orthogonale et Equation Cartesienne
    float d;
    float planX;
    float planY;
    float planZ;
    Vector3 pNormal;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void selectFaces()
    {
        BoxCollider selCollider = transform.GetComponent<BoxCollider>();
        objectCAD = GameObject.Find("CAD").GetComponent<MeshFilter>();
        Mesh mesh = objectCAD.mesh;
        Transform meshTransform = objectCAD.transform;
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        
        cylinderNormals.Clear();
      



      
        for (int i = 0; i < triangles.Length; i+=3)
        {
          


            Vector3 p0 = vertices[triangles[i]];
            Vector3 p1 = vertices[triangles[i + 1]];
            Vector3 p2 = vertices[triangles[i + 2]];
            Vector3 worldP0 = meshTransform.TransformPoint(p0);
            Vector3 worldP1 = meshTransform.TransformPoint(p1);
            Vector3 worldP2 = meshTransform.TransformPoint(p2);

            Vector3 worldPos = meshTransform.TransformPoint((p0+p1+p2)/3);
            if(EdgeIntersectsBox(selCollider,worldP0, worldP1) || EdgeIntersectsBox(selCollider, worldP0, worldP2) || EdgeIntersectsBox(selCollider, worldP1, worldP2)) {
                Vector3 normal = meshTransform.TransformDirection(Vector3.Cross(p1 - p0, p2 - p0).normalized);
                cylinderNormals.Add(normal);
                selectedVerts.Add(meshTransform.TransformPoint(p0));
                selectedVerts.Add(meshTransform.TransformPoint(p1));
                selectedVerts.Add(meshTransform.TransformPoint(p2));
            }
           //// if (PointInsideCollider(worldPos, selCollider))
           // {

            ////    Vector3 normal = meshTransform.TransformDirection( Vector3.Cross(p1 - p0, p2 - p0).normalized);
             //   cylinderNormals.Add(normal);
             //   selectedVerts.Add(meshTransform.TransformPoint(p0));
             ////   selectedVerts.Add(meshTransform.TransformPoint(p1));
               // selectedVerts.Add(meshTransform.TransformPoint(p2));

           // }
               
        }

        selectedVerts = selectedVerts.Distinct().ToList();
        Vector3 axis = Vector3.zero;
        for (int i = 0; i < cylinderNormals.Count; i++)
        {
            for (int j = i + 1; j < cylinderNormals.Count; j++)
            {
                axis += Vector3.Cross(cylinderNormals[i], cylinderNormals[j]);
            }
        }
        Vector3 centroid = Vector3.zero;
        foreach (var v in selectedVerts)
            centroid += v;
        centroid /= selectedVerts.Count;
        axis.Normalize();
       
      
       
        //:::Seperate
        Plane fitPlane = new Plane(axis, centroid);
        List<Vector3> projected = new List<Vector3>();

        foreach (var v in selectedVerts)
            projected.Add(fitPlane.ClosestPointOnPlane(v));

        // --- build plane basis ---
        BuildPlaneBasis(fitPlane.normal, out Vector3 u, out Vector3 v2);

      Vector3 projCentroid = Vector3.zero;
foreach (var p in projected)
    projCentroid += p;
projCentroid /= projected.Count;

Vector3 origin = projCentroid;
        List<Vector2> pts2D = new List<Vector2>();

        foreach (var p in projected)
        {
            Vector3 d = p - origin;
            pts2D.Add(new Vector2(Vector3.Dot(d, u), Vector3.Dot(d, v2)));
        }

        // --- circle fit ---
        Vector2 center2D = FitCircle2D(pts2D);
        Vector3 center =
            origin + u * center2D.x + v2 * center2D.y;

        // --- radius ---
        float radius = 0f;
        foreach (var p in projected)
            radius += Vector3.Distance(p, center);
        radius /= projected.Count;

        // --- spawn cylinder ---
        spawnedCylinder = Instantiate(selectionCylinder, center, Quaternion.identity);
        spawnedCylinder.transform.up =axis;
        spawnedCylinder.transform.localScale =
            new Vector3(radius*2.6f, radius*2.6f, radius * 2.6f);



        Destroy(transform.parent.parent.parent.gameObject);

        



    }
    //CODE MADE BY CHAT GPT
    Vector3 BestFitPlane(IList<Vector3> points)
    {
        Vector3 centroid = Vector3.zero;
        foreach (var p in points)
            centroid += p;
        centroid /= points.Count;

        float xx = 0, xy = 0, xz = 0;
        float yy = 0, yz = 0, zz = 0;

        foreach (var p in points)
        {
            Vector3 r = p - centroid;
            xx += r.x * r.x;
            xy += r.x * r.y;
            xz += r.x * r.z;
            yy += r.y * r.y;
            yz += r.y * r.z;
            zz += r.z * r.z;
        }

        Vector3 normal = new Vector3(
            (yy * zz - yz * yz),
            (xz * yz - xy * zz),
            (xy * yz - xz * yy)
        ).normalized;

        return normal;
    }


    bool PointInsideCollider(Vector3 worldPoint, BoxCollider box)
    {
        Vector3 local = box.transform.InverseTransformPoint(worldPoint) - box.center;
        Vector3 half = box.size * 0.5f;

        return Mathf.Abs(local.x) <= half.x &&
               Mathf.Abs(local.y) <= half.y &&
               Mathf.Abs(local.z) <= half.z;
    }


    public void GenerateEquationCartesienne(Vector3 planeNormal, Vector3 planePoint)
    {
        Vector3 A = planePoint;
        pNormal = planeNormal;
        planX = transform.up.x;
        planY = transform.up.y;
        planZ = transform.up.z;

        float Ax = A.x;
        float Ay = A.y;
        float Az = A.z;
        d = -(planX * Ax + planY * Ay + planZ * Az);
    }
    public Vector3 ProjectionOrthogonale(Vector3 pointProj)
    {

        float distancePoint = (pointProj.x * planX + pointProj.y * planY + pointProj.z * planZ) + d / Mathf.Sqrt(planX * planX + planY * planY + planZ * planZ);
        Vector3 projection = pointProj - distancePoint * pNormal;
        //Vector3 rayVector = transform.position-projection;
        return projection;
    }
    //CODE MADE BY CHAT GPT
    void BuildPlaneBasis(Vector3 normal, out Vector3 u, out Vector3 v)
    {
        u = Vector3.Cross(normal, Vector3.up);
        if (u.sqrMagnitude < 1e-6f)
            u = Vector3.Cross(normal, Vector3.right);
        u.Normalize();
        v = Vector3.Cross(normal, u);
    }
//CODE MADE BY CHAT GPT
    Vector2 FitCircle2D(List<Vector2> pts)
    {
        float sumX = 0, sumY = 0, sumX2 = 0, sumY2 = 0, sumXY = 0;
        float sumX3 = 0, sumY3 = 0, sumX2Y = 0, sumXY2 = 0;
        int n = pts.Count;

        foreach (var p in pts)
        {
            float x = p.x, y = p.y;
            float x2 = x * x, y2 = y * y;

            sumX += x; sumY += y;
            sumX2 += x2; sumY2 += y2;
            sumXY += x * y;
            sumX3 += x2 * x;
            sumY3 += y2 * y;
            sumX2Y += x2 * y;
            sumXY2 += x * y2;
        }

        float C = n * sumX2 - sumX * sumX;
        float D = n * sumXY - sumX * sumY;
        float E = n * sumY2 - sumY * sumY;
        float G = 0.5f * (n * (sumX3 + sumXY2) - sumX * (sumX2 + sumY2));
        float H = 0.5f * (n * (sumY3 + sumX2Y) - sumY * (sumX2 + sumY2));

        float denom = C * E - D * D;
        if (Mathf.Abs(denom) < 1e-6f) return Vector2.zero;

        return new Vector2(
            (G * E - D * H) / denom,
            (C * H - D * G) / denom
        );
    }

    bool EdgeIntersectsBox(BoxCollider box, Vector3 a, Vector3 b)
    {
        Vector3 dir = b - a;          
        float length = dir.magnitude; 
        dir /= length;               
        Ray ray = new Ray(a, dir);
        return box.Raycast(ray, out _, length);
    }
}
