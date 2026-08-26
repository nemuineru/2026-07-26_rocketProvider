using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageComps : MonoBehaviour
{
    Animator animator;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void PackageSealed()
    {
        animator.SetTrigger("PackageSealed");
        Debug.Log("Package Sealed");
    }
}

public class AnimFunctions
{
    //IEnumerableで設定後、
    IEnumerable Jarring(Manufacture obj)
    {
        float mtime = 0.5f;
        float time = mtime;
        //梱包アニメーション様にパッケージオブジェクト(箱)を作成する.
        GameObject package =
        GameObject.Instantiate(obj.PackagePrefab, obj.transform.position, obj.transform.rotation);
        while (time > 0)
        {
            time -= Time.deltaTime;
            foreach (Parts part in obj.InsideParts)
            {
                //ちょっと拡大して、その後縮小.
                Vector3 Scaling = Vector3.one * Mathf.Sin((time / mtime) * Mathf.PI * 0.5f) * 1.1f;
                part.transform.localScale = Vector3.Lerp(part.transform.localScale, Scaling, 0.5f);
                Vector3 twards = obj.transform.position - part.transform.position;
                float power = 20f;
                Vector3 ringMotion =
                Vector3.Cross(twards, obj.transform.up).normalized * power + (-obj.transform.up) * power;
                part.rb.AddForce(ringMotion);
            }
            yield return null;
        }
        //SendMessageするのはよろしく無いような。 ..まぁ手軽だし良いか
        package.SendMessage("PackageSealed");
    }
}