using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityUtils;

public class ObjectSpawner : NetworkBehaviour
{
    public GameObject prefab;
    public int numberOfPrefabs = 10;

    void Start()
    {
        if (!HasAuthority) return;
        if (!NetworkManager.LocalClient.IsSessionOwner) return;

        List<Vector3> randomPoints = new List<Vector3>();
        for (int i = 0; i < numberOfPrefabs; i++)
        {
            randomPoints.Add(Vector3.zero.RandomPointInAnnulus(5, 10));
        }

        for (int i = 0; i < numberOfPrefabs; i++)
        {
            var instance = Instantiate(prefab);
            var networkObject = instance.GetComponent<NetworkObject>();
            instance.transform.position = randomPoints[i];
            networkObject.Spawn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
