using UnityEngine;

//this dude rocks: https://www.youtube.com/watch?v=_yf5vzZ2sYE

public class SelectionScript0 : MonoBehaviour
{
    [SerializeField] private string selectableTag = "Selectable";
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private Material defaultMaterial;

    private Transform currentSelected;
    
    void Start()
    {


    }

    private void Update()
    {

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
                print("OMG SELECTED!!!");
                var selectionRenderer = selection.GetComponent<Renderer>();
                if (selectionRenderer != null)
                {
                    selectionRenderer.material = highlightMaterial;
                }

                currentSelected = selection;
            }
        }
    }
}