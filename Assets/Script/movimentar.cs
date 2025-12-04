using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using Cinemachine;

public class movimentar : MonoBehaviour
{
    public NetworkVariable<ulong> miraId = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> movimentoId = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> moverid = new NetworkVariable<ulong>();
    public Joystick mover;
    public GameObject[] instanciadeguerreiros, jogadores;
    private GameObject mira;
    public Transform warriorfathertransform;
    public NetworkRigidbody2D corpoRigido;
    public Rigidbody2D corpo_rigido;
    public float velocidade, velocidadeRotacao;
    public float graus;
    private Vector2 ultimaDirecao = Vector2.zero;
    public static Vector3 tamanhoOriginalMira;
    public Vector3 tamanhoMiraAumentado = new Vector3(0.6392742f, 0.6392742f, 1f);
    public Vector3 arqueiroAumentado = new Vector3(1.5f, 1.5f, 1f), sniperaumentado = new Vector3(1.5f, 1.5f, 1f), artesaoaumentado = new Vector3(1.5f, 1.5f, 1f), escudoespadaaumentado = new Vector3(1.5f, 1.5f, 1f), assassinoaumentado = new Vector3(1.5f, 1.5f, 1f);
    public Vector3 tamanhoriginaldoguerreiroatirador;
    public float distanciaMax = 0.25f;
    private float dist;
    private bool tamanhosOriginaisGuardados = false;
    private GameObject[] todosGuerreiros;
    private Vector3[] tamanhosOriginaisTodosGuerreiros;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float tamanhoCameraOriginal = 5f;
    [SerializeField] private float tamanhoCameraAumentado = 8f;
    public static bool naomiramais;

    private void Start()
    {
        tamanhoriginaldoguerreiroatirador = gameObject.transform.localScale;
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        var objetoderede = GetComponent<NetworkObject>();
        if (objetoderede != null)
        {
            if (!objetoderede.IsOwner) return;
        }
        StartCoroutine(aguardandomover());
    }
    private void mover_character()
    {
        int direction = mover.GetDirection(); // Obtém a direção do joystick
        if (direction != -1) // Se o joystick está em movimento
        {
            // Converte a direção em ângulo
            float novoAngulo = direction * (360f / 128f) - 180f; // Converte para ângulo
            Vector2 novaDirecao = new Vector2(Mathf.Cos(novoAngulo * Mathf.Deg2Rad), Mathf.Sin(novoAngulo * Mathf.Deg2Rad));

            // Atualiza a velocidade do personagem com base na nova direção
            corpo_rigido.velocity = novaDirecao * velocidade;
        }
        else // Se o joystick não está em movimento
        {
            // Para a movimentação se não houver input
            corpo_rigido.velocity = Vector2.zero;
        }
    }
    public void AjustarCamera(bool aumentar)
    {
        if (aumentar)
        {
            virtualCamera.m_Lens.OrthographicSize = tamanhoCameraAumentado;
        }
        else
        {
            virtualCamera.m_Lens.OrthographicSize = tamanhoCameraOriginal;
        }
    }
    private void movermira()
    {
        if (warrior_function.guerreiroativado.GetComponent<observarmira>())
        {
            warrior_function.guerreiroativado.GetComponent<observarmira>().enabled = true;
        }
        int direction = mover.GetDirection();
        if (direction != -1) // Se o joystick está em movimento
        {
            // Converte a direção em ângulo
            float novoAngulo = direction * (360f / 128f) - 180f; // Converte para ângulo
            Vector2 novaDirecao = new Vector2(Mathf.Cos(novoAngulo * Mathf.Deg2Rad), Mathf.Sin(novoAngulo * Mathf.Deg2Rad));

            // Verifica se a nova direção é diferente da última direção
            if (novaDirecao != ultimaDirecao)
            {
                // Atualiza a posição da mira com base na nova direção do joystick
                Vector2 movimentoMira = novaDirecao * velocidade * Time.fixedDeltaTime;
                mira.transform.position += new Vector3(movimentoMira.x, movimentoMira.y, 0);
            }
            else
            {
                // Se não houve mudança de direção, move a mira na velocidade fixa
                Vector2 movimentoMira = ultimaDirecao * velocidade * Time.fixedDeltaTime; // Usar a última direção
                mira.transform.position += new Vector3(movimentoMira.x, movimentoMira.y, 0);
            }

            // Atualiza a última direção da mira
            ultimaDirecao = novaDirecao;
            foreach (var guerreiro in instanciadeguerreiros)
            {
                if (guerreiro.GetComponent<observarmira>())
                {
                    if (guerreiro.GetComponent<observarmira>().enabled)
                    {
                        dist = Vector2.Distance(mira.transform.position, guerreiro.transform.position);
                        break;
                    }
                }
            }
            // usando o primeiro guerreiro como referência
            if (dist >= distanciaMax)
            {
                naomiramais = true;
                observarmira.guerreiroPrincipal.GetComponent<SpriteRenderer>().enabled = false;

                // Aumenta a mira
                mira.transform.localScale = tamanhoMiraAumentado;
                velocidade = 0.25f;
                // Aplica crescimento proporcional a cada guerreiro
                for (int i = 0; i < todosGuerreiros.Length; i++)
                {
                    if (todosGuerreiros[i].CompareTag("guerreiroarqueiro"))
                    {
                        todosGuerreiros[i].transform.localScale = arqueiroAumentado;
                    }
                    else if (todosGuerreiros[i].CompareTag("guerreirodesarmado"))
                    {
                        todosGuerreiros[i].transform.localScale = artesaoaumentado;
                    }
                    else if (todosGuerreiros[i].CompareTag("guerreirosniper"))
                    {
                        todosGuerreiros[i].transform.localScale = sniperaumentado;
                    }
                    else if (todosGuerreiros[i].CompareTag("guerreiroescudoespada"))
                    {
                        todosGuerreiros[i].transform.localScale = escudoespadaaumentado;
                    }
                    else if (todosGuerreiros[i].CompareTag("assassino"))
                    {
                        todosGuerreiros[i].transform.localScale = assassinoaumentado;
                    }
                }
                AjustarCamera(true);
            }
            else
            {
                // Restaura tamanhos originais
                mira.transform.localScale = tamanhoOriginalMira;

                for (int i = 0; i < todosGuerreiros.Length; i++)
                {
                    todosGuerreiros[i].transform.localScale = tamanhosOriginaisTodosGuerreiros[i];
                }

                observarmira.guerreiroPrincipal.GetComponent<SpriteRenderer>().enabled = true;
                velocidade = 3;
                AjustarCamera(false);
                if (naomiramais)
                {
                    naomiramais = false;
                    gameObject.SetActive(false);
                }
            }

        }
        else // Se o joystick não está em movimento
        {
            // Não faz nada ou pode manter a mira na última direção
            // Se desejar, você pode adicionar um comportamento aqui
        }
    }



    // Método para rotacionar o personagem suavemente em direção ao joystick
    private void RotacionarPersonagem()
    {
        // Verifica se o joystick está sendo movido
        if (mover.input != Vector2.zero)
        {
            // Calcula o ângulo desejado com base na direção do joystick
            float angle = Mathf.Atan2(mover.input.y, mover.input.x) * Mathf.Rad2Deg;

            // Corrige o ângulo para alinhar corretamente com a direção do joystick
            angle -= 90f; // Ajuste para corrigir a orientação

            // Atualiza a variável graus com o valor calculado
            graus = angle;

            // Aplica a rotação ao personagem diretamente
            transform.eulerAngles = new Vector3(0, 0, graus);
        }
    }

    private IEnumerator aguardandomover()
    {
        while (mover == null)
        {
            yield return null;
        }
        
        jogadores = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> listaTodos = new List<GameObject>();
        foreach (GameObject jogador in jogadores)
        {
            for (int contador = 0; contador < jogador.transform.childCount; contador++)
            {
                GameObject filho = jogador.transform.GetChild(contador).gameObject;

                if (!filho.name.Contains("balao") &&
                    (filho.CompareTag("guerreiroarqueiro") ||
                    filho.CompareTag("guerreirodesarmado") ||
                    filho.CompareTag("guerreirosniper") ||
                    filho.CompareTag("guerreiroescudoespada") ||
                    filho.CompareTag("assassino")) &&
                    filho.name.Contains("(Clone)"))
                {
                    listaTodos.Add(filho);
                }
            }
        }
        todosGuerreiros = listaTodos.ToArray();
        foreach (GameObject jogador in jogadores)
        {
            if (jogador.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                int indice = 1;
                warriorfathertransform = jogador.transform.Find("warrior's father(Clone)");
                foreach (mira_ativada mira0 in FindObjectsOfType<mira_ativada>(true))
                {
                    if (mira0.gameObject.name == "mira_0" && mira0.gameObject.hideFlags == HideFlags.None && mira0.gameObject.scene.IsValid())
                    {
                        mira = mira0.gameObject;
                    }
                }
                if (warriorfathertransform != null)
                {
                    for (int contador = 0; contador < jogador.transform.childCount; contador++)
                    {
                        if (!jogador.transform.GetChild(contador).name.Contains("balao")
                        && jogador.transform.GetChild(contador).tag.Contains("guerreiroarqueiro")
                        && jogador.transform.GetChild(contador).name.Contains("(Clone)")
                        || !jogador.transform.GetChild(contador).name.Contains("balao")
                        && jogador.transform.GetChild(contador).tag.Contains("guerreirodesarmado")
                        && jogador.transform.GetChild(contador).name.Contains("(Clone)")
                        || !jogador.transform.GetChild(contador).name.Contains("balao")
                        && jogador.transform.GetChild(contador).tag.Contains("guerreirosniper")
                        && jogador.transform.GetChild(contador).name.Contains("(Clone)")
                        || !jogador.transform.GetChild(contador).name.Contains("balao")
                        && jogador.transform.GetChild(contador).tag.Contains("guerreiroescudoespada")
                        && jogador.transform.GetChild(contador).name.Contains("(Clone)")
                        || !jogador.transform.GetChild(contador).name.Contains("balao")
                        && jogador.transform.GetChild(contador).tag.Contains("assassino")
                        && jogador.transform.GetChild(contador).name.Contains("(Clone)"))
                        {
                            Array.Resize(ref instanciadeguerreiros, indice);
                            instanciadeguerreiros[indice - 1] = jogador.transform.GetChild(contador).gameObject;
                            indice++;
                        }
                    }
                }
            }
            foreach (GameObject instanciadeguerreiro in instanciadeguerreiros)
            {
                corpoRigido = instanciadeguerreiro.GetComponent<NetworkRigidbody2D>();
                if (instanciadeguerreiro.GetComponent<movimentar>().enabled == true)
                {
                    mover_character();
                    RotacionarPersonagem();
                }
                else if (warrior_function.guerreiroqueativoumira == instanciadeguerreiro)
                {
                    movermira();
                }
            }
        }
        if (!tamanhosOriginaisGuardados)
        {
            if (mira != null)
                tamanhoOriginalMira = mira.transform.localScale;

            tamanhosOriginaisTodosGuerreiros = new Vector3[todosGuerreiros.Length];
            for (int i = 0; i < todosGuerreiros.Length; i++)
            {
                tamanhosOriginaisTodosGuerreiros[i] = todosGuerreiros[i].transform.localScale;
            }

            tamanhosOriginaisGuardados = true;
        }
    }
    private void OnEnable()
    {
        corpo_rigido = GetComponent<Rigidbody2D>();
    }
}