//These are resources I used
//https://docs.unity3d.com/Manual/class-TextMesh.html
//https://docs.unity3d.com/ScriptReference/Input-inputString.html

//this dude rocks: https://www.youtube.com/watch?v=_yf5vzZ2sYE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TreasureHunter : MonoBehaviour
{
    // Start is called before the first frame update
    public CollectibleTreasure[] treasures;
    public Inventory invent;

    public TextMesh win;

    public TextMesh score;

//selectable ness

    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;

    private Transform currentSelected;



    void Start()
    {
        //score = GetComponent<TextMesh>();
        invent.treasureInventory = new int[] {0,0,0};
        score.text = "";
        win.text = "";

    }

    // Update is called once per frame
    void Update()
    {

    /////selectableness

        if (currentSelected != null)
        {
            var selectionRenderer = currentSelected.GetComponent<Renderer>();
            selectionRenderer.material = defaultMaterial;
            currentSelected = null;
        }
        
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {

            var selection = hit.transform;
            if (selection.CompareTag(selectableTag))
            {
                foreach (char c in Input.inputString){
                    
                    if(c == 'g'){
                        print("OMG SELECTED!!!");

                        var valiii = selection.gameObject.GetComponent<CollectibleTreasure>();
                        print(valiii.value);
                        var n = valiii.pf;

                        int count;
                        invent.dict.TryGetValue((CollectibleTreasure)AssetDatabase.LoadAssetAtPath("Assets/" + n, typeof(CollectibleTreasure)), out count);
                        invent.dict[(CollectibleTreasure)AssetDatabase.LoadAssetAtPath("Assets/" + n, typeof(CollectibleTreasure))] = count + 1;

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

                win.text = "";
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
