using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RTSSelection : MonoBehaviour
{
    public LayerMask groundLayer;
    public LayerMask unitMask;
    public UnityEvent<List<RTSUnit>> OnUnitSelection;
    public List<RTSUnit> selectedUnits = new List<RTSUnit>();


    private Vector3 mouseStart = Vector3.zero;
    private Vector3 mouseEnd = Vector3.zero;
    private bool isDragging;


    private void Start()
    {
        transform.position = Vector3.zero;
        
        OnUnitSelection.AddListener((value) =>
        {
            print(value.Count);
        });
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseStart = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            mouseEnd = Input.mousePosition;
            if((mouseStart - mouseEnd).magnitude > 40)
            {
                isDragging = true;
            }
        }


        if (Input.GetMouseButtonUp(0))
        {
            if(isDragging)
                HandleSelect();
            else
                HandleClick();

            isDragging = false;
        }
    }
    private void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(mouseStart);
        RaycastHit hit;

        if (!Input.GetKey(KeyCode.LeftShift))
            selectedUnits = new List<RTSUnit>();
        
        
        // No Unit found
        if (!Physics.Raycast(ray, out hit, 1000, unitMask))
        {
            OnUnitSelection?.Invoke(selectedUnits);
            return;
        }
        
        // Get Unit
        RTSUnit rtsUnit = hit.transform.GetComponentInParent<RTSUnit>();
        if(rtsUnit)
            selectedUnits.Add(rtsUnit);
        
        OnUnitSelection?.Invoke(selectedUnits);

    }

    public Vector3 CastToGround(Vector2 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        RaycastHit hit;
        
        
        // No Unit found
        if (!Physics.Raycast(ray, out hit, 1000, groundLayer))
        {
            return hit.point;
        }
        
        return Vector3.zero;
    }

    void HandleSelect()
    {
        
        if (!Input.GetKey(KeyCode.LeftShift))
            selectedUnits = new List<RTSUnit>();
        
        print("Handling Select");
        Vector3[] verts = new Vector3[4];
        Vector3[] vecs = new Vector3[4];
        
        Vector2[] corners = getBoundingBox(mouseStart, mouseEnd);

        for (int i = 0; i < corners.Length; i++)
        {
            Ray ray = Camera.main.ScreenPointToRay(corners[i]);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, groundLayer))
            {
                verts[i] = hit.point;
                vecs[i] = ray.origin - hit.point;
                Debug.DrawLine(Camera.main.ScreenToWorldPoint(corners[i]), hit.point, Color.red, 1.0f);
            }
        }
        
        Mesh selectionMesh = generateSelectionMesh(verts,vecs);

        MeshCollider selectionBox = gameObject.AddComponent<MeshCollider>();
        selectionBox.sharedMesh = selectionMesh;
        selectionBox.convex = true;
        selectionBox.isTrigger = true;

    
        Destroy(selectionBox, 0.1f);
        
        Invoke("SendSelection", 0.05f);
    }

    private void SendSelection()
    {
        OnUnitSelection?.Invoke(selectedUnits);
    }
    
    private void OnTriggerEnter(Collider other)
    { 
        print("ENTERING");
        RTSUnit rtsUnit = other.transform.GetComponentInParent<RTSUnit>();
        if(rtsUnit)
            selectedUnits.Add(rtsUnit);
    }
    
    //generate a mesh from the 4 bottom points
    Mesh generateSelectionMesh(Vector3[] corners, Vector3[] vecs)
    {
        Vector3[] verts = new Vector3[8];
        int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 }; //map the tris of our cube

        for(int i = 0; i < 4; i++)
        {
            verts[i] = corners[i];
        }

        for(int j = 4; j < 8; j++)
        {
            verts[j] = corners[j - 4] + vecs[j - 4];
        }

        Mesh selectionMesh = new Mesh();
        selectionMesh.vertices = verts;
        selectionMesh.triangles = tris;

        return selectionMesh;
    }
    
    Vector2[] getBoundingBox(Vector2 p1,Vector2 p2)
    {
        Vector2 newP1;
        Vector2 newP2;
        Vector2 newP3;
        Vector2 newP4;

        if (p1.x < p2.x) //if p1 is to the left of p2
        {
            if (p1.y > p2.y) // if p1 is above p2
            {
                newP1 = p1;
                newP2 = new Vector2(p2.x, p1.y);
                newP3 = new Vector2(p1.x, p2.y);
                newP4 = p2;
            }
            else //if p1 is below p2
            {
                newP1 = new Vector2(p1.x, p2.y);
                newP2 = p2;
                newP3 = p1;
                newP4 = new Vector2(p2.x, p1.y);
            }
        }
        else //if p1 is to the right of p2
        {
            if (p1.y > p2.y) // if p1 is above p2
            {
                newP1 = new Vector2(p2.x, p1.y);
                newP2 = p1;
                newP3 = p2;
                newP4 = new Vector2(p1.x, p2.y);
            }
            else //if p1 is below p2
            {
                newP1 = p2;
                newP2 = new Vector2(p1.x, p2.y);
                newP3 = new Vector2(p2.x, p1.y);
                newP4 = p1;
            }

        }

        Vector2[] corners = { newP1, newP2, newP3, newP4 };
        return corners;

    }
    
    
    private void OnGUI()
    {
        if(isDragging)
        {
            var rect = Utils.GetScreenRect(mouseStart, mouseEnd);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }
}
