using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Text;

public class APIClient : MonoBehaviour
{
    public static APIClient Instance { get; private set; }
    private string API_BASE_URL;
    private string SESSION_ID;
    private string CSRF_TOKEN;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            API_BASE_URL = PlayerPrefs.GetString("apiEndpoint");
            SESSION_ID = PlayerPrefs.GetString("sessionID");
            CSRF_TOKEN = PlayerPrefs.GetString("csrftoken");
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    public IEnumerator CheckVoter(int userId, Action<bool, bool> onResponse)
    {
        string uri = $"{API_BASE_URL}/api/v1/voter?user_id={userId}";
        yield return StartCoroutine(SendRequest<VoterResponse>(uri, "GET", null, response =>
        {
            if (response != null)
            {
                onResponse?.Invoke(response.valied_vote, response.voted);
            }
        }));
    }

    public IEnumerator SubmitVote(int userId, int[] plans, Action<bool> onResponse)
    {
        string uri = $"{API_BASE_URL}/api/v1/vote/";
        VoteRequest data = new VoteRequest { user_id = userId, plans = plans };
        yield return StartCoroutine(SendRequest<VoteResponse>(uri, "POST", data, response =>
        {
            if (response != null)
            {
                onResponse?.Invoke(response.succeed);
            }
        }));
    }

    private IEnumerator SendRequest<T>(string uri, string method, object data, Action<T> onResponse) where T : class
    {
        UnityWebRequest webRequest = null;
        try
        {
            webRequest = new UnityWebRequest(uri, method);
            if (data != null)
            {
                string jsonData = JsonUtility.ToJson(data);
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.SetRequestHeader("Content-Type", "application/json");
            }

            webRequest.SetRequestHeader("Cookie", $"sessionid={SESSION_ID}; csrftoken={CSRF_TOKEN};");
            webRequest.SetRequestHeader("X-CSRFToken", CSRF_TOKEN);

            webRequest.downloadHandler = new DownloadHandlerBuffer();
        }
        catch (Exception e)
        {
            Debug.LogError("Error setting up web request" + e.ToString());
            GameManager.Instance.Dialog("NSERR");
            onResponse?.Invoke(null);
            yield break;
        }

        yield return webRequest.SendWebRequest();

        try
        {
            Debug.Log($"Response: {webRequest.downloadHandler.text}");

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
                if (webRequest.responseCode == 403)
                {
                    GameManager.Instance.Dialog("SE403");
                }
                else
                {
                    GameManager.Instance.Dialog("NSERR");
                }
                onResponse?.Invoke(null);
            }
            else
            {
                string responseText = webRequest.downloadHandler.text;

                T response = JsonUtility.FromJson<T>(responseText);
                onResponse?.Invoke(response);
                
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error processing web request response: " + e.ToString());
            GameManager.Instance.Dialog("NSERR");
            onResponse?.Invoke(null);
        }
        finally
        {
            webRequest?.Dispose();
        }
    }

    public void SetAPIEndpoint(string apiEndpoint)
    {
        API_BASE_URL = apiEndpoint;
        PlayerPrefs.SetString("apiEndpoint", API_BASE_URL);
        PlayerPrefs.Save();
    }

    public void SetSessionID(string sessionID)
    {
        SESSION_ID = sessionID;
        PlayerPrefs.SetString("sessionID", SESSION_ID);
        PlayerPrefs.Save();
    }

    public void SetCSRFToken(string csrftoken)
    {
        CSRF_TOKEN = csrftoken;
        PlayerPrefs.SetString("csrftoken", CSRF_TOKEN);
        PlayerPrefs.Save();
    }

    // データクラスの定義
    [System.Serializable]
    public class VoterRequest
    {
        public int user_id;
    }

    [System.Serializable]
    public class VoterResponse
    {
        public bool valied_vote;
        public bool voted;
        public int[] voted_plans;
    }

    [System.Serializable]
    public class VoterRegisterResponse
    {
        public bool result;
    }

    [System.Serializable]
    public class PlanResponse
    {
        public int plan_id;
        public string title;
        public string group;
        public int category;
    }

    [System.Serializable]
    public class VoteRequest
    {
        public int user_id;
        public int[] plans;
    }

    [System.Serializable]
    public class VoteResponse
    {
        public bool succeed;
        public string comment;
        public bool valied_vote;
        public bool[] valid_plans;
    }
}
