using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class botoes_pergaminho : MonoBehaviour
{
    public GameObject[] botoes_disputantes;
    public static GameObject botaocentral;
    public GameObject posicionador;
    private GameObject botaomaisproximo_abaixo, emandamento;
    public static Vector3 originalscale;
    private int apertado_botao_cima, apertado_botao_baixo;
    private static int cimafoiapertado, baixofoiapertado;
    private bool podedesligar;
    private static bool cimaprimeiro;

    void Start()
    {
        botaocentral = botoes_disputantes[0];
        originalscale = posicionador.transform.position;
        cimaprimeiro = false;
    }

    void Update()
    {
        originalscale = posicionador.transform.position;
        if (apertado_botao_cima == 1)
        {
            podedesligar = true;
            VerificarToquecima();
            cimafoiapertado++;
            if (baixofoiapertado == 0 && cimafoiapertado > 0)
            {
                cimaprimeiro = true;
            }
        }
        if (apertado_botao_baixo == 1)
        {
            VerificarToquebaixo();
            baixofoiapertado++;
        }
        foreach (var botao in botoes_disputantes)
        {
            if (botao == botaocentral)
            {
                instanciarguerreiros.arrastarbotaocentral = botao.GetComponent<DragCentralButton>();
            }
        }
        if (podedesligar)
        {
            podedesligar = false;
            foreach (var botao in botoes_disputantes)
            {
                if (botao == botaocentral)
                {
                    instanciarguerreiros.arrastarbotaocentral = botao.GetComponent<DragCentralButton>();
                }
                else
                {
                    botao.SetActive(false);
                }
            }
        }
    }

    public void apertar_botao_cima(int pressionado)
    {
        apertado_botao_cima = pressionado;
    }

    public void apertar_botao_baixo(int pressionado)
    {
        apertado_botao_baixo = pressionado;
    }

    void VerificarToquecima()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                foreach (var botao in botoes_disputantes)
                {
                    botao.SetActive(true);
                }
                TrocarBotoes_cima();
                podedesligar = true;
                botaocentral.transform.Find("Image").GetComponent<Image>().enabled = true;
            }
        }
    }

    void VerificarToquebaixo()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                foreach (var botao in botoes_disputantes)
                {
                    botao.SetActive(true);
                }
                TrocarBotoes_baixo();
                podedesligar = true;
                botaocentral.transform.Find("Image").GetComponent<Image>().enabled = true;
            }
        }
    }
    void TrocarBotoes_cima()
    {
        GameObject botaomaisproximo = EncontrarBotaoMaisProximo_baixo(botaocentral);
        GameObject botao_abaixo = botaocentral;
        TrocarPosicoes_combotaocentral(botaocentral, botaomaisproximo);
        do
        {
            botaomaisproximo = EncontrarBotaoMaisProximo_baixo(botao_abaixo);
            if (botaomaisproximo)
            {
                TrocarPosicoes(botao_abaixo, botaomaisproximo);
            }
        }
        while (botaomaisproximo);
        botaomaisproximo = EncontrarBotaoMaisProximo_cima(botaocentral);
        TrocarPosicoes(botao_abaixo, botaomaisproximo);
    }

    void TrocarBotoes_baixo()
    {
        GameObject botaoMaisProximo = EncontrarBotaoMaisProximo_cima(botaocentral);
        TrocarPosicoes_combotaocentral(botaocentral, botaoMaisProximo);
        botaomaisproximo_abaixo = EncontrarBotaoMaisProximo_cima(botaocentral);
        botaoMaisProximo = EncontrarBotaoMaisProximo_baixo(botaocentral);
        TrocarPosicoes(botaoMaisProximo, botaomaisproximo_abaixo);
        botaomaisproximo_abaixo = EncontrarBotaoMaisProximo_baixo(botaomaisproximo_abaixo);
        //TrocarPosicoes(botaoMaisProximo, botaomaisproximo_abaixo);
        emandamento = botaomaisproximo_abaixo;
        while (true)
        {
            botaomaisproximo_abaixo = EncontrarBotaoMaisProximo_baixo(emandamento);
            if (!botaomaisproximo_abaixo)
            {
                break;
            }
            else
            {
                botaoMaisProximo = EncontrarBotaoMaisProximo_baixo(botaomaisproximo_abaixo);
                if (botaoMaisProximo)
                {
                    TrocarPosicoes(botaoMaisProximo, botaomaisproximo_abaixo);
                    emandamento = botaoMaisProximo;
                }
                else
                {
                    TrocarPosicoes(botaomaisproximo_abaixo, emandamento);
                    break;
                }
            }
        }
        botaoMaisProximo = EncontrarBotaoMaisProximo_cima(botaocentral);
        TrocarPosicoes(botaoMaisProximo, botaomaisproximo_abaixo);
    }

    GameObject EncontrarBotaoMaisProximo_cima(GameObject botaoAtual)
    {
        GameObject botaoMaisProximo = null;
        float menorDistancia = Mathf.Infinity;
        Vector3 posicaoAtual = botaoAtual.transform.position;

        foreach (GameObject botao in botoes_disputantes)
        {
            if (botao != botaoAtual)
            {
                float distancia = botao.transform.position.y - posicaoAtual.y;
                if (distancia > 0 && distancia < menorDistancia)
                {
                    menorDistancia = distancia;
                    botaoMaisProximo = botao;
                }
            }
        }

        return botaoMaisProximo;
    }

    public GameObject EncontrarBotaoMaisProximo_baixo(GameObject botao_acima)
    {
        GameObject botao_abaixo = null;
        float menor_distancia = Mathf.NegativeInfinity;
        Vector3 posicao_botao_decima = botao_acima.transform.position;
        foreach (GameObject botao in botoes_disputantes)
        {
            if (botao != botao_acima)
            {
                float distancia = botao.transform.position.y - posicao_botao_decima.y;
                if (distancia < 0 && distancia > menor_distancia)
                {
                    menor_distancia = distancia;
                    botao_abaixo = botao;
                }
            }
        }
        return botao_abaixo;
    }

    IEnumerator botaocentralanimacao(Vector3 posicaoAlvo, float duracao)
    {
        Vector3 posicaoInicial = botaocentral.transform.position;
        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracao)
        {
            tempoDecorrido += Time.deltaTime;
            float t = Mathf.Clamp01(tempoDecorrido / duracao); // Garante que t esteja entre 0 e 1

            // Interpolação linear entre a posição inicial e a posição alvo
            botaocentral.transform.position = Vector3.Lerp(posicaoInicial, posicaoAlvo, t);

            yield return null; // Pausa até o próximo quadro
        }

        // Garante que o botão esteja exatamente na posição alvo no final da animação
        botaocentral.transform.position = posicaoAlvo;
    }

    void TrocarPosicoes_combotaocentral(GameObject botao1, GameObject botao2)
    {
        Vector3 posicaoCentral = botao1.transform.position;
        Vector3 posicaobotao2 = botao2.transform.position;
        botaocentral = botao2;
        StartCoroutine(botaocentralanimacao(originalscale, 0.0625f)); // Duração de 0.5 segundos para a animação
        botao1.transform.position = posicaobotao2;
        botao2.transform.position = posicaoCentral;
    }

    GameObject TrocarPosicoes(GameObject botao1, GameObject botao2)
    {

        Vector3 tempPos = botao1.transform.position;
        botao1.transform.position = botao2.transform.position;
        botao2.transform.position = tempPos;
        return botao2;

    }
}
