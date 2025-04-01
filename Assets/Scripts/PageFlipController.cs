using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;


public class PageFlipController : MonoBehaviour{
    public InputActionReference flipPageAction; // Assign Flip Page action in Inspector
    public Transform[] pagePivots; // Array to hold multiple pages
    public AudioSource pageFlipAudio; //Assign AudioSource in Inspector
    public XRBaseController hapticController;  //Assign Haptic Impulse Player in Inspector




    private int currentPage = 0; // Track current page index
    private bool isFlipping = false;
    private float[] targetRotations; // Track rotation state for each page

    void Start(){
        targetRotations = new float[pagePivots.Length]; // Initialize rotation tracking for each page
    }

    void Update(){
        if (flipPageAction.action.WasPressedThisFrame() && !isFlipping) {
            if (currentPage < pagePivots.Length){ // Flip only if there are pages left
                isFlipping = true;
                if (pageFlipAudio != null){ // Play sound when flipping
                    pageFlipAudio.Play(); // Plays the assigned flip sound
                }

                 if (hapticController != null){ // Play haptic feedback
                     hapticController.SendHapticImpulse(0.5f, 0.2f); // (Channel 0, Amplitude 0.5, Duration 0.2s)
                }

                targetRotations[currentPage] = (-180f);
                StartCoroutine(FlipPageAnimation(pagePivots[currentPage], targetRotations[currentPage]));
                currentPage++; // Move to the next page
            }
        }
    }

    private IEnumerator FlipPageAnimation(Transform pagePivot, float targetRotation){
        float elapsedTime = 0f;
        float duration = 0.5f; // Adjust speed as needed

        Quaternion startRotation = pagePivot.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, targetRotation);

        while (elapsedTime < duration){
            pagePivot.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        pagePivot.rotation = endRotation;
        isFlipping = false;
    }
}