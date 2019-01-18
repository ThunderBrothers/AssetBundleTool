using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using Newtonsoft.Json;
using static BundleEventTrigger;
using System;
using UnityEditor;
using System.Reflection;

namespace stARkit.Cloud.ARPackageSDK
{


    public class AssetBundleManager : MonoBehaviour
    {
        public MonoScript bundleEventTrigger;
        private List<Shader> shaderList = new List<Shader>();
        private List<Material> materials = new List<Material>();
        private List<GameObject> assetLoaded = new List<GameObject>();
        public static AssetBundleManager Instance = null;
        public string neme;
        public List<Component> components = new List<Component>();
        System.Reflection.Assembly dll = null;

        private void Awake()
        {
            Debug.Log(Application.persistentDataPath + "/test.assetbundle");
            Instance = this;
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(load());
            }
        }

        IEnumerator load() {
            
            name = @"C:\Users\Administrator\Desktop\aa\"+ neme + ".assetbundle";
            UnityWebRequest unityWebRequest = UnityWebRequestAssetBundle.GetAssetBundle(name, 0);
            yield return unityWebRequest.SendWebRequest();
            AssetBundle asset = DownloadHandlerAssetBundle.GetContent(unityWebRequest);
           
            LoadAssetBundle(asset, transform);
            yield return null;
            LoadJsonForTrigger(asset, transform);
            yield return null;
            HandleJsonForTrigger();
        }


        public void UnloadAsset()
        {
            //if (1 == 1) return;
            foreach (GameObject g in assetLoaded)
            {
                if (g != null) DestroyImmediate(g);
            }
            shaderList.Clear();
            materials.Clear();
            assetLoaded.Clear();
        }

        public void LoadAssetBundle(AssetBundle content, Transform container)
        {
            if (content == null) return;
            
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
                    if(gameObjectName != null)
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
                        if (trans != null && t != null)
                        {
                            res = trans.gameObject.AddComponent(t);
                        }
                    }
                }
                ResetGameObjectShader(obj.transform);
                //Shader.WarmupAllShaders();
                //需要修改一下释放资源逻辑
                //未完待续
                //content.Unload(false);





            }
        }

        Dictionary<string, List<BundleEventTriggerInfo>> triggerInfoCache = new Dictionary<string, List<BundleEventTriggerInfo>>();

        private void LoadJsonForTrigger(AssetBundle content,Transform root) {
            TextAsset aa = content.LoadAsset<TextAsset>("eventTriggerConfig");
            AllTriggerToDesingerJson p = JsonConvert.DeserializeObject<AllTriggerToDesingerJson>(aa.text);
           
            for (int i = 0;i < p.allJson.Count;i++)
            {
                string triggerName = p.allJson[i].objName;
                List<BundleEventTriggerInfo> bundleEventTriggerInfos = new List<BundleEventTriggerInfo>();
                BundleEventTriggerInfo temp = new BundleEventTriggerInfo();
                triggerInfoCache.Add(triggerName, bundleEventTriggerInfos);
                List<BundleEventTriggerJson> bundleEventTriggerJsons = p.allJson[i].bundleEventTriggerDesigners.bundleEventTriggerJsons;
                for (int j = 0; j < bundleEventTriggerJsons.Count; j++)
                {
                    GameObject target;
                    UnityEngine.Object method;
                    BundleEventTriggerType triggerType;
                    target = GameObject.Find(bundleEventTriggerJsons[j].target).gameObject;
                    //method = dll.GetType(bundleEventTriggerJsons[j].method);
                    //需要构建一个用名称改成的mehtod以供Trigger判断
                    //未完待续




                    MonoBehaviour[] mb = target.GetComponents<MonoBehaviour>();
                    temp.target = target;
                    temp.method = target;
                    temp.triggerType = (BundleEventTriggerType)bundleEventTriggerJsons[j].triggerType;
                    bundleEventTriggerInfos.Add(temp);
                }
                triggerInfoCache[triggerName] = bundleEventTriggerInfos;
            }
        }
        public List<BundleEventTriggerInfo> GetJsonForTrigger(string triggerName) {
            List<BundleEventTriggerInfo> selfBundleEventTriggerInfo = new List<BundleEventTriggerInfo>();
            if (triggerInfoCache.ContainsKey(triggerName))
            {
                selfBundleEventTriggerInfo = triggerInfoCache[triggerName];
            }
            return selfBundleEventTriggerInfo;
        }
       
        private void HandleJsonForTrigger() {
            for (int i = 0; i < components.Count;i++)
            {
                if (components[i].name == "Trigger--D[0]")
                {
                    Type t = components[i].GetType();
                    BundleEventTrigger type = (BundleEventTrigger)components[i].gameObject.GetComponent(t);
                    type.triggers = GetJsonForTrigger("Trigger--D[0]");
                }
            }
        }
   

        private void ResetAllMaterial(AssetBundle content)
        {
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
                    foreach(Shader sh in shaderList)
                    {
                        if(sh.name.Equals(shaderName))
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

        private IEnumerator SetMaterial(GameObject target, Material[] ms,bool activeSelf)
        {
            yield return 1;
            target.GetComponent<MeshRenderer>().sharedMaterials = ms;
            target.GetComponent<MeshRenderer>().sharedMaterial = ms[0];
            target.GetComponent<MeshRenderer>().materials = ms;
            target.GetComponent<MeshRenderer>().material = ms[0];
            target.SetActive(activeSelf);
        }

        private void ResetGameObjectShader(Transform target)
        {
            MeshRenderer meshRenderer = target.GetComponent<MeshRenderer>();

            if (meshRenderer != null)
            {
                int mcount = meshRenderer.sharedMaterials.Length;
                Material[] newMaterials =new  Material[mcount];
                for (int j = 0; j < mcount; j++)
                {
                    Material _mater = meshRenderer.sharedMaterials[j];
                   // Debug.Log("check material " + _mater.name);
                    bool resetMaterial = false;
                    if(_mater != null)
                    {
                        foreach(Material m in materials)
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
    public class BResult : UnityEngine.Object {
        public List<BundleEventTriggerInfo> selfBundleEventTriggerInfo = AssetBundleManager.Instance.GetJsonForTrigger("Trigger--D[0]");
    }
}



        
