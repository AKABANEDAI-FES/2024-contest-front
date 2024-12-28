using UnityEngine;
using TMPro;
using TMPro.SpriteAssetUtilities;
using UnityEngine.UI;

public class InputVoteNumber : MonoBehaviour
{
    [SerializeField] private GameObject numberButtonPanel; // 0~9のボタンが配置されたパネル
    [SerializeField] private GameObject voteCheckPanel; // 確認画面が表示されるパネル
    [SerializeField] private TextMeshProUGUI[] voteNumberText = new TextMeshProUGUI[3]; // 投票番号が表示されるテキスト
    [SerializeField] private TextMeshProUGUI[] organizationNameText = new TextMeshProUGUI[3]; // 団体名が表示されるテキスト
    [SerializeField] private TextMeshProUGUI[] projectNameText = new TextMeshProUGUI[3]; // 企画名が表示されるテキスト
    [SerializeField] private Image[] projectImage = new Image[3]; // 企画の画像
    [SerializeField] private Sprite noImage; // 企画画像がない場合の画像
    [SerializeField] private ProjectData[] projectDataList; // 企画データのリスト

    private int[] voteNumbers = new int[3];
    private int voteNumberIndex = 0;

    private void Start()
    {
        ResetVoteInput();
    }

    public void OnClickNumberButton(int number)
    {
        //voteNumberText[voteNumberIndex].text = "";

        if (voteNumberText[voteNumberIndex].text.Length < 4)
        {
            voteNumberText[voteNumberIndex].text += number.ToString();
            if (voteNumberText[voteNumberIndex].text.Length == 4)
            {
                SetPanelVisibility(true, voteNumberIndex);
                DisplayProjectInfo(int.Parse(voteNumberText[voteNumberIndex].text), voteNumberIndex);
            }
        }
    }

    public void OnClickClearButton()
    {
        ResetVoteInput();
    }

    public void OnClickBackButton()
    {
        if (voteNumberText[voteNumberIndex].text.Length > 0)
        {
            voteNumberText[voteNumberIndex].text = voteNumberText[voteNumberIndex].text.Substring(0, voteNumberText[voteNumberIndex].text.Length - 1);
            SetPanelVisibility(false, voteNumberIndex);
        }
    }

    public void OnClickSubmitButton()
    {
        if (int.TryParse(voteNumberText[voteNumberIndex].text, out int voteNumber))
        {
            voteNumbers[voteNumberIndex] = voteNumber;
            //voteListText[voteNumberIndex].text = voteNumberText.text;
            voteNumberIndex++;

            if (voteNumberIndex < 3)
            {
                ResetVoteInput();
            }
            else
            {
                GameManager.Instance.SubmitVote(voteNumbers);
            }
        }
    }

    public void OnClickRestartButton()
    {
        GameManager.Instance.Restart();
    }

    private void ResetVoteInput()
    {
        voteNumberText[voteNumberIndex].text = "";
        SetPanelVisibility(false, voteNumberIndex);
    }

    private void SetPanelVisibility(bool showVoteCheck, int index)
    {
        voteCheckPanel.SetActive(showVoteCheck);
        voteNumberText[index].gameObject.SetActive(!showVoteCheck);
        projectNameText[index].gameObject.SetActive(showVoteCheck);
        organizationNameText[index].gameObject.SetActive(showVoteCheck);
        projectImage[index].gameObject.SetActive(showVoteCheck);
        numberButtonPanel.SetActive(!showVoteCheck);
    }

    private void DisplayProjectInfo(int projectNumber, int index)
    {
        foreach (int voteNumber in voteNumbers)
        {
            if (projectNumber == 1801 || projectNumber == 1802 || projectNumber == 1701 || projectNumber == 5006)
            {
                GameManager.Instance.Dialog("PN-S-001");
                ResetVoteInput();
                return;
            }
            if (projectNumber == 0)
            {
                GameManager.Instance.Dialog("PN404");
                ResetVoteInput();
                return;
            }
            if (projectNumber == voteNumber)
            {
                GameManager.Instance.Dialog("PN重複");
                ResetVoteInput();
                return;
            }
        }
        ProjectData selectedProject = null;

        foreach (var project in projectDataList)
        {
            if (project.projectNumber == projectNumber)
            {
                selectedProject = project;
                break;
            }
        }

        if (selectedProject != null)
        {
            projectNameText[index].text = selectedProject.projectName;
            organizationNameText[index].text = selectedProject.organizationName;
            projectImage[index].sprite = selectedProject.icon != null ? selectedProject.icon : noImage;
        }
        else
        {
            GameManager.Instance.Dialog("PN404");
            ResetVoteInput();
        }
    }
}
