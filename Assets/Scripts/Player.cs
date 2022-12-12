using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Player : MonoBehaviour
{
    [SerializeField] private List<GameObject> waterfalls;
    [SerializeField] private float normalVoroniSpeed = 0.3f;
	[SerializeField] private float highVoroniSpeed = 0.6f;

	[SerializeField] private float woobbleAmountNormal = 0;
	[SerializeField] private float woobbleAmountHigh = 0.1f;
	[SerializeField] private float woobbleFrequancyNormal = 0;
	[SerializeField] private float woobbleFrequancyHigh = 0.1f;
	[SerializeField] private float woobbleHeightNormal = 0;
	[SerializeField] private float woobbleHeightHigh = 0.1f;
	[SerializeField] private float woobbleSpeedNormal = 0;
	[SerializeField] private float woobbleSpeedHigh = 0.1f;

	private float currentSpeed;
	private float currentWoobbleAmount;
	private float currentWoobbleFrequancy;
	private float currentWoobbleHeight;
	private float currentWoobbleSpeed;

	private void Update()
    {
        if (Input.touchCount > 0)
        {
			currentSpeed = highVoroniSpeed;
			currentWoobbleAmount = woobbleAmountHigh;
			currentWoobbleFrequancy = woobbleFrequancyHigh;
			currentWoobbleHeight = woobbleHeightHigh;
			currentWoobbleSpeed = woobbleSpeedHigh;
		}
        else
        {
			currentSpeed = normalVoroniSpeed;
			currentWoobbleAmount = woobbleAmountNormal;
			currentWoobbleFrequancy = woobbleFrequancyNormal;
			currentWoobbleHeight = woobbleHeightNormal;
			currentWoobbleSpeed = woobbleSpeedNormal;
		}

		foreach (var item in waterfalls)
		{
			item.GetComponent<MeshRenderer>().material.SetVector("_VoroniSpeed", new Vector2(0, currentSpeed));
			item.GetComponent<MeshRenderer>().material.SetFloat("_WoobbleAmount", currentWoobbleAmount);
			item.GetComponent<MeshRenderer>().material.SetFloat("_WoobbleFrequancy", currentWoobbleFrequancy);
			item.GetComponent<MeshRenderer>().material.SetFloat("_WoobbleHeight", currentWoobbleHeight);
			item.GetComponent<MeshRenderer>().material.SetFloat("_WoobbleSpeed", currentWoobbleSpeed);
		}
	}
}
