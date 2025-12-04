using Unity.Netcode;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ClienteProntoHandler : MonoBehaviour
{
    private bool jaDisparou = false;
    private bool jaDisparouWarriorFather = false;

    private IEnumerator Start()
    {
        // 🔹 Espera o NetworkManager existir
        while (NetworkManager.Singleton == null)
            yield return null;

        // 🔹 Espera o NetworkManager começar a escutar conexões
        while (!NetworkManager.Singleton.IsListening)
            yield return null;

        // 🔹 Espera ser cliente
        while (!NetworkManager.Singleton.IsClient)
            yield return null;

        // 🔹 Espera estar conectado
        while (!NetworkManager.Singleton.IsConnectedClient)
            yield return null;

        // --- Busca scripts IAposClientePronto que já existem na cena ---
        IAposClientePronto[] todosScripts = null;

        while (true)
        {
            todosScripts = FindObjectsOfType<MonoBehaviour>(true)
                .OfType<IAposClientePronto>()
                .Where(script =>
                {
                    var mono = script as MonoBehaviour;
                    if (mono == null) return false;
                    var netObj = mono.GetComponentInParent<NetworkObject>();
                    return netObj == null || netObj.IsOwner;
                })
                .ToArray();

            //Debug.Log($"[ClienteProntoHandler] Encontrados {todosScripts.Length} scripts IAposClientePronto (locais e do cliente)");

            if (todosScripts.Length > 0)
                break;

            yield return null;
        }

        // 🔹 Pequeno delay para garantir que tudo spawnou
        yield return new WaitForSeconds(0.1f);

        // --- Dispara ClientePronto para scripts existentes ---
        if (!jaDisparou)
        {
            jaDisparou = true;

            foreach (var script in todosScripts)
            {
                var mono = script as MonoBehaviour;
                if (mono == null) continue;

                var netObj = mono.GetComponentInParent<NetworkObject>();
                bool isNetworked = netObj != null;
                bool isOwner = isNetworked && netObj.IsOwner;
                string dono = isNetworked ? netObj.OwnerClientId.ToString() : "Sem NetworkObject";

                //Debug.Log($"[ClienteProntoHandler] >>> Executando ClientePronto em {script.GetType().Name}, GameObject: {mono.gameObject.name}, OwnerClientId: {dono}, IsOwner: {isOwner}");

                script.ClientePronto();
            }
        }

        // 🔹 Começa a esperar pelo warrior's father(Clone)
        StartCoroutine(AguardarWarriorFather());
    }

    private IEnumerator AguardarWarriorFather()
    {
        GameObject warriorFather = null;
        // 🔹 Espera até ser filho do jogador local
        GameObject jogadorLocal = null;
        while (jogadorLocal == null)
        {
            jogadorLocal = GameObject.FindGameObjectsWithTag("Player")
                .FirstOrDefault(obj =>
                {
                    var n = obj.GetComponent<NetworkObject>();
                    return n != null && n.IsOwner;
                });

            yield return null;
        }
        while (warriorFather == null)
        {
            warriorFather = jogadorLocal.transform.Find("warrior's father(Clone)")?.gameObject;
            yield return null;
        }
        var netObj = warriorFather.GetComponent<NetworkObject>();
        while (netObj == null || !netObj.IsSpawned)
            yield return null;

        // 🔹 Espera até que o warriorFather já seja filho do jogador local
        while (warriorFather.transform.parent != jogadorLocal.transform)
            yield return null;

        // 🔹 Dispara ClientePronto nos scripts filhos
        if (!jaDisparouWarriorFather)
        {
            jaDisparouWarriorFather = true;

            //Debug.Log("[ClienteProntoHandler] warrior's father(Clone) é filho do jogador local. Executando ClientePronto nos scripts filhos.");

            var scripts = warriorFather.GetComponentsInChildren<MonoBehaviour>(true)
                .OfType<IAposClientePronto>()
                .ToArray();

            foreach (var script in scripts)
            {
                var mono = script as MonoBehaviour;
                if (mono == null) continue;

                var netObjScript = mono.GetComponentInParent<NetworkObject>();
                bool isNetworked = netObjScript != null;
                bool isOwner = isNetworked && netObjScript.IsOwner;
                string dono = isNetworked ? netObjScript.OwnerClientId.ToString() : "Sem NetworkObject";

                //Debug.Log($"[ClienteProntoHandler] >>> Executando ClientePronto (warrior's father) em {script.GetType().Name}, GameObject: {mono.gameObject.name}, OwnerClientId: {dono}, IsOwner: {isOwner}");

                script.ClientePronto();
            }
        }
    }
}
