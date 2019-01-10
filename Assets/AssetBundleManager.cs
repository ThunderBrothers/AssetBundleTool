using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class AssetBundleManager : MonoBehaviour {
    private List<Shader> shaderList = new List<Shader>();
    private List<Material> materials = new List<Material>();
    private List<GameObject> assetLoaded = new List<GameObject>();
    public static AssetBundleManager Instance = null;

    private void Awake() {

    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            StartCoroutine(load());
        }
    }
    IEnumerator load() {
        string path = @"C:\Users\Administrator\Desktop\aa\0107\Empty_pc_636824280246229933-3.09_1.22_2.12.assetbundle";
        UnityWebRequest unityWebRequest = UnityWebRequestAssetBundle.GetAssetBundle(path);
        yield return unityWebRequest.SendWebRequest();
        AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(unityWebRequest);

        LoadAssetBundle(assetBundle,transform);

    }





    public void LoadAssetBundle(AssetBundle content, Transform container) {
        if (content == null) return;
        System.Reflection.Assembly dll = null;
        string[] names = content.GetAllAssetNames();
        foreach (string n in names)
        {
            if (n.Contains(".shader"))
            {
                Shader s = content.LoadAsset<Shader>(n);
                Debug.Log("add shader " + s.name);
                shaderList.Add(s);
            }
        }
        Shader.WarmupAllShaders();
        ResetAllMaterial(content);

        //Debug.Log("resource end");
        TextAsset config = content.LoadAsset<TextAsset>("bundleRecord");
        List<string> gameObjectNames = new List<string>();
        List<string> scriptNames = new List<string>();
        string contentName = "";
        if (config != null)
        {
            byte[] c = config.bytes;
            MemoryStream ms = new MemoryStream(c);
            StreamReader sr = new StreamReader(ms);

            contentName = sr.ReadLine();// +".prefab";
            Debug.Log("contentName:" + contentName);
            string count = sr.ReadLine();
            string scriptName = sr.ReadLine();
            string gameObjectName = "";
            Debug.Log("count:" + count);
            while (!string.IsNullOrEmpty(scriptName))
            {
                scriptNames.Add(scriptName);
                gameObjectName = sr.ReadLine();
                if (gameObjectName != null)
                    gameObjectNames.Add(gameObjectName);
                scriptName = sr.ReadLine();
            }

            TextAsset asset = content.LoadAsset<TextAsset>("output");
            if (asset != null)
            {
                dll = System.Reflection.Assembly.Load(asset.bytes);
                /**
                Debug.Log("load dll success!");
                System.Type[] tps = dll.GetTypes();
                if (tps != null && tps.Length > 0)
                {

                    foreach (System.Type tp in tps)
                    {
                        Debug.Log("found in dll " + tp.FullName);
                    }
                }
                **/
            }
            sr.Close();
            ms.Close();

            GameObject obj = (GameObject)GameObject.Instantiate((GameObject)content.LoadAsset(contentName));
            assetLoaded.Add(obj);
            obj.transform.parent = container;
            obj.transform.position = container.position;
            obj.transform.localScale = Vector3.one;

            obj.name = contentName;
            if (dll != null)
            {
                for (int i = 0; i < gameObjectNames.Count; i++)
                {
                    //Debug.Log("try to load " + scriptNames[i] + " from dll in bundle");
                    System.Type t = dll.GetType(scriptNames[i]);

                    Transform trans = obj.transform.parent.Find(gameObjectNames[i]);

                    // Debug.Log(gameObjectNames[i]);
                    //if (trans != null) Debug.Log("gameobject found:" + trans.gameObject);
                    //else Debug.Log("gameobject not exist!");
                    //if (t != null) Debug.Log("scrpte exist:" + t.Name);//+" basetype:" + t.BaseType.Name);
                    //else Debug.Log("script not exist" + scriptNames[i]);
                    Component res = null;
                    if (trans != null && t != null) res = trans.gameObject.AddComponent(t);
                    //Debug.Log("add component success?" + (res != null));
                }
            }

            //Debug.Log("start reset shader!!!!");



            //Shader.WarmupAllShaders();
            content.Unload(false);
        }


    }

    private void ResetAllMaterial(AssetBundle content) {
        object[] ms = content.LoadAllAssets(typeof(Material));
        //if(ms != null) Debug.Log("material count " + ms.Length);
        foreach (object o in ms)
        {
            Material m = (Material)o;
            materials.Add(m);
            var shaderName = m.shader.name;

            var newShader = Shader.Find(shaderName);
            if (newShader != null)
            {
                m.shader = newShader;
                // Debug.Log("refresh shader success for " + shaderName+ " in material " + m.name);
            }
            else
            {
                bool find = false;
                foreach (Shader sh in shaderList)
                {
                    if (sh.name.Equals(shaderName))
                    {
                        find = true;
                        m.shader = sh;
                        // Debug.Log("refresh shader success for " + shaderName + " in material " + m.name);
                        break;
                    }
                }
                // if(!find)Debug.Log("unable to refresh shader: " + shaderName + " in material " + m.name);
            }
        }

    }

    private IEnumerator SetMaterial(GameObject target, Material[] ms, bool activeSelf) {
        yield return 1;
        target.GetComponent<MeshRenderer>().sharedMaterials = ms;
        target.GetComponent<MeshRenderer>().sharedMaterial = ms[0];
        target.GetComponent<MeshRenderer>().materials = ms;
        target.GetComponent<MeshRenderer>().material = ms[0];
        target.SetActive(activeSelf);
    }

    private void ResetGameObjectShader(Transform target) {
        MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            int mcount = meshRenderer.sharedMaterials.Length;
            Material[] newMaterials = new Material[mcount];
            for (int j = 0; j < mcount; j++)
            {
                Material _mater = meshRenderer.sharedMaterials[j];
                // Debug.Log("check material " + _mater.name);
                bool resetMaterial = false;
                if (_mater != null)
                {
                    foreach (Material m in materials)
                    {
                        if (m.name.Equals(_mater.name))
                        {
                            // Debug.Log("reset material " + m.name);
                            Material nm = new Material(m);
                            newMaterials[j] = nm;
                            resetMaterial = true;
                            continue;
                        }
                    }
                }
                if (resetMaterial) continue;
                newMaterials[j] = _mater;
                if (_mater == null || _mater.shader == null) continue;
                Shader s = Shader.Find(_mater.shader.name);
                if (s != null) newMaterials[j].shader = s;
                else
                {
                    foreach (Shader sh in shaderList)
                    {
                        if (sh == null) continue;
                        if (sh.name.Equals(_mater.shader.name))
                        {
                            newMaterials[j].shader = sh;
                            continue;
                        }
                    }
                }
            }


            StartCoroutine(SetMaterial(target.gameObject, newMaterials, target.gameObject.activeSelf));
            target.gameObject.SetActive(false);
        }


        for (int i = 0; i < target.childCount; i++)
        {
            ResetGameObjectShader(target.GetChild(i));
        }
    }
}



