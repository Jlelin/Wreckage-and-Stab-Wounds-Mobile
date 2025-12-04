using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.Netcode;
using System;

public class qual_guerreiro : MonoBehaviour
{
    public qual_guerreirodireito guerreirodireito;
    public GameObject[] balao, balao_diferentesguerreiros_vetor;
    private GameObject warrior;
    public SpriteRenderer receberbalao_vermelho, quero_balaobranco;
    public Sprite balao_branco, balaobrancoarmado, balaobrancodesarmado, balaoarqueiro, balaoassassino;
    public Sprite[] balao_diferentesguerreiros;
    public int apertado_botao, balao_selecionado, guardarbaloes, umclickdireito, quantoscliks, contarclick;
    public bool um_click, botaodireitoclicado;
    private bool breaklaco;

    void Awake()
    {
        var canvas = GameObject.Find("Canvas");
        warrior = canvas.transform.Find("warrior").gameObject;
        guerreirodireito = FindFirstObjectByType<qual_guerreirodireito>();
        StartCoroutine(aguardandorede());
        balao_atualizar();

    }

    // Start is called before the first frame update
    void Start()
    {
        um_click = false;
        balao_selecionado = 0;
        botaodireitoclicado = true;
        warrior_function.tamanho_vetor = balao.Length - 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (guerreirodireito.apertado_botao == 4 && guerreirodireito.click_x1 == false)
        {
            if (guerreirodireito.ultimo_elemento != 0)
            {
                balao_selecionado = guerreirodireito.ultimo_elemento - 1;
            }
            contarclick = 0;
        }
        if (apertado_botao == 3 && balao_selecionado < balao.Length)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    warrior.SetActive(true);
                    contarclick++;
                    quantoscliks++;
                    if (quantoscliks > guerreirodireito.clicado_primeiro && botaodireitoclicado == true)
                    {
                        guerreirodireito.ultimo_elemento = balao_selecionado;
                        botaodireitoclicado = false;
                        receberbalao_vermelho = balao[balao_selecionado].GetComponent<SpriteRenderer>();
                        for (int baloes = 0; baloes < balao_diferentesguerreiros.Length; baloes++)
                        {
                            if (balao[balao_selecionado].CompareTag("guerreiroescudoespada") && balao_diferentesguerreiros_vetor[baloes].CompareTag("guerreiroescudoespada"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                            if (balao[balao_selecionado].CompareTag("guerreirosniper") && balao_diferentesguerreiros_vetor[baloes].CompareTag("guerreirosniper"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                            if (balao[balao_selecionado].CompareTag("guerreirodesarmado") && balao_diferentesguerreiros_vetor[baloes].CompareTag("guerreirodesarmado"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                            if (balao[balao_selecionado].CompareTag("guerreiroarqueiro") && balao_diferentesguerreiros_vetor[baloes].CompareTag("guerreiroarqueiro"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                            if (balao[balao_selecionado].CompareTag("assassino") && balao_diferentesguerreiros_vetor[baloes].CompareTag("assassino"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                        }
                    }
                    else
                    {
                        if (balao_selecionado - 1 >= 0 && balao_selecionado != guerreirodireito.ultimo_elemento - 1)
                        {
                            balao_selecionado--;
                        }
                        if (contarclick == 2 && balao_selecionado - 1 >= 0)
                        {
                            contarclick = 1;
                            balao_selecionado--;
                        }
                        foreach (var balaovermelho in balao_diferentesguerreiros)
                        {
                            if (balao[balao_selecionado].GetComponent<SpriteRenderer>().sprite == balaovermelho)
                            {
                                if (balao_selecionado == balao.Length - 1)
                                {
                                    balao_selecionado--;
                                    guerreirodireito.ultimo_elemento = balao_selecionado + 1;
                                    break;
                                }
                                for (int n = balao.Length - 2; n > 0; n--)
                                {
                                    if (balao_selecionado == n)
                                    {
                                        balao_selecionado--;
                                        guerreirodireito.ultimo_elemento = balao_selecionado + 1;
                                        breaklaco = true;
                                        break;
                                    }
                                }
                                if (breaklaco)
                                {
                                    breaklaco = false;
                                    break;
                                }
                                if (balao_selecionado == 0)
                                {
                                    balao_selecionado = balao.Length - 1;
                                    break;
                                }
                            }
                        }
                        botaodireitoclicado = false;
                        receberbalao_vermelho = balao[balao_selecionado].GetComponent<SpriteRenderer>();
                        for (int baloes = 0; baloes < balao_diferentesguerreiros.Length; baloes++)
                        {
                            if (balao[balao_selecionado].CompareTag("guerreiroescudoespada") && balao_diferentesguerreiros_vetor[baloes].CompareTag("guerreiroescudoespada"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                            if (balao[balao_selecionado].CompareTag("guerreirosniper") && balao_diferentesguerreiros_vetor[baloes].CompareTag("guerreirosniper"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                            if (balao[balao_selecionado].CompareTag("guerreirodesarmado") && balao_diferentesguerreiros_vetor[baloes].CompareTag("guerreirodesarmado"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                            if (balao[balao_selecionado].CompareTag("guerreiroarqueiro") && balao_diferentesguerreiros_vetor[baloes].CompareTag("guerreiroarqueiro"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                            if (balao[balao_selecionado].CompareTag("assassino") && balao_diferentesguerreiros_vetor[baloes].CompareTag("assassino"))
                            {
                                receberbalao_vermelho.sprite = balao_diferentesguerreiros[baloes];
                                guardarbaloes = balao_selecionado;
                            }
                        }
                    }
                    for (int o = 0; o < balao.Length; o++)
                    {
                        if (balao[o] != null)
                        {
                            quero_balaobranco = balao[o].GetComponent<SpriteRenderer>();
                            if (guardarbaloes != o)
                            {
                                if (quero_balaobranco.sprite != balao_branco && quero_balaobranco.sprite != balaobrancoarmado &&
                                quero_balaobranco.sprite != balaobrancodesarmado && quero_balaobranco.sprite != balaoarqueiro)
                                {
                                    if (balao[o].CompareTag("guerreiroescudoespada"))
                                    {
                                        quero_balaobranco.sprite = balao_branco;
                                    }
                                    if (balao[o].CompareTag("guerreirosniper"))
                                    {
                                        quero_balaobranco.sprite = balaobrancoarmado;
                                    }
                                    if (balao[o].CompareTag("guerreirodesarmado"))
                                    {
                                        quero_balaobranco.sprite = balaobrancodesarmado;
                                    }
                                    if (balao[o].CompareTag("guerreiroarqueiro"))
                                    {
                                        quero_balaobranco.sprite = balaoarqueiro;
                                    }
                                    if (balao[o].CompareTag("assassino"))
                                    {
                                        quero_balaobranco.sprite = balaoassassino;
                                    }
                                }
                            }
                        }
                        else
                        {

                        }

                    }
                }
            }
        }
    }
    public void atualizar_clickdireito(int click_direito)
    {
        umclickdireito = click_direito;
    }

    public void balao_atualizar()
    {
        guerreirodireito.balao_atualizado(balao);
    }

    public void apertar_botao(int pressionado)
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
                        balao_diferentesguerreiros_vetor[tamanho - 1] = jogadorlocal.transform.GetChild(contador).gameObject;
                        tamanho++;
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        botaox.guerreiroesquerdo = this;      
    }
}
