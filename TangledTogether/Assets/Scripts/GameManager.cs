﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameObject[] players;

    void OnApplicationQuit()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject player in players)
		{
            player.GetComponent<PlayerInputHolder>().playerInput.disableMovement = false;
            player.GetComponent<PlayerInputHolder>().playerInput.disableThrow = false;
		}
    }
}
