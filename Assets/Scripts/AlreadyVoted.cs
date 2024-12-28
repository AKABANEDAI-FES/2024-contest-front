using UnityEngine;

public class AlreadyVoted : MonoBehaviour
{
    public void OnClickRestartButton()
    {
        GameManager.Instance.Restart();
    }

    public void OnClickSubmitButton()
    {
        GameManager.Instance.LoadScene("Vote");
    }
}
