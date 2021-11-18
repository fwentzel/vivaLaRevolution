
using UnityEngine;

public class FollowCamAtHeight : MonoBehaviour
{
    [SerializeField] 
    private float heightToFollowAt=0;

    private Transform mainCam;
    void Start()
    {
        mainCam=Camera.main.transform;
    }

    
    void Update()
    {
        transform.position=new Vector3(mainCam.position.x,heightToFollowAt,mainCam.position.z);
    }
}
