using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "DOTweenAnimationsConfig", menuName = "Scriptable Objects/DOTween Animations", order = 3)]
public class DOTweenConfig : ScriptableObject
{
    [Header("UI - PopUps")]
    public Ease popUpEase = Ease.OutBack;
    public float popUpTime = 0.5f;

    [Header("UI - Paneles")]
    public Ease panelEase = Ease.InOutSine;
    public float panelTime = 0.4f;

    [Header("UI - Botones")]
    public Ease buttonEase = Ease.OutBounce;
    public float buttonTime = 0.3f;

    [Header("Objetos 3D - Escala")]
    public Ease objectScaleEase = Ease.OutElastic;
    public float objectScaleTime = 0.8f;

    [Header("Objetos 3D - Movimiento")]
    public Ease objectMoveEase = Ease.InOutQuad;
    public float objectMoveTime = 1.0f;

    [Header("Objetos 3D - Rotación")]
    public Ease objectRotateEase = Ease.InOutSine;
    public float objectRotateTime = 0.6f;

    [Header("Transiciones de Escena")]
    public Ease sceneTransitionEase = Ease.Linear;
    public float sceneTransitionTime = 1.2f;
}
