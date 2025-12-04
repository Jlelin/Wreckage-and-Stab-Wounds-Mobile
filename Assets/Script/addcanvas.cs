using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class addcanvas : MonoBehaviour, IAposClientePronto
{
    public delegate void assinaturapararodarmetodofocofunctioninicial();
    public event assinaturapararodarmetodofocofunctioninicial valoresatribuidosafocofunction;
    public foco_function focofunction;
    public GameObject focofunctionobject;
    public static GameObject instanciacanvas;
    private GameObject[] jogadores;
    private bool pronto;
    // Start is called before the first frame update

    public void ClientePronto()
    {
        pronto = true;
    }
    void Awake()
    {
        focofunction = focofunctionobject.GetComponent<foco_function>();
    }
    void Start()
    {
        StartCoroutine(aguardandorede());
    }

    // Update is called once per frame
    void Update()
    {

    }
    private IEnumerator aguardandorede()
    {
        while (!NetworkManager.Singleton.IsListening)
            yield return null;
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
        if (!pronto) yield return null;
        jogadores = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject jogadorlocal in jogadores)
        {
            if (jogadorlocal.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                    foreach (GameObject canvasworld in Resources.FindObjectsOfTypeAll<GameObject>())
                    {
                        if (canvasworld.name == "Canvas" && canvasworld.hideFlags == HideFlags.None && canvasworld.scene.IsValid())
                        {
                            instanciacanvas = canvasworld;
                            var botaodesenho = instanciacanvas.transform.Find("botaodesenho").gameObject;
                            var botaodesenhoscript = botaodesenho.GetComponent<botaodesenho>();
                            var focofunctionObject = jogadorlocal.transform.Find("focofunction");
                            var focofunctionscript = focofunctionObject.GetComponent<foco_function>();
                            focofunctionscript.botaodesenho = botaodesenho;
                            var warrior = jogadorlocal.transform.Find("warriorfunction(Clone)")?.gameObject;
                            while(warrior == null)
                            {
                                warrior = jogadorlocal.transform.Find("warriorfunction(Clone)")?.gameObject;
                                yield return null;
                            }
                            focofunctionscript.warriorobject = warrior;
                            var left = instanciacanvas.transform.Find("Select Left").gameObject;
                            var guerreiroesquerdo = left.GetComponent<qual_guerreiro>();
                            focofunctionscript.guerreiroesquerdo = guerreiroesquerdo;
                            botaodesenhoscript.foco = focofunctionscript;
                            valoresatribuidosafocofunction?.Invoke();
                            break;
                        }
                    }
            }
        }
    }
}
