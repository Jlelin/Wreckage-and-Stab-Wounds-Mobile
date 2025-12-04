using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class instanciarwarriorsfather : NetworkBehaviour, IAposClientePronto
{
    [SerializeField] private GameObject warriorfunc, warriorfather;
    private GameObject warriorfuncins, warriorfatherins;
    private bool pronto;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        DragCentralButton.OnPronto += AtivarInstanciarGuerreiros;
    }
    private void AtivarInstanciarGuerreiros()
    {
        StartCoroutine(aguardandowarriorfather());
    }
    public void ClientePronto()
    {
        pronto = true;
    }
    private void OnEnable()
    {
        StartCoroutine(aguardandorede());
    }
    private IEnumerator aguardandorede()
    {
        while (!pronto)
        {
            yield return null;
        }
        if (IsServer)
        {
            warriorfuncins = Instantiate(warriorfunc);
            warriorfuncins.GetComponent<NetworkObject>().SpawnWithOwnership(transform.parent.GetComponent<NetworkObject>().OwnerClientId);
            warriorfuncins.GetComponent<NetworkObject>().TrySetParent(transform.parent.GetComponent<NetworkObject>());
            warriorfatherins = Instantiate(warriorfather);
            warriorfatherins.GetComponent<NetworkObject>().SpawnWithOwnership(transform.parent.GetComponent<NetworkObject>().OwnerClientId);
            warriorfatherins.GetComponent<NetworkObject>().TrySetParent(transform.parent.GetComponent<NetworkObject>());
        }
        else
        {
            spawneparentescoServerRpc(transform.parent.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void spawneparentescoServerRpc(ulong clientid)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(clientid, out var clientidnetworkobject))
        {
                warriorfuncins = Instantiate(warriorfunc, clientidnetworkobject.transform);
                warriorfuncins.GetComponent<NetworkObject>().SpawnWithOwnership(clientidnetworkobject.OwnerClientId);
                warriorfuncins.GetComponent<NetworkObject>().TrySetParent(clientidnetworkobject, false);
                warriorfatherins = Instantiate(warriorfather, clientidnetworkobject.transform);
                warriorfatherins.GetComponent<NetworkObject>().SpawnWithOwnership(clientidnetworkobject.OwnerClientId);
                warriorfatherins.GetComponent<NetworkObject>().TrySetParent(clientidnetworkobject, false);
        }
    }
    private IEnumerator aguardandowarriorfather()
    {
        while (!transform.parent.Find("warrior's father(Clone)"))
        {
            yield return null;
        }
        transform.parent.Find("warrior's father(Clone)").GetComponent<instanciarguerreiros>().enabled = true;
    }
}
