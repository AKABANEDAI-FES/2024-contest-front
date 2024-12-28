using UnityEngine;
using TMPro;

public class QRManager : MonoBehaviour
{
    [SerializeField] public TMP_InputField debugInputText;
    public GameObject debugPanel;
    public string customerID;
    private const string PREFIX = "akabanedai";
    private const string STAFF = "staffakafes8";
    public bool openSettings = false;
    private float span = 2.0f;
    private float time = 0;

    void Start()
    {
        debugPanel.SetActive(GameManager.Instance.debugMode);
    }

    void Update()
    {
        time += Time.deltaTime;
        if (Input.anyKeyDown && !GameManager.Instance.debugMode && !openSettings)
        {
            time = 0;
            string inputString = Input.inputString;
            if (inputString == "\n" || inputString == "\r")
            {
                CheckVoter(customerID);
                ResetField();
            }
            else if (!string.IsNullOrEmpty(inputString))
            {
                customerID += inputString;
            }
        }
        if (customerID != "" && !GameManager.Instance.debugMode && !openSettings && time > span)
        {
            ResetField();
            time = 0;
        }
    }

    public void DebugMode(bool value)
    {
        GameManager.Instance.debugMode = value;
        debugPanel.SetActive(value);
    }

    public void CheckVoter(string id)
    {
        if (id.StartsWith(PREFIX))
        {
            string remainingText = id.Substring(PREFIX.Length);

            if (int.TryParse(remainingText, out int userId))
            {
                GameManager.Instance.HandleCheckVoter(userId);
            }
            else
            {
                Debug.LogError("Invalid User ID format after removing prefix");
                GameManager.Instance.Dialog("CN404");
            }
        }
        else if (id == STAFF)
        {
            GameManager.Instance.SettingsDialog();
        }
        else
        {
            Debug.LogError($"Input does not start with '{PREFIX}'");
            GameManager.Instance.Dialog("CN404");
        }
    }

    public void ResetField()
    {
        debugInputText.text = "";
        customerID = "";
    }

    public void OnClickSubmitButton()
    {
        string text = debugInputText.text;
        if (!string.IsNullOrEmpty(text))
        {
            CheckVoter(text);
            ResetField();
        }
    }

    public void OnClickSettingsButton()
    {
        GameManager.Instance.SettingsDialog();
    }
}
