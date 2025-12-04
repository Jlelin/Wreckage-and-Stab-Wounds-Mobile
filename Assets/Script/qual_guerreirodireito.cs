using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;


public class qual_guerreirodireito : MonoBehaviour
{
    public qual_guerreiro guerreiroesquerdo;
    public GameObject[] balao_dguerreiros, meus_baloes;
    private GameObject warrior;
    public SpriteRenderer balao_branco_elementofinal, elemento_diferente;
    public Sprite white_balao, balaobrancoarmado, balaobrancodesarmado, balaoarqueiro, balaoassassino;
    public Sprite[] balao_vermelho;
    public int ultimo_elemento, apertado_botao, clicado_primeiro, guardarbaloes, quantoscliks;
    private int meusbaloesdefasado;
    public bool click_x1;
    private bool breaklaco;
    void Awake()
    {
        var canvas = GameObject.Find("Canvas");
        warrior = canvas.transform.Find("warrior").gameObject;
        foreach (var selectleft in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (selectleft.name.Contains("Select Left"))
            {
                guerreiroesquerdo = selectleft.GetComponent<qual_guerreiro>();
            }
        }
        StartCoroutine(aguardandorede());
    }
    // Start is called before the first frame update
    void Start()
    {
        ultimo_elemento = meus_baloes.Length - 1;
        click_x1 = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (guerreiroesquerdo.gameObject.activeSelf)
        {
            if (guerreiroesquerdo.apertado_botao == 3 && guerreiroesquerdo.botaodireitoclicado == false)
            {
                quantoscliks = 0;
                if (guerreiroesquerdo.balao_selecionado < meus_baloes.Length - 1)
                {
                    ultimo_elemento = guerreiroesquerdo.balao_selecionado + 1;
                }
            }
        }
        if (apertado_botao == 4)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    warrior.SetActive(true);
                    meusbaloesdefasado = meus_baloes.Length;
                    quantoscliks++;
                    clicado_primeiro++;
                    if (clicado_primeiro > guerreiroesquerdo.quantoscliks && click_x1 == true)
                    {
                        guerreiroesquerdo.balao_selecionado = ultimo_elemento;
                        click_x1 = false;
                        balao_branco_elementofinal = meus_baloes[ultimo_elemento].GetComponent<SpriteRenderer>();
                        for (int baloes = 0; baloes < balao_dguerreiros.Length; baloes++)
                        {
                            if (meus_baloes[ultimo_elemento].CompareTag("guerreiroescudoespada") && balao_dguerreiros[baloes].CompareTag("guerreiroescudoespada"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                            if (meus_baloes[ultimo_elemento].CompareTag("guerreirosniper") && balao_dguerreiros[baloes].CompareTag("guerreirosniper"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                            if (meus_baloes[ultimo_elemento].CompareTag("guerreirodesarmado") && balao_dguerreiros[baloes].CompareTag("guerreirodesarmado"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                            if (meus_baloes[ultimo_elemento].CompareTag("guerreiroarqueiro") && balao_dguerreiros[baloes].CompareTag("guerreiroarqueiro"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                            if (meus_baloes[ultimo_elemento].CompareTag("assassino") && balao_dguerreiros[baloes].CompareTag("assassino"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                        }
                    }
                    else
                    {
                        if (ultimo_elemento == meus_baloes.Length - 1 && ultimo_elemento + 1 < meus_baloes.Length)
                        {
                            ultimo_elemento = guerreiroesquerdo.balao_selecionado;
                        }
                        if (ultimo_elemento + 1 < meus_baloes.Length && ultimo_elemento != guerreiroesquerdo.balao_selecionado + 1)
                        {
                            ultimo_elemento++;
                        }
                        if (quantoscliks == 2 && ultimo_elemento + 1 < meus_baloes.Length)
                        {
                            quantoscliks = 1;
                            ultimo_elemento++;
                        }
                        foreach (var balaovermelho in balao_vermelho)
                        {
                            if (meus_baloes[ultimo_elemento].GetComponent<SpriteRenderer>().sprite == balaovermelho)
                            {
                                if (meusbaloesdefasado >= 3)
                                {
                                    if (ultimo_elemento == meus_baloes.Length - 2 && ultimo_elemento + 1 < meus_baloes.Length)
                                    {
                                        ultimo_elemento++;
                                        break;
                                    }
                                }
                                for (int n = 0; n < meus_baloes.Length; n++)
                                {
                                    if (ultimo_elemento == meus_baloes.Length - n && ultimo_elemento + 1 < meus_baloes.Length)
                                    {
                                        ultimo_elemento++;
                                        guerreiroesquerdo.balao_selecionado = ultimo_elemento - 1;
                                        breaklaco = true;
                                        break;
                                    }
                                }
                                if (breaklaco)
                                {
                                    breaklaco = false;
                                    break;
                                }
                                if (ultimo_elemento + 1 == meus_baloes.Length)
                                {
                                    ultimo_elemento = 0;
                                    guerreiroesquerdo.balao_selecionado = guerreiroesquerdo.balao.Length - 1;
                                    break;
                                }
                            }
                        }
                        click_x1 = false;
                        balao_branco_elementofinal = meus_baloes[ultimo_elemento].GetComponent<SpriteRenderer>();
                        for (int baloes = 0; baloes < balao_dguerreiros.Length; baloes++)
                        {
                            if (meus_baloes[ultimo_elemento].CompareTag("guerreiroescudoespada") && balao_dguerreiros[baloes].CompareTag("guerreiroescudoespada"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                            if (meus_baloes[ultimo_elemento].CompareTag("guerreirosniper") && balao_dguerreiros[baloes].CompareTag("guerreirosniper"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                            if (meus_baloes[ultimo_elemento].CompareTag("guerreirodesarmado") && balao_dguerreiros[baloes].CompareTag("guerreirodesarmado"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                            if (meus_baloes[ultimo_elemento].CompareTag("guerreiroarqueiro") && balao_dguerreiros[baloes].CompareTag("guerreiroarqueiro"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                            if (meus_baloes[ultimo_elemento].CompareTag("assassino") && balao_dguerreiros[baloes].CompareTag("assassino"))
                            {
                                balao_branco_elementofinal.sprite = balao_vermelho[baloes];
                                guardarbaloes = ultimo_elemento;
                            }
                        }
                    }
                    for (int o = 0; o < meus_baloes.Length; o++)
                    {
                        elemento_diferente = meus_baloes[o].GetComponent<SpriteRenderer>();
                        if (guardarbaloes != o)
                        {
                            if (elemento_diferente.sprite != white_balao && elemento_diferente.sprite != balaobrancoarmado &&
                            elemento_diferente.sprite != balaobrancodesarmado && elemento_diferente.sprite != balaoarqueiro)
                            {
                                if (meus_baloes[o].CompareTag("guerreiroescudoespada"))
                                {
                                    elemento_diferente.sprite = white_balao;
                                }
                                if (meus_baloes[o].CompareTag("guerreirosniper"))
                                {
                                    elemento_diferente.sprite = balaobrancoarmado;
                                }
                                if (meus_baloes[o].CompareTag("guerreirodesarmado"))
                                {
                                    elemento_diferente.sprite = balaobrancodesarmado;
                                }
                                if (meus_baloes[o].CompareTag("guerreiroarqueiro"))
                                {
                                    elemento_diferente.sprite = balaoarqueiro;
                                }
                                if (meus_baloes[o].CompareTag("assassino"))
                                {
                                    elemento_diferente.sprite = balaoassassino;
                                }
                            }
                        }

                    }
                }
            }
        }
    }
    public void balao_atualizado(GameObject[] balao)
    {
        meus_baloes = balao;
    }

    public void apertar_botao_direito(int pressionado)
    {
        apertado_botao = pressionado;
    }
    private IEnumerator aguardandorede()
    {

        var jogadores = GameObject.FindGameObjectsWithTag("Player");
        while (jogadores == null)
        {
            jogadores = GameObject.FindGameObjectsWithTag("Player");
            yield return null;
        }
        foreach (GameObject jogadorlocal in jogadores)
        {
            if (jogadorlocal.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId)
            {
                var tamanho = 1;
                for (int contador = 0; contador < jogadorlocal.transform.childCount; contador++)
                {
                    if (jogadorlocal.transform.GetChild(contador).name.Contains("tag"))
                    {
                        balao_dguerreiros[tamanho - 1] = jogadorlocal.transform.GetChild(contador).gameObject;
                        tamanho++;
                    }
                }
            }
        }
    }
}