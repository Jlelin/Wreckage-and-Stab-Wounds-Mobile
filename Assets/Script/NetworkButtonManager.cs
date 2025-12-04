using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;
using Unity.Netcode.Transports.UTP;

public class NetworkButtonManager : MonoBehaviour
{
    public Button hostButton, clientButton;
    public GameObject playerCharacterPrefab, canvas, canvasworldplayer;
    public static GameObject canvasHostClient; // Prefab do player que você vai instanciar
    public SpriteRenderer terreno;
    public Camera maincamera;
    public Transform cameratransform;

    void Awake()
    {
        canvasHostClient = canvas;
    }

    private void Start()
    {
        // Adiciona os ouvintes de clique para os botões
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
    }

    private void StartHost()
    {
        canvasworldplayer.SetActive(true);
        NetworkManager.Singleton.StartHost();
        canvas.GetComponent<Canvas>().enabled = false;
    }

    private void StartClient()
    {
        string adress = toget_ip.instance.colocarip();
        UnityTransport utp = NetworkManager.Singleton.GetComponent<UnityTransport>();
        utp.SetConnectionData(adress, 7777);
        canvasworldplayer.SetActive(true);
        NetworkManager.Singleton.StartClient();
        canvas.GetComponent<Canvas>().enabled = false;
    }
}
