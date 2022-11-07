using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanArea : MonoBehaviour
{


    public GameObject GameObject;
    public int MinAmount;
    public int MaxAmount;
    public bool isRotate;
    public bool isSnapGround;
    public float[] Scale = new float[2] { 1, 1 };
    public float[] Radius = new float[2] { 0 , 3 };
    public Lines Line;

#if UNITY_EDITOR
    public RuntimeBtn RadiusSpwan = new RuntimeBtn("Run", (r) => {
       var g = (GameObject)UnityEditor.Selection.activeObject;
        var SpwanArea = g.GetComponent<SpwanArea>();
        if (SpwanArea != null) {

            int count = Random.RandomRange(SpwanArea.MinAmount, SpwanArea.MaxAmount);
            count.Loop(()=>{
               var newGameobj =  SpwanArea.GameObject.Create(SpwanArea.transform);
               var post = SpwanArea.transform.RandomPointOnXZCircle(Random.RandomRange(SpwanArea.Radius[0], SpwanArea.Radius[1]));
               newGameobj.transform.localPosition = post;
                SpwanArea.Handle(newGameobj);
            });
        }
    });
#endif



    public void Handle(GameObject g) {

        g.transform.localScale = Vector3.one * Random.RandomRange(Scale[0], Scale[1]);
        if (isRotate) g.transform.eulerAngles = new Vector3(0.0f,360.Random(),0.0f);
        if(isSnapGround) Ground(g.transform);
    }
    public  void Ground(Transform selete)
    {
        var hits = Physics.RaycastAll(selete.position + Vector3.up, Vector3.down, 10f);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject == selete.gameObject)
                continue;

            selete.position = hit.point;
            break;
        }
    }








#if UNITY_EDITOR
    public RuntimeBtn LineSpwan = new RuntimeBtn("Run", (r) => {
        var g = (GameObject)UnityEditor.Selection.activeObject;
        var SpwanArea = g.GetComponent<SpwanArea>();
        if (SpwanArea != null)
        {

            int count = Random.RandomRange(SpwanArea.MinAmount, SpwanArea.MaxAmount);
            count.Loop(() => {
                var newGameobj = SpwanArea.GameObject.Create(SpwanArea.transform);
                //var post = SpwanArea.transform.RandomPointOnXZCircle(Random.RandomRange(SpwanArea.Radius[0], SpwanArea.Radius[1]));

                var line = SpwanArea.Line;
                var x = 1.0f.Random();
                var posx = Vector3.Lerp(line.LineDatas[0].Position, line.LineDatas[1].Position, x);

                var y = 1.0f.Random();
                var posy = Vector3.Lerp(line.LineDatas[2].Position, line.LineDatas[3].Position, y);



                var s = 1.0f.Random();
                var post = Vector3.Lerp(posx, posy, s);


                newGameobj.transform.localPosition = post;
                SpwanArea.Handle(newGameobj);
            });
        }
    });
#endif
    public RuntimeBtn ClearSpwan = new RuntimeBtn("Run", (r) => {

#if UNITY_EDITOR


        var g = (GameObject)UnityEditor.Selection.activeObject;
        var SpwanArea = g.GetComponent<SpwanArea>();
        if (SpwanArea != null)
        {
            foreach (var m in SpwanArea.transform.GetAllParent()) {

                DestroyImmediate(m);
            
            }
        }

#endif

    });













}
