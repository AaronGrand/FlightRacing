using System.Collections;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;
using System.Collections.Generic;

public class FlightController3D : NetworkBehaviour
{
    #region Variables

    [SerializeField] private Rigidbody rb;


    private bool crashed = false;
    private bool firstCrash = true;

    private Menu menu;

    public Renderer[] render;

    [SerializeField] private GameObject[] models;
    private bool firstModelChange = true;

    [Header("NetworkVariables")]
    [SerializeField] public NetworkVariable<FixedString32Bytes> userName = new NetworkVariable<FixedString32Bytes>("",NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] public NetworkVariable<Color> networkedColor = new NetworkVariable<Color>(new Color(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] public NetworkVariable<int> modelIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI userNameText;


    [Header("Movement Variables")]
    [SerializeField] private float maxThrust = 500f;
    [SerializeField] private int throttleSpeedInc;

    private float currentYawSpeed;
    private float currentPitchSpeed;
    private float currentRollSpeed;

    private float angleOfAttackPitch;
    private float angleOfAttackYaw;

    private int throttleInput;
    private float throttle;


    [Header("Rotating speeds")]
    [Range(5f, 500f)]
    [SerializeField] private float yawSpeed = 50f;

    [Range(5f, 500f)]
    [SerializeField] private float pitchSpeed = 100f;

    [Range(5f, 500f)]
    [SerializeField] private float rollSpeed = 200f;

    [Header("Colliders")]
    [SerializeField] private Transform collidersRoot;

    [Header("Physics")]
    private Vector3 localVelocity;
    private Vector3 localAngularVelocity;
    
    [SerializeField] private AnimationCurve dragForward;
    [SerializeField] private AnimationCurve dragBack;
    [SerializeField] private AnimationCurve dragLeft;
    [SerializeField] private AnimationCurve dragRight;
    [SerializeField] private AnimationCurve dragTop;
    [SerializeField] private AnimationCurve dragBottom;

    [SerializeField] private float inducedDrag;
    [SerializeField] private AnimationCurve inducedDragCurve;

    [SerializeField] private float liftPower;
    [SerializeField] private AnimationCurve liftAngleOfAttackCurve;

    [SerializeField] float rudderPower;
    [SerializeField] AnimationCurve rudderAOACurve;
    [SerializeField] AnimationCurve rudderInducedDragCurve;

    [SerializeField] Vector3 angularDrag;

    [Header("Animations")]
    [SerializeField] private GameObject[] Prop;
    [SerializeField] private float rotationSpeed;



    #endregion

    #region Unity functions

    public override void OnNetworkSpawn()
    {
        StartCoroutine(SetSpawnLocation(0.5f));
    }

    private void Awake()
    {
        render = gameObject.GetComponentsInChildren<Renderer>(true);
    }


    private void Start()
    {
        crashed = false;
        firstCrash = true;

        GameState.Instance.localGameFinished = false;
        if (IsHost)
        {
            userName.Value = GameState.Instance.userName;
        }
        if (IsLocalPlayer && IsClient && !IsHost)
        {
            SetNameServerRpc(GameState.Instance.userName);
        }
        UpdateName();

        if (IsLocalPlayer)
        {
            foreach(Transform transform in collidersRoot.GetComponentInChildren<Transform>())
            {
                transform.GetComponent<Collider>().isTrigger = false;
            }
        }

        UpdateColor();
        UpdateModel();
    }

    private bool spawned = false;

    private void Update()
    {

        if(GameState.Instance.GetState() == STATE.NOT_STARTED)
        {
            Prop[modelIndex.Value].transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotationSpeed));
            if (!spawned)
            {
                spawned = true;
                StartCoroutine(SetSpawnLocation(1f));
            }
        }

        if (GameState.Instance.GetState() == STATE.INGAME && !GameState.Instance.localGameFinished)
        {
            //INGAME
            Prop[modelIndex.Value].transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotationSpeed));

            if (IsOwner)
            {
                //ALL LOCAL CLIENT THINGS HERE

                Movement();
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }

        if(GameState.Instance.localGameFinished)
        {
            rb.useGravity = false;
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner && GameState.Instance.GetState() == STATE.INGAME && !GameState.Instance.localGameFinished)
        {
            float dt = Time.deltaTime;

            CalculateAngleOfAttack();

            UpdateThrottle(dt);

            UpdateDrag();
            UpdateAngularDrag();

            UpdateThrust();

            UpdateLift();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Terrain"))
        {
            crashed = true;
            EndGame();
        }
    }

    #endregion

    #region Private methods

    private void Movement()
    {
        //Rotate airplane by inputs
        transform.Rotate(Vector3.forward * -Input.GetAxis("Horizontal") * currentRollSpeed * Time.deltaTime);
        transform.Rotate(Vector3.right * Input.GetAxis("Vertical") * currentPitchSpeed * Time.deltaTime);

        //Rotate yaw
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(Vector3.up * currentYawSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(-Vector3.up * currentYawSpeed * Time.deltaTime);
        }

        //Accelerate and deacclerate
        if (Input.GetKey(KeyCode.Space))
        {
            throttleInput = 1;
        }
        else
        {
            throttleInput = 1;
        }

        currentYawSpeed = yawSpeed;
        currentPitchSpeed = pitchSpeed;
        currentRollSpeed = rollSpeed;
    }

    private void UpdateThrust()
    {
        rb.AddRelativeForce(throttle * maxThrust * Vector3.forward);
    }

    private void UpdateDrag()
    {
        var lv = localVelocity;
        //velocity squared
        var lv2 = lv.sqrMagnitude;

        //calculate coeffictient of drag depending on direction on velcity
        var coefficient = Scale6(
            lv.normalized,
            dragRight.Evaluate(Mathf.Abs(lv.x)), dragLeft.Evaluate(Mathf.Abs(lv.x)),
            dragTop.Evaluate(Mathf.Abs(lv.y)), dragBottom.Evaluate(Mathf.Abs(lv.y)),
            dragForward.Evaluate(Mathf.Abs(lv.z)), dragBack.Evaluate(Mathf.Abs(lv.z)));

        //drag is the opposite direction of velocity
        var drag = coefficient.magnitude * lv2 * -lv.normalized;

        rb.AddRelativeForce(drag);
    }

    void UpdateAngularDrag()
    {
        var av = localAngularVelocity;
        var drag = av.sqrMagnitude * -av.normalized;    //squared, opposite direction of angular velocity
        rb.AddRelativeTorque(Vector3.Scale(drag, angularDrag), ForceMode.Acceleration);  //ignore rigidbody mass
    }

    void UpdateThrottle(float dt)
    {
        float target = 0;
        if (throttleInput > 0) target = 1;

        //throttle input is [-1, 1]
        //throttle is [0, 1]
        throttle = MoveTo(throttle, target, throttleSpeedInc * Mathf.Abs(throttleInput), dt);
    }

    private void CalculateAngleOfAttack()
    {
        if (localVelocity.sqrMagnitude < 0.1f)
        {
            angleOfAttackPitch = 0;
            angleOfAttackYaw = 0;
            return;
        }

        angleOfAttackPitch = Mathf.Atan2(-localVelocity.y, localVelocity.z);
        angleOfAttackYaw = Mathf.Atan2(localVelocity.x, localVelocity.z);
    }

    private void UpdateLift()
    {
        if (localVelocity.sqrMagnitude < 1f) return;

        var liftForce = CalculateLift(
            angleOfAttackPitch, Vector3.right,
            liftPower,
            liftAngleOfAttackCurve,
            inducedDragCurve
            );

        var yawForce = CalculateLift(angleOfAttackYaw, Vector3.up, rudderPower, rudderAOACurve, rudderInducedDragCurve);

        rb.AddRelativeForce(liftForce);
        rb.AddRelativeForce(yawForce);
    }

    private Vector3 CalculateLift(float angleOfAttack, Vector3 rightAxis, float liftPower, AnimationCurve aoaCurve, AnimationCurve inducedDragCurve)
    {
        //projects velocity onto YZ plane
        var liftVelocity = Vector3.ProjectOnPlane(Vector3.zero, rightAxis);
        //square of velocity
        var v2 = liftVelocity.sqrMagnitude;

        //lift = velocity^2 * coefficient * liftPower
        //coefficient varies with AOA
        var liftCoefficient = aoaCurve.Evaluate(angleOfAttack * Mathf.Rad2Deg);
        var liftForce = v2 * liftCoefficient * liftPower;

        //lift is perpendicular to velocity
        var liftDirection = Vector3.Cross(liftVelocity.normalized, rightAxis);
        var lift = liftDirection * liftForce;

        //induced drag varies with square of lift coefficient
        var dragForce = liftCoefficient * liftCoefficient;
        var dragDirection = -liftVelocity.normalized;
        var inducedDrag = dragDirection * v2 * dragForce * this.inducedDrag * inducedDragCurve.Evaluate(Mathf.Max(0, localVelocity.z));

        return lift + inducedDrag;
    }

    #endregion

    #region Public methods

    public void EndGame()
    {
        rb.useGravity = false;
        GameState.Instance.localGameFinished = true;


        if (firstCrash && crashed)
        {
            menu.SetPlayer(0.0f, this.userName.Value);
            firstCrash = false;

            rb.isKinematic = true;
        }
    }

    public IEnumerator SetSpawnLocation(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        GameObject pos = null;

        if (GameState.Instance.GetState() == STATE.NOT_STARTED)
        {
            pos = GameObject.FindGameObjectWithTag("StartPosition");
        }
        else if (GameState.Instance.GetState() == STATE.LOBBY)
        {
            Debug.Log((gameObject.GetComponent<NetworkObject>().OwnerClientId + 1).ToString());
            pos = GameObject.Find("SpawnPlayer" + (gameObject.GetComponent<NetworkObject>().OwnerClientId + 1).ToString());
        }

        if (pos == null)
        {
            if (IsLocalPlayer)
            {
                NetworkManager.Singleton.Shutdown();
                Application.Quit();
                yield break;
            }
            Destroy(gameObject);
        }

        this.gameObject.transform.position = pos.transform.position;
        this.gameObject.transform.rotation = pos.transform.rotation;

        if (GameState.Instance.GetState() == STATE.NOT_STARTED && IsLocalPlayer)
        {
            menu = GameObject.FindGameObjectWithTag("Menu").GetComponent<Menu>();
        }
    }

    #endregion

    #region Helper methods

    public float MoveTo(float value, float target, float speed, float deltaTime, float min = 0, float max = 1)
    {
        float diff = target - value;
        float delta = Mathf.Clamp(diff, -speed * deltaTime, speed * deltaTime);
        return Mathf.Clamp(value + delta, min, max);
    }

    public static Vector3 Scale6(
    Vector3 value,
    float posX, float negX,
    float posY, float negY,
    float posZ, float negZ
    )
    {
        Vector3 result = value;

        if (result.x > 0)
        {
            result.x *= posX;
        }
        else if (result.x < 0)
        {
            result.x *= negX;
        }

        if (result.y > 0)
        {
            result.y *= posY;
        }
        else if (result.y < 0)
        {
            result.y *= negY;
        }

        if (result.z > 0)
        {
            result.z *= posZ;
        }
        else if (result.z < 0)
        {
            result.z *= negZ;
        }

        return result;
    }

    //SET CLIENT NAME ON HOST
    [ServerRpc(RequireOwnership = false)]
    private void SetNameServerRpc(FixedString32Bytes _name)
    {
        if (!IsLocalPlayer)
        {
            userName.Value = _name;
        }
        SetNameClientRpc(userName.Value);
        UpdateName();
    }
    
    //SET NAMES ON CLIENT
    [ClientRpc]
    private void SetNameClientRpc(FixedString32Bytes name)
    {
        userNameText.text = name.ToString();
    }

    private void UpdateName()
    {
        userNameText.text = userName.Value.ToString();
    }

    
    public void SetColor(Color newColor)
    {
        if (IsHost)
        {
            networkedColor.Value = newColor;
            SetColorClientRpc(newColor);
        }
        if (IsLocalPlayer && IsClient && !IsHost)
        {
            SetColorServerRpc(newColor);
        }
        UpdateColor();
    }

    //SET COLOR ON HOST
    [ServerRpc(RequireOwnership = false)]
    private void SetColorServerRpc(Color newColor)
    {
        if (!IsLocalPlayer)
        {
            networkedColor.Value = newColor;
        }
        SetColorClientRpc(networkedColor.Value);
        UpdateColor();
    }

    //SET COLORS ON CLIENT
    [ClientRpc]
    private void SetColorClientRpc(Color newColor)
    {
        UpdateColor();
    }
    
    private void UpdateColor()
    {
        foreach (Renderer render in render)
        {
            Material[] materials = render.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                Color currentColor = materials[i].color;
                Color.RGBToHSV(currentColor, out float hue, out float saturation, out float brightness);

                Color.RGBToHSV(networkedColor.Value, out float newHue, out float newSaturation, out float _);
                Color newColor = Color.HSVToRGB(newHue, newSaturation, brightness);
                newColor.a = currentColor.a;

                materials[i].color = newColor;
            }
        }
    }

    public void NextModel()
    {
        if (IsHost)
        {
            modelIndex.Value = (modelIndex.Value + 1) % models.Length;
            NextModelClientRpc();
        }
        if (IsLocalPlayer && IsClient && !IsHost)
        {
            NextModelServerRpc();
        }
    }

    //SET MODEL ON HOST
    [ServerRpc(RequireOwnership = false)]
    private void NextModelServerRpc()
    {
        if (!IsLocalPlayer)
        {
            modelIndex.Value = (modelIndex.Value + 1) % models.Length;
        }
        NextModelClientRpc();
        UpdateModel();
    }

    //SET MODELS ON CLIENT
    [ClientRpc]
    private void NextModelClientRpc()
    {
        UpdateModel();
    }

    private void UpdateModel()
    {
        for (int i = 0; i < models.Length; i++)
        {
            models[i].SetActive(false);
        }

        if(IsClient && IsHost)
        {
            models[modelIndex.Value % models.Length].SetActive(true);
        }
        else
        {
            if (firstModelChange)
            {
                models[modelIndex.Value].SetActive(true);
            } else
            {
                if (modelIndex.Value == 0)
                {
                    models[models.Length - 1].SetActive(true);
                }
                else
                {
                    models[modelIndex.Value - 1 % models.Length].SetActive(true);
                }
            }
        }

        firstModelChange = false;

        UpdateColor();

        //models[modelIndex.Value].SetActive(true);
    }

    #endregion
}
