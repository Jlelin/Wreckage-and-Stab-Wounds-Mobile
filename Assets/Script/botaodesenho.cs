using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class botaodesenho : MonoBehaviour, IAposClientePronto
{
    public int apertado_botao;
    public static GameObject paradestruir;
    public GameObject pergamnihoenrolado, papelaberto, joystick, warrior, focos, select, right, left, xzinho;
    public GameObject escudoespada, armafogo, desarmado, arqueiro, mascara, guerreironacena;
    private Image maskImage;
    private Image pergaminhoImage;
    public foco_function foco;
    public Canvas canvas;
    private bool pronto;
    public void ClientePronto()
    {
        pronto = true;
    }
    void Start()
    {
        RectTransform rectTransform = (RectTransform)transform;
        Vector2 posicao = rectTransform.anchoredPosition;
        posicao.x = 0;
        rectTransform.anchoredPosition = posicao;
        pergamnihoenrolado.SetActive(false);
        papelaberto.SetActive(false);
        maskImage = mascara.GetComponent<Image>();
        maskImage.enabled = false;
        Mask componentemascara = mascara.GetComponent<Mask>();
        componentemascara.enabled = false;
        pergaminhoImage = pergamnihoenrolado.GetComponent<Image>();
        pergaminhoImage.enabled = false;
    }

    void Update()
    {
        //if (!pronto) return;
        /*foreach (var guerreiros in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if ((guerreiros.name.Contains("arqueiro_0") || guerreiros.name.Contains("guerreiro_0") || guerreiros.name.Contains("guerreiro_armado_0") || guerreiros.name.Contains("guerreiro_escudo_0")) && guerreiros.hideFlags == HideFlags.None && guerreiros.scene.IsValid())
            {
                guerreironacena = guerreiros;
                break;
            }
        }
        if (guerreironacena == null)
        {
            warrior.SetActive(false);
        }*/
        if (apertado_botao == 5)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    foco.apertado_botao = 1;
                    foco.Update();
                    foco.apertado_botao = 0;
                    pergamnihoenrolado.SetActive(true);
                    papelaberto.SetActive(true);
                    xzinho.SetActive(true);
                    escudoespada.SetActive(true);
                    arqueiro.SetActive(true);
                    desarmado.SetActive(true);
                    armafogo.SetActive(true);
                    pergaminhoImage.enabled = true;
                    maskImage.enabled = true;
                    Mask componentemascara = mascara.GetComponent<Mask>();
                    componentemascara.enabled = true;
                    joystick.SetActive(false);
                    warrior.SetActive(false);
                    focos.SetActive(false);
                    select.SetActive(false);
                    right.SetActive(false);
                    left.SetActive(false);

                    apertado_botao = 0;
                    gameObject.SetActive(false);

                    if (paradestruir != null)
                    {
                        Destroy(paradestruir);
                    }
                }
            }
        }
    }

    public void apertar_botao(int pressionado)
    {
        apertado_botao = pressionado;
    }
}
