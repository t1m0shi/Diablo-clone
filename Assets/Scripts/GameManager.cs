using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int DifficultyTier = 1;

	void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("More than one instance of Game Manager found!");
			return;
		}
		instance = this;

	}

}
