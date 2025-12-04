using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using Unity.VisualScripting;


public class warrior_function : NetworkBehaviour
{
    public qual_guerreiro guerreiroesquerdo;
    public qual_guerreirodireito guerreirodireito;
    public GameObject cinemachine, botaofoco, esquerdo, direito, selectwarrior, atirar, proibidoatirar, proibidoatacar, warrior;
    public static CinemachineVirtualCamera cinemachinecamera;
    public GameObject[] guerreiros, jogadores;
    public GameObject botaodesenho, ataque, mira;
    public static GameObject guerreiroqueativoumira;
    public static GameObject guerreiroativado;
    public Transform[] warriorschild, filhosdewarriorfatherfilhos;
    public Transform filhosdewarriorfather;
    public Rigidbody2D constraints;
    public SpriteRenderer balaorenderer, balaoatual;
    public static NetworkObject instanciaguerreiro;
    public static NetworkVariable<ulong> guerreirosID = new NetworkVariable<ulong>();
    public int orderinlayer, indices = 1;
    public int apertado_botao, guardar_o, guardar_p;
    public bool selecionarguerreiro;
    public static int tamanho_vetor;
    private bool proximo, pronto;
    void Awake()
    {
        StartCoroutine(EsperarBotaowarrior());
        StartCoroutine(aguardandobjetosnacena());
    }
    // Start is called before the first frame update
    void Start()
    {
        proximo = false;
        selecionarguerreiro = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (apertado_botao == 2)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    apertado_botao = 0;
                    warrior.SetActive(false);
                    var canvas = GameObject.Find("Canvas");
                    canvas.transform.Find("Fixed Joystick").gameObject.SetActive(true);
                    selecionarguerreiro = true;
                    esquerdo.SetActive(false);
                    direito.SetActive(false);
                    selectwarrior.SetActive(false);
                    botaofoco.SetActive(true);
                    botaodesenho.SetActive(false);
                }
            }
        }
        if (guerreiroesquerdo != null)
        {
            if (guerreiroesquerdo.gameObject.activeSelf)
            {
                if (guerreiroesquerdo.apertado_botao == 3)
                {
                    balaoatual = guerreiroesquerdo.balao[guerreiroesquerdo.balao_selecionado].GetComponent<SpriteRenderer>();
                    for (int k = 0; k < guerreiroesquerdo.balao_diferentesguerreiros.Length; k++)
                    {
                        if (balaoatual.sprite == guerreiroesquerdo.balao_diferentesguerreiros[k])
                        {
                            guardar_p = k;
                        }
                    }
                }
            }
        }
        if (guerreirodireito != null)
        {
            if (guerreirodireito.gameObject.activeSelf)
                {
                    if (guerreirodireito.apertado_botao == 4)
                    {
                        StartCoroutine(aguardandobalaobrancoelementofinal());
                    }
                }
        }
        if (selecionarguerreiro == true)
        {
            selecionarguerreiro = false;
            jogadores = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject jogadorlocal in jogadores)
            {
                var jogadornetwork = jogadorlocal.GetComponent<NetworkObject>();
                if (jogadornetwork.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    cinemachine.SetActive(true);
                    for (int o = 0; o < guerreiros.Length; o++)
                    {
                        guerreiros[o].SetActive(true);
                        instanciaguerreiro = guerreiros[o].GetComponent<NetworkObject>();
                        instanciaguerreiro.enabled = true;
                        if (instanciaguerreiro.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                        {
                            if ((instanciaguerreiro.CompareTag("guerreiroarqueiro") && instanciaguerreiro.IsSpawned == true) || (instanciaguerreiro.CompareTag("guerreirosniper") && instanciaguerreiro.IsSpawned == true))
                            {
                                balaorenderer = guerreiroesquerdo.balao[o].GetComponent<SpriteRenderer>();
                                for (int p = 0; p < guerreiroesquerdo.balao_diferentesguerreiros_vetor.Length; p++)
                                {
                                    if (balaorenderer.sprite == guerreiroesquerdo.balao_diferentesguerreiros[p] || balaorenderer.sprite == guerreirodireito.balao_vermelho[p])
                                    {
                                        var canvas = GameObject.Find("Canvas").gameObject;
                                        for (int contador = 0; contador < canvas.transform.childCount; contador++)
                                        {
                                            if (canvas.transform.GetChild(contador).gameObject.name.Contains("Ataque(Clone)"))
                                            {
                                                ataque = canvas.transform.GetChild(contador).gameObject;
                                            }
                                        }
                                        ataque.SetActive(true);
                                    }
                                }
                            }
                            indices = 1;
                            filhosdewarriorfather = jogadorlocal.transform.Find("warrior's father(Clone)");
                            if (filhosdewarriorfather != null)
                            {
                                var indicesfilhos = 1;
                                for (int j = 0; j < filhosdewarriorfather.transform.childCount; j++)
                                {
                                    var sprite = filhosdewarriorfather.transform.GetChild(j).GetComponent<SpriteRenderer>();
                                    if (sprite.sprite != null)
                                    {
                                        if (!sprite.sprite.name.Contains("balao"))
                                        {
                                            Array.Resize(ref filhosdewarriorfatherfilhos, indicesfilhos);
                                            filhosdewarriorfatherfilhos[indicesfilhos - 1] = filhosdewarriorfather.GetChild(j);
                                            indicesfilhos++;
                                        }
                                    }
                                }
                            }
                        }
                        balaorenderer = guerreiroesquerdo.balao[o].GetComponent<SpriteRenderer>();
                        for (int p = 0; p < guerreiroesquerdo.balao_diferentesguerreiros_vetor.Length; p++)
                        {
                            if (balaorenderer.sprite == guerreiroesquerdo.balao_diferentesguerreiros[p] || balaorenderer.sprite == guerreirodireito.balao_vermelho[p])
                            {
                                instanciaguerreiro.GetComponent<SpriteRenderer>().enabled = true;
                                instanciaguerreiro.GetComponent<movimentar>().enabled = true;
                                guerreiroativado = instanciaguerreiro.gameObject;
                                guerreiroqueativoumira = instanciaguerreiro.gameObject;
                                foco_function.guerreiroativo = instanciaguerreiro;
                                if (IsClient && !IsHost)
                                {
                                    notificarservidormovimentartrueServerRpc(instanciaguerreiro.NetworkObjectId);
                                }
                                else
                                {
                                    ligaroutrosguerreirosClientRpc(instanciaguerreiro.NetworkObjectId);
                                }
                                if (instanciaguerreiro.CompareTag("guerreiroarqueiro") || instanciaguerreiro.CompareTag("guerreirosniper"))
                                {
                                    ataque.GetComponent<atackbutton>().enabled = false;
                                }
                                else
                                {
                                    ataque.GetComponent<gunbow>().enabled = false;
                                }
                                cinemachinecamera = cinemachine.GetComponent<CinemachineVirtualCamera>();
                                cinemachinecamera.Follow = instanciaguerreiro.transform;
                                if (instanciaguerreiro.CompareTag("guerreiroarqueiro") || instanciaguerreiro.CompareTag("guerreirosniper"))
                                {
                                    observarmira.atribuirvalorawarriorfunctionviadragcentralbutton();
                                }
                                balaorenderer.sortingOrder = orderinlayer;
                                constraints = instanciaguerreiro.GetComponent<Rigidbody2D>();
                                constraints.constraints &= ~(RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY);
                                guardar_p = p;
                                guardar_o = o;
                                break;
                            }
                            else
                            {
                                constraints = instanciaguerreiro.GetComponent<Rigidbody2D>();
                                constraints.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                            }
                        }
                    }
                }
                else
                {
                    for (int o = 0; o < guerreiroesquerdo.balao.Length; o++)
                    {
                        var warriorfunction = jogadorlocal.transform.Find("warriorfunction(Clone)").GetComponent<warrior_function>();
                        var scriptwarrior = warriorfunction.GetComponent<warrior_function>();
                        if (scriptwarrior.guerreiros.Length > 0)
                        {
                            scriptwarrior.guerreiros[o].SetActive(true);
                            instanciaguerreiro = scriptwarrior.guerreiros[o].GetComponent<NetworkObject>();
                            instanciaguerreiro.enabled = true;
                            if (instanciaguerreiro.OwnerClientId != NetworkManager.Singleton.LocalClientId)
                            {
                                indices = 1;
                                filhosdewarriorfather = jogadorlocal.transform.Find("warrior's father(Clone)");
                                if (filhosdewarriorfather != null)
                                {
                                    var indicesfilhos = 1;
                                    for (int j = 0; j < filhosdewarriorfather.transform.childCount; j++)
                                    {
                                        var sprite = filhosdewarriorfather.transform.GetChild(j).GetComponent<SpriteRenderer>();
                                        if (sprite.sprite != null)
                                        {
                                            if (!sprite.sprite.name.Contains("balao"))
                                            {
                                                Array.Resize(ref filhosdewarriorfatherfilhos, indicesfilhos);
                                                filhosdewarriorfatherfilhos[indicesfilhos - 1] = filhosdewarriorfather.GetChild(j);
                                                indicesfilhos++;
                                            }
                                        }
                                    }
                                }
                            }
                            constraints = instanciaguerreiro.GetComponent<Rigidbody2D>();
                            constraints.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                        }
                    }
                }
            }
            for (int i = 0; i <= guerreiroesquerdo.balao.Length-1; i++)
            {
                guerreiroesquerdo.balao[i].SetActive(false);
            }
        }
        if (IsServer)
        {
            if (guerreiroesquerdo != null)
            {
                if (guerreiroesquerdo.balao[instanciarguerreiros.indices] != null)
                {
                    for (int m = 0; m < guerreiroesquerdo.balao.Length; m++)
                    {
                        balaorenderer = guerreiroesquerdo.balao[m].GetComponent<SpriteRenderer>();
                        if (m != guardar_o)
                        {
                            if (balaorenderer.sprite != guerreiroesquerdo.balao_diferentesguerreiros[guardar_p] || balaorenderer.sprite != guerreirodireito.balao_vermelho[guardar_p])
                            {
                                balaorenderer.sortingOrder = 4;
                            }
                            if (balaorenderer.sprite == guerreiroesquerdo.balao_diferentesguerreiros[guardar_p] || balaorenderer.sprite == guerreirodireito.balao_vermelho[guardar_p] && proximo == true)
                            {
                                balaorenderer.sortingOrder = orderinlayer;
                                if (proximo == true)
                                {
                                    if (guerreiroesquerdo.apertado_botao == 3)
                                    {
                                        guardar_o--;
                                    }
                                    if (guerreirodireito.apertado_botao == 4)
                                    {
                                        guardar_o++;
                                    }
                                }
                            }
                            proximo = true;
                        }
                    }
                }
            }
        }
        else
        {
            if (guerreiroesquerdo != null)
            {
                if (guerreiroesquerdo.gameObject.activeSelf)
                {
                    if (guerreiroesquerdo.balao.Length == instanciarguerreiros.indicescliente + 1)
                    {
                        if (guerreiroesquerdo.balao[instanciarguerreiros.indicescliente] != null)
                        {
                            for (int m = 0; m < guerreiroesquerdo.balao.Length; m++)
                            {
                                balaorenderer = guerreiroesquerdo.balao[m].GetComponent<SpriteRenderer>();
                                if (m != guardar_o)
                                {
                                    if (balaorenderer.sprite != guerreiroesquerdo.balao_diferentesguerreiros[guardar_p] || balaorenderer.sprite != guerreirodireito.balao_vermelho[guardar_p])
                                    {
                                        balaorenderer.sortingOrder = 4;
                                    }
                                    if (balaorenderer.sprite == guerreiroesquerdo.balao_diferentesguerreiros[guardar_p] || balaorenderer.sprite == guerreirodireito.balao_vermelho[guardar_p] && proximo == true)
                                    {
                                        balaorenderer.sortingOrder = orderinlayer;
                                        if (proximo == true)
                                        {
                                            if (guerreiroesquerdo.apertado_botao == 3)
                                            {
                                                guardar_o--;
                                            }
                                            if (guerreirodireito.apertado_botao == 4)
                                            {
                                                guardar_o++;
                                            }
                                        }
                                    }
                                    proximo = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private IEnumerator aguardandojogadorclicarwserver(NetworkObject guerreironetworkobject)
    {
        if (instanciaguerreiro != null)
        {
            while (!instanciaguerreiro.gameObject.activeSelf)
            {
                yield return null;
            }
            guerreironetworkobject.gameObject.SetActive(true);
            guerreironetworkobject.enabled = true;
            guerreironetworkobject.GetComponent<movimentar>().enabled = true;
            GameObject[] jogadores = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject jogador in jogadores)
            {
                if (jogador.GetComponent<NetworkObject>().OwnerClientId != NetworkManager.Singleton.LocalClientId)
                {
                    var warriorfather = jogador.transform.Find("warrior's father(Clone)");
                    while (warriorfather == null)
                    {
                        yield return null;
                        warriorfather = jogador.transform.Find("warrior's father(Clone)");
                    }
                    for (int contador = 0; contador < warriorfather.childCount; contador++)
                    {
                        if ((warriorfather.GetChild(contador).name.Contains("guerreiro") || warriorfather.GetChild(contador).name.Contains("arqueiro"))
                        && !warriorfather.GetChild(contador).name.Contains("balao"))
                        {
                            warriorfather.GetChild(contador).gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    private IEnumerator aguardandojogadorclicarwclient(NetworkObject guerreironetworkobject)
    {
        if (instanciaguerreiro != null)
        {
            while (!instanciaguerreiro.gameObject.activeSelf)
            {
                yield return null;
            }
            NetworkObject meuPlayer = null;
            while (meuPlayer == null)
            {
                var jogadores = GameObject.FindGameObjectsWithTag("Player");
                foreach (var jogador in jogadores)
                {
                    var netObj = jogador.GetComponent<NetworkObject>();
                    if (netObj != null && netObj.IsSpawned && netObj.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                    {
                        meuPlayer = netObj;
                        break;
                    }
                }
                yield return null;
            }
            foreach (GameObject jogador in jogadores)
            {
                if (jogador.GetComponent<NetworkObject>().OwnerClientId != NetworkManager.Singleton.LocalClientId)
                {
                    var warriorfather = jogador.transform.Find("warrior's father(Clone)");
                    while (warriorfather == null)
                    {
                        yield return null;
                        warriorfather = jogador.transform.Find("warrior's father(Clone)");
                    }
                    for (int contador = 0; contador < warriorfather.childCount; contador++)
                    {
                        if ((warriorfather.GetChild(contador).name.Contains("guerreiro") || warriorfather.GetChild(contador).name.Contains("arqueiro"))
                        && !warriorfather.GetChild(contador).name.Contains("balao"))
                        {
                            warriorfather.GetChild(contador).gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    [ServerRpc]
    private void notificarservidormovimentartrueServerRpc(ulong guerreiroID)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(guerreiroID, out var guerreironetworkobject))
        {
            StartCoroutine(aguardandojogadorclicarwserver(guerreironetworkobject));
        }
    }
    [ClientRpc]
    private void ligaroutrosguerreirosClientRpc(ulong guerreiroID)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(guerreiroID, out var guerreironetworkobject))
        {
            StartCoroutine(aguardandojogadorclicarwclient(guerreironetworkobject));
        }
    }

    public void apertar_botao(int pressionado)
    {
        apertado_botao = pressionado;
    }
    IEnumerator EsperarBotaowarrior()
    {
        // Aguarda até que botaofoco seja atribuído
        while (warrior == null)
        {
            var jogadores = GameObject.FindGameObjectsWithTag("Player");
            foreach (var jogador in jogadores)
            {
                if (jogador.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                {
                    var canvas = GameObject.Find("Canvas");
                    warrior = canvas?.transform.Find("warrior").gameObject;
                }
            }
            yield return null;
        }

        // Selecione o EventTrigger do botaofoco
        EventTrigger trigger = warrior.GetComponent<EventTrigger>();

        if (trigger != null) // Verifica se o EventTrigger existe no botaofoco
        {
            // Se já existir o evento PointerDown, adicione a função apertar_botao(1)
            foreach (var entry in trigger.triggers)
            {
                if (entry.eventID == EventTriggerType.PointerDown)
                {
                    entry.callback.AddListener((eventData) => { apertar_botao(2); });
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
    IEnumerator aguardandobalaobrancoelementofinal()
    {
        while (guerreirodireito == null)
        {
            yield return null;
        }
        while (guerreirodireito.balao_branco_elementofinal == null)
            {
                yield return null;
            }
        for (int f = 0; f < guerreirodireito.balao_vermelho.Length; f++)
        {
            if (guerreirodireito.balao_branco_elementofinal.sprite == guerreirodireito.balao_vermelho[f])
            {
                guardar_p = f;
            }
        }
    }
    IEnumerator aguardandobjetosnacena()
    {
        var canvas = GameObject.Find("Canvas")?.gameObject;
        while (canvas == null)
        {
            canvas = GameObject.Find("Canvas")?.gameObject;
            yield return null;
        }
        while (cinemachine == null || botaofoco == null || esquerdo == null || direito == null || selectwarrior == null
        || atirar == null || proibidoatacar == null || proibidoatirar == null || botaodesenho == null || mira == null
        || guerreiroesquerdo == null || guerreirodireito == null)
        {
            foreach (var camerapersonagem in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (camerapersonagem.name == "character_camera" && camerapersonagem.hideFlags == HideFlags.None && camerapersonagem.scene.IsValid())
                {
                    cinemachine = camerapersonagem;
                }
            }
            botaofoco = canvas.transform.Find("foco")?.gameObject;
            esquerdo = canvas.transform.Find("Select Left")?.gameObject;
            direito = canvas.transform.Find("Select Right")?.gameObject;
            selectwarrior = canvas.transform.Find("Select Warrior")?.gameObject;
            atirar = canvas.transform.Find("atirar")?.gameObject;
            proibidoatirar = canvas.transform.Find("atirarproibido")?.gameObject;
            proibidoatacar = canvas.transform.Find("ataqueproibido")?.gameObject;
            botaodesenho = canvas.transform.Find("botaodesenho")?.gameObject;
            foreach (GameObject mira0 in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                if (mira0.name == "mira_0" && mira0.hideFlags == HideFlags.None && mira0.scene.IsValid())
                {
                    mira = mira0;
                }
                if (mira0.name.Contains("Select Left"))
                {
                    guerreiroesquerdo = mira0?.GetComponent<qual_guerreiro>();
                }
                if (mira0.name.Contains("Select Right"))
                {
                    guerreirodireito = mira0?.GetComponent<qual_guerreirodireito>();
                }
                if (mira0.name.Contains("Select Warrior"))
                {

                }
            }
            yield return null;
        }
        orderinlayer = 4;
        botaofoco.SetActive(false);
        atirar.SetActive(false);
        mira.SetActive(false);
        proibidoatacar.SetActive(false);
        proibidoatirar.SetActive(false);
    }
}
