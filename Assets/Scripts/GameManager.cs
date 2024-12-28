using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/*
            %%   %%  %%   %%    %%%    %%
   ∧   ∧    %%%  %%   %% %%   %%   %%  %%
 N(i v i)N  %% %%%%    %%%    %%%%%%%  %%
 G(    )/   %%  %%%    %%%    %%   %%
            %%   %%    %%%    %%   %%  %%
*/

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private APIClient apiClient;
    [SerializeField] private ErrorManager errorManager;
    public int UserId { get; private set; }

    public GameObject dialog;
    public GameObject settingsDialog;
    public bool debugMode = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        apiClient = APIClient.Instance;
        errorManager = ErrorManager.Instance;
    }

    public void HandleCheckVoter(int userId)
    {
        UserId = userId;
        StartCoroutine(apiClient.CheckVoter(userId, OnCheckVoterResponse));
    }

    private void OnCheckVoterResponse(bool validVote, bool voted)
    {
        if (validVote && !voted)
        {
            LoadScene("Vote");
        }
        else if (!validVote)
        {
            Dialog("CN404");
            try
            {
                QRManager qRManager = GameObject.Find("QRManager").GetComponent<QRManager>();
                qRManager.ResetField();
            }
            catch
            {
                Dialog("RE");
            }
        }
        else if (voted)
        {
            LoadScene("AlreadyVoted");
        }
    }

    public void SubmitVote(int[] voteNumbers)
    {
        StartCoroutine(apiClient.SubmitVote(UserId, voteNumbers, FinishVote));
    }

    private void FinishVote(bool result)
    {
        if (result)
        {
            LoadScene("DoneVote");
        }
        else
        {
            Debug.LogError("Failed to submit vote");
        }
    }

    public void Restart()
    {
        UserId = 0;
        LoadScene("WaitQR");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void Dialog(string errorCode)
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        GameObject dialog = Instantiate(this.dialog);
        dialog.transform.SetParent(canvas.transform, false);
        Dialog script = dialog.GetComponent<Dialog>();
        ErrorMessage errorMessage = errorManager.GetErrorMessageByCode(errorCode);
        script.OpenDialog(errorMessage.title, errorMessage.description);
    }

    public void SettingsDialog()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        GameObject dialog = Instantiate(settingsDialog);
        dialog.transform.SetParent(canvas.transform, false);
        SettingsDialog script = dialog.GetComponent<SettingsDialog>();
        QRManager qRManager = GameObject.Find("QRManager").GetComponent<QRManager>();
        qRManager.openSettings = true;
        script.OpenDialog();
    }

    public void SaveSettings(string apiEndpoint, string sessionID, string csrftoken, bool debugMode)
    {
        apiClient.SetAPIEndpoint(apiEndpoint);
        apiClient.SetSessionID(sessionID);
        apiClient.SetCSRFToken(csrftoken);
        this.debugMode = debugMode;
        QRManager qRManager = GameObject.Find("QRManager").GetComponent<QRManager>();
        qRManager.openSettings = false;
        UserId = 0;
        qRManager.ResetField();
        qRManager.DebugMode(debugMode);
    }
}
