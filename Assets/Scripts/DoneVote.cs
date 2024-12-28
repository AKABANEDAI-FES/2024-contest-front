using UnityEngine;

public class DoneVote : MonoBehaviour
{
    // 5秒後にScene切り替え
    private void Start()
    {
        Invoke("OnClickRestartButton", 5f);
    }

    public void OnClickRestartButton()
    {
        GameManager.Instance.Restart();
    }
}
