using System.Collections.Generic;
using UnityEngine;

public class CalculationManager : MonoBehaviour
{
    public List<GameObject> calculatorsCylindrical = new List<GameObject>();
    public List<GameObject> calculatorsPlanar = new List<GameObject>();
    MeshFilter printedObject;
    LayerMask layerPrint;

    [SerializeField]
    GameObject controller;

    List<VertexDat> allVerts = new List<VertexDat>();
    List<List<VertexDat>> ObjectVertData;
    List<Color> meshColors;
    private int zoneCount = 0;
   // type of Zone 0 corresponds to cylindrical
   // type of Zone 1 is planar
   bool scandone = false;
    public bool CADvsPrint=false;
    void Start()
    {
        layerPrint = LayerMask.GetMask("Print");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
         calculateCylindricalSurfaces();
        }
        if (scandone == false)
        {
            if (Input.GetMouseButtonDown(1))
            {
                calculatePlanarSurfaces();
                scandone = true;
            }

        }
      
    }



    public void addObjectToList(List<GameObject> list, GameObject added)
    {
        list.Add(added);
    }

    public void calculateCylindricalSurfaces()
    {
        List<VertexDat> tempVert = new List<VertexDat>();
        for (int i = 0; calculatorsCylindrical.Count > i; i++)
        {
            calculatorsCylindrical[i].GetComponent<CylinderScan>().Calculation();
            //AddVerts
            tempVert = calculatorsCylindrical[i].GetComponent<CylinderScan>().selectedVerts;
            for(int j = 0; j < tempVert.Count; j++)
            {
                tempVert[j].setZoneNumber(zoneCount);
                tempVert[j].setTypeOfZone(0);
                allVerts.Add(tempVert[j]);
                zoneCount++;
               
            }
        }
    }

    public void calculatePlanarSurfaces()
    {
        List<VertexDat> tempVert = new List<VertexDat>();
        for (int i = 0; calculatorsPlanar.Count > i; i++)
        {
            calculatorsPlanar[i].GetComponent<PlanarScan>().Calculation();
            //AddVerts
            tempVert = calculatorsCylindrical[i].GetComponent<CylinderScan>().selectedVerts;
            for (int j = 0; j < tempVert.Count; j++)
            {
                tempVert[j].setZoneNumber(zoneCount);
                tempVert[j].setTypeOfZone(0);
                allVerts.Add(tempVert[j]);
                zoneCount++;

            }
        }
    
    }
    public void calculateSurfaces()
    {
        calculateCylindricalSurfaces();
        calculatePlanarSurfaces();
    }


    public float DisplayVertDistance()
    {
        float vertDistance=0;
        int vert1=0;
        int vert2=0;
        int vert3=0;
        float dist1 = 0;
        float dist2 = 0;
        float dist3 = 0;
        bool vert1Found=false;
        bool vert2Found=false;
        bool vert3Found=false;

        printedObject = GameObject.Find("Printed").GetComponent<MeshFilter>();
        






        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(controller.transform.position, controller.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerPrint))

        {
           
            MeshCollider meshCollider = hit.collider as MeshCollider;
            Mesh mesh = meshCollider.sharedMesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
            Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
            Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
            Transform hitTransform = hit.collider.transform;


             vert1= hit.triangleIndex * 3 + 0;
             vert2 = hit.triangleIndex * 3 + 1;
             vert3 = hit.triangleIndex * 3 + 1;





            //Visual Debug Stuff

            Debug.DrawLine(p0, p1);
            Debug.DrawLine(p1, p2);
            Debug.DrawLine(p2, p0);
           

        }
        else //::::FAIL CONDITION DISPLAY VERTEX NOT CALCULATED::::::::://
        {
            vertDistance = -2000;
        }
        List<VertexDat> vertsToSearch;
        if (CADvsPrint == false)
        {
            vertsToSearch = allVerts;
        }
        else
        {
            vertsToSearch=transform.GetComponent<CAD_PRINT_Compare>().calcdVerts;
        }


    
            for (int i = 0; i < allVerts.Count; i++)
            {

                if (allVerts[i].vertNum == vert1)
                {
                    vert1Found = true;
                    dist1 = allVerts[i].distance;
                }
                if (allVerts[i].vertNum == vert2)
                {
                    vert2Found = true;
                    dist2 = allVerts[i].distance;
                }
                if (allVerts[i].vertNum == vert3)
                {
                    vert3Found = true;
                    dist3 = allVerts[i].distance;

                }

                if (vert1Found && vert2Found && vert3Found)
                    i = allVerts.Count;
            }
        vertDistance = (dist1 + dist2 + dist3) / 3;

        return vertDistance;
    }
}
