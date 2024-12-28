using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsDialog : MonoBehaviour
{
    public Animator animator;
    public TMP_InputField apiEndpointInput;
    public TMP_InputField sessionIDInput;
    public TMP_InputField csrftokenInput;
    public Toggle debugModeToggle;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenDialog()
    {
        animator = gameObject.GetComponent<Animator>();
        apiEndpointInput.text = PlayerPrefs.GetString("apiEndpoint");
        sessionIDInput.text = PlayerPrefs.GetString("sessionID");
        csrftokenInput.text = PlayerPrefs.GetString("csrftoken");
        debugModeToggle.isOn = GameManager.Instance.debugMode ? true : false;
        animator.SetTrigger("open");
    }

    public void CloseDialog()
    {
        GameManager.Instance.SaveSettings(apiEndpointInput.text, sessionIDInput.text, csrftokenInput.text, debugModeToggle.isOn);
        animator.SetTrigger("close");
    }

    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }
}
