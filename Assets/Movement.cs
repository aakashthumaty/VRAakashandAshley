using System.Collections;
using System.Collections.Generic;
using UnityEngine;

////////////////////////////References//////////////////////////////
//https://www.youtube.com/watch?v=4Qq7d9elXNA
//https://answers.unity.com/questions/1373810/how-to-move-the-character-using-wasd.html
//https://www.studytonight.com/game-development-in-2D/making-player-move
// https://www.youtube.com/watch?v=N88P06Ylu48 this guy did smth with a plane!




public class Movement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Rigidbody rb;

    private Vector3 iv;
    private float speed;
    //public float speed;

    void Start()
    {

        rb = GetComponent<Rigidbody>();
        speed = 5;
        
    }

    // Update is called once per frame
     private void Update() {
        //print(Input.GetAxis("Horizontal"));
        rb.velocity = Vector3.right;


        iv = new Vector3(Input.GetAxisRaw("Horizontal")*speed, 0, Input.GetAxisRaw("Vertical")*speed);
        rb.velocity = iv;

 
     }
}
