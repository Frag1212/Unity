using UnityEngine;
using System.Collections;

public class SkeletonScript : MonoBehaviour
{
    Animator anim;
    CharacterController cc;
    float MaxGroundSpeed = 3f;
    float MaxMovingTime = 1f;
    float MovingTimeLeft = 0f;
    Vector3 speed = Vector3.zero;
    
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        MovingTimeLeft -= Time.deltaTime;
        if (cc.isGrounded)
        {
            speed.y = 0;
            if (MovingTimeLeft <= 0)
            {
                speed = Vector3.zero;
                if (Random.Range(0, 2) == 0)
                {
                    anim.SetBool("Stand", true);
                }
                else
                {
                    anim.SetBool("Stand", false);
                    speed.x = Random.Range(-MaxGroundSpeed, MaxGroundSpeed);
                    speed.z = Random.Range(-MaxGroundSpeed, MaxGroundSpeed);
                    transform.rotation = Quaternion.FromToRotation(Vector3.forward, speed);
                }
                anim.SetFloat("Speed", Mathf.Sqrt(speed.x * speed.x + speed.z * speed.z));
                MovingTimeLeft = Random.Range(0f, MaxMovingTime);
            }
        }
        else
        {
            speed.y += Physics.gravity.y * Time.deltaTime;
        }
        cc.Move(speed * Time.deltaTime);
        /*Animator anim;
        int jumpHash = Animator.StringToHash("Jump");
        int runStateHash = Animator.StringToHash("Base Layer.Run");


        void Start()
        {
            anim = GetComponent<Animator>();
        }


        void Update()
        {
            float move = Input.GetAxis("Vertical");
            anim.SetFloat("Speed", move);

            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (Input.GetKeyDown(KeyCode.Space) && stateInfo.nameHash == runStateHash)
            {
                anim.SetTrigger(jumpHash);
            }
        }*/
    }
}