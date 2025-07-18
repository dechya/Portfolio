using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
	[Serializable] public class Map
	{
		public List<GameObject> map = new List<GameObject>();
	}

	public List<Map> stageMap = new List<Map>();
	[SerializeField] private GameObject noneMap;

	private MapMove mapMove;

	private void Awake()
	{
		mapMove = GetComponent<MapMove>();
	}

	private void Update()
	{
		if (mapMove.currentDistance > mapMove.maxDistance)
		{
			if(DataManager.Instance.isEnd)
			{
				GameObject go1 = Instantiate(noneMap);

				mapMove.distance += mapMove.maxDistance;
				mapMove.currentDistance -= mapMove.maxDistance;
				mapMove.maxDistance = go1.GetComponent<MapSetting>().Distance;
				go1.transform.position = transform.localPosition + Vector3.right * mapMove.distance;
				go1.transform.parent = transform;
				return;
			}
			int random = UnityEngine.Random.Range(0, stageMap[DataManager.Instance.Stage].map.Count);
			GameObject go = Instantiate(stageMap[DataManager.Instance.Stage].map[random]);
			
			mapMove.distance += mapMove.maxDistance;
			mapMove.currentDistance -= mapMove.maxDistance;
			mapMove.maxDistance = go.GetComponent<MapSetting>().Distance;
			go.transform.position = transform.localPosition + Vector3.right * mapMove.distance;
			go.transform.parent = transform;
		}
	}
}
