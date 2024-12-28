using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;

public class ErrorManager : MonoBehaviour
{
    public static ErrorManager Instance { get; private set; }
    private Dictionary<string, ErrorMessage> errorMessages;

    private string slackWebhookUrl = "https://hooks.slack.com/services/hogehoge";

    void Awake()
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
        errorMessages = new Dictionary<string, ErrorMessage>
        {
            { "PN404", new ErrorMessage("無効な企画番号", "企画が見つかりませんでした。\n企画番号をお確かめください。", false) },
            { "PN重複", new ErrorMessage("重複した企画番号", "同じ企画には投票できません。別の企画を選んでください。", false)},
            { "CN404", new ErrorMessage("無効なQRコード", "お持ちのQRコードは、投票できるように設定されていません。スタッフにお声がけください。", true)},
            { "SE403", new ErrorMessage("サーバー通信失敗", "サーバーに接続できませんでした。\nスタッフにお声がけください。", true) }, //サーバーエラー403,認証エラー
            { "NSERR", new ErrorMessage("内部処理の失敗", "システム内部での処理に失敗しました。\nスタッフにお声がけください。", true)}, //内部処理エラー,出てほしくない
            { "PN-S-001", new ErrorMessage("企画番号エラー", "選択いただいた出展団体が学生団体ではないため投票できません。お手数をおかけしますが、他の企画に投票をお願いします。", false) },
        };
    }

    public ErrorMessage GetErrorMessageByCode(string errorCode)
    {
        if (errorMessages.TryGetValue(errorCode, out ErrorMessage errorMessage))
        {
            //StartCoroutine(SendErrorToSlack(errorCode, errorMessage));
            return new ErrorMessage(errorMessage.title, errorMessage.description + "\n（" + errorCode + "）", false);
        }
        else
        {
            Debug.LogError("無効なエラーコード: " + errorCode);
            //StartCoroutine(SendErrorToSlack(errorCode, new ErrorMessage("不明なエラー", "想定外のエラー", true)));
            return new ErrorMessage("不明なエラー", "想定外のエラーが発生しました。\nスタッフにお声がけください。\n（エラーコード：AMARI-DENAI-ERROR）", false);
        }
    }

    /*
        private IEnumerator SendErrorToSlack(string errorCode, ErrorMessage errorMessage)
        {
            if (!errorMessage.isSentToSlack)
            {
                yield return null;
            }
            // Slackに送信するメッセージをフォーマット
            string jsonPayload = "{\"text\":\"Error Code: " + errorCode + "\\nTitle: " + errorMessage.title + "\\nDescription: " + errorMessage.description + "\"}";

            // HTTPリクエストを作成
            using (UnityWebRequest www = new UnityWebRequest(slackWebhookUrl, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                // リクエストを送信してレスポンスを待つ
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Slackへの通知に失敗しました: " + www.error);
                }
                else
                {
                    Debug.Log("Slackにエラー通知を送信しました: " + errorCode);
                }
            }
        }
    */
}

[System.Serializable]
public class ErrorMessage
{
    public string title;
    public string description;
    public bool isSentToSlack;

    public ErrorMessage(string title, string description, bool isSentToSlack)
    {
        this.title = title;
        this.description = description;
        this.isSentToSlack = false;
    }
}
