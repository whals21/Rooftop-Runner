using UnityEngine;
using UnityEngine.UI;

public class ControlsUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private KeyCode toggleKey = KeyCode.F1;

    [Header("Settings")]
    [SerializeField] private bool startVisible = true;

    void Start()
    {
        // controlsPanel이 할당되지 않았다면 이 오브젝트를 사용
        if (controlsPanel == null)
        {
            controlsPanel = gameObject;
        }

        // 시작 시 표시 여부 설정
        controlsPanel.SetActive(startVisible);
    }

    void Update()
    {
        // F1 키로 조작키 UI 토글
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleControls();
        }
    }

    public void ToggleControls()
    {
        controlsPanel.SetActive(!controlsPanel.activeSelf);
    }

    public void ShowControls()
    {
        controlsPanel.SetActive(true);
    }

    public void HideControls()
    {
        controlsPanel.SetActive(false);
    }
}
