using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class atirar : MonoBehaviour
{
    private Coroutine executando_ounao;
    public gunbowatack gunbowatack;
    public GameObject atirarr, ataque, proibidoatirar;
    public static life life;

    // Start is called before the first frame update
    void Start()
    {
        gunbowatack = atirarr.GetComponent<gunbowatack>();

        // Inscreva-se no evento para iniciar a corrotina no tempo exato
        gunbowatack.OnCorrotinaIniciaTempo += ExecutarCorrotinaNoPonto;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // Método chamado pelo evento
    private void ExecutarCorrotinaNoPonto()
    {
        executando_ounao = StartCoroutine(CorrotinaNoPonto());
    }

    private IEnumerator CorrotinaNoPonto()
    {
        yield return new WaitForSeconds(gunbowatack.tempoderecuperacao);
        proibidoatirar.SetActive(false);
        gunbowatack.GetComponent<UnityEngine.UI.Button>().enabled = true;
        gunbowatack.GetComponent<UnityEngine.UI.Image>().enabled = true;
        gunbowatack.canAttack = true;
    }
}
