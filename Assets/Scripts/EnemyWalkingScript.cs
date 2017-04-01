using UnityEngine;//Todo make one on one enemy hard стреляет только на упреждение?
using System.Collections;

public class EnemyWalkingScript : MonoBehaviour
{//Todo Enemy who shoots projectiles and never stops (no charge)
    public LineRenderer LineRendererPrefab;
    //public GameObject ParticleSystemDebrisPrefab;

    //float MaxHealth = 100f;
    //float Health = 100f;
    float MaxCooldownTime = 1.0f;
    float ChargingTime = 0.5f;
    float ShotTimeLeft = 1.0f;
    float ShootDirectlyAtTargetChance = 50f;
    float MaxGroundSpeed = 10f;
    float MaxMovingTime = 1f;
    float MovingTimeLeft = 0f;
    float ShotSpreadHorizontallyAngle = 1f;
    float ShotSpreadVerticallyAngle = 1f;

    float verticalVelocity = 0.0f;
    float xselfmovingVelocity = 0.0f;
    float zselfmovingVelocity = 0.0f;
    CharacterController cc = null;
    bool ShotCharging = false;
    Vector3 lrdirection;

    void Start ()
    {
        //Health = MaxHealth;
        ShotTimeLeft = Random.Range(0f, MaxCooldownTime);
        cc = GetComponent<CharacterController>();
    }
	
    /*public bool TakeDamage(float damage, Vector3 Point, Vector3 ricochetVector)//true if dies
    {//Todo special effect for dying
        if (ParticleSystemDebrisPrefab != null)
        {
            GameObject pso = Instantiate(ParticleSystemDebrisPrefab, Point, Quaternion.FromToRotation(new Vector3(0, 0, 1), ricochetVector)) as GameObject;
            ParticleSystem ps = pso.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.startSize = 2*damage/MaxHealth;
                if (ps.startSize > 2f)
                    ps.startSize = 2f;
            }
        }
        Health -= damage;
        if (Health <= 0)
            Destroy(gameObject);
        else
        {
            MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
            if(mr != null)//Todo more visible color change, jumping enemies are killed by one strong hit and two weak ones Debug.Log("lalala");
            {
                float t = (Health / MaxHealth);
                mr.material.color = new Color(t, t, t);
            }
        }
        return Health > 0;
    }*/

	void Update ()//Todo Добавить возможность прерывать заряжение выстрела если игрок на него смотрит. Прямо на него если один или в его сторону если несколько.
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (LineRendererPrefab != null)
        {
            if(ShotCharging)
            {
                ShotTimeLeft -= Time.deltaTime;
                if (ShotTimeLeft <= 0)//StartDamaging
                {
                    LineRenderer lr = Instantiate(LineRendererPrefab);
                    lr.transform.parent = gameObject.transform;
                    //lr.material = new Material(Shader.Find("Particles/Additive"));
                    lr.SetColors(new Color(255, 0, 0), new Color(255, 0, 0));
                    LineRendererScript lrs = lr.GetComponent<LineRendererScript>();
                    if (lrs != null)
                    {
                        lrs.SetLifeTime(0.1f);
                        Ray ray = new Ray(transform.position + new Vector3(0, 1, 0), lrdirection);
                        RaycastHit[] hitinfoarray;
                        hitinfoarray = Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.GetMask("Default", "Character"));
                        if (hitinfoarray.Length > 0)
                        {
                            for (int i = 0; i < hitinfoarray.Length; i++)
                            {
                                if (hitinfoarray[i].collider.gameObject.tag == "Player")
                                {
                                    FirstPersonController fpc = hitinfoarray[i].collider.gameObject.GetComponent<FirstPersonController>();
                                    if (fpc != null)
                                    {
                                        fpc.TakeDamage(20);
                                    }
                                }
                            }
                        }
                        lr.SetPosition(0, transform.position + new Vector3(0, 1, 0));
                        lr.SetPosition(1, transform.position + new Vector3(0, 1, 0) + lrdirection * 2000);
                        GetComponent<AudioSource>().Play();
                    }
                    ShotCharging = false;
                    ShotTimeLeft = Random.Range(0f, MaxCooldownTime);
                }
            }
            else
            {
                ShotTimeLeft -= Time.deltaTime;
                if(ShotTimeLeft <= 0 && cc.isGrounded && player != null)//StartCharging
                {
                    LineRenderer lr = Instantiate(LineRendererPrefab);
                    lr.transform.parent = gameObject.transform;
                    //lr.material = new Material(Shader.Find("Particles/Additive"));
                    lr.SetColors(new Color(255, 255, 0), new Color(255, 255, 0));
                    LineRendererScript lrs = lr.GetComponent<LineRendererScript>();
                    if (lrs != null)
                        lrs.SetLifeTime(ChargingTime);
                    Vector3 targetposition;
                    if (Random.Range(0f, 100f) <= ShootDirectlyAtTargetChance)//Todo Add also option to shoot in one of possible player move positions (just some place around player where he can walk or jump in charge time) including places in midair or where player can get by moving with not maximum speed
                        targetposition = player.transform.position;//Todo Если врагов много они не стреляют все в одно место а рассредотачивают выстрелы по упреждению, позиции игрока, рандомные позиции вокруг них, рандомные позиции куда игрок может добраться за время зарядки выстрела
                    else
                    {
                        targetposition = player.transform.position + player.GetComponent<CharacterController>().velocity * ChargingTime;
                        targetposition.y = player.transform.position.y + player.GetComponent<CharacterController>().velocity.y * ChargingTime + Physics.gravity.y * ChargingTime * ChargingTime / 2;
                        if (targetposition.y < 0)
                            targetposition.y = 0;
                    }
                    lrdirection = targetposition - transform.position;
                    Quaternion RandomRotation = Quaternion.AngleAxis(Random.Range(-ShotSpreadHorizontallyAngle, ShotSpreadHorizontallyAngle), new Vector3(0, 1, 0));
                    lrdirection = RandomRotation * lrdirection;
                    if (!player.GetComponent<CharacterController>().isGrounded)
                    {
                        RandomRotation = Quaternion.AngleAxis(Random.Range(-ShotSpreadVerticallyAngle, ShotSpreadVerticallyAngle), new Vector3(targetposition.z, 0, -targetposition.x));
                        lrdirection = RandomRotation * lrdirection;
                    }
                    lrdirection.Normalize();
                    lr.SetPosition(0, transform.position + new Vector3(0, 1, 0));
                    lr.SetPosition(1, transform.position + new Vector3(0, 1, 0) + lrdirection * 2000);
                    ShotCharging = true;
                    ShotTimeLeft = ChargingTime;
                }
            }
        }
        /*if (cc.isGrounded)
        {
            if (Random.Range(0, 10) > 8)
                verticalVelocity = 7.0f;
            else
                verticalVelocity = 0.0f;
        }*/
        MovingTimeLeft -= Time.deltaTime;
        if (cc != null)
        {
            Vector3 speed = Vector3.zero;
            if (cc.isGrounded)//Todo add parenting line renderer
            {
                verticalVelocity = 0.0f;
                if(ShotCharging)
                {
                    xselfmovingVelocity = 0;
                    zselfmovingVelocity = 0;
                    MovingTimeLeft = 0;
                }
                else
                    if (MovingTimeLeft <= 0)
                    {
                        xselfmovingVelocity = Random.Range(-MaxGroundSpeed, MaxGroundSpeed);
                        zselfmovingVelocity = Random.Range(-MaxGroundSpeed, MaxGroundSpeed);
                        if (Random.Range(0, 10) > 8)
                            verticalVelocity = 5.0f;
                        else
                            verticalVelocity = 0.0f;
                        MovingTimeLeft = Random.Range(0f, MaxMovingTime);
                    }
            }
            else
            {
                verticalVelocity += Physics.gravity.y * Time.deltaTime;
            }
            speed.x = xselfmovingVelocity;
            speed.z = zselfmovingVelocity;
            speed.y = verticalVelocity;
            cc.Move(speed * Time.deltaTime);
        }
        /*Vector3 speed;
        if (ShotCharging == false && cc != null)
        {
            speed = new Vector3(Random.Range(-MaxGroundSpeed, MaxGroundSpeed), 0, Random.Range(-MaxGroundSpeed, MaxGroundSpeed));
        }*/
        /*Vector3 speed = new Vector3(Random.Range(-10,10), 0, Random.Range(-10, 10));
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            speed = player.transform.position - transform.position;
            if (speed.magnitude < 2.1f)
                Destroy(player);
            speed = Vector3.Normalize(speed) * 10;
        }
        speed.y = verticalVelocity;
        cc.Move(speed * Time.deltaTime);*/
        /*speed = new Vector3(0, verticalVelocity, 0);
        cc.Move(speed * Time.deltaTime);
        if (!cc.isGrounded)
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        else
            verticalVelocity = 0.0f;*/
    }
}
