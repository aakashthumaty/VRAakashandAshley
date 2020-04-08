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

        // if (Input.GetKey(KeyCode.A))
        //      rb.AddForce(Vector3.left);
        //  if (Input.GetKey(KeyCode.D))
        //      rb.AddForce(Vector3.right);
        //  if (Input.GetKey(KeyCode.W))
        //      rb.AddForce(Vector3.up);
        //  if (Input.GetKey(KeyCode.S))
        //      rb.AddForce(Vector3.down);

        //rb.velocity = Vector3.right;


        iv = new Vector3(Input.GetAxisRaw("Horizontal")*speed, 0, Input.GetAxisRaw("Vertical")*speed);


        var input = iv.normalized;
 
     Vector3 temp = Vector3.zero;
     if (input.z == 1)
     {
         temp += transform.forward;
     }
     else if (input.z == -1)
     {
         temp += transform.forward * -1;
     }
 
     if (input.x == 1)
     {
         temp += transform.right;
     }
     else if (input.x == -1)
     {
         temp += transform.right * -1;
     }

    Vector3 vel = temp * speed;
    vel.y = 0;
        rb.velocity = vel;

 
     }
}
