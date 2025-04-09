using System.Collections.Generic;
using UnityEngine;
public class BookAnimationHandler : MonoBehaviour
{
    private int m_pagePivotCounter = 0;
    private int m_subPageCounter = 0;

    private bool m_pinchFlag = true;
    private Transform m_currentBookPage;
    [SerializeField] private List<Transform> m_pagePivots = new();

    private void Start()
    {
        TogglePageIndex();
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RHand)/* && m_pinchFlag*/)
        {
            //m_pinchFlag = false;
            UpdatePageAnimation();
            //Invoke(nameof(TogglePinchFlag), 1f);
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.A))
        {
            UpdatePageAnimation();
        }

#endif
    }

    private void UpdatePageAnimation()
    {
        m_subPageCounter++;
        if (m_subPageCounter > 1)
        {
            m_currentBookPage.transform.Rotate(0, 0, 180); // rotate back the book page
            m_pagePivotCounter++;
            if (m_pagePivotCounter > 4)
            {
                ResetAnimationPage();
            }
            m_subPageCounter = 0;
            TogglePageIndex();
        }

        else
        {
            m_currentBookPage.transform.GetChild(0).gameObject.SetActive(false);
            m_currentBookPage.transform.GetChild(1).gameObject.SetActive(true);
            m_currentBookPage.transform.Rotate(0, 0, 180);
        }

    }

    private void TogglePageIndex()
    {
        foreach (var page in m_pagePivots)
        {
            page.gameObject.SetActive(false);
        }
        m_pagePivots[m_pagePivotCounter].gameObject.SetActive(true);
        m_currentBookPage = m_pagePivots[m_pagePivotCounter].GetChild(0);
        m_currentBookPage.transform.Rotate(0, 0, 0);

        // Inactive the later page and active the first page
        m_currentBookPage.transform.GetChild(1).gameObject.SetActive(false);
        m_currentBookPage.transform.GetChild(0).gameObject.SetActive(true);
    }

    private void TogglePinchFlag()
    {
        m_pinchFlag = !m_pinchFlag;
    }

    private void ResetAnimationPage()
    {
        m_pagePivotCounter = 0;
        m_subPageCounter = 0;
        TogglePageIndex();
    }
}
