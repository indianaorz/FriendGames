using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPositionControl : MonoBehaviour {

    public int maxPlayers = 12;
    public float verticalOffset = 20;
    public GameObject spawnPositionPrefab;

	// Use this for initialization
	void Start () {
		for(int i = 0; i < maxPlayers; i++)
        {
           GameObject g = Instantiate(spawnPositionPrefab, transform.position + (Vector3.down * verticalOffset * i), Quaternion.identity);
            g.transform.parent = transform;
        }
	}
    
	
	// Update is called once per frame
	void Update () {
		
	}
}
