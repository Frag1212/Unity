using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour
{

    GameObject player;
    CharacterController cc = null;
    Vector3 Velocity = Vector3.zero;

    float GravityCoefficient = 5f;//Пусть ускорение падения зависит от гравитации так что если гравитация изменится то и оно изменится
    float MaxJumpSpeed = 20f;
    float MaxHorizontalSpeed = 15f;
    float Damage = 20f;

    void Start ()
    {
        cc = GetComponent<CharacterController>();
        player = GameObject.FindGameObjectWithTag("Player");
	}

    void Update()//Todo в фиксед упдейт проверка не лежит ли на месте и если лежит пусть подпрыгивает Пускай высоко прыгает если игрока не перескочит
    {//Todo Enemies constantly emit sound you hear it louder the closer you are to them, this enemy emits additional sound when its close haw haw haw haw like it want to bite you
        if (cc.isGrounded)
        {
            //VerticalVelocity = 10f;
        }
        else
        {
            Velocity.y += GravityCoefficient * Physics.gravity.y * Time.deltaTime;
        }
        cc.Move(Velocity * Time.deltaTime);
    }
    /*Vector3 target;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(transform.position, target);
        Gizmos.DrawSphere(target, 1);
    }*/

    /*float vpx, vpz, px, pz, sx, sz, v;
    float vx, vz, t;

        px+vpx*t=sx+vx*t
        pz+vpz*t=sz+vz*t
        vx*vx+vz*vz=v*v

        vx=(px+vpx*t-sx)/t

        float D = 4*(vpx*(px-sx)+vpz*(pz-sz))^2+4*(v^2-vpx^2-vpz^2)*((px-sx)^2+(pz-sz^2))
        
        Если D<0 идем на позицию игрока
        Если D=0 берем t если t<0 на позицию игрока
        Если D>0 берем t=(-b-sqrt(D))/2a если t<0 берем t=(-b+sqrt(D))/2a если t<0 на позицию игрока
        У нас есть t>0 находим точку по скорорсти игрока и идем туда
        */
    Vector2 Intercept(float startx, float starty, float speed, float targetx, float targety, float targetspeedx, float targetspeedy)
	{
	    float a = (speed * speed - targetspeedx * targetspeedx - targetspeedy * targetspeedy);
	    float b = 2 * ((targetspeedx * (startx - targetx)) + (targetspeedy * (starty - targety)));
	    float c = - (targetx - startx) * (targetx - startx) - (targety - starty) * (targety - starty);
	    float D = b * b - 4 * a * c;
	    //float D = 4*(targetspeedx*(targetx-startx)+targetspeedy*(targety-stsrty))^2+4*(speed^2-targetspeedx^2-targetspeedy^2)*((targetx-startx)^2+(targety-starty^2));
	    if(D < 0)
		    return new Vector2(targetx,targety);
	    float t;
	    if(D == 0)
		    t= - b / (2 * a);
	    else
	    {
		    float t1 = (- b - Mathf.Sqrt(D)) / (2 * a);
		    float t2 = (- b + Mathf.Sqrt(D)) / (2 * a);
		    if(t1 < 0 )
			    t = t2;
		    else
			    if(t2 < 0)
				    t = t1;
			    else
				    t = Mathf.Min(t1, t2);
	    }
	    if(t < 0)
		    return new Vector2(targetx,targety);
	    float x = targetx + targetspeedx * t;
	    float y = targety + targetspeedy * t;
	    return new Vector2(x, y);
	}

    //Как надо ускорятся (одинаково в течении всего пути) чтобы попасть куда надо?
    Vector2 Acceleration(float startx, float starty, float startspeedx, float startspeedy, float targetx, float targety)
    {
        return Vector2.zero;
    }
       
    void OnControllerColliderHit(ControllerColliderHit hit)//Если игрок близко рассчитываем высоту чтобы не перепрыгнуть
    {
        if (hit.gameObject.isStatic)
        {
            if (player != null)
            {
                //Vector3 target = FirstOrderIntercept(transform.position, Vector3.zero, 20f, player.transform.position, player.GetComponent<CharacterController>().velocity);
                Vector2 target2d = Intercept(transform.position.x, transform.position.z, MaxHorizontalSpeed, player.transform.position.x, player.transform.position.z, player.GetComponent<CharacterController>().velocity.x, player.GetComponent<CharacterController>().velocity.z);
                Vector3 target = new Vector3(target2d.x, 0f, target2d.y);
                Vector3 positionxz = new Vector3(transform.position.x, 0f, transform.position.z);
                Vector3 speed = target - positionxz;
                speed.Normalize();
                speed *= MaxHorizontalSpeed;
                float t = (target - positionxz).magnitude / MaxHorizontalSpeed;
                speed.y = Mathf.Min( -Physics.gravity.y * GravityCoefficient * t / 2, MaxJumpSpeed);
                Velocity = speed;
                //Debug.DrawLine(transform.position, target,Color.white,10f,false);
                /*Vector3 speed = target - transform.position;
                speed.y = 0f;
                speed.Normalize();
                speed *= MaxHorizontalSpeed;
                float t = Mathf.Sqrt((player.transform.position.x - transform.position.x) * (player.transform.position.x - transform.position.x) + (player.transform.position.z - transform.position.z) * (player.transform.position.z - transform.position.z)) / MaxHorizontalSpeed;
                speed.y = Mathf.Min((player.transform.position.y + 1 - transform.position.y - Physics.gravity.y * GravityCoefficient * t * t / 2) / t, MaxJumpSpeed);
                Velocity = speed;*/
            }
        }
        else
        {
            Velocity = hit.normal * 5;
            if (Velocity.y >= 0)
                Velocity.y += 20f;
            FirstPersonController fpc = hit.gameObject.GetComponent<FirstPersonController>();
            if (fpc != null)
            {
                fpc.TakeDamage(Damage);
            }
        }
        //rb.velocity += collision.contacts[0].normal * 5;
        //rb.velocity += collision.relativeVelocity;
        //print(collision.relativeVelocity);
        //print(rb.velocity);
        //rb.velocity = collision.contacts[0].normal * 10;
        //collision.relativeVelocity
        //Vector3 NormalVelocity = Vector3.Project(rb.velocity, collision.contacts[0].normal);
        //Vector3 ParallelVelocity = rb.velocity - NormalVelocity;
        //rb.velocity = rb.velocity - 2*NormalVelocity;
        //print(rb.velocity);
        //print(NormalVelocity);
        //print(ParallelVelocity);
        //rb.velocity = ParallelVelocity - NormalVelocity;
    }
    /*void OnCollisionExit(Collision collision)
    {
        CollisionEnterTime = float.MaxValue;
    }*/

    //void Update ()//Todo в фиксед упдейт проверка не лежит ли на месте и если лежит пусть подпрыгивает
    //{
        /*if(player != null)
        {
            rb.velocity = rb.velocity + Vector3.Normalize(player.transform.position - transform.position);
        }*/

        /*if (TimeUntilNextUpdate <= 0)
        {
            if (player != null)
            {
                //GetComponent<Rigidbody>().AddForce(Vector3.Normalize(player.transform.position - transform.position) * 1000);
                //GetComponent<Rigidbody>().AddForce(Vector3.Normalize(Vector3.Normalize(player.transform.position - transform.position) - Vector3.Normalize(GetComponent<Rigidbody>().velocity)),ForceMode.Impulse);
                //GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity + Vector3.Normalize(player.transform.position - transform.position - GetComponent<Rigidbody>().velocity);
            }
            //TimeUntilNextUpdate = 1f;
        }
        else
        {
            //TimeUntilNextUpdate -= Time.deltaTime;
        }*/
	//}
}
