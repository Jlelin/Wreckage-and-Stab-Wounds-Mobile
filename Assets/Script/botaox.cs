using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class botaox : MonoBehaviour
{
    public int apertado_botao;
    public GameObject pergaminho_enrolado, desenrolado, left, right, select_, joystick, foco, warrior, desenho, canvas, camaracharacter;
    private RectTransform position;
    private Vector3 originalposition;
    public botaodesenho dedesenho;
    public static qual_guerreiro guerreiroesquerdo;
    private bool breaklaco;

    void Awake()
    {
        guerreiroesquerdo = left.GetComponent<qual_guerreiro>();
        dedesenho = FindFirstObjectByType<botaodesenho>();
        position = dedesenho.GetComponent<RectTransform>();
        originalposition = position.position;
    }

    void Update()
    {
        if (apertado_botao == 6)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    desenho.SetActive(true);
                    if (guerreiroesquerdo.balao != null)
                    {
                        if (guerreiroesquerdo.balao[0] != null)
                        {
                            left.SetActive(true);
                            right.SetActive(true);
                            select_.SetActive(true);
                            foreach (var balaovermelho in guerreiroesquerdo.balao_diferentesguerreiros)
                            {
                                foreach (var balaobranco in guerreiroesquerdo.balao)
                                {
                                    if (balaobranco.GetComponent<SpriteRenderer>().sprite == balaovermelho)
                                    {
                                        warrior.SetActive(true);
                                        breaklaco = true;
                                        break;
                                    }
                                }
                                if (breaklaco)
                                {
                                    breaklaco = false;
                                    break;
                                }
                            }
                        }
                    }
                    pergaminho_enrolado.SetActive(false);
                    desenrolado.SetActive(false);
                    apertado_botao = 0;
                    gameObject.SetActive(false);
                }
            }
        }
    }

    public void apertar_botao(int pressionado)
    {
        apertado_botao = pressionado;
    }
}
