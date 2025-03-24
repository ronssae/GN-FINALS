using UnityEngine;
using Unity.Netcode;

public class OwnerShipPickup : NetworkBehaviour
{
    [SerializeField] float pickupRadius = 2f;
    [SerializeField] string pickupTag = "Pickup";

    private void Update()
    {
        if (!HasAuthority || !IsSpawned) return;

        var nearbyColliders = Physics.OverlapSphere(transform.position, pickupRadius);
        foreach (Collider collider in nearbyColliders)
        {
            if (!collider.CompareTag(pickupTag)) continue;

            NetworkObject networkObject = collider.GetComponent<NetworkObject>();
            if (networkObject != null || !networkObject.IsSpawned) continue;
            
            if (!networkObject.IsOwner)
            {
                Debug.Log($"Change ownership of {networkObject} to {NetworkManager.LocalClientId}");
                networkObject.ChangeOwnership(NetworkManager.LocalClientId);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
    }
}
