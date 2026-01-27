using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;



//::::::::PUT ON MANAGER:::::://
public class CAD_PRINT_Compare : MonoBehaviour
{
    public MeshFilter printedObject;
    public MeshFilter CADObject;
    public List<VertexDat> calcdVerts = new List<VertexDat>();





    //Valeur Pour L'utilisateur
    [SerializeField]
    private float Tolerance;
    public float maxDistance = 0;
    public float minDistance = 10000;

    //For Above
    public float greenValueMin;
    public float greenValueMax;
    public float yellowValueMax;
    public float redValueMax;

    //For Under
    public float cyanValueMin;
    public float cyanValueMax;
    public float blueValueMax;
    public float purpleValueMax;


    public float scale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        

    }



    public void ColorVerts()
    {
        printedObject = GameObject.Find("Printed").GetComponent<MeshFilter>();

        float thirdMaxError = (maxDistance - Tolerance) / 3;
        float thirdMinError = (minDistance + Tolerance) / 3;
        greenValueMin = Tolerance;
        greenValueMax = Tolerance + thirdMaxError;
        yellowValueMax = Tolerance + thirdMaxError * 2;
        redValueMax = Tolerance + thirdMaxError * 3;

        cyanValueMin = -Tolerance;
        cyanValueMax = thirdMinError - Tolerance;
        blueValueMax = thirdMinError * 2 - Tolerance;
        purpleValueMax = thirdMinError * 3- Tolerance;





        //:::COLORVERTEX:::///
        Color finalColor;

        Mesh mesh = printedObject.mesh;
        Vector3[] vertices = mesh.vertices;
        Color[] colors;
        if (mesh.colors.Length > 0)
            colors = mesh.colors;
        else
            colors = new Color[vertices.Length];

        for (int i = 0; calcdVerts.Count > i; i++)
        {
            
            if (Mathf.Abs(calcdVerts[i].distance) > Tolerance) // La valeur est plus grande que celle toleree
            {
                finalColor = DetermineColor(thirdMinError,thirdMaxError, i);
                //FIND AND COLOR VERTEX
               
                colors[calcdVerts[i].vertNum] = finalColor;
            }
            
        }
        mesh.colors = colors;
    }
    private Color DetermineColor(float thirdMinError,float thirdMaxError, int i)
    {
        Color finalColor;
        float gradient;

        if (calcdVerts[i].distance > 0) // If CONVEX
        {

            if (Mathf.Abs(calcdVerts[i].distance) - Tolerance < thirdMaxError)
            {

                gradient = Map(Mathf.Abs(calcdVerts[i].distance), Tolerance, Tolerance + thirdMaxError, 0, 1);
                finalColor = new Color(0, gradient, 0, 1);
            }
            else if (Mathf.Abs(calcdVerts[i].distance) - Tolerance < thirdMaxError * 2)
            {
                gradient = Map(Mathf.Abs(calcdVerts[i].distance), Tolerance + thirdMaxError, Tolerance + thirdMaxError * 2, 0, 1);
                finalColor = new Color(gradient, 1, 0, 1);
            }
            else
            {
                gradient = Map(Mathf.Abs(calcdVerts[i].distance), Tolerance + thirdMaxError * 2, Tolerance + thirdMaxError * 3, 1, 0);
                finalColor = new Color(1, gradient, 0, 1);
            }

        }
        else                     //If CONCAVE
        {
            if (calcdVerts[i].distance + Tolerance > thirdMinError)
            {
                gradient = Map(calcdVerts[i].distance, -Tolerance,  thirdMinError-Tolerance, 0, 1);
                finalColor = new Color(0, gradient, gradient, 1);
            }
            else if (calcdVerts[i].distance + Tolerance > thirdMinError * 2)
            {
                gradient = Map(calcdVerts[i].distance, -Tolerance + thirdMinError,  thirdMinError * 2-Tolerance, 1, 0);
                finalColor = new Color(0, gradient, 1, 1);
            }
            else
            {
                gradient = Map(calcdVerts[i].distance, -Tolerance + thirdMinError * 2, thirdMinError * 3-Tolerance, 0, 1);
                finalColor = new Color(gradient,0 , 1, 1);
                Debug.Log(gradient);
            }

        }
        

        return finalColor;
    }


    private void calcMaxMinDistance()
    {
        for (int i = 0; calcdVerts.Count > i; i++)
        {
            if (calcdVerts[i].distance>maxDistance)
                maxDistance =calcdVerts[i].distance;
            if (calcdVerts[i].distance < minDistance)
                minDistance = calcdVerts[i].distance;

        }
    }

    private void DistanceMeasuring()
    {

        // Get all Printed Info
        printedObject = GameObject.Find("Printed").GetComponent<MeshFilter>();
        Mesh printedMesh = printedObject.mesh;
        Vector3[] printedVerts = printedMesh.vertices;
        int[] printedTriangles = printedMesh.triangles;
        Transform printedTransform = printedObject.transform;

        //Get all CAD Info
        CADObject = GameObject.Find("CAD").GetComponent<MeshFilter>();
        Mesh CADMesh = CADObject.mesh;
        Vector3[] CADverts = CADMesh.vertices;
        int[] CADTriangles = CADMesh.triangles;
        Transform CADTransform = CADObject.transform;

        Vector3 printedPos;
        Vector3 projectedPrintedPos;
        Vector3 CADp0=Vector3.zero;
        Vector3 CADp1;
        Vector3 CADp2;
        Vector3 CADplaneNormal=Vector3.zero;
        Vector3 projectedToPrint; 
        float shortestDistance = 5000;
        float printedVertDistance = 0;
        float distance;
        float multiplier;

        for (int i = 0; printedVerts.Length > i; i++)
        {
            shortestDistance = 5000;
            printedPos = printedTransform.TransformPoint(printedVerts[i]);

            for (int j = 0;CADTriangles.Length> j; j+=3)
            {
                
                CADp0 = CADTransform.TransformPoint(CADverts[CADTriangles[j]]);
                CADp1= CADTransform.TransformPoint(CADverts[CADTriangles[j + 1]]);
                CADp2 = CADTransform.TransformPoint(CADverts[CADTriangles[j + 2]]);

                 CADplaneNormal = Vector3.Cross(CADp1 - CADp0, CADp2 - CADp0).normalized;
             

          

               projectedPrintedPos= ClosestPointOnTriangle(printedPos, CADp0, CADp1, CADp2);
               distance= Vector3.Distance(projectedPrintedPos, printedPos);
                projectedToPrint = (projectedPrintedPos - printedPos).normalized;
              

                    if (shortestDistance > distance)
                    {

                    if (Vector3.Dot(CADplaneNormal, projectedToPrint) < 0)
                    {
                        multiplier = 1;
                    }
                    else {
                        multiplier = -1;
                    }


                        shortestDistance = distance;
                        printedVertDistance = distance*multiplier;
                     
                    }

                
               

            }
           
            VertexDat NewVert = new VertexDat(i, printedVerts[i], printedVertDistance);
            calcdVerts.Add(new VertexDat(i, printedVerts[i], printedVertDistance));
        }
    }

  

   
    Vector3 ClosestPointOnTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        // Edge vectors
        Vector3 ab = b - a;
        Vector3 ac = c - a;
        Vector3 ap = p - a;

        float d1 = Vector3.Dot(ab, ap);
        float d2 = Vector3.Dot(ac, ap);
        if (d1 <= 0 && d2 <= 0) return a; // Closest to vertex a

        Vector3 bp = p - b;
        float d3 = Vector3.Dot(ab, bp);
        float d4 = Vector3.Dot(ac, bp);
        if (d3 >= 0 && d4 <= d3) return b; // Closest to vertex b

        Vector3 cp = p - c;
        float d5 = Vector3.Dot(ab, cp);
        float d6 = Vector3.Dot(ac, cp);
        if (d6 >= 0 && d5 <= d6) return c; // Closest to vertex c

        float vc = d1 * d4 - d3 * d2;
        if (vc <= 0 && d1 >= 0 && d3 <= 0) return a + d1 / (d1 - d3) * ab;

        float vb = d5 * d2 - d1 * d6;
        if (vb <= 0 && d2 >= 0 && d6 <= 0) return a + d2 / (d2 - d6) * ac;

        float va = d3 * d6 - d5 * d4;
        if (va <= 0 && (d4 - d3) >= 0 && (d5 - d6) >= 0) return b + (d4 - d3) / ((d4 - d3) + (d5 - d6)) * (c - b);

        // Inside triangle
        float denom = 1.0f / (va + vb + vc);
        float vFace = vb * denom;
        float wFace = vc * denom;
        return a + ab * vFace + ac * wFace;
    }
    

    public static float Map(float input, float min1, float max1, float min2, float max2)
    {
        return min2 + (input - min1) * (max2 - min2) / (max1 - min1);
    }
    private void GiveVertexDistance()
    {

    }

    private void Calculate()
    {
        DistanceMeasuring();
        calcMaxMinDistance();
        ColorVerts();
    }







}
