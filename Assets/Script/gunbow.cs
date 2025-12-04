using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class gunbow : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private int apertado_botao;
    public static int contador;
    public GameObject atirar, ataque, jogadorobject, guerreirodowarriorfunction;
    public static GameObject mira, scriptwarrior;
    public static Vector3 miras, guerreiro;
    private Transform guerreirocinemachine;
    public movimentar movimento;
    public static warrior_function warriorfunction;

    void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (apertado_botao == 1)
        {
            if (Input.touchCount > 0)
            {
                contador++;
                // Define a posição da mira e ativa ela após um frame
                for (int i = 0; i < warriorfunction.guerreiros.Length; i++)
                {
                    guerreirodowarriorfunction = warriorfunction.guerreiros[i];
                    movimento = guerreirodowarriorfunction.GetComponent<movimentar>();
                    if (movimento.enabled == true)
                    {
                        movimento.enabled = false;
                        guerreiro = warriorfunction.guerreiros[i].transform.position;
                        miras = guerreiro;
                        // Força a atualização da cena
                        StartCoroutine(ActivateMiraAfterUpdate());
                        break;
                    }
                }
            }
        }
        else if (apertado_botao == 2)
        {
            if (Input.touchCount > 0)
            {
                if (warrior_function.guerreiroativado && warrior_function.guerreiroativado.activeSelf)
                {
                    var mira = GameObject.Find("mira_0");
                    if (mira)
                    {
                        mira.transform.position = warrior_function.guerreiroativado.transform.position;
                        mira.transform.localScale = movimentar.tamanhoOriginalMira;
                    }
                    movimento = warrior_function.guerreiroativado.GetComponent<movimentar>();
                    warrior_function.guerreiroativado.transform.localScale = movimento.tamanhoriginaldoguerreiroatirador;
                    warrior_function.guerreiroativado.GetComponent<SpriteRenderer>().enabled = true;
                    if (warrior_function.guerreiroativado.GetComponent<observarmira>())
                    {
                        warrior_function.guerreiroativado.GetComponent<observarmira>().enabled = false;
                    }
                    movimento.AjustarCamera(false);
                    movimentar.naomiramais = false;
                }
                contador = 0;
                mira.SetActive(false);
                for (int i = 0; i < warriorfunction.guerreiros.Length; i++)
                {
                    guerreirocinemachine = warriorfunction.guerreiros[i].transform;
                    guerreirodowarriorfunction = warriorfunction.guerreiros[i];
                    if (warrior_function.cinemachinecamera.Follow == guerreirocinemachine)
                    {
                        guerreirodowarriorfunction.GetComponent<movimentar>().enabled = true;
                    }
                }
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (contador == 0)
        {
            apertar_botao(1); // Define apertado_botao como 1 quando o ponteiro é pressionado
        }
        else
        {
            apertar_botao(2);
        }
    }

    // Método chamado quando o ponteiro é liberado do GameObject
    public void OnPointerUp(PointerEventData eventData)
    {
        apertar_botao(0); // Define apertado_botao como 0 quando o ponteiro é liberado
    }

    public void apertar_botao(int pressionado)
    {
        apertado_botao = pressionado;
    }
    public void receberscriptwarrior()
    {
        GameObject[] jogadores = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject jogador in jogadores)
        {
            if (jogador.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                warriorfunction = jogador.transform.Find("warriorfunction(Clone)").GetComponent<warrior_function>();
                var canvas = GameObject.Find("Canvas").gameObject;
                var warrior = canvas.transform.Find("warrior").gameObject;
                scriptwarrior = warrior;
                warriorfunction = scriptwarrior.GetComponent<warrior_function>();
            }
        }
    }
    private IEnumerator ActivateMiraAfterUpdate()
    {
        yield return null; // Aguarda um frame para garantir que a posição foi atualizada
        GameObject[] jogadores = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject jogador in jogadores)
        {
            if (jogador.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                foreach (GameObject mira0 in Resources.FindObjectsOfTypeAll<GameObject>())
                {
                    if (mira0.name == "mira_0" && mira0.hideFlags == HideFlags.None && mira0.scene.IsValid())
                    {
                        mira = mira0;
                    }
                }
            }
        }
        mira.transform.position = miras;
        mira.SetActive(true); // Ativa a mira após um frame
    }

    private void OnEnable()
    {
        var objetoswarriorfunction = FindObjectsOfType<warrior_function>();
        foreach (var objetowarriorfunction in objetoswarriorfunction)
        {
            if (objetowarriorfunction.IsOwner)
            {
                warriorfunction = objetowarriorfunction;
                scriptwarrior = objetowarriorfunction.gameObject;
            }
        }
        foreach (var mira0 in FindObjectsOfType<observarmira>())
        {
            if (mira0.gameObject.name == "mira_0" && mira0.gameObject.hideFlags == HideFlags.None && mira0.gameObject.scene.IsValid())
            {
                mira = mira0.gameObject;
            }
        }
        if (mira != null)
        {
            miras = mira.transform.position;
        }
    }
}
