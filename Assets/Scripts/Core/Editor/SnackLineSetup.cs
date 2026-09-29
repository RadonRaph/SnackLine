using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// [CORE - Éditeur] Menus "SnackLine > ..." pour l'enseignant.
///
/// - Construire les prefabs : un prefab Item par modèle du Food Kit (Prefabs/Items)
///   et les machines de base (Prefabs/Machines).
/// - Construire les scènes : Scenes/SnackLine_Exemple (petite ligne qui fonctionne)
///   et Scenes/SnackLine_TP (usine vide, point de départ des étudiants).
/// Relancer un menu ÉCRASE les fichiers générés (les liens vers les prefabs sont conservés).
/// </summary>
public static class SnackLineSetup
{
    const string FoodKitFolder = "Assets/Kenney/FoodKit";
    const string FactoryKitFolder = "Assets/Kenney/FactoryKit";
    const string ItemsFolder = "Assets/Prefabs/Items";
    const string MachinesFolder = "Assets/Prefabs/Machines";
    const string MaterialsFolder = "Assets/Materials";
    const string ExampleScenePath = "Assets/Scenes/SnackLine_Exemple.unity";
    const string TPScenePath = "Assets/Scenes/SnackLine_TP.unity";

    // Taille d'un item sur un tapis de 1 m (sa plus grande largeur)
    const float MinItemSize = 0.25f;
    const float MaxItemSize = 0.5f;

    // Milieu d'un virage de rayon 0.5 centré sur le coin de la case
    const float CornerMiddle = 0.146f;

    [MenuItem("SnackLine/Construire les prefabs")]
    static void BuildPrefabsMenu()
    {
        if (EditorUtility.DisplayDialog("SnackLine",
                "Générer les prefabs (Prefabs/Items et Prefabs/Machines) ?\nLes prefabs générés existants seront écrasés.",
                "Générer", "Annuler"))
        {
            BuildPrefabs();
        }
    }

    [MenuItem("SnackLine/Construire les scènes")]
    static void BuildScenesMenu()
    {
        if (EditorUtility.DisplayDialog("SnackLine",
                "Générer les scènes SnackLine_Exemple et SnackLine_TP ?\nLes scènes existantes seront écrasées.",
                "Générer", "Annuler")
            && EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            BuildScenes();
        }
    }

    /// <summary>Pour la ligne de commande : Unity -batchmode -executeMethod SnackLineSetup.BuildAllBatch</summary>
    public static void BuildAllBatch()
    {
        BuildPrefabs();
        BuildScenes();
    }

    // =====================================================================
    // Prefabs
    // =====================================================================

    public static void BuildPrefabs()
    {
        EnsureFolder(ItemsFolder);
        EnsureFolder(MachinesFolder);
        EnsureFolder(MaterialsFolder);

        string[] foodGuids = AssetDatabase.FindAssets("t:Model", new[] { FoodKitFolder });
        foreach (string guid in foodGuids)
        {
            BuildItemPrefab(AssetDatabase.GUIDToAssetPath(guid));
        }

        float beltTop = MeasureTop("conveyor", Vector3.zero);
        float cornerTop = MeasureTop("conveyor-corner", new Vector3(CornerMiddle, 0f, -CornerMiddle));

        BuildConveyor(beltTop);
        BuildCorner("Conveyor_CornerRight", cornerTop, 1f, 0f);
        BuildCorner("Conveyor_CornerLeft", cornerTop, -1f, 90f);
        BuildSpawner(beltTop);
        BuildSellZone();
        BuildMachineTemplate(beltTop);

        AssetDatabase.SaveAssets();
        Debug.Log("[SnackLine] " + foodGuids.Length + " prefabs d'items et 6 prefabs de machines générés.");
    }

    static void BuildItemPrefab(string modelPath)
    {
        string itemName = Path.GetFileNameWithoutExtension(modelPath);
        GameObject root = new GameObject(itemName);
        GameObject model = AddModel(root, modelPath);

        // Mise à l'échelle : l'item doit tenir sur un tapis de 1 m
        Bounds bounds = GetBounds(model);
        float biggest = Mathf.Max(bounds.size.x, bounds.size.z);
        float scale = Mathf.Clamp(biggest, MinItemSize, MaxItemSize) / Mathf.Max(biggest, 0.001f);
        model.transform.localScale = Vector3.one * scale;
        model.transform.localPosition = new Vector3(-bounds.center.x, -bounds.min.y, -bounds.center.z) * scale;

        Vector3 size = bounds.size * scale;
        BoxCollider box = root.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = new Vector3(Mathf.Max(size.x, 0.1f), Mathf.Max(size.y, 0.1f), Mathf.Max(size.z, 0.1f));
        box.center = new Vector3(0f, box.size.y / 2f, 0f);

        Rigidbody body = root.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;

        root.AddComponent<Item>().itemName = itemName;

        Save(root, ItemsFolder + "/" + itemName + ".prefab");
    }

    static void BuildConveyor(float top)
    {
        GameObject root = CreateMachineRoot("Conveyor");
        // Le modèle Kenney avance selon X : on le tourne pour que le tapis avance vers +Z (flèche bleue)
        AddModel(root, FactoryModel("conveyor")).transform.localRotation = Quaternion.Euler(0f, 90f, 0f);

        Conveyor conveyor = root.AddComponent<Conveyor>();
        conveyor.progressTime = 1f;
        conveyor.inputZone = AddInputZone(root, new Vector3(0.9f, 0.6f, 1f), new Vector3(0f, top + 0.2f, 0f));
        conveyor.startPoint = AddPoint(root, "StartPoint", new Vector3(0f, top, -0.5f));
        conveyor.outputPoint = AddPoint(root, "OutputPoint", new Vector3(0f, top, 0.5f));

        Save(root, MachinesFolder + "/Conveyor.prefab");
    }

    // side = 1 : tourne à droite (sortie vers +X) / side = -1 : tourne à gauche (sortie vers -X)
    static void BuildCorner(string prefabName, float top, float side, float modelRotation)
    {
        GameObject root = CreateMachineRoot(prefabName);
        // Le virage Kenney relie les bords -Z et +X. Tourné de 90°, il relie -Z et -X.
        AddModel(root, FactoryModel("conveyor-corner")).transform.localRotation = Quaternion.Euler(0f, modelRotation, 0f);

        Conveyor conveyor = root.AddComponent<Conveyor>();
        conveyor.progressTime = 1f;
        conveyor.inputZone = AddInputZone(root, new Vector3(1f, 0.6f, 1f), new Vector3(0f, top + 0.2f, 0f));
        conveyor.startPoint = AddPoint(root, "StartPoint", new Vector3(0f, top, -0.5f));
        conveyor.middlePoint = AddPoint(root, "MiddlePoint", new Vector3(side * CornerMiddle, top, -CornerMiddle));
        conveyor.outputPoint = AddPoint(root, "OutputPoint", new Vector3(side * 0.5f, top, 0f));

        Save(root, MachinesFolder + "/" + prefabName + ".prefab");
    }

    static void BuildSpawner(float beltTop)
    {
        GameObject root = new GameObject("Spawner");
        AddModel(root, FactoryModel("hopper-square")).transform.localScale = Vector3.one * 0.85f;

        ItemSpawner spawner = root.AddComponent<ItemSpawner>();
        spawner.interval = 2f;
        spawner.itemPrefab = AssetDatabase.LoadAssetAtPath<Item>(ItemsFolder + "/meat-raw.prefab");
        // Juste devant la source, sur la case du convoyeur suivant
        spawner.outputPoint = AddPoint(root, "OutputPoint", new Vector3(0f, beltTop, 0.6f));

        Save(root, MachinesFolder + "/Spawner.prefab");
    }

    static void BuildSellZone()
    {
        GameObject root = CreateMachineRoot("SellZone");
        FitFootprint(AddModel(root, FactoryModel("box-large")), 0.9f);

        SellZone sellZone = root.AddComponent<SellZone>();
        sellZone.progressTime = 0.3f;
        sellZone.inputZone = AddInputZone(root, new Vector3(1f, 1f, 1f), new Vector3(0f, 0.5f, 0f));

        Save(root, MachinesFolder + "/SellZone.prefab");
    }

    // Base des machines des étudiants : il ne manque que leur script
    static void BuildMachineTemplate(float beltTop)
    {
        GameObject root = CreateMachineRoot("Machine_Modele");
        GameObject model = AddModel(root, FactoryModel("machine"));
        FitFootprint(model, 1f);
        float top = GetBounds(model).max.y;

        AddInputZone(root, new Vector3(1f, 1f, 1f), new Vector3(0f, 0.5f, 0f));
        AddPoint(root, "OutputPoint", new Vector3(0f, beltTop, 0.6f));

        // Barre de progression : un fond gris + "ProgressBar" (pivot à gauche) qui s'agrandit
        Material background = GetMaterial("ProgressBar_Fond", new Color(0.15f, 0.15f, 0.15f));
        Material fill = GetMaterial("ProgressBar_Rempli", new Color(0.3f, 0.9f, 0.3f));
        Vector3 barPosition = new Vector3(0f, top + 0.3f, 0f);
        AddCube(root.transform, "ProgressBar_Fond", barPosition, new Vector3(0.8f, 0.08f, 0.08f), background);
        Transform bar = AddPoint(root, "ProgressBar", barPosition + new Vector3(-0.4f, 0f, 0f));
        AddCube(bar, "Rempli", new Vector3(0.4f, 0f, 0f), new Vector3(0.8f, 0.1f, 0.1f), fill);

        Save(root, MachinesFolder + "/Machine_Modele.prefab");
    }

    // =====================================================================
    // Scènes
    // =====================================================================

    public static void BuildScenes()
    {
        if (AssetDatabase.LoadAssetAtPath<GameObject>(MachinesFolder + "/Conveyor.prefab") == null)
        {
            BuildPrefabs();
        }
        EnsureFolder("Assets/Scenes");

        // Scène d'exemple : source -> 4 convoyeurs -> virage à droite -> 3 convoyeurs -> vente
        var example = CreateFactoryScene();
        Transform line = new GameObject("Ligne de production").transform;
        Place("Spawner", line, new Vector3(0f, 0f, -4f), 0f);
        for (int z = -3; z <= 0; z++)
        {
            Place("Conveyor", line, new Vector3(0f, 0f, z), 0f);
        }
        Place("Conveyor_CornerRight", line, new Vector3(0f, 0f, 1f), 0f);
        for (int x = 1; x <= 3; x++)
        {
            Place("Conveyor", line, new Vector3(x, 0f, 1f), 90f);
        }
        Place("SellZone", line, new Vector3(4f, 0f, 1f), 0f);
        EditorSceneManager.SaveScene(example, ExampleScenePath);

        // Scène du TP : l'usine est vide
        var tp = CreateFactoryScene();
        new GameObject("Usine");
        EditorSceneManager.SaveScene(tp, TPScenePath);

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(TPScenePath, true),
            new EditorBuildSettingsScene(ExampleScenePath, true),
        };
        Debug.Log("[SnackLine] Scènes générées : " + ExampleScenePath + " et " + TPScenePath);
    }

    // Nouvelle scène avec caméra, lumière et sol de 16 x 16 m
    static UnityEngine.SceneManagement.Scene CreateFactoryScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        Camera.main.transform.SetPositionAndRotation(new Vector3(-3f, 7f, -8f), Quaternion.Euler(40f, 20f, 0f));

        Light sun = Object.FindFirstObjectByType<Light>();
        sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        sun.shadows = LightShadows.Soft;

        Transform floor = new GameObject("Sol").transform;
        GameObject floorTile = LoadModel(FactoryModel("floor-large"));
        for (int x = -4; x < 4; x++)
        {
            for (int z = -4; z < 4; z++)
            {
                GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(floorTile, floor);
                // Dalles de 2 m alignées sur la grille des convoyeurs (cases de 1 m centrées sur les entiers)
                tile.transform.position = new Vector3(x * 2f + 0.5f, 0f, z * 2f + 0.5f);
            }
        }
        return scene;
    }

    static void Place(string prefabName, Transform parent, Vector3 position, float rotationY)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(MachinesFolder + "/" + prefabName + ".prefab");
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
        instance.transform.SetPositionAndRotation(position, Quaternion.Euler(0f, rotationY, 0f));
    }

    // =====================================================================
    // Outils
    // =====================================================================

    static string FactoryModel(string modelName)
    {
        return FactoryKitFolder + "/" + modelName + ".fbx";
    }

    static GameObject LoadModel(string path)
    {
        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (model == null)
        {
            throw new FileNotFoundException("[SnackLine] Modèle introuvable : " + path);
        }
        return model;
    }

    static GameObject AddModel(GameObject root, string path)
    {
        GameObject model = (GameObject)PrefabUtility.InstantiatePrefab(LoadModel(path), root.transform);
        model.transform.localPosition = Vector3.zero;
        return model;
    }

    static GameObject CreateMachineRoot(string machineName)
    {
        GameObject root = new GameObject(machineName);
        Rigidbody body = root.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
        return root;
    }

    static Transform AddInputZone(GameObject root, Vector3 size, Vector3 center)
    {
        Transform zone = AddPoint(root, "InputZone", center);
        BoxCollider box = zone.gameObject.AddComponent<BoxCollider>();
        box.isTrigger = true;
        box.size = size;
        return zone;
    }

    static Transform AddPoint(GameObject root, string pointName, Vector3 localPosition)
    {
        Transform point = new GameObject(pointName).transform;
        point.SetParent(root.transform, false);
        point.localPosition = localPosition;
        return point;
    }

    static void AddCube(Transform parent, string cubeName, Vector3 localPosition, Vector3 size, Material material)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Object.DestroyImmediate(cube.GetComponent<Collider>());
        cube.name = cubeName;
        cube.transform.SetParent(parent, false);
        cube.transform.localPosition = localPosition;
        cube.transform.localScale = size;
        cube.GetComponent<Renderer>().sharedMaterial = material;
    }

    static Material GetMaterial(string materialName, Color color)
    {
        string path = MaterialsFolder + "/" + materialName + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(material, path);
        }
        material.SetColor("_BaseColor", color);
        EditorUtility.SetDirty(material);
        return material;
    }

    static Bounds GetBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return new Bounds(go.transform.position, Vector3.one * 0.3f);
        }

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer rend in renderers)
        {
            bounds.Encapsulate(rend.bounds);
        }
        return bounds;
    }

    static void FitFootprint(GameObject model, float footprint)
    {
        Bounds bounds = GetBounds(model);
        float biggest = Mathf.Max(bounds.size.x, bounds.size.z);
        model.transform.localScale = Vector3.one * (footprint / biggest);
    }

    // Hauteur du dessus d'un modèle à un endroit donné (ex : surface du tapis)
    static float MeasureTop(string modelName, Vector3 localPoint)
    {
        // Mesuré loin de la scène pour ne toucher que ce modèle
        Vector3 origin = new Vector3(5000f, 0f, 5000f);
        GameObject model = (GameObject)PrefabUtility.InstantiatePrefab(LoadModel(FactoryModel(modelName)));
        model.transform.position = origin;
        foreach (MeshFilter filter in model.GetComponentsInChildren<MeshFilter>())
        {
            filter.gameObject.AddComponent<MeshCollider>().sharedMesh = filter.sharedMesh;
        }
        Physics.SyncTransforms();

        float top = GetBounds(model).max.y;
        RaycastHit hit;
        if (Physics.Raycast(origin + localPoint + Vector3.up * 10f, Vector3.down, out hit, 20f))
        {
            top = hit.point.y;
        }

        Object.DestroyImmediate(model);
        return top;
    }

    static void Save(GameObject root, string path)
    {
        PrefabUtility.SaveAsPrefabAsset(root, path);
        Object.DestroyImmediate(root);
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }
        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
    }
}
