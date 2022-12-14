using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using static UnityEditor.Progress;
using PathCreation;

public class Player : MonoBehaviour
{
	[SerializeField] private PathCreator creator;
	[SerializeField] private GameObject waterBall;
	[SerializeField] private float spawnDistance = 0.2f;
	[SerializeField] private float spawnScaleMin = 0.3f;
	[SerializeField] private float spawnScaleMax = 1f;
	[SerializeField] private float speed = 1;
	private float distance = 0;

	private List<GameObject> balls;

	private void Start()
	{
		balls = new List<GameObject>();

		for (float i = 0; i < creator.path.length; i+=spawnDistance)
		{
			GameObject spawned = Instantiate(waterBall, creator.path.GetPointAtDistance(i), Quaternion.identity);
			spawned.transform.localScale = Vector3.one * Random.Range(spawnScaleMin, spawnScaleMax);
			balls.Add(spawned);
		}
	}

	private void Update()
	{
		distance += speed * Time.deltaTime;

		for (int i = 0; i < balls.Count; i++)
		{
			balls[i].transform.position = creator.path.GetPointAtDistance(i * spawnDistance + distance);
		}
	}
}
