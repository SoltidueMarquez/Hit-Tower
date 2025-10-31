namespace EditorTools.Editor
{
    public class BaseCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            InspectorGUIHandle();
            
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void InspectorGUIHandle() { }
    }
}
