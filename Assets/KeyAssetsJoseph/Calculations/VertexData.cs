using UnityEngine;



[System.Serializable]
public struct VertexDat
{
    public int vertNum;
    public Vector3 vertPos;
    public float distance;
    public int zoneNumber;
    public int typeOfZone;
    public VertexDat(int vertNumIn, Vector3 vertPosIn, float distanceIn)
    {
        zoneNumber = 0;
        typeOfZone = 0;
        vertNum = vertNumIn;
        vertPos = vertPosIn;
        distance = distanceIn;
    }
    public void setZoneNumber(int zone)
    {
        zoneNumber=zone;
    }
    public void setTypeOfZone(int zone)
    {
        typeOfZone = zone;
    }
}




