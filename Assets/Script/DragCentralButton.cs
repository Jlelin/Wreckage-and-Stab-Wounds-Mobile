using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Unity.Netcode;
using System.Linq;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
public class DragCentralButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public static event Action OnPronto; 
    public observarmira observarmira;
    private bool isButtonPressed = false;
    public Vector3 scaleNormal = Vector3.one; // Escala normal do botão
    private Coroutine shrinkCoroutine;
    private Vector3 offset;

    public GameObject mask, escudoespada, arcoflecha, desarmado, armado, assassino; // Objetos relacionados
    public GameObject joystick;
    public botaodesenho desenho;
    public qual_guerreiro guerreiroesquerdo;
    public qual_guerreirodireito guerreirodireito;
    public warrior_function warriorfunction;

    [Header("Scale Settings")]
    public float scaleDuration = 2f; // Duração da animação de escala
    public float smallSizeThreshold; // Limite para determinar quando o botão está pequeno]
    [SerializeField]
    public static bool hasInstantiated = false; // Controle para garantir que a instância ocorra apenas uma vez

    private AnimationCurve scaleCurve; // Curva de animação para controle de escala
    private ScaleType currentScaleType; // Tipo de variação de escala atual

    [Header("Score")]
    public float totalScore = 0f; // Pontuação total acumulada do jogador

    private void Awake()
    {
        guerreiroesquerdo = FindFirstObjectByType<qual_guerreiro>();
        guerreirodireito = FindFirstObjectByType<qual_guerreirodireito>();
        desenho = FindFirstObjectByType<botaodesenho>();
        warriorfunction = FindFirstObjectByType<warrior_function>();
    }

    void Start()
    {

    }

    void Update()
    {
        if (isButtonPressed)
        {
            // A escala é controlada pela coroutine
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPronto?.Invoke();
        instanciarguerreiros.botaoguerreirodragcentralbutton = gameObject;
        hasInstantiated = true;
        // Indicar que o botão está pressionado
        isButtonPressed = true;
        if (escudoespada != botoes_pergaminho.botaocentral)
        {
            escudoespada.SetActive(false);
        }
        if (arcoflecha != botoes_pergaminho.botaocentral)
        {
            arcoflecha.SetActive(false);
        }
        if (desarmado != botoes_pergaminho.botaocentral)
        {
            desarmado.SetActive(false);
        }
        if (armado != botoes_pergaminho.botaocentral)
        {
            armado.SetActive(false);
        }
        if (assassino != botoes_pergaminho.botaocentral)
        {
            assassino.SetActive(false);
        }
        Image maskImage = mask.GetComponent<Image>();
        Mask mascara = mask.GetComponent<Mask>();
        maskImage.enabled = false;
        mascara.enabled = false;
        // Salvar a escala normal do botão quando é pressionado
        scaleNormal = transform.localScale;
        // Selecionar aleatoriamente um tipo de variação de escala
        currentScaleType = ScaleType.SlowOscillationToThreshold;

        // Atualizar a curva de escala com base no tipo selecionado
        scaleCurve = GetCurveForType(currentScaleType);

        // Iniciar a diminuição contínua da escala
        shrinkCoroutine = StartCoroutine(ShrinkButton());

        // Calcular o offset para arrastar o botão
        RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out var globalMousePos);
        offset = transform.position - globalMousePos;
        StartCoroutine(ChangeScaleTypeRandomly());
    }

    IEnumerator ChangeScaleTypeRandomly()
    {
        while (isButtonPressed)
        {
            float waitTime = UnityEngine.Random.Range(0.05f, scaleDuration); // Tempo aleatório para trocar a variação
            yield return new WaitForSeconds(waitTime);

            // Escolher uma nova variação de escala aleatoriamente
            currentScaleType = GetRandomScaleType();

            // Atualizar a curva de escala com base no novo tipo selecionado
            scaleCurve = GetCurveForType(currentScaleType);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Indicar que o botão não está mais pressionado
        isButtonPressed = false;

        // Cancelar a diminuição contínua da escala quando o botão é solto
        if (shrinkCoroutine != null)
        {
            StopCoroutine(shrinkCoroutine);
        }

        // Calcular e somar a pontuação com base na escala atual no eixo Y
        float score = CalculateScore();
        totalScore += score; // Acumula a pontuação total
        transform.localScale = scaleNormal;
        Image maskImage = mask.GetComponent<Image>();
        Mask mascara = mask.GetComponent<Mask>();
        if (maskImage != null)
        {
            // Desativar o componente Image
            maskImage.enabled = false;
            mascara.enabled = false;
            if (escudoespada != botoes_pergaminho.botaocentral)
            {
                escudoespada.SetActive(true);
                escudoespada.transform.Find("Image").GetComponent<Image>().enabled = false;
            }
            if (arcoflecha != botoes_pergaminho.botaocentral)
            {
                arcoflecha.SetActive(true);
                arcoflecha.transform.Find("Image").GetComponent<Image>().enabled = false;
            }
            if (desarmado != botoes_pergaminho.botaocentral)
            {
                desarmado.SetActive(true);
                desarmado.transform.Find("Image").GetComponent<Image>().enabled = false;
            }
            if (armado != botoes_pergaminho.botaocentral)
            {
                armado.SetActive(true);
                armado.transform.Find("Image").GetComponent<Image>().enabled = false;
            }
            if (assassino != botoes_pergaminho.botaocentral)
            {
                assassino.SetActive(true);
                assassino.transform.Find("Image").GetComponent<Image>().enabled = false;
            }
        }
        else
        {
            Debug.LogWarning("O componente Image não foi encontrado no GameObject mask.");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isButtonPressed)
        {
            // Move o botão para a nova posição do ponteiro com o deslocamento
            RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out var globalMousePos);
            transform.position = globalMousePos + offset;
        }
    }

    IEnumerator ShrinkButton()
    {
        float elapsedTime = 0f;

        while (elapsedTime < scaleDuration)
        {
            elapsedTime += Time.deltaTime;
            float scaleMultiplier = scaleCurve.Evaluate(elapsedTime / scaleDuration);
            Vector3 currentScale = scaleNormal * scaleMultiplier;
            transform.localScale = currentScale;
            yield return null;
        }
        // Após a animação, defina a escala para a normal
            transform.localScale = scaleNormal;
    }

    private float CalculateScore()
    {
        // Calcula a pontuação com base na escala atual do botão no eixo Y
        float currentScaleY = transform.localScale.y;
        // Ajuste para que a pontuação seja positiva
        float score = 1f - currentScaleY;
        return score; // Multiplicando por 100 para tornar a pontuação mais significativa
    }

    private ScaleType GetRandomScaleType()
    {
        ScaleType[] scaleTypes = (ScaleType[])System.Enum.GetValues(typeof(ScaleType));
        int randomIndex = UnityEngine.Random.Range(0, scaleTypes.Length);
        return scaleTypes[randomIndex];
    }

    private AnimationCurve GetCurveForType(ScaleType type)
    {
        AnimationCurve curve;
        Keyframe[] keyframes;
        switch (type)
        {
            case ScaleType.SlowDecrease:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, 0f)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.AcceleratedDecrease:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, 0f)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.DeceleratedDecrease:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, 0f)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.FastDecrease:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, 0f)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.SlowThenFastDecrease:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, 0f)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.FastThenSlowDecrease:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, 0f)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.SlowDecreaseToThreshold:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, smallSizeThreshold)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.FastDecreaseToThreshold:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, smallSizeThreshold)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.SmoothDecreaseToThreshold:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, smallSizeThreshold)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.RapidOscillationToThreshold:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration * 0.25f, smallSizeThreshold * 1.2f),
                    new Keyframe(scaleDuration * 0.5f, smallSizeThreshold * 0.8f),
                    new Keyframe(scaleDuration * 0.75f, smallSizeThreshold * 1.1f),
                    new Keyframe(scaleDuration, smallSizeThreshold)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.SuddenDropToThreshold:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration * 0.5f, 1f),
                    new Keyframe(scaleDuration, smallSizeThreshold)
                };
                curve = new AnimationCurve(keyframes);
                break;
            case ScaleType.SlowOscillationToThreshold:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration * 0.33f, smallSizeThreshold * 1.2f),
                    new Keyframe(scaleDuration * 0.66f, smallSizeThreshold * 0.8f),
                    new Keyframe(scaleDuration, smallSizeThreshold)
                };
                curve = new AnimationCurve(keyframes);
                break;
            default:
                keyframes = new Keyframe[]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(scaleDuration, 0.5f)
                };
                curve = new AnimationCurve(keyframes);
                break;
        }
        return curve;
    }

    private enum ScaleType
    {
        SlowDecrease,
        AcceleratedDecrease,
        DeceleratedDecrease,
        FastDecrease,
        SlowThenFastDecrease,
        FastThenSlowDecrease,
        SlowDecreaseToThreshold,
        FastDecreaseToThreshold,
        SmoothDecreaseToThreshold,
        RapidOscillationToThreshold,
        SuddenDropToThreshold,
        SlowOscillationToThreshold
    }
}
