using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "DOTweenAnimationsConfig", menuName = "Scriptable Objects/AnimationsDotween/DOTween Animations", order = 3)]
public class DOTweenConfig : ScriptableObject
{
    [Header("--- UI---")]
    [Header("UI - PopUps")]
    public Ease popUpEase = Ease.OutBack;
    public float popUpTime = 0.5f;

    [Header("UI - Paneles")]
    public Ease panelEase = Ease.InOutSine;
    public float panelTime = 0.4f;

    [Header("UI - Paneles Intro")]
    public Ease introPanelEase = Ease.InOutSine;
    public float introPanelTime = 0.4f;
    public Ease blinkEase = Ease.Linear;
    public float blinkTime = 0.5f;

    [Header("Subtítulos")]
    public float subtitleScaleTime = 0.3f;
    public Ease subtitleScaleEase = Ease.OutBack;
    public float subtitleTypingDuration = 0.5f; 
    public float subtitleFadeOutTime = 0.3f;

    [Header("UI - Apertura (Cinemática)")]
    public float endValueFade = 1f;
    public float durationFade = 0.2f;
    public float textScaleTime = 0.5f;
    public Ease textScaleEase = Ease.OutBack;
    public float textReadTime = 2f;
    public float textFadeOutTime = 1f;

    [Header("Apertura de Ojos (Párpados)")]
    public float eyelidMoveDistance = 1500f;
    public float eyelidMoveTime = 1.5f;
    public Ease eyelidMoveEase = Ease.InOutSine;

    [Header("UI - Botones")]
    public Ease buttonEase = Ease.OutBounce;
    public float buttonTime = 0.3f;

    [Header("Objetos 3D - Escala")]
    public Ease objectScaleEase = Ease.OutElastic;
    public float objectScaleTime = 0.8f;

    [Header("Objetos 3D - Movimiento")]
    public Ease objectMoveEase = Ease.InOutQuad;
    public float objectMoveTime = 1.0f;
    public float objectMovePower = 1.0f;
    public int objectNumberMoves = 1;

    [Header("Objetos 3D - Rotación")]
    public Ease objectRotateEase = Ease.InOutSine;
    public RotateMode objectRotateMode;
    public float objectRotateTime = 0.6f;

    [Header("Transiciones de Escena")]
    public Ease sceneTransitionEase = Ease.Linear;
    public float simpleFadeTime = 1.2f;

    [Header("--- INTERACCIONES Y ENTORNO ---")]
    [Header("Sello Validador")]
    public float stampPunchTime = 0.2f;
    public Vector3 stampPunchVector = new Vector3(0, -0.1f, 0);
    public int stampPunchVibrato = 1;
    public float stampPunchElasticity = 0.5f;

    [Header("Temblor del Camión (Viaje)")]
    public Ease truckMoveEase;
    public float truckShakeTime = 10f; 
    public Vector3 truckShakeStrength = new Vector3(0.1f, 0.1f, 0.1f);
    public int truckShakeVibrato = 10;

    [Header("Frenado del Camión (Impacto)")]
    public float truckBrakeTime = 0.4f;
    public Vector3 truckBrakePunch = new Vector3(-3f, 0, 0); 
    public int truckBrakeVibrato = 5;

    [Header("Animación de Ruedas")]
    public float wheelRotationSpeed = 360f;
    public Ease wheelEase = Ease.Linear;

    [Header("Apertura de Garaje")]
    public float garageOpenTime = 1.5f;
    public Ease garageOpenEase = Ease.InOutSine;

    [Header("Aparición de Cajas")]
    public float boxPopTime = 0.5f;
    public Ease boxPopEase = Ease.OutBack;

    [Header("Flecha Guía")]
    public float arrowMoveDistance = 0.5f;
    public float arrowMoveTime = 0.5f;
    public Ease arrowMoveEase = Ease.InOutSine;

    [Header("Salida del Camión")]
    public float dispatchAnticipationDist = 1.5f;
    public float dispatchAnticipationTime = 0.6f;
    public Ease dispatchAnticipationEase = Ease.OutQuad;
    public Vector3 dispatchAnticipationRot = new Vector3(-4f, 0, 0);
    public float dispatchFastWheelTime = 0.05f;
    public float dispatchFugaTime = 1.5f;
    public Ease dispatchFugaEase = Ease.InExpo;
    public float dispatchTrompaBajaTime = 0.2f;
    public Vector3 dispatchFugaShakeStrength = new Vector3(0, 0.15f, 0.15f);
    public int dispatchFugaShakeVibrato = 20;

    [Header("Regreso al Punto de Inicio")]
    public float dispatchReturnTime = 1f;
    public Ease dispatchReturnEase = Ease.InOutSine;

    [Header("--- MINIJUEGOS ---")]
    [Header("Minijuego Cinta - Animación Caja")]
    public float boxFlipJumpPower = 1f;
    public float boxFlipDuration = 0.6f;
    public Ease boxFlipEase = Ease.OutQuad;
    public float boxCloseTapasDuration = 0.3f;
    public Ease boxCloseTapasEase = Ease.OutBounce;
    public float minigameUIAnimTime = 0.3f;

    [Header("Minijuego Cinta - Tensión y Errores")]
    public Vector3 tapeClickPunchScale = new Vector3(0.1f, 0.1f, 0f);
    public float tapeClickPunchDuration = 0.2f;
    public float tapeTensionShakeDuration = 0.1f;
    public float tapeTensionShakeStrength = 2f;
    public int tapeTensionShakeVibrato = 10;
    public float tapeErrorShakeDuration = 0.3f;
    public float tapeErrorShakeStrength = 15f;
    public int tapeErrorShakeVibrato = 20;

    [Header("Minijuego Impresora")]
    public Ease printerUIPopEase = Ease.OutBack;
    public Ease printerNeedleEase = Ease.InOutSine;
    public float printerUIPopTime = 0.3f;
    public float printerNeedleSpeed = 1.2f;
    public float printerButtonPunchTime = 0.1f;
    public float printerJumpPower = 0.15f;
    public float printerJumpTime = 0.25f;
    public float printerLabelScaleTime = 0.2f;
    public float printerLabelJumpPower = 0.4f;
    public float printerLabelJumpTime = 0.4f;
    public float printerLabelRotateTime = 0.4f;

    [Header("Minijuego Impresora - Errores")]
    public float printerErrorShakeDuration = 0.3f;
    public float printerErrorShakeStrength = 15f;
    public int printerErrorShakeVibrato = 20;

    [Header("Minijuego Despacho (Camión)")]
    public float dispatchBoxJumpPower = 1.5f;
    public float dispatchBoxJumpTime = 0.8f;
    public Ease dispatchBoxJumpEase = Ease.OutQuad;

    public Vector3 rotacionIzqAbierta = new Vector3(0, 90, 0);
    public Vector3 rotacionDerAbierta = new Vector3(0, -90, 0);
    public float dispatchDoorCloseTime = 0.8f;
    public Ease dispatchDoorCloseEase = Ease.InQuad;
    public Ease doorsOpened = Ease.OutBack;

    public float dispatchTruckPunchTime = 0.5f;
    public Vector3 dispatchTruckPunchForce = new Vector3(0, 0.1f, 0);

    public float dispatchTruckDriveTime = 2.5f;
    public Ease dispatchTruckDriveEase = Ease.InBack;

    [Header("--- EVALUACIÓN FINAL ---")]
    public float starFadeDuration = 0.3f;
    [Tooltip("Usa valores pequeños como 0.2 para evitar que se salgan de la pantalla")]
    public Vector3 starPunchScale = new Vector3(0.2f, 0.2f, 0.2f);
    public float starPunchDuration = 0.5f;
    public int starPunchVibrato = 5;
    public float starInterval = 0.2f;
}

