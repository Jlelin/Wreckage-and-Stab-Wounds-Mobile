using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class canvascomponent : MonoBehaviour, IAposClientePronto
{
    public delegate void assinaturapararodarmetodofocofunctioninicial();
    public event assinaturapararodarmetodofocofunctioninicial valoresatribuidosafocofunction;
    public delegate void assinaturaparaatribuirvalordescriptwarrior();
    public event assinaturaparaatribuirvalordescriptwarrior scriptwarriortemvalor;
    public GameObject ataque, warriorfunctionobject;
    private GameObject instanciaataque;
    private static bool bastaumavez, ataqueficafalseumavez, warriorfunctiontemvalor;
    private bool pronto;
    public void ClientePronto()
    {
        pronto = true;
    }
    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(aguardandorede());
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator aguardandorede()
    {
        while (!NetworkManager.Singleton.IsListening)
            yield return null;
        NetworkObject meuPlayer = null;
        while (!pronto)
        {
            yield return null;
        }
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
        GameObject canvas = null;        
        foreach (GameObject jogador in jogadores)
        {
            if (jogador.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                canvas = GameObject.Find("Canvas").gameObject;
                warriorfunctionobject = jogador.transform.Find("warriorfunction(Clone)")?.gameObject;
                while (warriorfunctionobject == null)
                {
                    warriorfunctionobject = jogador.transform.Find("warriorfunction(Clone)")?.gameObject;
                    yield return null;
                }
            }
        }
        instanciaataque = Instantiate(ataque, transform);
        var warrior = canvas.transform.Find("warrior");
        RectTransform warriorRectTransform = warrior.GetComponent<RectTransform>();
        RectTransform ataqueRectTransform = instanciaataque.GetComponent<RectTransform>();
        ataqueRectTransform.anchoredPosition = warriorRectTransform.anchoredPosition;
        ataqueRectTransform.sizeDelta = warriorRectTransform.sizeDelta;
        instanciaataque.transform.localScale = warrior.localScale;
        bastaumavez = true;
        GameObject[] jogadoreslocais = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject jogadorlocal in jogadoreslocais)
        {
            if (jogadorlocal.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                instanciaataque = canvas.transform.Find("Ataque(Clone)").gameObject;
                var warriorfunction = warriorfunctionobject.GetComponent<warrior_function>();
                warriorfunction.ataque = instanciaataque;
                var macasradesenrolado = transform.Find("mascara para pergaminho desenrolado");
                var scrollbar = macasradesenrolado.transform.Find("Scrollbar");
                var slidingarea = scrollbar.transform.Find("Sliding Area");
                var handle = slidingarea.transform.Find("Handle");
                var mascarabotoesdesenrolado = handle.transform.Find("mascara");
                for (int contador = 0; contador < mascarabotoesdesenrolado.childCount; contador++)
                {
                    if (mascarabotoesdesenrolado.GetChild(contador).name.Contains("guerreiro"))
                    {
                        var dragcentralbutton = mascarabotoesdesenrolado.GetChild(contador).GetComponent<DragCentralButton>();
                    }
                }
                var focofunctionobject = jogadorlocal.transform.Find("focofunction");
                var focofunctionscript = focofunctionobject.GetComponent<foco_function>();
                focofunctionscript.ataque = instanciaataque;
                var gunbowscript = instanciaataque.GetComponent<gunbow>();
                gunbow.scriptwarrior = warrior.gameObject;
                if (!ataqueficafalseumavez)
                {
                    instanciaataque.SetActive(false);
                    ataqueficafalseumavez = true;
                }
                var atirar = transform.Find("atirar").gameObject;
                focofunctionscript.atirar = atirar;
                var left = transform.Find("Select Left").gameObject;
                focofunctionscript.esquerdo = left;
                var right = transform.Find("Select Right").gameObject;
                focofunctionscript.direito = right;
                var selectwarrior = transform.Find("Select Warrior").gameObject;
                focofunctionscript.selectwarrior = selectwarrior;
                var foco = canvas.transform.Find("foco").gameObject;
                if (!warriorfunctiontemvalor)
                {
                    scriptwarriortemvalor += gunbowscript.receberscriptwarrior;
                    scriptwarriortemvalor?.Invoke();
                    warriorfunctiontemvalor = true;
                }
            }
        }
    }
}
