using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class foco_function : NetworkBehaviour, IAposClientePronto
{
    public int apertado_botao;
    public static NetworkObject guerreiroativo;
    public NetworkButtonManager networkbutton_manager;
    public qual_guerreiro guerreiroesquerdo;
    private qual_guerreirodireito guerreirodireito;
    public warrior_function warrior;
    private foco_function foco;
    private movimentar movimento;
    public Camera maincamera;
    public SpriteRenderer terreno, balaorenderer;
    public GameObject warriorobject, ataque, esquerdo, direito, selectwarrior, botaodesenho, atirar;
    public GameObject mira, charactercamera;
    private GameObject botaofoco;
    private int ativoucoroutine; // Mova a declaração para o escopo da classe
    public float largura, altura;
    private bool pronto;
    public void ClientePronto()
    {
        pronto = true;
    }
    void Awake()
    {
        var canvas = GameObject.Find("Canvas");
        botaodesenho = canvas.transform.Find("botaodesenho").gameObject;
        botaodesenho.GetComponent<botaodesenho>().foco = this;
        botaofoco = canvas.transform.Find("foco").gameObject;
        foreach (var selecionarguerreiro in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (selecionarguerreiro.name.Contains("Select Left"))
            {
                esquerdo = selecionarguerreiro;
                guerreiroesquerdo = selecionarguerreiro.GetComponent<qual_guerreiro>();
            }
            if (selecionarguerreiro.name.Contains("Select Right"))
            {
                direito = selecionarguerreiro;
                guerreirodireito = selecionarguerreiro.GetComponent<qual_guerreirodireito>();
            }
            if (selecionarguerreiro.name.Contains("Select Warrior"))
            {
                selectwarrior = selecionarguerreiro;
            }
            if (selecionarguerreiro.name == "character_camera" && selecionarguerreiro.hideFlags == HideFlags.None && selecionarguerreiro.scene.IsValid())
            {
                charactercamera = selecionarguerreiro;
            }
            if (selecionarguerreiro.name.Contains("mira_0") && selecionarguerreiro.hideFlags == HideFlags.None && selecionarguerreiro.scene.IsValid())
            {
                mira = selecionarguerreiro;
                if (selecionarguerreiro.activeSelf)
                {
                    mira.SetActive(false);
                }
            }
        }
        maincamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        terreno = GameObject.Find("terreno_base_0").GetComponent<SpriteRenderer>();
        altura = terreno.bounds.size.y;
        altura = altura * 100;
        largura = altura / 100f;
        largura = largura / 2f;
        maincamera.orthographicSize = largura;
        warrior = FindFirstObjectByType<warrior_function>();
        networkbutton_manager = NetworkButtonManager.canvasHostClient.GetComponent<NetworkButtonManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EsperarBotaofoco());
    }
    // Update is called once per frame
    public void Update()
    {
        if (!pronto) return;
        if (apertado_botao == 1)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    if (warrior_function.guerreiroativado && warrior_function.guerreiroativado.activeSelf)
                    {
                        var mira = GameObject.Find("mira_0");
                        if (mira)
                        {
                            mira.transform.position = warrior_function.guerreiroativado.transform.position;
                            mira.transform.localScale = movimentar.tamanhoOriginalMira;
                            movimentar.naomiramais = false;
                        }
                        movimento = warrior_function.guerreiroativado.GetComponent<movimentar>();
                        warrior_function.guerreiroativado.transform.localScale = movimento.tamanhoriginaldoguerreiroatirador;
                        if (warrior_function.guerreiroativado.GetComponent<observarmira>())
                        {
                            warrior_function.guerreiroativado.GetComponent<observarmira>().enabled = false;
                        }
                    }
                    var canvasjoystick = GameObject.Find("Canvas");
                    canvasjoystick.transform.Find("Fixed Joystick").gameObject.SetActive(false);
                    maincamera.gameObject.transform.position = new Vector3(0, 0, -1);
                    maincamera.orthographicSize = largura;
                    ataque.SetActive(false);
                    atirar.SetActive(false);
                    mira.SetActive(false);
                    if (gunbow.contador > 1)
                    {
                        gunbow.contador = 0;
                    }
                    if (guerreiroesquerdo.balao[0] != null)
                    {
                        for (int i = 0; i <= guerreiroesquerdo.balao.Length; i++)
                        { // Corrija a condição aqui
                            if (i < guerreiroesquerdo.balao.Length)
                            {
                                guerreiroesquerdo.balao[i].SetActive(true);
                                GameObject[] jogadores = GameObject.FindGameObjectsWithTag("Player");
                                foreach (GameObject jogador in jogadores)
                                {
                                    if (jogador.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                                    {
                                        var canvas = GameObject.Find("Canvas");
                                        warrior = jogador.transform.Find("warriorfunction(Clone)").gameObject.GetComponent<warrior_function>();
                                        warriorobject = canvas.transform.Find("warrior").gameObject;
                                    }
                                    else
                                    {
                                        var warriorfather = jogador.transform.Find("warrior's father(Clone)");
                                        if (warriorfather != null)
                                        {
                                            for (int contador = 0; contador < warriorfather.childCount; contador++)
                                            {
                                                if ((warriorfather.GetChild(contador).name.Contains("guerreiro") || warriorfather.GetChild(contador).name.Contains("arqueiro"))
                                                && !warriorfather.GetChild(contador).name.Contains("balao"))
                                                {
                                                    warriorfather.GetChild(contador).gameObject.SetActive(false);
                                                }
                                            }
                                        }
                                    }
                                }
                                if (i < warrior.guerreiros.Length)
                                {
                                    warrior.guerreiros[i].SetActive(false);
                                    warrior.guerreiros[i].GetComponent<movimentar>().enabled = false;
                                }
                            }
                        }
                    }
                    if (ataque.GetComponent<atackbutton>().enabled == true)
                    {
                        ataque.GetComponent<gunbow>().enabled = true;
                    }
                    else
                    {
                        ataque.GetComponent<atackbutton>().enabled = true;
                    }
                    foreach (var guerreiros in FindObjectsOfType<life>(true))
                    {
                        var jogadores = GameObject.FindGameObjectsWithTag("Player");
                        foreach (var jogador in jogadores)
                        {
                            if (jogador.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                            {
                                if ((guerreiros.gameObject.name.Contains("arqueiro_0")
                                || guerreiros.gameObject.name.Contains("guerreiro_0")
                                || guerreiros.gameObject.name.Contains("guerreiro_armado_0")
                                || guerreiros.gameObject.name.Contains("guerreiro_escudo_0")
                                || guerreiros.gameObject.name.Contains("guerreiroassassino"))
                                && (guerreiros.gameObject.hideFlags == HideFlags.None && guerreiros.gameObject.scene.IsValid())
                                && guerreiros.GetComponent<NetworkObject>().OwnerClientId == jogador.GetComponent<NetworkObject>().OwnerClientId)
                                {
                                    esquerdo.SetActive(true);
                                    direito.SetActive(true);
                                    selectwarrior.SetActive(true);
                                    for (int o = 0; o < guerreiroesquerdo.balao.Length; o++)
                                    {
                                        balaorenderer = guerreiroesquerdo.balao[o].GetComponent<SpriteRenderer>();
                                        for (int p = 0; p < guerreiroesquerdo.balao_diferentesguerreiros_vetor.Length; p++)
                                        {
                                            if (balaorenderer.sprite == guerreiroesquerdo.balao_diferentesguerreiros[p] || balaorenderer.sprite == guerreirodireito.balao_vermelho[p])
                                            {
                                                warriorobject.SetActive(true);
                                            }
                                        }
                                    }
                                }
                                break;
                            }
                        }
                    }
                    botaodesenho.SetActive(true);
                    charactercamera.SetActive(false);
                    var canvass = GameObject.Find("Canvas");
                    canvass.transform.Find("foco").gameObject.SetActive(false);
                    apertado_botao = 0;
                }
            }
        }
        if (foco != null)
        {
            StartCoroutine(EsperarGuerreiroNaRede());
        }
    }
private IEnumerator EsperarGuerreiroNaRede()
{
    // Espera até guerreiroativo existir e estar spawnado na rede
    while (guerreiroativo == null ||
           !guerreiroativo || // destruído
           !guerreiroativo.GetComponent<NetworkObject>().IsSpawned)
    {
        yield return null; // espera um frame
    }
    if (foco.gameObject.activeSelf && guerreiroativo != null && ativoucoroutine < 1 && warrior != null)
    {
        if (guerreiroativo.gameObject.activeSelf)
        {
            StartCoroutine(enquantoestaativo());
            ativoucoroutine++;
        }
    }
}

    public void apertar_botao(int pressionado)
    {
        apertado_botao = pressionado;
    }
    private IEnumerator enquantoestaativo()
    {
        while (guerreiroativo.gameObject.activeSelf)
        {
            yield return null;
            GameObject[] todosobjetos = FindObjectsOfType<GameObject>(true);
            foreach (GameObject objeto in todosobjetos)
            {
                if (foco.gameObject.activeSelf)
                {
                    if ((objeto.name.Contains("guerreiro") || objeto.name.Contains("arqueiro")) && !objeto.name.Contains("balao") && !objeto.name.Contains("tag"))
                    {
                        if (!objeto.activeSelf)
                        {
                            objeto.SetActive(true);
                        }
                    }
                }
            }
        }
        ativoucoroutine = 0;
    }

    IEnumerator EsperarBotaofoco()
    {
        // Aguarda até que botaofoco seja atribuído
        while (botaofoco.gameObject == null)
        {
            yield return null; // Espera até o próximo frame para verificar novamente
        }

        // Selecione o EventTrigger do botaofoco
        EventTrigger trigger = botaofoco.gameObject.GetComponent<EventTrigger>();

        if (trigger != null) // Verifica se o EventTrigger existe no botaofoco
        {
            // Se já existir o evento PointerDown, adicione a função apertar_botao(1)
            foreach (var entry in trigger.triggers)
            {
                if (entry.eventID == EventTriggerType.PointerDown)
                {
                    entry.callback.AddListener((eventData) => { apertar_botao(1); });
                }
                else if (entry.eventID == EventTriggerType.PointerUp)
                {
                    entry.callback.AddListener((eventData) => { apertar_botao(0); });
                }
            }
        }
        else
        {
            Debug.LogWarning("EventTrigger não encontrado no objeto botaofoco.");
        }
    }
}
