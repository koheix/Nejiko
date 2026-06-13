using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NejikoControllers : MonoBehaviour
{
    const int MinLane = -2;
    const int MaxLane = 2;
    const float LaneWidth = 1.0f;
    const int DefaultLife = 3;
    const float StunDuration = 0.5f;

    CharacterController controller;
    Animator animator;

    Vector3 moveDirection = Vector3.zero;
    int targetLane;
    int life = DefaultLife;
    float recoverTime = 0.0f;

    public float gravity;
    public float speedZ;
    public float speedX;
    public float speedJump;
    public float accelerationZ;

    public int GetLife(){
        return life;
    }

    bool IsStan(){
        return recoverTime > 0.0f || life <= 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // if(controller.isGrounded)
        // {
        //     if(Input.GetAxis("Vertical") > 0.0f){
        //         moveDirection.z = Input.GetAxis("Vertical") * speedZ;
        //     }
        //     else{
        //         moveDirection.z = 0;
        //     }
        //     transform.Rotate(0, Input.GetAxis("Horizontal") * 3, 0);
        //     if(Input.GetButton("Jump"))
        //     {
        //         moveDirection.y = speedJump;
        //         animator.SetTrigger("jump");
        //     }
        // }
        // for debug
        if(Input.GetKeyDown("left")) MoveToLeft();
        if(Input.GetKeyDown("right")) MoveToRight();
        if(Input.GetKeyDown("space")) Jump();

        if(IsStan()){
            //stop moving if stunned and countdown recover time
            moveDirection.x = 0.0f;
            moveDirection.z = 0.0f;
            recoverTime -= Time.deltaTime;
        }
        else{
            //accelerate gradually and move forward forever in z axis
            float acceleratedZ = moveDirection.z + (accelerationZ * Time.deltaTime);
            moveDirection.z = Mathf.Clamp(acceleratedZ, 0, speedZ);

            //calculate x 
            float ratioX = (targetLane * LaneWidth - transform.position.x) / LaneWidth;
            moveDirection.x = ratioX * speedX;
        }

        moveDirection.y -= gravity * Time.deltaTime;

        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        controller.Move(globalDirection * Time.deltaTime);

        if(controller.isGrounded) moveDirection.y = 0;

        animator.SetBool("run", moveDirection.z > 0.0f);
    }

    public void MoveToLeft(){
        if(IsStan()) return;
        if(controller.isGrounded && targetLane > MinLane) targetLane--;
    }

    public void MoveToRight(){
        if(IsStan()) return;
        if(controller.isGrounded && targetLane < MaxLane) targetLane++;
    }

    public void Jump(){
        if(IsStan()) return;
        if(controller.isGrounded){
            moveDirection.y= speedJump;

            animator.SetTrigger("jump");
        }
    }

    //charactercontroller collisions with enemy
    void OnControllerColliderHit(ControllerColliderHit hit){
        if(IsStan()) return;

        if(hit.gameObject.tag == "Robo"){
            life--;
            recoverTime = StunDuration;
            animator.SetTrigger("damage");
            Destroy(hit.gameObject);
        }
    } 
}
