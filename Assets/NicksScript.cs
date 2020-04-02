using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class NicksScript : MonoBehaviour
{
    public float translationalSpeed=0.1f;
    public float rotationalSpeed=2.0f;
    public Camera aerialViewCam;
    public bool shouldLockMouse=true;
    // Start is called before the first frame update

    private Vector3 prevForwardVector;
    private float prevYawRelativeToCenter;

    private GameObject VRTrackingOrigin;  
    private GameObject theCam;

    void Start()
    {
        //verified
        VRTrackingOrigin = this.transform.parent.gameObject;
        theCam = this.gameObject;

        GameObject ovrCamRigGO=this.transform.parent.transform.parent.gameObject;
        ovrCamRigGO.GetComponent<OVRCameraRig>().enabled=false;
        ovrCamRigGO.GetComponent<OVRManager>().enabled=false;
        ovrCamRigGO.GetComponent<OVRHeadsetEmulator>().enabled=false;
        Transform possibleOVRPlayCont=ovrCamRigGO.transform.parent;
        if (possibleOVRPlayCont != null){
            possibleOVRPlayCont.gameObject.GetComponent<CharacterController>().enabled=false;
            possibleOVRPlayCont.gameObject.GetComponent<OVRPlayerController>().enabled=false;
            possibleOVRPlayCont.gameObject.GetComponent<OVRSceneSampleController>().enabled=false;
            possibleOVRPlayCont.gameObject.GetComponent<OVRDebugInfo>().enabled=false;
        } else{
            print("not using player controller... probably just rig");
        }
        this.gameObject.tag="Untagged";
        this.GetComponent<Camera>().enabled=false;
        aerialViewCam.gameObject.tag="MainCamera";
        aerialViewCam.transform.parent=this.transform;
        aerialViewCam.transform.localPosition=new Vector3(0, aerialViewCam.transform.localPosition.y, 0);
        aerialViewCam.transform.eulerAngles=new Vector3(aerialViewCam.transform.eulerAngles.x, this.transform.eulerAngles.y, aerialViewCam.transform.eulerAngles.z);
        //hit escape to unlock
        Cursor.lockState=shouldLockMouse?CursorLockMode.Locked:CursorLockMode.None;

        //start method stuff for s2c
        //d(Vector3 A, Vector3 B, Vector3 C)=(A.x−B.x)(C.z−B.z)−(A.z−B.z)(C.x−B.x)

         prevForwardVector=theCam.transform.forward;
         prevYawRelativeToCenter = angleBetweenVectors(theCam.transform.forward,VRTrackingOrigin.transform.position-theCam.transform.position);
    }

    float d(Vector3 A,Vector3 B,Vector3 C){

       
        var sol = ((A.x-B.x)*(C.z-B.z))-((A.z-B.z)*(C.x-B.x));
    
        return sol;
        //d(pointToTest,vectorSource,vectorDestination)=(pointToTest.x−vectorSource.x)(vectorDestination.y−vectorSource.y)−(pointToTest.y−vectorSource.y)(vectorDestination.x−vectorSource.x)
    }
 
    // Update is called once per frame
    float angleBetweenVectors(Vector3 A, Vector3  B){

        A.y = 0;
        B.y = 0;
       return Mathf.Rad2Deg*Mathf.Acos(Vector3.Dot(Vector3.Normalize(A), Vector3.Normalize(B)));

    }
    void Update()
    {
        this.transform.position += (Input.GetKey(KeyCode.W)?1:Input.GetKey(KeyCode.S)?-1:0)* translationalSpeed* new Vector3(this.transform.forward.x,0,this.transform.forward.z) + 
        (Input.GetKey(KeyCode.A)?-1:Input.GetKey(KeyCode.D)?1:0)* translationalSpeed* new Vector3(this.transform.right.x,0,this.transform.right.z);
        //only yaw should change for aerial view
        //only using mouse x b/c left/right makes more sense for aerial yaw than up/down
        this.transform.eulerAngles=new Vector3(0, this.transform.eulerAngles.y+Input.GetAxis("Mouse X")*rotationalSpeed, 0);        


        //update method stuff for S2C
        var howMuchUserRotated = angleBetweenVectors(prevForwardVector,theCam.transform.forward);

        var directionUserRotated=(d(theCam.transform.position+prevForwardVector, theCam.transform.position, theCam.transform.position + theCam.transform.forward)<0)?-1:1; 
        var deltaYawRelativeToCenter=prevYawRelativeToCenter-angleBetweenVectors(theCam.transform.forward,VRTrackingOrigin.transform.position-theCam.transform.position);
        var distanceFromCenter= theCam.transform.localPosition.magnitude;
        
        var longestDimensionOfPE= 5;
        var howMuchToAccel =((deltaYawRelativeToCenter<0)? -0.13f: 0.30f) * howMuchUserRotated * directionUserRotated * Mathf.Clamp(distanceFromCenter/longestDimensionOfPE/2,0,1);
        
        if(Mathf.Abs(howMuchToAccel)>0){
            VRTrackingOrigin.transform.RotateAround(theCam.transform.position,new Vector3(0,1,0),(float)howMuchToAccel);
        }

        prevForwardVector=theCam.transform.forward;
        prevYawRelativeToCenter=angleBetweenVectors(theCam.transform.forward,VRTrackingOrigin.transform.position-theCam.transform.position);
        
    }
}
