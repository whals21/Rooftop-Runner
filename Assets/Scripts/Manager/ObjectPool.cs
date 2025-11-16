using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 20;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        // 풀에 오브젝트 미리 생성
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(transform); // 풀 오브젝트의 자식으로
            pool.Enqueue(obj);
        }

        Debug.Log($"Object Pool Initialized: {poolSize} objects");
    }

    // 풀에서 오브젝트 가져오기
    public GameObject Get()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // 풀이 부족하면 새로 생성
            Debug.LogWarning("Pool is empty! Creating new object.");
            GameObject obj = Instantiate(prefab);
            obj.transform.SetParent(transform);
            return obj;
        }
    }

    // 풀에 오브젝트 반환
    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Enqueue(obj);
    }
}
