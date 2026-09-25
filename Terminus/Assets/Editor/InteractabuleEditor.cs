using Unity.VisualScripting;
using UnityEditor;

public class InteractiablueEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Interactabule interactabule = (Interactabule)target;
        if (target.GetType() == typeof(EventOnlyInteractabule))
        {
            interactabule.promptMessage = EditorGUILayout.TextField("Prompt Message", interactabule.promptMessage);
            EditorGUILayout.HelpBox("EventOnlyInteract can Only use UnityEvents.", MessageType.Info);
            if (interactabule.GetComponent<InteractionEvent>() == null)
            {
                interactabule.useEvents = true;
                interactabule.gameObject.AddComponent<InteractionEvent>();
            }
        }
        base.OnInspectorGUI();
        if (interactabule.useEvents)
        {
            //we are using events. add the component
            if (interactabule.GetComponent<InteractionEvent>() == null)
                interactabule.gameObject.AddComponent<InteractionEvent>();
        }
        else
        {
            //we are not using events. remove the component.
            if (interactabule.GetComponent<InteractionEvent>() != null)
                DestroyImmediate(interactabule.GetComponent<InteractionEvent>());

        }

    }

}
