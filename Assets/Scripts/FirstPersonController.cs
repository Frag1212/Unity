using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FirstPersonController : MonoBehaviour
{
    float verticalVelocity = 0.0f;
    CharacterController cc = null;
    AudioSource audiosource = null;
    AudioSource DamageAudioSource = null;
    AudioSource HurtAudioSource = null;
    AudioSource ShotPenaltyAudioSource = null;
    AudioSource StrongShotAudioSource = null;
    AudioSource EnemyDiesAudioSource = null;
    public LineRenderer LineRendererPrefab;
    public GameObject DebrisPrefab;
    public Text HealthText;
    public AudioClip DamageSound;
    public AudioClip HurtSound;
    public AudioClip ShotPenaltySound;
    public AudioClip StrongShotSound;
    public AudioClip EnemyDiesSound;
    Vector3 s = new Vector3(1, -1, 0);
    Vector3[] lrv = new Vector3[2];
    float LastShotTime = 0f;
    float CantShootTimeLeft = 0f;
    float LastShootInterval = 0.5f;
    float StrongShotCooldownTimeLeft = 0f;

    float Health = 100f;
    float WeakDamage = 20f;
    float NoPenaltyShotInterval = 0.25f;
    float CantShootTimePenalty = 1f;
    float StrongShotCooldown = 5f;
    float StrongShotDamage = 80f;

    AudioSource AddAudioSource(AudioClip Sound)
    {
        if (Sound == null)
            return null;
        AudioSource NewAudioSource = gameObject.AddComponent<AudioSource>();
        NewAudioSource.clip = Sound;
        NewAudioSource.loop = false;
        NewAudioSource.playOnAwake = false;
        NewAudioSource.volume = 1;
        return NewAudioSource;
    }

    // Use this for initialization
    void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cc = GetComponent<CharacterController>();
        audiosource = GetComponent<AudioSource>();
        LastShootInterval = NoPenaltyShotInterval;
        HealthText.text = Health.ToString();
        DamageAudioSource = AddAudioSource(DamageSound);
        HurtAudioSource = AddAudioSource(HurtSound);
        ShotPenaltyAudioSource = AddAudioSource(ShotPenaltySound);
        StrongShotAudioSource = AddAudioSource(StrongShotSound);
        EnemyDiesAudioSource = AddAudioSource(EnemyDiesSound);
    }
	
	// Update is called once per frame
	void Update ()
    {
        //HealthText.text = Health.ToString();
        if(Input.GetButton("Cancel") == true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        StrongShotCooldownTimeLeft -= Time.deltaTime;//Todo Sound for shot ready
        //Todo Отравляющий выстрел против сильных врагов, Стан, Танк если не убил пока он не разогнался  и небежал в бок достаточно долго то можно его застанить и уйти с дороги пока он застанен если заряд стана есть рядом с танком медик может бегать надо его первым снять
        if (Input.GetMouseButtonDown(1) && StrongShotCooldownTimeLeft <= 0f)//Todo Strong shot with cool special effect reloads if kills target 80dmg(first weak shot then this to have it reloaded)
        {//Todo Health bars for enemies
            StrongShotCooldownTimeLeft = StrongShotCooldown;//Todo Special effect for hitting floor
            lrv[0] = Camera.main.transform.position + transform.rotation * s;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit[] hitinfoarray;
            hitinfoarray = Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.GetMask("Default", "Character"));
            float MinDistance = -1f;
            if (hitinfoarray.Length > 0)
            {
                RaycastHit TargetHit = hitinfoarray[0];
                for (int i = 0; i < hitinfoarray.Length; i++)
                {
                    if (hitinfoarray[i].collider != cc && (hitinfoarray[i].distance < MinDistance || MinDistance < 0))
                    {
                        MinDistance = hitinfoarray[i].distance;
                        TargetHit = hitinfoarray[i];
                    }
                }
                if (MinDistance >= 0)
                {
                    lrv[1] = TargetHit.point;
                    Vector3 ricochetVector = -Camera.main.transform.forward;
                    Quaternion q = Quaternion.FromToRotation(ricochetVector, TargetHit.normal);
                    ricochetVector = q * q * ricochetVector;
                    if (DebrisPrefab != null)
                    {
                        GameObject debris = Instantiate(DebrisPrefab, TargetHit.point, Quaternion.identity) as GameObject;
                        DebrisScript ds = debris.GetComponent<DebrisScript>();
                        if (ds != null)
                        {
                            ds.StartForce = ricochetVector * 1000;
                            Renderer debrisRenderer = debris.GetComponent<Renderer>();
                            Renderer targetRenderer = TargetHit.collider.gameObject.GetComponent<Renderer>();
                            if (targetRenderer != null)
                                debrisRenderer.sharedMaterial = targetRenderer.sharedMaterial;
                        }
                        else
                        {
                            Destroy(debris);
                        }
                    }
                    DamageScript damagescript = TargetHit.collider.gameObject.GetComponent<DamageScript>();
                    if (damagescript != null)
                    {
                        if (damagescript.TakeDamage(StrongShotDamage, TargetHit.point, ricochetVector))
                        {
                            StrongShotCooldownTimeLeft = 0f;
                            EnemyDiesAudioSource.Play();
                        }
                        DamageAudioSource.Play();
                    }
                }
            }
            if (MinDistance < 0)
            {
                lrv[1] = Camera.main.transform.position + Camera.main.transform.forward * 2000f;
            }
            if (LineRendererPrefab != null)
            {
                LineRenderer lr = Instantiate(LineRendererPrefab);
                lr.material.mainTexture = Resources.Load("Textures/StrongShotTexture", typeof(Texture)) as Texture;
                //lr.SetWidth(4f, 4f);
                lr.material.SetTextureScale("_MainTex", new Vector2(10, 1));
                lr.SetColors(new Color(0, 255, 0), new Color(0, 255, 0));
                lr.SetPositions(lrv);
            }
            StrongShotAudioSource.Play();
            //print(LastShotTime);
            /*print("All " + Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)).Length);
            print("Textures " + Resources.FindObjectsOfTypeAll(typeof(Texture)).Length);
            print("AudioClips " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length);
            print("Meshes " + Resources.FindObjectsOfTypeAll(typeof(Mesh)).Length);
            print("Materials " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length);
            print("GameObjects " + Resources.FindObjectsOfTypeAll(typeof(GameObject)).Length);
            print("Components " + Resources.FindObjectsOfTypeAll(typeof(Component)).Length);*/
        }
        transform.Rotate(0, Input.GetAxis("Mouse X") * 5.0f, 0);
        Camera.main.transform.Rotate(Input.GetAxis("Mouse Y") * -5.0f, 0, 0);
        Vector3 speed = new Vector3(Input.GetAxis("Horizontal") * 10, 0, Input.GetAxis("Vertical") * 10);//Сделать ускорение дольше держишь быстрее бежишь?
        speed = transform.rotation * speed;
        if (cc.isGrounded)
        {
            if (Input.GetButton("Jump") == true)
            {
                verticalVelocity = 7.0f;
            }
            else
                verticalVelocity = 0.0f;
        }
        speed.y = verticalVelocity;
        cc.Move(speed * Time.deltaTime);
        if (!cc.isGrounded)
            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        CantShootTimeLeft -= Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && (CantShootTimeLeft <= 0))
        {
            lrv[0] = Camera.main.transform.position + transform.rotation * s;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit[] hitinfoarray;
            hitinfoarray = Physics.RaycastAll(ray, Mathf.Infinity, LayerMask.GetMask("Default", "Character"));
            float MinDistance = -1f;
            if (hitinfoarray.Length > 0)
            {
                RaycastHit TargetHit = hitinfoarray[0];
                for (int i = 0; i < hitinfoarray.Length; i++)
                {
                    if (hitinfoarray[i].collider != cc && (hitinfoarray[i].distance < MinDistance || MinDistance < 0))
                    {
                        MinDistance = hitinfoarray[i].distance;
                        TargetHit = hitinfoarray[i];
                    }
                }
                if (MinDistance >= 0)
                {
                    lrv[1] = TargetHit.point;
                    Vector3 ricochetVector = -Camera.main.transform.forward;
                    Quaternion q = Quaternion.FromToRotation(ricochetVector, TargetHit.normal);
                    ricochetVector = q * q * ricochetVector;
                    if (DebrisPrefab != null)
                    {
                        GameObject debris = Instantiate(DebrisPrefab, TargetHit.point, Quaternion.identity) as GameObject;
                        DebrisScript ds = debris.GetComponent<DebrisScript>();
                        if (ds != null)
                        {
                            ds.StartForce = ricochetVector * 1000;
                            Renderer debrisRenderer = debris.GetComponent<Renderer>();
                            Renderer targetRenderer = TargetHit.collider.gameObject.GetComponent<Renderer>();
                            if(targetRenderer != null)
                                debrisRenderer.sharedMaterial = targetRenderer.sharedMaterial;
                            //ds.customMaterial = true;
                        }
                        else
                        {
                            Destroy(debris);
                        }
                    }
                    DamageScript damagescript = TargetHit.collider.gameObject.GetComponent<DamageScript>();
                    if (damagescript != null)
                    {
                        if(damagescript.TakeDamage(WeakDamage, TargetHit.point, ricochetVector))
                            EnemyDiesAudioSource.Play();
                        DamageAudioSource.Play();
                    }
                    /*EnemyWalkingScript ewc = TargetHit.collider.gameObject.GetComponent<EnemyWalkingScript>();
                    if(ewc != null)
                    {
                        ewc.TakeDamage(StrongDamage, TargetHit.point, ricochetVector);
                    }*/
                }
            }
            if (MinDistance < 0)
            {
                lrv[1] = Camera.main.transform.position + Camera.main.transform.forward * 2000f;
                //Debug.Log("Nothing");
            }
            if (LineRendererPrefab != null)//Todo make not red not yellow color different from enemies
            {
                LineRenderer lr = Instantiate(LineRendererPrefab);
                lr.material.SetTextureScale("_MainTex", new Vector2(10, 1));//(lrv[1] - lrv[0]).magnitude / 100
                /*float t = ShotChargeTime - Time.time + LastShotTime;
                if (t < 0)
                    t = 0;
                if (t > ShotChargeTime)
                    t = ShotChargeTime;
                lr.SetColors(new Color(255f*t/ShotChargeTime, 255, 255f * t / ShotChargeTime), new Color(255f * t / ShotChargeTime, 255, 255f * t / ShotChargeTime));*/
                /*if (Time.time - LastShotTime < ShotChargeTime)
                    lr.SetColors(new Color(125, 125, 125), new Color(125, 125, 125));
                else*/
                lr.SetColors(new Color(0, 255, 0), new Color(0, 255, 0));
                lr.SetPositions(lrv);
            }
            audiosource.Play();
            float CurrentShotInterval = Time.time - LastShotTime;
            if ((NoPenaltyShotInterval > CurrentShotInterval) && (CurrentShotInterval > LastShootInterval))//Todo На оружии или интерфейсе вокруг прицела показывается интервал стрельбы в который надо угодить?
            {
                CantShootTimeLeft = CantShootTimePenalty;
                ShotPenaltyAudioSource.Play();
            }
            LastShootInterval = CurrentShotInterval;
            LastShotTime = Time.time;
        }
    }
    public void TakeDamage(float damage)//Todo special effect
    {
        if(HurtAudioSource != null && HurtAudioSource.enabled)
            HurtAudioSource.Play();
        Health -= damage;
        HealthText.text = Health.ToString();
        if (Health <= 0)
            Die();
    }
    void Die()
    {
        MainCameraScript mcs = null;
        if (Camera.main != null)
            mcs = Camera.main.GetComponent<MainCameraScript>();
        if (mcs)
            mcs.OnPlayerDeath();
        //if(gameObject != null)
            Destroy(gameObject);
    }
    void OnDestroy()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}