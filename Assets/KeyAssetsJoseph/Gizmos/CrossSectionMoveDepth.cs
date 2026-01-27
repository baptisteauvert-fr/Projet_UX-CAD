using UnityEngine;

public class CrossSectionMoveDepth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField]
    private GameObject selectionBox;


  
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        move();
        
    }

    private void  move()
    {
       selectionBox.transform.localPosition = new Vector3(selectionBox.transform.localPosition.x, selectionBox.transform.localPosition.y, transform.localPosition.z);
    }


        
}
