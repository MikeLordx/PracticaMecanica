using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Añade movimiento de la camara y sensibilidad
    public Transform camTrans;
    public CharacterController controller;

    //Gravedad
    [Header("Gravedad y salto")]
    public float stgf = 10;
    public float gravedad = 9.81f;
    public float fuerzaSalto = 10;
    private float velocidadVertical;

    //GroundCheck
    [Header("Ground Check")]
    public Transform gCheck;
    public LayerMask gLayer;
    public float radioGCheck;
    public bool piso;
    
    //Configuraciones para el jugador
    public float camSen;
    public float velMov;
    public float mIdz; // Zona muerta
    
    //Controles de la camara
    private Vector2 lookI;
    private float pitchCam;

    //Deteccion del touch
    int izqDid, derDid;

    //Division de la pantalla
    float pantalla;
    
    //Movimiento del jugador
    private Vector2 movTpuntoInicio;
    private Vector2 entradaMovimeinto;
    
    void Start()
    {
        //Desactiva trackeo de los dedos, aun en 0 detecta dedos por lo que se hace negativo
        izqDid = -1;
        derDid = -1;
        //Calcula el tama�o de la pantalla y la divide en 2
        pantalla = Screen.width / 2;
        //Calcula zona muerte
        mIdz = Mathf.Pow(Screen.height / mIdz, 2);
    }

    private void FixedUpdate()
    {
        piso = Physics.CheckSphere(gCheck.position, radioGCheck, gLayer);
    }

    private void Update()
    {
        Gravedad();
        EntradasTouch();

        if (derDid != -1)
        {
            Debug.Log("RotatoPotato");
            RotacionCamara();
        }

        if (izqDid != -1)
        {
            Debug.Log("movimiento");
            Movimiento();
        } 

        if (derDid != -1 && izqDid >= 1)
        {
            Salto();
        }
    }

    void EntradasTouch()
    {
        //Hacer el conteo de los dedos en la pantalla
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            //Evalua fases del touch
            switch (t.phase)
            {
                case TouchPhase.Began:
                    if (t.position.x < pantalla && izqDid == -1)
                    {
                        //Checamos el trackeo izquierdo
                        izqDid = t.fingerId;
                        Debug.Log("trackeo izquierdo");
                        //Establecer posicion inicial del movimiento
                        movTpuntoInicio = t.position;
                    }

                    if (t.position.x > pantalla && derDid == -1)
                    {
                        //Checamos el trackeo derecho
                        derDid = t.fingerId;
                        Debug.Log("trackeo derecho");
                    }

                    break;

                case TouchPhase.Ended:

                case TouchPhase.Canceled:
                    if (t.fingerId == izqDid)
                    {
                        //Detecta que paramos el trackeo del dedo izquierdo
                        izqDid = -1;
                        Debug.Log("Acabas de quitar el dedo izquierdo");
                    }

                    if (t.fingerId == derDid)
                    {
                        //Detecta que paramos el trackeo del dedo derecho
                        derDid = -1;
                        Debug.Log("Acabas de quitar el dedo derecho");
                    }

                    break;

                case TouchPhase.Moved:
                    //Entrada para mover la camara
                    if (t.fingerId == derDid)
                    {
                        lookI = t.deltaPosition * camSen * Time.deltaTime;
                    }
                    else if (t.fingerId == izqDid)
                    {
                        //Calcula la posicion delta de la posicion inicial para el movimiento
                        entradaMovimeinto = t.position - movTpuntoInicio;
                    }
                    break;

                case TouchPhase.Stationary:
                    //Trackea la permanencia de los dedos
                    if (t.fingerId == derDid)
                    {
                        lookI = Vector2.zero;
                    }
                    break;
            }
        }
    }

    private void RotacionCamara()
    {
        //Rotacion vertical (pitch)
        pitchCam = Mathf.Clamp(pitchCam - lookI.y, -90f, 90f);
        camTrans.localRotation = Quaternion.Euler(pitchCam, 0, 0);
        transform.Rotate(transform.up,lookI.x);
    }

    private void Movimiento()
    {
        if (entradaMovimeinto.sqrMagnitude <= mIdz)
        {
            return;
        }

        Vector2 dirMov = entradaMovimeinto.normalized * velMov * Time.deltaTime;
        controller.Move(transform.right * dirMov.x + transform.forward * dirMov.y);
    }

    void Gravedad()
    {
        //Calculo movimiento en vertical
        if (piso && velocidadVertical <= 0)
        {
            velocidadVertical = -stgf * Time.deltaTime;
        }
        else
        {
            velocidadVertical -= gravedad * Time.deltaTime;
        }

        //Movimiento vertical
        Vector3 movimientoVertical = transform.up * velocidadVertical;
        controller.Move(movimientoVertical * Time.deltaTime);
    }

    public void Salto()
    {
        if (piso)
        {
            velocidadVertical = fuerzaSalto;
        }
    }
}
