using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace OSK
{
    public class LoadingScene : MonoBehaviour
    {
        public Text tipsText;
        public Image progressPic;
        //public TextMeshProUGUI percentLoading;

        AsyncOperation async;
        Vector2 progrespicSize;

        float m_delay;
        bool m_startLoad;

        // public DialogueConfig loadtips;
        //  void Awake()
        // {
        // 	ztipsText.text = TextConfig.GetIns().GetText(loadtips.dialogues[UnityEngine.Random.Range(0, loadtips.dialogues.Length)]);
        // }

        void OnEnable() => Debug.Log("Start loading!");
        void OnDisable() => Debug.Log("End loading!");

        void Start()
        {
            GC.Collect();
            progrespicSize = progressPic.GetComponent<RectTransform>().sizeDelta;
            base.StartCoroutine(LoadScene());
        }

        IEnumerator LoadScene()
        {
            yield return new WaitForSeconds(1f);
            async = SceneManager.LoadSceneAsync("GAME");
            yield return async;
            yield break;
        }
        void Update()
        {
            m_delay += Time.deltaTime;
            if (m_delay > 1f) m_delay = 1f;

            float num = m_delay * 50f;
            if (async != null) num += async.progress * 50f;
            progressPic.fillAmount = num / 100f;
            //percentLoading.text = "LOADING " + percent.ToString("0") + "%";
        }
    }
}