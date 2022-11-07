using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Metaverse.Data;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TestScript : MonoBehaviour
{

    public void Action()
    {

        IEnumerator onwait()
        {
            Debug.Log("START");
            yield return new WaitForSeconds(2.5f);
            Debug.Log("END");
        }
        onwait().StartCorotine();


    }



    private void Update()
    {
        if ( Input.GetKey( KeyCode.LeftShift ) && Input.GetKeyDown(KeyCode.F1)) 
        {
            UIAdmin.Open();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F2))
        {
           var admin = UIAdmin.Open();
            admin.OnEnter("api");
        }
    }


}



#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(TestScript))]
public class UITestScript : Editor
{

    Transform TRS = null;
    public static string userID, message;
    TestScript service => (TestScript)target;
    static bool speedtree = false;
    static int countReward = 100;



    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Display(service);
    }
    public static void Display(TestScript testsript)
    {

        if (GUILayout.Button($"Add Coin"))
        {

            testsript.Action();

        }


        if (EditorGUIService.DrawHeader("Tree", "Tree.PageTestScript", true, false)) 
        {
            EditorGUIService.BeginContents(false);

            EditorGUIService.EndContents();
        }




        if (EditorGUIService.DrawHeader("Page", "Open.PageTestScript", true, false))
        {
            EditorGUIService.BeginContents(false);
          

            EditorGUIService.EndContents();
        }







    
    }
}
#endif