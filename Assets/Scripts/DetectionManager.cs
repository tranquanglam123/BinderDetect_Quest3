using System.Collections;
using System.Collections.Generic;
using PassthroughCameraSamples.MultiObjectDetection;
using PassthroughCameraSamples;
using UnityEngine;
using UnityEngine.Events;

public class DetectionManager : MonoBehaviour
{
    [SerializeField] private WebCamTextureManager m_webCamTextureManager;

    [Header("Controls configuration")]
    [SerializeField] private OVRInput.RawButton m_actionButton = OVRInput.RawButton.A;

    [Header("Ui references")]
    [SerializeField] private DetectionUiMenuManager m_uiMenuManager;

    [Header("Placement configureation")]
    [SerializeField] private GameObject m_spawnBinder;
    [SerializeField] private EnvironmentRayCastSampleManager m_environmentRaycast;
    [SerializeField] private float m_spawnDistance = 0.25f;

    [Header("QR Scanner ref")]
    [SerializeField] private QRScanner m_qRScannerInference;

    [Space(10)]
    public UnityEvent<int> OnObjectsIdentified;

    private bool m_isPaused = true;
    private List<GameObject> m_spwanedEntities = new();
    private bool m_isStarted = false;
    private bool m_isQRScannerReady = false;
    private float m_delayPauseBackTime = 0;

    #region Unity Functions
    private void Awake() => OVRManager.display.RecenteredPose += CleanMarkersCallBack;


    /// <summary>
    /// If there is no WebcamTextureManager, no scan QR
    /// </summary>
    /// <returns></returns>
    private IEnumerator Start()
    {
        // Wait until Sentis model is loaded
        var webcamTextureManager = FindObjectOfType<WebCamTextureManager>();
        while (webcamTextureManager != null)
        {
            yield return null;
        }
        m_isQRScannerReady = true;
    }

    private void Update()
    {
        // Get the WebCamTexture CPU image
        var hasWebCamTextureData = m_webCamTextureManager.WebCamTexture != null;

        if (!m_isStarted)
        {
            // Manage the Initial Ui Menu
            if (hasWebCamTextureData && m_isQRScannerReady)
            {
                m_uiMenuManager.OnInitialMenu(m_environmentRaycast.HasScenePermission());
                m_isStarted = true;
            }
        }
        else
        {
            // Press A button to spawn 3d markers
            if (OVRInput.GetUp(m_actionButton) && m_delayPauseBackTime <= 0)
            {
                SpawnCurrentDetectedBinder();
            }
            // Cooldown for the A button after return from the pause menu
            m_delayPauseBackTime -= Time.deltaTime;
            if (m_delayPauseBackTime <= 0)
            {
                m_delayPauseBackTime = 0;
            }
        }

        // Not start a sentis inference if the app is paused or we don't have a valid WebCamTexture
        if (m_isPaused || !hasWebCamTextureData)
        {
            if (m_isPaused)
            {
                // Set the delay time for the A button to return from the pause menu
                m_delayPauseBackTime = 0.1f;
            }
            return;
        }

        //// Run a new inference when the current inference finishes
        //if (!m_runInference.IsRunning())
        //{
        //    m_runInference.RunInference(m_webCamTextureManager.WebCamTexture);
        //}
    }
    #endregion

    #region Binder Detect Functions
    /// <summary>
    /// Clean 3d markers when the tracking space is re-centered.
    /// </summary>
    private void CleanMarkersCallBack()
    {
        foreach (var e in m_spwanedEntities)
        {
            Destroy(e, 0.1f);
        }
        m_spwanedEntities.Clear();
        OnObjectsIdentified?.Invoke(-1);
    }
    /// <summary>
    /// Spwan 3d markers for the detected objects
    /// </summary>
    private void SpawnCurrentDetectedBinder()
    {
        //var count = 0;
        //foreach (var box in m_uiInference.BoxDrawn)
        //{
        //    if (PlaceMarkerUsingEnvironmentRaycast(box.WorldPos, box.ClassName))
        //    {
        //        count++;
        //    }
        //}
        //if (count > 0)
        //{
        //    // Play sound if a new marker is placed.
        //    m_placeSound.Play();
        //}
        //OnObjectsIdentified?.Invoke(count);
    }

    /// <summary>
    /// Place a marker using the environment raycast
    /// </summary>
    private bool PlaceMarkerUsingEnvironmentRaycast(Vector3 boxWorldPos, string className)
    {
        // Get the real transform using DepthApi
        var markerTransform = m_environmentRaycast.PlaceGameObject(boxWorldPos);
        if (!markerTransform)
        {
            return false;
        }

        // Check if you spanwed the same object before
        var existMarker = false;
        foreach (var e in m_spwanedEntities)
        {
            var markerClass = e.GetComponent<DetectionSpawnMarkerAnim>();
            if (markerClass)
            {
                var dist = Vector3.Distance(e.transform.position, markerTransform.position);
                if (dist < m_spawnDistance && markerClass.GetYoloClassName() == className)
                {
                    existMarker = true;
                    break;
                }
            }
        }

        if (!existMarker)
        {
            // spawn a visual marker
            var eMarker = Instantiate(m_spawnBinder);
            m_spwanedEntities.Add(eMarker);

            // Update marker transform with the real world transform
            eMarker.transform.SetPositionAndRotation(markerTransform.position, markerTransform.rotation);
            eMarker.GetComponent<DetectionSpawnMarkerAnim>().SetYoloClassName(className);
        }

        return !existMarker;
    }
    #endregion

}
