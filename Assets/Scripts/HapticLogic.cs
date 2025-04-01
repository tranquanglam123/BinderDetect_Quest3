using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class HapticLogic : MonoBehaviour
{
[Range(0,1)]
public float intensity;
public float duration;
// Start is called before the first frame update
void Start()
{
UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
interactable.activated.AddListener(TriggerHaptic);
}
public void TriggerHaptic(BaseInteractionEventArgs eventArgs)
{
if(eventArgs.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor controllerInteractor)
{
TriggerHaptic(controllerInteractor.xrController);
}
}
public void TriggerHaptic(XRBaseController controller)
{
if (intensity > 0)
controller.SendHapticImpulse(intensity, duration);
}
}