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


public class instanciarguerreiros : NetworkBehaviour, IAposClientePronto
{
    private delegate void assinaturaposclientrpcatribuirinstanciaguerreiro();
    private event assinaturaposclientrpcatribuirinstanciaguerreiro valoratribuidoainstanciaguerreiro;
    public NetworkVariable<ulong> guerreiroID = new NetworkVariable<ulong>();
    public NetworkVariable<ulong> guerreiroInstanciadoID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<ulong> avoID = new NetworkVariable<ulong>();
    public static DragCentralButton arrastarbotaocentral;
    public GameObject[] jogadores, guerreirosinstanciados;
    public observarmira observarmira;
    public static int indices = 0, indicescliente = 0;
    public int maisumguerreiro, indicedosguerreirosnovos;
    public static int tamanho;
    public GameObject[] guerreiros, baloes;
    public static GameObject guerreiroInstanciado, botaoguerreirodragcentralbutton;
    public GameObject pergaminho_enrolado, desenrolado;
    public GameObject left, right, select_, warrior, focus, joystick, botaodedesenho, warriorfunctionobject;
    public GameObject ataquebutton, mira, balaoInstanciado;
    public static GameObject mirainstance;
    public static NetworkObject instanciaguerreiro;
    public botaodesenho desenho;
    public qual_guerreiro guerreiroesquerdo;
    public qual_guerreirodireito guerreirodireito;
    public warrior_function warriorfunction;
    public sowrdshieldfight escudoespadaluta;
    private movimentar movimento;
    public NetworkObject jogador;
    public int tamanhototal = 0;
    public string tagCentral;
    private bool possuiindicevazio, pronto;
    private int tagcentral;
    public struct GuerreiroSpawnData : INetworkSerializable
    {
        public int tagcentral;
        public ulong clientId;
        public float posX, posY, posZ;

        public Vector3 Position
        {
            get => new Vector3(posX, posY, posZ);
            set
            {
                posX = value.x;
                posY = value.y;
                posZ = value.z;
            }
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref tagcentral);
            serializer.SerializeValue(ref clientId);
            serializer.SerializeValue(ref posX);
            serializer.SerializeValue(ref posY);
            serializer.SerializeValue(ref posZ);
        }
    }
    public void ClientePronto()
    {
        pronto = true;
    }
    private void Awake()
    {
        StartCoroutine(aguardandorede());
        indicedosguerreirosnovos++;
        valoratribuidoainstanciaguerreiro += depoisdeinstanciaguerreiro;
    }
    // Start is called before the first frame update
    void Start()
    {
        maisumguerreiro = 0;
        tamanho = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!pronto) return;
        // Verificar se o botão atingiu o tamanho pequeno
        if (DragCentralButton.hasInstantiated && botaoguerreirodragcentralbutton.transform.localScale.y < arrastarbotaocentral.smallSizeThreshold)
        {
            DragCentralButton.hasInstantiated = false;
            InstantiateBalaoEGuerreiro();
        }
    }
    private void InstantiateBalaoEGuerreiro()
    {
        tagCentral = arrastarbotaocentral.tag;
        if (tagCentral.Contains("arqueiro"))
        {
            tagcentral = 0;
        }
        else if (tagCentral.Contains("desarmado"))
        {
            tagcentral = 1;
        }
        else if (tagCentral.Contains("sniper"))
        {
            tagcentral = 2;
        }
        else if (tagCentral.Contains("escudoespada"))
        {
            tagcentral = 3;
        }
        else
        {
            tagcentral = 4;
        }
        right.SetActive(true);
        select_.SetActive(true);
        left.SetActive(true);
        arrastarbotaocentral.transform.localScale = arrastarbotaocentral.scaleNormal;
        Vector3 fixedPosition = new Vector3(-11.83f, 0.6f, 0.5042808f);
        balaoInstanciado = null;

        foreach (GameObject guerreiro in guerreiros)
        {
            if (guerreiro.CompareTag(tagCentral))
            {
                foreach (GameObject jogador in GameObject.FindGameObjectsWithTag("Player"))
                {
                    NetworkObject jogadorrede = jogador.GetComponent<NetworkObject>();
                    if (jogadorrede.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                    {
                        if (IsClient && !IsHost && !IsServer)
                        {
                            if (jogador.transform.Find("warrior's father(Clone)"))
                            {
                                warriorfunctionobject = jogadorrede.transform.Find("warriorfunction(Clone)").gameObject;
                                GuerreiroSpawnData gsd = new GuerreiroSpawnData();
                                gsd.tagcentral = tagcentral;
                                gsd.clientId = jogadorrede.OwnerClientId;
                                gsd.Position = fixedPosition;
                                instanciadeguerreirosServerRpc(gsd);
                            }
                        }
                        if (IsServer)
                        {
                            jogadores = GameObject.FindGameObjectsWithTag("Player");
                            foreach (GameObject jogadorserver in jogadores)
                            {
                                if (jogadorserver.GetComponent<NetworkObject>().IsOwnedByServer)
                                {
                                    var canvas = GameObject.Find("Canvas");
                                    ataquebutton = canvas.transform.Find("Ataque(Clone)").gameObject;
                                }
                            }
                            guerreiroInstanciado = Instantiate(guerreiro, fixedPosition, Quaternion.identity, jogador.transform);
                            Array.Resize(ref guerreirosinstanciados, indicedosguerreirosnovos);
                            foreach (var indicevazio in guerreirosinstanciados)
                            {
                                if (indicevazio == null)
                                {
                                    possuiindicevazio = true;
                                    for (int contador = 0; contador < guerreirosinstanciados.Length; contador++)
                                    {
                                        if (guerreirosinstanciados[contador] == indicevazio)
                                        {
                                            guerreirosinstanciados[contador] = guerreiroInstanciado;
                                        }
                                    }
                                }
                            }
                            if (!possuiindicevazio)
                            {
                                indicedosguerreirosnovos++;
                                Array.Resize(ref guerreirosinstanciados, indicedosguerreirosnovos);
                                guerreirosinstanciados[indicedosguerreirosnovos - 1] = guerreiroInstanciado;
                            }
                            else
                            {
                                possuiindicevazio = false;
                            }
                            warriorfunctionobject = jogador.transform.Find("warriorfunction(Clone)").gameObject;
                            warriorfunctionobject.GetComponent<warrior_function>().guerreiros = guerreirosinstanciados;
                            focus = jogador.transform.Find("focofunction").gameObject;
                            instanciaguerreiro = guerreiroInstanciado.GetComponent<NetworkObject>();
                            avoID.Value = jogador.GetComponent<NetworkObject>().NetworkObjectId;
                            var canvasjoystick = GameObject.Find("Canvas").gameObject;
                            instanciaguerreiro.GetComponent<movimentar>().mover = canvasjoystick.transform.Find("Fixed Joystick").GetComponent<FixedJoystick>(); 
                            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(avoID.Value, out var networkObject))
                            {
                                var avo = networkObject;
                                instanciaguerreiro.SpawnWithOwnership(avo.OwnerClientId);
                            }
                            guerreiroID.Value = instanciaguerreiro.NetworkObjectId;
                            if (ataquebutton != null && instanciaguerreiro.CompareTag("guerreiroescudoespada"))
                            {
                                sowrdshieldfight scriptataqueespada = instanciaguerreiro.GetComponent<sowrdshieldfight>();
                                scriptataqueespada.ataquebutton = ataquebutton;
                            }
                            if (instanciaguerreiro != null && instanciaguerreiro.CompareTag("guerreiroescudoespada"))
                            {
                                escudoespadaluta = instanciaguerreiro.GetComponent<sowrdshieldfight>();
                                escudoespadaluta.ataquebutton = ataquebutton;
                            }
                            instanciaguerreiro.transform.SetParent(jogador.transform, false);
                            instanciaguerreiro.transform.position = fixedPosition;
                            if (instanciaguerreiro.CompareTag("guerreiroarqueiro") || instanciaguerreiro.CompareTag("guerreirosniper"))
                            {
                                foreach (mira_ativada mira0 in FindObjectsOfType<mira_ativada>(true))
                                {
                                    if (mira0.gameObject.name == "mira_0" && mira0.gameObject.hideFlags == HideFlags.None && mira0.gameObject.scene.IsValid())
                                    {
                                        mirainstance = mira0.gameObject;
                                    }
                                }
                                if (instanciaguerreiro.CompareTag("guerreiroarqueiro") || instanciaguerreiro.CompareTag("guerreirosniper"))
                                {
                                    mirainstance.GetComponent<movimentar>().mover = joystick.GetComponent<FixedJoystick>();
                                    observarmira = instanciaguerreiro.GetComponent<observarmira>();
                                    observarmira.mira = mirainstance;
                                }
                                if (mira != null)
                                {
                                    movimento = instanciaguerreiro.GetComponent<movimentar>();
                                }
                                guerreiroID.Value = instanciaguerreiro.NetworkObjectId;
                            }
                        }
                    }
                }
                break;
            }
        }

        foreach (GameObject balao in baloes)
        {
            if (balao.CompareTag(tagCentral))
            {
                if (IsClient && !IsHost)
                {
                    jogadores = GameObject.FindGameObjectsWithTag("Player");
                    foreach (var jogadorlocal in jogadores)
                    {
                        if (jogadorlocal.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                        {
                            if (jogadorlocal.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                            {
                                balaoInstanciado = Instantiate(balao, fixedPosition, Quaternion.identity, jogadorlocal.transform);
                            }
                        }
                    }
                    if (guerreiroesquerdo.balao[0] != null)
                    {
                        Array.Resize(ref guerreiroesquerdo.balao, guerreiroesquerdo.balao.Length + 1);
                        tamanhototal = guerreiroesquerdo.balao.Length - 1;
                    }
                    guerreiroesquerdo.balao[tamanhototal] = balaoInstanciado;
                    guerreiroesquerdo.balao_atualizar();
                    balaoInstanciado.transform.position = fixedPosition;
                    botoes_pergaminho.botaocentral.transform.position = botoes_pergaminho.originalscale;
                    botaodedesenho.SetActive(true);
                    pergaminho_enrolado.SetActive(false);
                    desenrolado.SetActive(false);
                    break;
                }
                if (IsServer)
                {
                    var jogadores = GameObject.FindGameObjectsWithTag("Player");
                    foreach (var jogador in jogadores)
                    {
                        if (jogador.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
                        {
                            balaoInstanciado = Instantiate(balao, fixedPosition, Quaternion.identity, jogador.transform);
                        }
                    }
                    if (guerreiroesquerdo.balao[0] != null)
                    {
                        Array.Resize(ref guerreiroesquerdo.balao, guerreiroesquerdo.balao.Length + 1);
                        tamanhototal = guerreiroesquerdo.balao.Length - 1;
                    }
                    guerreiroesquerdo.balao[tamanhototal] = balaoInstanciado;
                    guerreiroesquerdo.balao_atualizar();
                    avoID.Value = jogador.NetworkObjectId;
                    guerreiroID.Value = instanciaguerreiro.NetworkObjectId;
                    if (instanciaguerreiro.CompareTag("guerreirosniper") || instanciaguerreiro.CompareTag("guerreiroarqueiro"))
                    {
                        warriorfunctionobject = jogador.transform.Find("warriorfunction(Clone)").gameObject;
                        foreach (var camerapersonagem in Resources.FindObjectsOfTypeAll<GameObject>())
                        {
                            if (camerapersonagem.name == "character_camera" && camerapersonagem.hideFlags == HideFlags.None && camerapersonagem.scene.IsValid())
                            {
                                warrior_function.cinemachinecamera = camerapersonagem.GetComponent<CinemachineVirtualCamera>(); ;
                            }
                        }
                        observarmira.arrastarbotaocentral = arrastarbotaocentral;
                    }
                    break;
                }
            }
        }
        if (guerreiroInstanciado == null || balaoInstanciado == null)
        {
            Debug.LogWarning("Não foi possível encontrar guerreiro ou balão com a tag do botão central.");
        }
        else
        {
            // Desativar o pergaminho enrolado e desenrolado
            botoes_pergaminho.botaocentral.transform.position = botoes_pergaminho.originalscale;
            left.SetActive(true);
            right.SetActive(true);
            select_.SetActive(true);
            botaodedesenho.SetActive(true);
            pergaminho_enrolado.SetActive(false);
            desenrolado.SetActive(false);
            if (IsClient && !IsHost)
            {
                Balao balaoScript = balaoInstanciado.GetComponent<Balao>();
                if (balaoScript != null)
                {
                    if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(guerreiroInstanciadoID.Value, out var guerreiroInstanciadonetworkobject))
                        balaoScript.guerreiro = guerreiroInstanciadonetworkobject.transform;
                    balaoScript.diferencaInicial = new Vector3(-1.18f, 1.008f, 0);
                }
            }
            else
            {
                // Configurar o balão e guerreiro instanciado
                Balao balaoScript = balaoInstanciado.GetComponent<Balao>();
                if (balaoScript != null)
                {
                    balaoScript.guerreiro = guerreiroInstanciado.transform;
                    balaoScript.diferencaInicial = new Vector3(-1.18f, 1.008f, 0);
                }
                else
                {
                    Debug.LogWarning("O balão instanciado não possui o componente BalaoScript.");
                }
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void instanciadeguerreirosServerRpc(GuerreiroSpawnData gsd)
    {
        Vector3 posicaoguerreiro = gsd.Position;
        var clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { gsd.clientId }
            }
        };
        jogadores = GameObject.FindGameObjectsWithTag("Player");
        classes classesdeguerreiros = (classes)gsd.tagcentral;
        foreach (var jogadorlocal in jogadores)
        {
            var jogadorlocalnetwork = jogadorlocal.GetComponent<NetworkObject>();
            if (gsd.clientId == jogadorlocalnetwork.OwnerClientId)
            {
                switch (classesdeguerreiros)
                {
                    case classes.arqueiro:
                        guerreiroInstanciado = Instantiate(guerreiros[gsd.tagcentral], posicaoguerreiro, Quaternion.identity, jogadorlocal.transform);
                        break;
                    case classes.desarmado:
                        guerreiroInstanciado = Instantiate(guerreiros[gsd.tagcentral], posicaoguerreiro, Quaternion.identity, jogadorlocal.transform);
                        break;
                    case classes.sniper:
                        guerreiroInstanciado = Instantiate(guerreiros[gsd.tagcentral], posicaoguerreiro, Quaternion.identity, jogadorlocal.transform);
                        break;
                    case classes.escudoespada:
                        guerreiroInstanciado = Instantiate(guerreiros[gsd.tagcentral], posicaoguerreiro, Quaternion.identity, jogadorlocal.transform);
                        break;
                    case classes.assassino:
                        guerreiroInstanciado = Instantiate(guerreiros[gsd.tagcentral], posicaoguerreiro, Quaternion.identity, jogadorlocal.transform);
                        break;
                }
                focus = jogadorlocal.transform.Find("focofunction").gameObject;
                instanciaguerreiro = guerreiroInstanciado.GetComponent<NetworkObject>();
                instanciaguerreiro.SpawnWithOwnership(gsd.clientId);
                instanciaguerreiro.transform.SetParent(jogadorlocalnetwork.transform, false);
                instanciaguerreiro.ChangeOwnership(gsd.clientId);
                inseriratributoslocaisnosguerreirosClientRpc(instanciaguerreiro.NetworkObjectId, clientRpcParams);
            }
        }
        Debug.Log(instanciaguerreiro);
    }
    private IEnumerator aguardandorede()
    {
        while (!pronto || !enabled)
        {
            yield return null;
        }
        while (!NetworkManager.Singleton.IsListening)
            yield return null;
        NetworkObject meuPlayer = null;
        GameObject[] jogadores = GameObject.FindGameObjectsWithTag("Player");
        while (meuPlayer == null)
        {
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
        foreach (var selects in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (selects.name.Contains("Select Left"))
            {
                left = selects;
                guerreiroesquerdo = selects.GetComponent<qual_guerreiro>();
            }
            if (selects.name.Contains("Select Right"))
            {
                right = selects;
                guerreirodireito = selects.GetComponent<qual_guerreirodireito>();
            }
            if (selects.name.Contains("Select Warrior"))
            {
                select_ = selects;
            }
        }
        desenho = FindFirstObjectByType<botaodesenho>();
        warriorfunction = FindFirstObjectByType<warrior_function>();
        var canvas = GameObject.Find("Canvas");
        pergaminho_enrolado = canvas.transform.Find("pergaminho enrolado").gameObject;
        desenrolado = canvas.transform.Find("mascara para pergaminho desenrolado").gameObject;
        botaodedesenho = canvas.transform.Find("botaodesenho").gameObject;
        joystick = canvas.transform.Find("Fixed Joystick").gameObject;
        mira = Resources.Load<GameObject>("Prefabs/mira_0");
        left = canvas.transform.Find("Select Left").gameObject;
        right = canvas.transform.Find("Select Right").gameObject;
        select_ = canvas.transform.Find("Select Warrior").gameObject;
        warrior = canvas.transform.Find("warrior").gameObject;
        var jogadoreslocais = GameObject.FindGameObjectsWithTag("Player");
        foreach (var jogador in jogadoreslocais)
        {
            if (jogador.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                this.jogador = jogador.GetComponent<NetworkObject>();
            }
        }
    }
    [ClientRpc]
    private void inseriratributoslocaisnosguerreirosClientRpc(ulong idguerreiro, ClientRpcParams clientRpcParams = default)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(idguerreiro, out var guerreironetworkobject))
        {
            instanciaguerreiro = guerreironetworkobject;
        }
        var canvas = GameObject.Find("Canvas");
        ataquebutton = canvas.transform.Find("Ataque(Clone)").gameObject;
        /*if (ataquebutton != null && instanciaguerreiro.CompareTag("guerreiroescudoespada"))
        {
            sowrdshieldfight scriptataqueespada = instanciaguerreiro.GetComponent<sowrdshieldfight>();
            scriptataqueespada.ataquebutton = ataquebutton;
        }*/
        /*if (instanciaguerreiro != null && instanciaguerreiro.CompareTag("guerreiroescudoespada"))
        {
            escudoespadaluta = instanciaguerreiro.GetComponent<sowrdshieldfight>();
            escudoespadaluta.ataquebutton = ataquebutton;
        }*/
        var desenrolado = canvas.transform.Find("mascara para pergaminho desenrolado");
        var scrollbar = desenrolado.transform.Find("Scrollbar");
        var slidingarea = scrollbar.transform.Find("Sliding Area");
        var handle = slidingarea.transform.Find("Handle");
        var mascarabotoespergaminho = handle.transform.Find("mascara");
        foreach (var mira0 in FindObjectsOfType<mira_ativada>(true))
        {
            if (mira0.gameObject.name == "mira_0" && mira0.gameObject.hideFlags == HideFlags.None && mira0.gameObject.scene.IsValid())
            {
                mirainstance = mira0.gameObject;
            }
        }
        for (int contador = 0; contador < mascarabotoespergaminho.childCount; contador++)
        {
            if (instanciaguerreiro.tag == mascarabotoespergaminho.GetChild(contador).tag)
            {
                var guerreirobotao = mascarabotoespergaminho.transform.Find(mascarabotoespergaminho.GetChild(contador).gameObject.name);
                var scriptguerreirobotao = guerreirobotao.GetComponent<DragCentralButton>();
                observarmira.arrastarbotaocentral = scriptguerreirobotao;
                if (instanciaguerreiro.CompareTag("guerreiroarqueiro") || instanciaguerreiro.CompareTag("guerreirosniper"))
                {
                    mirainstance.GetComponent<movimentar>().mover = scriptguerreirobotao.joystick.GetComponent<FixedJoystick>();
                    scriptguerreirobotao.observarmira = instanciaguerreiro.GetComponent<observarmira>();
                    observarmira.mira = mira;
                }
            }
        }
        valoratribuidoainstanciaguerreiro?.Invoke();
    }
    private enum classes : int
    {
        arqueiro,
        desarmado,
        sniper,
        escudoespada,
        assassino
    }
    private void depoisdeinstanciaguerreiro()
    {
        FixedJoystick fixedJoystick = joystick.GetComponent<FixedJoystick>();
        if (fixedJoystick != null)
        {
            movimentar guerreiroScript = instanciaguerreiro.GetComponent<movimentar>();
            if (guerreiroScript != null)
            {
                guerreiroScript.mover = fixedJoystick;
                guerreiroScript.enabled = false;
                instanciaguerreiro.gameObject.SetActive(false);
            }
        }
        Array.Resize(ref guerreirosinstanciados, indicedosguerreirosnovos);
        foreach (var indicevazio in guerreirosinstanciados)
        {
            if (indicevazio == null)
            {
                possuiindicevazio = true;
                for (int contador = 0; contador < guerreirosinstanciados.Length; contador++)
                {
                    if (guerreirosinstanciados[contador] == indicevazio)
                    {
                        guerreirosinstanciados[contador] = instanciaguerreiro.gameObject;
                    }
                }
            }
        }
        if (!possuiindicevazio)
        {
            indicedosguerreirosnovos++;
            guerreirosinstanciados[indicedosguerreirosnovos] = instanciaguerreiro.gameObject;
        }
        else
        {
            possuiindicevazio = false;
        }
        warriorfunctionobject.GetComponent<warrior_function>().guerreiros = guerreirosinstanciados;
    }
}
