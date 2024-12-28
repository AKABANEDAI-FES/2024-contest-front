using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{
    public Animator animator;
    public TextMeshProUGUI title;
    public TextMeshProUGUI discription;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OpenDialog(string title, string discription)
    {
        animator = gameObject.GetComponent<Animator>();
        this.title.text = title;
        this.discription.text = discription;
        animator.SetTrigger("open");
    }

    public void CloseDialog()
    {
        animator.SetTrigger("close");
    }

    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }
}
