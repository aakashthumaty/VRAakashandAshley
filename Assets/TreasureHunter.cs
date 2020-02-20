//These are resources I used
//https://docs.unity3d.com/Manual/class-TextMesh.html
//https://docs.unity3d.com/ScriptReference/Input-inputString.html

//this dude rocks: https://www.youtube.com/watch?v=_yf5vzZ2sYE

//we referenced lots of nicks code for the grabbing/GO children/raycasting with the oculus controllers

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum AttachmentRule{KeepRelative,KeepWorld,SnapToTarget}


public class TreasureHunter : MonoBehaviour
{
    // Start is called before the first frame update
    public CollectibleTreasure[] treasures;
    public Inventory invent;

    public TextMesh win;

    public TextMesh score;

    CollectibleTreasure thingIGrabbed;

    Vector3 previousPointerPos;
    Vector3 myv;

//selectable ness

    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;

    private Transform currentSelected;

    public GameObject leftPointerObject;
    public GameObject rightPointerObject;
    public GameObject selfGO;


    void Start()
    {
        //score = GetComponent<TextMesh>();
        invent.treasureInventory = new int[] {0,0,0};
        score.text = "";
        win.text = "";

    }

    public void attachGameObjectToAChildGameObject(GameObject GOToAttach, GameObject newParent, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule, bool weld){
        GOToAttach.transform.parent=newParent.transform;
        handleAttachmentRules(GOToAttach,locationRule,rotationRule,scaleRule);
        if (weld){
            simulatePhysics(GOToAttach,Vector3.zero,false);
        }
    }

    public static void detachGameObject(GameObject GOToDetach, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule){
        //making the parent null sets its parent to the world origin (meaning relative & global transforms become the same)
        GOToDetach.transform.parent=null;
        handleAttachmentRules(GOToDetach,locationRule,rotationRule,scaleRule);
    }

        public static void handleAttachmentRules(GameObject GOToHandle, AttachmentRule locationRule, AttachmentRule rotationRule, AttachmentRule scaleRule){
        GOToHandle.transform.localPosition=
        (locationRule==AttachmentRule.KeepRelative)?GOToHandle.transform.position:
        //technically don't need to change anything but I wanted to compress into ternary
        (locationRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localPosition:
        new Vector3(0,0,0);

        //localRotation in Unity is actually a Quaternion, so we need to specifically ask for Euler angles
        GOToHandle.transform.localEulerAngles=
        (rotationRule==AttachmentRule.KeepRelative)?GOToHandle.transform.eulerAngles:
        //technically don't need to change anything but I wanted to compress into ternary
        (rotationRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localEulerAngles:
        new Vector3(0,0,0);

        GOToHandle.transform.localScale=
        (scaleRule==AttachmentRule.KeepRelative)?GOToHandle.transform.lossyScale:
        //technically don't need to change anything but I wanted to compress into ternary
        (scaleRule==AttachmentRule.KeepWorld)?GOToHandle.transform.localScale:
        new Vector3(1,1,1);
    }

    public void simulatePhysics(GameObject target,Vector3 oldParentVelocity,bool simulate){
        Rigidbody rb=target.GetComponent<Rigidbody>();
        if (rb){
            if (!simulate){
                Destroy(rb);
            } 
        } else{
            if (simulate){
                //there's actually a problem here relative to the UE4 version since Unity doesn't have this simple "simulate physics" option
                //The object will NOT preserve momentum when you throw it like in UE4.
                //need to set its velocity itself.... even if you switch the kinematic/gravity settings around instead of deleting/adding rb
                //maybe dont comment out this next line
                //Rigidbody newRB=target.AddComponent<Rigidbody>();
                //newRB.velocity=oldParentVelocity;
            }
        }
    }

        void forceGrab(bool pressedA){
        print("yo you force grabbed");
        RaycastHit outHit;
        //notice I'm using the layer mask again
        if (Physics.Raycast(rightPointerObject.transform.position, rightPointerObject.transform.forward, out outHit, 1000.0f))//,collectiblesMask))
        {
            AttachmentRule howToAttach=pressedA?AttachmentRule.KeepWorld:AttachmentRule.SnapToTarget;
            attachGameObjectToAChildGameObject(outHit.collider.gameObject,rightPointerObject.gameObject,howToAttach,howToAttach,AttachmentRule.KeepWorld,true);
            thingIGrabbed=outHit.collider.gameObject.GetComponent<CollectibleTreasure>();
        }
    }

    void letGo(){
        win.text = "not even letgo";
        if(thingIGrabbed){
        detachGameObject(thingIGrabbed.gameObject,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld);
        simulatePhysics(thingIGrabbed.gameObject,Vector3.zero,true);
        win.text = "let go but nothing else.";
        float firstx, firsty, firstz, secondx, secondy, secondz;
        firstx = rightPointerObject.transform.position.x;
        firsty = rightPointerObject.transform.position.y;
        firstz = rightPointerObject.transform.position.z;
        secondx = selfGO.transform.position.x;
        secondy = selfGO.transform.position.y;
        secondz = selfGO.transform.position.z;
        
        myv = new Vector3(firstx - secondx, firsty - secondy, firstz - secondz);
        
        if (myv.x <= 0.5 && myv.y <= 0.5 && myv.z <= 0.5){
            print("OMG SELECTED!!! woot woot woot");
            win.text = "added to waist";

            var v = thingIGrabbed.gameObject.GetComponent<CollectibleTreasure>();
            print(v.value);
            var n = v.pf;
            win.text = n;
            int count;
            print(v.name);
            print(n);
            invent.dict.TryGetValue((CollectibleTreasure)Resources.Load(n, typeof(CollectibleTreasure)), out count);
            invent.dict[(CollectibleTreasure)Resources.Load(n, typeof(CollectibleTreasure))] = count + 1;

            Destroy(thingIGrabbed.gameObject);

            int point = 0;
            int kc = 0;
            score.fontSize = 40;
            score.font.material.color = Color.red;
            score.text = "got here 1";
            foreach(KeyValuePair<CollectibleTreasure,int> iv in invent.dict)
                    {
                        print("this is the invent stuff: " + invent.dict);
                        print(iv);
                        kc += invent.dict[iv.Key];
                        point += invent.dict[iv.Key]*iv.Key.value;
                        print(invent.dict[iv.Key]);

                        Debug.Log(iv.Key);
                        Debug.Log(iv.Value);
                        score.text = "got here 2";
                        
                    }

                win.text = "";
                win.text = "Hi. This is Ashley and Aakash. You have " + kc + " items. Worth " + point + " points.";
            

        }
        //myv = Vector3.Distance(rightPointerObject.transform.position, selfGO.transform.position);

    
        //second param : (thingIGrabbed.gameObject.transform.position-previousPointerPos)/Time.deltaTime


        // if (thingIGrabbed){
        //     Collider[] overlappingThingsWithLeftHand=Physics.OverlapSphere(leftPointerObject.transform.position,0.01f);
        //     if (overlappingThingsWithLeftHand.Length>0){
        //         // if (thingOnGun){
        //         //     detachGameObject(thingOnGun,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld);
        //         //     simulatePhysics(thingOnGun,Vector3.zero,true);
        //         // }
        //         attachGameObjectToAChildGameObject(overlappingThingsWithLeftHand[0].gameObject,leftPointerObject,AttachmentRule.SnapToTarget,AttachmentRule.SnapToTarget,AttachmentRule.KeepWorld,true);
        //         //thingOnGun=overlappingThingsWithLeftHand[0].gameObject;
        //         thingIGrabbed=null;
        //     }else{
        //         detachGameObject(thingIGrabbed.gameObject,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld,AttachmentRule.KeepWorld);
        //         simulatePhysics(thingIGrabbed.gameObject,(rightPointerObject.gameObject.transform.position-previousPointerPos)/Time.deltaTime,true);
        //         thingIGrabbed=null;
        //     }
        // }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //var selection;
       if (currentSelected != null)
        {
            var selectionRenderer = currentSelected.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            currentSelected = null;
        }
        ///oculus raycasting
        if (OVRInput.GetDown(OVRInput.RawButton.A)){
            //var ray = 
            RaycastHit outHit;
            if (Physics.Raycast(rightPointerObject.transform.position, rightPointerObject.transform.forward, out outHit, 1000.0f))
            {
                win.fontSize = 20;
                win.font.material.color = Color.blue;
                win.text = "Hi Ashley" + outHit.transform.gameObject.name;
                print("RAY RAY RAY!");
                var selection = outHit.transform;
            if (selection.CompareTag(selectableTag))
            {
                forceGrab(false);
                foreach (char c in Input.inputString){
                    
                    if(c == 'g'){
                        print("OMG SELECTED!!!");

                        var valiii = selection.gameObject.GetComponent<CollectibleTreasure>();
                        print(valiii.value);
                        var n = valiii.pf;

                        int count;
                        invent.dict.TryGetValue((CollectibleTreasure)Resources.Load("Assets/" + n, typeof(CollectibleTreasure)), out count);
                        invent.dict[(CollectibleTreasure)Resources.Load("Assets/" + n, typeof(CollectibleTreasure))] = count + 1;

                        Destroy(selection.gameObject);
                    }

                }

                
                var selectionRenderer = selection.GetComponent<Renderer>();

                if (selectionRenderer != null)
                {
                    selectionRenderer.material = highlightMaterial;
                }

                currentSelected = selection;
            }
            }
        }else if (OVRInput.GetUp(OVRInput.RawButton.A) ){
            letGo();
        }

    /////selectableness

        
        // var rayforkb = Camera.main.ScreenPointToRay(Input.mousePosition);
        // RaycastHit hit;
        // if (Physics.Raycast(rayforkb, out hit))
        // {

        //     //var 
        //     var selection = hit.transform;
        //     if (selection.CompareTag(selectableTag))
        //     {
        //         foreach (char c in Input.inputString){
                    
        //             if(c == 'g'){
        //                 print("OMG SELECTED!!!");

        //                 var valiii = selection.gameObject.GetComponent<CollectibleTreasure>();
        //                 print(valiii.value);
        //                 var n = valiii.pf;

        //                 int count;
        //                 invent.dict.TryGetValue((CollectibleTreasure)Resources.Load(n, typeof(CollectibleTreasure)), out count);
        //                 invent.dict[(CollectibleTreasure)Resources.Load(n, typeof(CollectibleTreasure))] = count + 1;

        //                 Destroy(selection.gameObject);
        //             }

        //         }

                
        //         var selectionRenderer = selection.GetComponent<Renderer>();

        //         if (selectionRenderer != null)
        //         {
        //             selectionRenderer.material = highlightMaterial;
        //         }

        //         currentSelected = selection;
        //     }
        // }

    /////selectableness


        foreach (char c in Input.inputString)
        {
        //     if(c == '1'){
        //         if(!invent.colTres.Contains(treasures[0])){
                    
        //             invent.colTres.Add(treasures[0]);
        //             print("wow that was a 1");

        //         }
        //         //This is for when we eventually want to have more than 1 of each inventory
        //         // invent.treasureInventory[0] += 1;
        //         // print(invent.treasureInventory[0]);


        //     }else if (c == '2'){
        //         if(!invent.colTres.Contains(treasures[1])){
                    
        //             invent.colTres.Add(treasures[1]);
        //             print("wow that was a 2");

        //         }
        //         //This is for when we eventually want to have more than 1 of each inventory
        //         // invent.treasureInventory[1] += 1;
        //         // print(invent.treasureInventory[1]);

        //     }else if (c == '3'){

        //         if(!invent.colTres.Contains(treasures[2])){
                    
        //             invent.colTres.Add(treasures[2]);
        //             print("wow that was a 3");

        //         }
        //         //This is for when we eventually want to have more than 1 of each inventory
        //         // invent.treasureInventory[2] += 1;
        //         // print(invent.treasureInventory[2]);

        //     }else 
        if (c == 's'){
                print("wow that was a s");
                // int position = Array.IndexOf(invent.treasureInventory, 0);
                // if(position < 0){

                //     // yay we collected all 3
                // }
                print("something is happening");
                win.fontSize = 20;
                win.font.material.color = Color.blue;
                int point = 0;
                int kc = 0;

                // foreach (CollectibleTreasure t in invent.dict){
                //     point += t.value;
                // }
                
                 foreach(KeyValuePair<CollectibleTreasure,int> iv in invent.dict)
                    {
                        
                        kc += invent.dict[iv.Key];
                        point += invent.dict[iv.Key]*iv.Key.value;
                        print(invent.dict[iv.Key]);

                        Debug.Log(iv.Key);
                        Debug.Log(iv.Value);
                        
                    }

                win.text = "somethingsomething";
                win.text = "Hi. This is Ashley and Aakash. You have " + kc + " items. Worth " + point + " points.";
            }


            if(invent.colTres.Count == 3){
                print("Dude, you win.");
                score.fontSize = 33;
                score.text = "Dude. You Win. This is Aakash again.";
            }

        }
        
    }
}
