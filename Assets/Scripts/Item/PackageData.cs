using UnityEngine;

[CreateAssetMenu(fileName = "PackageData", menuName = "Game/Package Data")]
public class PackageData : ScriptableObject
{
    [Header("Package Info")]
    public string packageName = "일반 배송";
    public int scoreValue = 10;
    public Color packageColor = Color.yellow;

    [Header("Physics")]
    public float weight = 1f;

    [Header("Visual")]
    public GameObject visualPrefab;
}