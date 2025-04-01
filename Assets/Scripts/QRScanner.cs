using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using PassthroughCameraSamples;
using TMPro;

public class QRScanner : MonoBehaviour
{
    //private WebCamTexture m_webcamTexture;
    private string m_qrCode = string.Empty;
    [SerializeField] private WebCamTextureManager m_manager;
    [SerializeField] private GameObject m_cube;
    [SerializeField] private TextMeshPro m_debugText;
    private bool m_isScanning = false;
    private void Awake()
    {
        if (m_manager == null)
        {
            m_manager = FindAnyObjectByType<WebCamTextureManager>();
        }
    }
    private void Start()
    {
        var renderer = GetComponent<RawImage>();
        //m_webcamTexture = m_manager.WebCamTexture;
        renderer.texture = m_manager.WebCamTexture;
        //renderer.material.mainTexture = webcamTexture;
    }

    public IEnumerator GetQRCode()
    {
        m_debugText.text = "Scanning QR Code";
        IBarcodeReader barCodeReader = new BarcodeReader();
        m_manager.WebCamTexture.Play();
        var snap = new Texture2D(m_manager.WebCamTexture.width, m_manager.WebCamTexture.height, TextureFormat.ARGB32, false);
        while (string.IsNullOrEmpty(m_qrCode))
        {
            try
            {
                snap.SetPixels32(m_manager.WebCamTexture.GetPixels32());
                var result = barCodeReader.Decode(snap.GetRawTextureData(), m_manager.WebCamTexture.width, m_manager.WebCamTexture.height, RGBLuminanceSource.BitmapFormat.ARGB32);
                if (result != null)
                {
                    m_qrCode = result.Text;
                    if (!string.IsNullOrEmpty(m_qrCode))
                    {
                        m_debugText.text = "DECODED TEXT FROM QR: " + m_qrCode;
                    }
                    if (m_qrCode == "0742-PHA-FIP-CL2-3100")
                    {
                        m_cube.SetActive(!m_cube.gameObject.activeSelf);
                        break;
                    }
                }
            }
            catch (Exception ex) { Debug.LogWarning(ex.Message); m_debugText.text = ex.Message; }
            yield return null;
        }
        m_manager.WebCamTexture.Stop();
        m_isScanning = false;
    }

    private void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        var style = new GUIStyle();

        var rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 50;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);
        var text = m_qrCode;
        GUI.Label(rect, text, style);
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LHand) && !m_isScanning)
        {
            m_isScanning = true;
            StartCoroutine(GetQRCode());
        }
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RHand))
        {
            m_cube.SetActive(!m_cube.gameObject.activeSelf);
        }
    }
}