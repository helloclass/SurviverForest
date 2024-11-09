using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
  Dictionary<string,int> _DicSample;
  _DicSample= new Dictionary<string,int>();
  _DicSample.Add( "나이", 32 );
  _DicSample.Add( "키", 180 );

  foreach (KeyValuePair<string, int> each in _Dic )
  {
     string K = each.Key;
     int V = each.Value;
  }
 */

[Serializable]
public class NodeInfo
{
    public string nodeName;

    public Rect nodeRect;
    public bool isDraggingNode;

    public NodeInfo InputNode;
    public List<NodeInfo> OutputNode;

    public enum NodeInfoType
    {
        LeafNode,
        RootNode,
        SequenceNode,
        SelectorNode,
        ConditionNode
    };

    public NodeInfoType nodeType;

    // for Loop
    public int minCount;
    public int maxCount;

    // for Condition
    public string VariableTypeName;
    public string VariableName;
    public string CompareName;
    public string TargetName;

    protected NodeInfo()
    {
        nodeName = "";

        minCount = 1;
        maxCount = 1;

        nodeRect = new Rect(200, 200, 100, 50);
        isDraggingNode = false;

        InputNode = null;
        OutputNode = new List<NodeInfo>();
    }

    public NodeInfo(string initName)
    {
        nodeName = initName;

        minCount = 1;
        maxCount = 1;

        nodeRect = new Rect(200, 200, 100, 50);
        isDraggingNode = false;

        InputNode = null;
        OutputNode = new List<NodeInfo>();
    }

    public NodeInfo(string initName, Rect initRect)
    {
        nodeName = initName;

        minCount = 1;
        maxCount = 1;

        nodeRect = initRect;
        isDraggingNode = false;

        InputNode = null;
        OutputNode = new List<NodeInfo>();
    }

    public NodeInfo(string initName, int x, int y)
    {
        nodeName = initName;

        minCount = 1;
        maxCount = 1;

        nodeRect = new Rect(x, y, 100, 50);
        isDraggingNode = false;

        InputNode = null;
        OutputNode = new List<NodeInfo>();
    }

    public NodeInfo(string initName, int x, int y, int width, int height)
    {
        nodeName = initName;

        minCount = 1;
        maxCount = 1;

        nodeRect = new Rect(x, y, width, height);
        isDraggingNode = false;

        InputNode = null;
        OutputNode = new List<NodeInfo>();
    }
}

[Serializable]
public class RootNodeInfo : NodeInfo
{
    public RootNodeInfo()
    {
        nodeType = NodeInfoType.RootNode;
        nodeName = "Root";

        minCount = 1;
        maxCount = 1;
    }
}

[Serializable]
public class SequenceInfo : NodeInfo
{
    public SequenceInfo()
    {
        nodeType = NodeInfoType.SequenceNode;
        nodeName = "Sequence";

        minCount = 1;
        maxCount = 1;
    }
}

[Serializable]
public class SelectorInfo : NodeInfo
{
    public SelectorInfo()
    {
        nodeType = NodeInfoType.SelectorNode;
        nodeName = "Selector";

        minCount = 1;
        maxCount = 1;
    }
}

[Serializable]
public class ConditionInfo : NodeInfo
{
    public ConditionInfo()
    {
        nodeType = NodeInfoType.ConditionNode;
        nodeName = "Condition";

        minCount = 1;
        maxCount = 1;
    }
}

[Serializable]
class VariableType
{
    public string type;
    public string name;

    public VariableType()
    {
        this.type = "";
        this.name = "";
    }
    public VariableType(string type, string name)
    {
        this.type = type;
        this.name = name;
    }
}

[Serializable]
class VariableList
{
    public List<VariableType> Variables;

    public VariableList()
    {
        Variables = new List<VariableType>();
    }
}

class NodeList
{
    public RootNodeInfo RootNodeList;
    public Dictionary<string, NodeInfo> NodeLists;
    public List<SequenceInfo> SequenceLists;
    public List<SelectorInfo> SelectorLists;
    public List<ConditionInfo> ConditionLists;

    public NodeList()
    {
        RootNodeList = new RootNodeInfo();

        NodeLists = new Dictionary<string, NodeInfo>();

        SequenceLists = new List<SequenceInfo>();
        SelectorLists = new List<SelectorInfo>();
        ConditionLists = new List<ConditionInfo>();
    }
}

class Utility
{
    private string filePath;
    private string compiledFileContent;

    public VariableList variableList;
    public NodeList nodeList;

    public Utility()
    {
        variableList = new VariableList();
        nodeList = new NodeList();
    }

    public void SaveToJson()
    {
        //                   
        filePath = Path.Combine(Application.persistentDataPath, "NodeData.json");

        //            
        SaveData<NodeInfo>(nodeList.RootNodeList);

        //                   
        filePath = Path.Combine(Application.persistentDataPath, "VariableData.json");

        //            
        SaveData<VariableList>(variableList);

    }

    public void LoadFromJson()
    {
        //                   
        filePath = Path.Combine(Application.persistentDataPath, "NodeData.json");

        //            
        LoadNodeData();

        //                   
        filePath = Path.Combine(Application.persistentDataPath, "VariableData.json");

        //            
        LoadVariableData();

    }

    public void CompileFromJson(string FileName)
    {
        //                   
        filePath = Path.Combine(Application.persistentDataPath, "NodeData.json");

        //            
        CompileNodeData(FileName);

        //                   
        filePath = Path.Combine(Application.persistentDataPath, "VariableData.json");

        //            
        //LoadVariableData();

    }

    public void SaveData<T>(T data)
    {
        // JSON       ȯ
        string json = JsonUtility.ToJson(data, true);

        //    Ͽ  JSON     
        File.WriteAllText(filePath, json);

        Debug.Log("Data saved to " + filePath);
    }

    public bool LoadVariableData()
    {
        //             ϴ    Ȯ  
        if (File.Exists(filePath))
        {
            //    Ͽ    JSON  б 
            string json = File.ReadAllText(filePath);

            // JSON     ü     ȯ
            VariableList data = JsonUtility.FromJson<VariableList>(json);

            for (int idx = 0; idx < data.Variables.Count; idx++)
            {
                variableList.Variables.Add(new VariableType(
                    data.Variables[idx].type,
                    data.Variables[idx].name
                ));
            }

            return true;
        }
        else
        {
            Debug.LogError("Save file not found at " + filePath);

            return false;
        }
    }

    public NodeInfo LoadNodeToRecursion(NodeInfo parentDatas, NodeInfo datas)
    {
        if (datas.nodeType == NodeInfo.NodeInfoType.RootNode)
        {
            nodeList.RootNodeList.nodeType = datas.nodeType;

            nodeList.RootNodeList.nodeName = datas.nodeName;
            nodeList.RootNodeList.nodeRect = datas.nodeRect;
            nodeList.RootNodeList.isDraggingNode = datas.isDraggingNode;

            //nodeList.RootNodeList.InputNode = datas.InputNode;
            //nodeList.RootNodeList.OutputNode = datas.OutputNode;

            nodeList.RootNodeList.VariableTypeName = "";
            nodeList.RootNodeList.VariableName = "";
            nodeList.RootNodeList.CompareName = "";
            nodeList.RootNodeList.TargetName = "";

            nodeList.RootNodeList.InputNode = parentDatas;
            foreach (var child in datas.OutputNode)
            {
                nodeList.RootNodeList.OutputNode.Add(LoadNodeToRecursion(nodeList.RootNodeList, child));
            }

            return nodeList.RootNodeList;
        }
        else if (datas.nodeType == NodeInfo.NodeInfoType.SequenceNode)
        {
            SequenceInfo sequenceInfo = new SequenceInfo();

            sequenceInfo.nodeType = datas.nodeType;

            sequenceInfo.nodeName = datas.nodeName;
            sequenceInfo.nodeRect = datas.nodeRect;
            sequenceInfo.isDraggingNode = datas.isDraggingNode;

            //sequenceInfo.InputNode = datas.InputNode;
            //sequenceInfo.OutputNode = datas.OutputNode;

            sequenceInfo.VariableTypeName = "";
            sequenceInfo.VariableName = "";
            sequenceInfo.CompareName = "";
            sequenceInfo.TargetName = "";

            sequenceInfo.InputNode = parentDatas;
            foreach (var child in datas.OutputNode)
            {
                sequenceInfo.OutputNode.Add(LoadNodeToRecursion(sequenceInfo, child));
            }

            nodeList.SequenceLists.Add(sequenceInfo);

            return sequenceInfo;
        }
        else if (datas.nodeType == NodeInfo.NodeInfoType.SelectorNode)
        {
            SelectorInfo selectorInfo = new SelectorInfo();

            selectorInfo.nodeType = datas.nodeType;

            selectorInfo.nodeName = datas.nodeName;
            selectorInfo.nodeRect = datas.nodeRect;
            selectorInfo.isDraggingNode = datas.isDraggingNode;

            //selectorInfo.InputNode = datas.InputNode;
            //selectorInfo.OutputNode = datas.OutputNode;

            selectorInfo.VariableTypeName = "";
            selectorInfo.VariableName = "";
            selectorInfo.CompareName = "";
            selectorInfo.TargetName = "";

            selectorInfo.InputNode = parentDatas;
            foreach (var child in datas.OutputNode)
            {
                selectorInfo.OutputNode.Add(LoadNodeToRecursion(selectorInfo, child));
            }

            nodeList.SelectorLists.Add(selectorInfo);

            return selectorInfo;
        }
        else if (datas.nodeType == NodeInfo.NodeInfoType.ConditionNode)
        {
            ConditionInfo conditionInfo = new ConditionInfo();

            conditionInfo.nodeType = datas.nodeType;

            conditionInfo.nodeName = datas.nodeName;
            conditionInfo.nodeRect = datas.nodeRect;
            conditionInfo.isDraggingNode = datas.isDraggingNode;

            //conditionInfo.InputNode = datas.InputNode;
            //conditionInfo.OutputNode = datas.OutputNode;

            conditionInfo.VariableTypeName = datas.VariableTypeName;
            conditionInfo.VariableName = datas.VariableName;
            conditionInfo.CompareName = datas.CompareName;
            conditionInfo.TargetName = datas.TargetName;

            conditionInfo.InputNode = parentDatas;
            foreach (var child in datas.OutputNode)
            {
                conditionInfo.OutputNode.Add(LoadNodeToRecursion(conditionInfo, child));
            }

            nodeList.ConditionLists.Add(conditionInfo);

            return conditionInfo;
        }
        else
        {
            NodeInfo nodeInfo = new NodeInfo(datas.nodeName);

            nodeInfo.nodeType = datas.nodeType;

            nodeInfo.nodeName = datas.nodeName;
            nodeInfo.nodeRect = datas.nodeRect;
            nodeInfo.isDraggingNode = datas.isDraggingNode;

            //nodeInfo.InputNode = datas.InputNode;
            //nodeInfo.OutputNode = datas.OutputNode;

            nodeInfo.VariableTypeName = datas.VariableTypeName;
            nodeInfo.VariableName = datas.VariableName;
            nodeInfo.CompareName = datas.CompareName;
            nodeInfo.TargetName = datas.TargetName;

            nodeInfo.InputNode = parentDatas;
            foreach (var child in datas.OutputNode)
            {
                nodeInfo.OutputNode.Add(LoadNodeToRecursion(nodeInfo, child));
            }

            nodeList.NodeLists.Add(nodeInfo.nodeName, nodeInfo);

            return nodeInfo;
        }
    }

    public bool LoadNodeData()
    {
        //             ϴ    Ȯ  
        if (File.Exists(filePath))
        {
            //    Ͽ    JSON  б 
            string json = File.ReadAllText(filePath);

            // JSON     ü     ȯ
            NodeInfo data = JsonUtility.FromJson<NodeInfo>(json);
            LoadNodeToRecursion(null, data);

            return true;
        }
        else
        {
            Debug.LogError("Save file not found at " + filePath);

            return false;
        }
    }

    private int AssignedSequenceIndex = 0;

    public void CompileNodeToRecursion(NodeInfo parentDatas, NodeInfo datas)
    {
        if (datas.nodeType == NodeInfo.NodeInfoType.RootNode)
        {
            foreach (var child in datas.OutputNode)
            {
                CompileNodeToRecursion(datas, child);
            }
        }
        else if (datas.nodeType == NodeInfo.NodeInfoType.SequenceNode)
        {
            int sequenceCount = 0;
            foreach (var child in datas.OutputNode)
            {
                //compiledFileContent += "if (sequenceStateCache[" + AssignedSequenceIndex.ToString() + "] == " + sequenceCount.ToString() + ") {\n";
                CompileNodeToRecursion(datas, child);
                //compiledFileContent += "\n}\n";

                sequenceCount++;
            }

            if (parentDatas.nodeType == NodeInfo.NodeInfoType.ConditionNode)
            { 
                compiledFileContent += "\n}\n";
                compiledFileContent += "else { \n";
                compiledFileContent += "return false;\n";
            }

            AssignedSequenceIndex++;
        }
        else if (datas.nodeType == NodeInfo.NodeInfoType.SelectorNode)
        {
            compiledFileContent += "selector = Random.Range(0, " + datas.OutputNode.Count.ToString() + ");\n";

            int selectorCount = 0;
            foreach (var child in datas.OutputNode)
            {
                compiledFileContent += "if (selector == " + selectorCount.ToString() + ") {\n";
                CompileNodeToRecursion(datas, child);
                compiledFileContent += "\n}\n";

                selectorCount++;
            }

            if (parentDatas.nodeType == NodeInfo.NodeInfoType.ConditionNode)
            {
                compiledFileContent += "\n}\n";
                compiledFileContent += "else { \n";
                compiledFileContent += "return false;\n";
            }
        }
        else if (datas.nodeType == NodeInfo.NodeInfoType.ConditionNode)
        {
            compiledFileContent += "if (" + datas.VariableName + " " + datas.CompareName + " " + datas.TargetName + ") {\n";
            foreach (var child in datas.OutputNode)
            {
                CompileNodeToRecursion(datas, child);
            }

            if (parentDatas.nodeType != NodeInfo.NodeInfoType.ConditionNode)
            {
                compiledFileContent += "\n}\n";
            }

            if (parentDatas.nodeType == NodeInfo.NodeInfoType.ConditionNode)
            {
                compiledFileContent += "\n}\n";
                compiledFileContent += "\n}\n";
                compiledFileContent += "else { \n";
                compiledFileContent += "return false;\n";
            }
        }
        else
        {
            compiledFileContent +=
                "loopCount = Random.Range(" + datas.minCount + ", " + datas.maxCount + ");\n"
                + "for (int i = 0; i < loopCount; i++)\n"
                + "{\n"
                + "animName.Enqueue(\"" + datas.nodeName + "\");\n"
                + "}\n";

            foreach (var child in datas.OutputNode)
            {
                CompileNodeToRecursion(datas, child);
            }

            if (parentDatas.nodeType == NodeInfo.NodeInfoType.ConditionNode)
            {
                compiledFileContent += "}\n";
                compiledFileContent += "else { \n";
                compiledFileContent += "return false;\n";
            }
        }
    }

    // Ctrl-A -> Ctrl-K, F
    public bool CompileNodeData(string FileName)
    {
        // ������ �����ϴ��� Ȯ��
        if (File.Exists(filePath))
        {
            // ���Ͽ��� JSON �б�
            string json = File.ReadAllText(filePath);

            AssignedSequenceIndex = 0;

            compiledFileContent =
                "using System.Collections;\n" +
                "using System.Collections.Generic;\n" +
                "using UnityEngine;\n\n";


            compiledFileContent += "public class " + FileName + "\n";

            compiledFileContent +=
                "{\n"
                + "public Animator animator;\n"
                + "float prevAnimTime, curAnimTime;\n\n"

                + "const float transitionDuration = 0.25f;\n"
                + "const float boundAnimTime = 0.9f;\n\n"

                + "int selector = new int();\n"
                + "int loopCount = new int();\n"
                + "Queue<string> animName = new Queue<string>();\n\n";

            // Append Variable
            for (int vIDX = 0; vIDX < variableList.Variables.Count; vIDX++)
            {
                compiledFileContent += 
                    "public " +
                    variableList.Variables[vIDX].type + 
                    " " +
                    variableList.Variables[vIDX].name +
                    ";\n";
            }

            compiledFileContent +=
                "public " + FileName + "(Animator animator)"
                + "{\n"
                + "this.animator = animator;\n\n"

                + "prevAnimTime = 0;\n"
                + "curAnimTime = 0;\n"
                + "}\n\n"

                + "public void AnimationStart()"
                + "{\n"
                + "animator.Play(animName.Dequeue());\n"
                + "}\n\n"

                + "public void AnimationUpdate()"
                + "{\n"
                + "curAnimTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;\n\n"

                + "if (prevAnimTime < boundAnimTime && boundAnimTime <= curAnimTime)\n"
                + "{\n"
                + "     string anim;\n"

                + "     if (0 < animName.Count)\n"
                + "     {\n"
                + "         anim = animName.Dequeue();\n"
                + "         animator.CrossFade(anim, transitionDuration);\n"
                + "     }\n"

                + "     prevAnimTime = curAnimTime;\n"
                + "}\n\n"

                + "public IEnumerator AnimUpdate()\n"
                + "{\n"
                + "while (true)\n"
                + "{\n"
                + "if (0 < animName.Count)"
                + "{\n"
                + "yield return new WaitForSeconds(0.1f);\n"
                + "}\n";


            // JSON�� ��ü�� ȯ
            NodeInfo data = JsonUtility.FromJson<NodeInfo>(json);
            data.nodeType = NodeInfo.NodeInfoType.RootNode;
            CompileNodeToRecursion(null, data);

            compiledFileContent += "\n}\n";
            compiledFileContent += "\n}\n";
            compiledFileContent += "\n}\n";

            string resultFilePath = Path.Combine(Application.dataPath, FileName + ".cs");
            File.WriteAllText(resultFilePath, compiledFileContent);

            Debug.Log("Compile Success! ( " + resultFilePath + " )");

            return true;
        }
        else
        {
            Debug.LogError("Compile file not found at " + filePath);

            return false;
        }

        return false;
    }
}

public class BehaviourTreee : EditorWindow
{
    private Utility util;

    private string SelectedNodeName;
    private NodeInfo SelectedNode;

    public float zoomScale = 1.0f; // 기본 줌 스케일
    private Vector2 panOffset = Vector2.zero; // 화면 이동 오프셋
    private Vector2 lastMousePosition = Vector2.zero;

    private GUIStyle boxStyle;
    private Vector2 dragOffset;

    // Line
    private Texture2D lineTexture;

    private bool isFindConnectedTarget;
    private string newFileName;
    private string newNodeName;
    private string convertNodeName;
    private string newVariableName;
    private string targetValueName;

    private string minValueName;
    private string maxValueName;

    // Variable Type
    private string[] TypeItem = { "bool", "int", "float", "string" };
    private bool foldout;
    private string selectType;

    // Variable List
    private bool foldoutOfVariable;
    private VariableType selectTypeOfVariable;

    // Compare Type
    private string[] CompareTypeItem = { "==", "<", ">", "<=", ">=" };
    private bool foldoutOfCompare;
    private string selectTypeOfCompare;

    // %g -> 단축키설정 (ctrl + g)
    [MenuItem("MyTool/OpenTool %g")]
    static void Open()
    {
        var window = GetWindow<BehaviourTreee>("BehaviourTree");
        window.minSize = new Vector2(0.0f, 0.0f);
        window.maxSize = new Vector2(10000.0f, 10000.0f);
    }

    /*
        // 런타임/Editor 사용 가능
        GUI.Label(new Rect(..), "텍스트");
        // with Layout
        GUILayout.Label("텍스트");

        // only Editor 사용 가능
        EditorGUI.LabelField(new Rect(..), "텍스트");
        // with Layout
        EditorLayout.LableField("텍스트");
     */

    private void CreateGUI()
    {
        SelectedNodeName = "";
        SelectedNode = null;

        util = new Utility();

        foldout = new bool();
        selectType = new string("Select Type");

        foldoutOfVariable = new bool();
        selectTypeOfVariable = new VariableType();
        selectTypeOfVariable.name = "Select Value";

        foldoutOfCompare = new bool();
        selectTypeOfCompare = new string("Select Compare");

        isFindConnectedTarget = false;

        newFileName = new string("");
        newNodeName = new string("");
        convertNodeName = new string("");
        newVariableName = new string("");

    }

    private void OnGUI()
    {
        // GUIStyle 초기화
        if (boxStyle == null)
        {
            boxStyle = new GUIStyle(GUI.skin.box);

            // 배경색 설정
            Texture2D backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, Color.blue); // 원하는 색으로 설정 (예: 파란색)
            backgroundTexture.Apply();

            boxStyle.normal.background = backgroundTexture;

            // 텍스트 색상 설정
            boxStyle.normal.textColor = Color.white;
        }

        GUILayout.BeginArea(new Rect(30, 30, 200, 1000));

        newFileName = GUILayout.TextField(newFileName);
        if (GUILayout.Button("Save"))
        {
            util.SaveToJson();
        }
        if (GUILayout.Button("Load"))
        {
            util.LoadFromJson();
        }
        if (GUILayout.Button("Compile"))
        {
            util.CompileFromJson(newFileName);
        }

        GUILayout.Space(10);

        newNodeName = GUILayout.TextField(newNodeName);
        if (GUILayout.Button("New Node"))
        {
            bool isCreateAble = true;

            if (newNodeName == "")
            {
                isCreateAble = false;
            }
            foreach (KeyValuePair<string, NodeInfo> node in util.nodeList.NodeLists)
            {
                if (newNodeName == node.Value.nodeName)
                {
                    isCreateAble = false;
                    break;
                }
            }

            if (isCreateAble)
            {
                NodeInfo newNode = new NodeInfo(newNodeName, 100, 100);
                util.nodeList.NodeLists.Add(newNode.nodeName, newNode);
            }

            newNodeName = "";
        }

        if (GUILayout.Button("New Sequence"))
        {
            util.nodeList.SequenceLists.Add(new SequenceInfo());
        }

        if (GUILayout.Button("New Select"))
        {
            util.nodeList.SelectorLists.Add(new SelectorInfo());
        }

        if (GUILayout.Button("New Condition"))
        {
            util.nodeList.ConditionLists.Add(new ConditionInfo());
        }

        GUILayout.Space(10);

        GUILayout.Label("Variable");

        foldout = EditorGUILayout.Foldout(foldout, selectType);

        // 항목이 펼쳐진 경우, 추가적인 옵션을 표시
        if (foldout)
        {
            foreach(string name in TypeItem)
            {
                if(GUILayout.Button(name))
                {
                    foldout = false;
                    selectType = name;
                }
            }
        }

        newVariableName = GUILayout.TextField(newVariableName);

        if (GUILayout.Button("New Var"))
        {
            VariableType newVariable = new VariableType(selectType, newVariableName);

            bool isCreateAble = true;

            if (newVariable.name == "")
            {
                isCreateAble = false;
            }

            foreach (VariableType v in util.variableList.Variables)
            {
                if (newVariable.name == v.name)
                {
                    isCreateAble = false;
                    break;
                }
            }

            if (isCreateAble)
            {
                util.variableList.Variables.Add(newVariable);
            }
        }

        GUILayout.Space(10);

        foreach (VariableType v in util.variableList.Variables)
        {
            GUILayout.Label("<" + v.type + "> " + v.name);
        }


        GUILayout.EndArea();

        // Variable Name
        // Create Button

        // Variable List

        GUILayout.Space(10);

        GUILayout.BeginArea(new Rect(260, 30, 200, 1000));

        if (SelectedNode != null)
        {
            GUILayout.Label(SelectedNode.nodeName);
            GUILayout.Label("min : " + SelectedNode.minCount.ToString());
            GUILayout.Label("max : " + SelectedNode.maxCount.ToString());

            minValueName = GUILayout.TextField(minValueName);
            maxValueName = GUILayout.TextField(maxValueName);

            if (GUILayout.Button("Loop Change"))
            {
                SelectedNode.minCount = int.Parse(minValueName);
                SelectedNode.maxCount = int.Parse(maxValueName);

                Debug.Log("WQA!");
            }

            if (GUILayout.Button("Delete"))
            {
                // Clear Edge
                if(SelectedNode.InputNode != null)
                {
                    SelectedNode.InputNode.OutputNode.Remove(SelectedNode);
                }
                SelectedNode.OutputNode.Clear();

                if (SelectedNode.nodeType == NodeInfo.NodeInfoType.SequenceNode)
                {
                    util.nodeList.SequenceLists.Remove((SequenceInfo)SelectedNode);
                }
                else if (SelectedNode.nodeType == NodeInfo.NodeInfoType.SelectorNode)
                {
                    util.nodeList.SelectorLists.Remove((SelectorInfo)SelectedNode);
                }
                else if (SelectedNode.nodeType == NodeInfo.NodeInfoType.ConditionNode)
                {
                    util.nodeList.ConditionLists.Remove((ConditionInfo)SelectedNode);
                }
                else
                {
                    util.nodeList.NodeLists.Remove(SelectedNodeName);
                }

                SelectedNodeName = "";
                SelectedNode = null;
            }

            convertNodeName = GUILayout.TextField(convertNodeName);
            if (GUILayout.Button("Change Name"))
            {
                bool isConvertAble = true;

                if (convertNodeName == "")
                {
                    isConvertAble = false;
                }
                foreach (KeyValuePair<string, NodeInfo> node in util.nodeList.NodeLists)
                {
                    if (convertNodeName == node.Value.nodeName)
                    {
                        isConvertAble = false;
                        break;
                    }
                }

                if (isConvertAble)
                {
                    SelectedNode.nodeName = convertNodeName;
                }

                convertNodeName = "";
            }

            if (SelectedNode.nodeType != NodeInfo.NodeInfoType.LeafNode)
            {
                if (GUILayout.Button("Connet Node"))
                {
                    isFindConnectedTarget = true;
                }
            }

            GUILayout.Space(10);

            if (SelectedNode.nodeType == NodeInfo.NodeInfoType.ConditionNode)
            {
                ConditionInfo condition = (ConditionInfo)SelectedNode;

                // Select Value
                foldoutOfVariable = EditorGUILayout.Foldout(foldoutOfVariable, selectTypeOfVariable.name);

                // 항목이 펼쳐진 경우, 추가적인 옵션을 표시
                if (foldoutOfVariable)
                {
                    foreach (VariableType v in util.variableList.Variables)
                    {
                        if (GUILayout.Button(v.name))
                        {
                            foldoutOfVariable = false;
                            selectTypeOfVariable = v;
                        }
                    }
                }

                // Compare Value
                foldoutOfCompare = EditorGUILayout.Foldout(foldoutOfCompare, selectTypeOfCompare);

                // 항목이 펼쳐진 경우, 추가적인 옵션을 표시
                if (foldoutOfCompare)
                {
                    foreach (string name in CompareTypeItem)
                    {
                        if (GUILayout.Button(name))
                        {
                            foldoutOfCompare = false;
                            selectTypeOfCompare = name;
                        }
                    }
                }

                // Target
                targetValueName = GUILayout.TextField(targetValueName);

                if (GUILayout.Button("Set"))
                {
                    condition.VariableTypeName = selectTypeOfVariable.type;
                    condition.VariableName = selectTypeOfVariable.name;
                    condition.CompareName = selectTypeOfCompare;
                    condition.TargetName = targetValueName;
                }

                GUILayout.Space(10);

                GUILayout.Label("Operator :");
                GUILayout.Label("<" + condition.VariableTypeName + ">");
                GUILayout.Label(condition.VariableName + " " + condition.CompareName + " " + condition.TargetName);
            }
        }

        GUILayout.EndArea();

        GUI.BeginGroup(new Rect(130, 30, 30000, 30000));

        ProcessEdgeEvents(Event.current);

        // 화면 줌과 팬 적용
        GUI.matrix = Matrix4x4.TRS(
            new Vector3(panOffset.x, panOffset.y, 0),
            Quaternion.identity,
            new Vector3(zoomScale, zoomScale, 1)
        );

        ProcessNodeEvents(Event.current);

        GUI.EndGroup();

        HandleMouseInput();
    }

    private Vector2 previousMousePosition;
    private Vector2 currentMousePosition;

    private void HandleMouseInput()
    {
        if (Event.current.type == EventType.MouseDown)
        {
            previousMousePosition = Event.current.mousePosition;
        }
        else if (Event.current.type == EventType.MouseDrag && Event.current.button == 1)
        {
            // 마우스 드래그 중일 때 카메라 이동
            currentMousePosition = Event.current.mousePosition;

            Vector2 delta = previousMousePosition - currentMousePosition;
            previousMousePosition = currentMousePosition;

            panOffset -= delta;
        }

        lastMousePosition = Event.current.mousePosition;
    }

    private void ProcessNodeEvents(Event e)
    {
        // RootNode
        {
            DrawNodePins(util.nodeList.RootNodeList.nodeRect, util.nodeList.RootNodeList.nodeName, Color.white, Color.black, Color.red);

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (util.nodeList.RootNodeList.nodeRect.Contains(e.mousePosition))
                    {
                        if (isFindConnectedTarget)
                        {
                            isFindConnectedTarget = false;

                            if (SelectedNode != null)
                            {
                                util.nodeList.RootNodeList.InputNode = SelectedNode;
                                SelectedNode.OutputNode.Add(util.nodeList.RootNodeList);
                            }

                            break;
                        }

                        util.nodeList.RootNodeList.isDraggingNode = true;
                        dragOffset = 
                            e.mousePosition - 
                            new Vector2(util.nodeList.RootNodeList.nodeRect.x, util.nodeList.RootNodeList.nodeRect.y);
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (util.nodeList.RootNodeList.nodeRect.Contains(e.mousePosition))
                    {
                        SelectedNodeName = "Root";
                        SelectedNode = util.nodeList.RootNodeList;
                        util.nodeList.RootNodeList.isDraggingNode = false;
                    }
                    break;

                case EventType.MouseDrag:
                    if (util.nodeList.RootNodeList.isDraggingNode)
                    {
                        util.nodeList.RootNodeList.nodeRect.position = e.mousePosition - dragOffset;
                        e.Use();
                    }
                    break;
            }
        }

        foreach (KeyValuePair<string, NodeInfo> node in util.nodeList.NodeLists)
        {
            DrawNodePins(node.Value.nodeRect, node.Value.nodeName, Color.grey, Color.black, Color.yellow);

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (node.Value.nodeRect.Contains(e.mousePosition))
                    {
                        if (isFindConnectedTarget)
                        {
                            isFindConnectedTarget = false;

                            if (SelectedNode != null)
                            {
                                node.Value.InputNode = SelectedNode;
                                SelectedNode.OutputNode.Add(node.Value);
                            }

                            break;
                        }

                        node.Value.isDraggingNode = true;
                        dragOffset = e.mousePosition - new Vector2(node.Value.nodeRect.x, node.Value.nodeRect.y);
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (node.Value.nodeRect.Contains(e.mousePosition))
                    {
                        SelectedNodeName = node.Key;
                        SelectedNode = node.Value;
                        node.Value.isDraggingNode = false;
                    }
                    break;

                case EventType.MouseDrag:
                    if (node.Value.isDraggingNode)
                    {
                        node.Value.nodeRect.position = e.mousePosition - dragOffset;
                        e.Use();
                    }
                    break;
            }
        }

        // Sequence
        foreach (SequenceInfo node in util.nodeList.SequenceLists)
        {
            DrawNodePins(node.nodeRect, node.nodeName, Color.red, Color.black, Color.yellow);

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (node.nodeRect.Contains(e.mousePosition))
                    {
                        if (isFindConnectedTarget)
                        {
                            isFindConnectedTarget = false;

                            if (SelectedNode != null)
                            {
                                node.InputNode = SelectedNode;
                                SelectedNode.OutputNode.Add(node);
                            }

                            break;
                        }

                        node.isDraggingNode = true;
                        dragOffset = e.mousePosition - new Vector2(node.nodeRect.x, node.nodeRect.y);
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (node.nodeRect.Contains(e.mousePosition))
                    {
                        SelectedNodeName = "Sequence";
                        SelectedNode = node;
                        node.isDraggingNode = false;
                    }
                    break;

                case EventType.MouseDrag:
                    if (node.isDraggingNode)
                    {
                        node.nodeRect.position = e.mousePosition - dragOffset;
                        e.Use();
                    }
                    break;
            }
        }

        // Selector
        foreach (SelectorInfo node in util.nodeList.SelectorLists)
        {
            DrawNodePins(node.nodeRect, node.nodeName, Color.blue, Color.black, Color.yellow);

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (node.nodeRect.Contains(e.mousePosition))
                    {
                        if (isFindConnectedTarget)
                        {
                            isFindConnectedTarget = false;

                            if (SelectedNode != null)
                            {
                                node.InputNode = SelectedNode;
                                SelectedNode.OutputNode.Add(node);
                            }

                            break;
                        }

                        node.isDraggingNode = true;
                        dragOffset = e.mousePosition - new Vector2(node.nodeRect.x, node.nodeRect.y);
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (node.nodeRect.Contains(e.mousePosition))
                    {
                        SelectedNodeName = "Selector";
                        SelectedNode = node;
                        node.isDraggingNode = false;
                    }
                    break;

                case EventType.MouseDrag:
                    if (node.isDraggingNode)
                    {
                        node.nodeRect.position = e.mousePosition - dragOffset;
                        e.Use();
                    }
                    break;
            }
        }

        // Condition
        foreach (ConditionInfo node in util.nodeList.ConditionLists)
        {
            DrawNodePins(node.nodeRect, node.nodeName, Color.green, Color.black, Color.yellow);

            switch (e.type)
            {
                case EventType.MouseDown:
                    if (node.nodeRect.Contains(e.mousePosition))
                    {
                        if (isFindConnectedTarget)
                        {
                            isFindConnectedTarget = false;

                            if (SelectedNode != null)
                            {
                                node.InputNode = SelectedNode;
                                SelectedNode.OutputNode.Add(node);
                            }

                            break;
                        }

                        node.isDraggingNode = true;
                        dragOffset = e.mousePosition - new Vector2(node.nodeRect.x, node.nodeRect.y);
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (node.nodeRect.Contains(e.mousePosition))
                    {
                        SelectedNodeName = "Condition";
                        SelectedNode = node;
                        node.isDraggingNode = false;
                    }
                    break;

                case EventType.MouseDrag:
                    if (node.isDraggingNode)
                    {
                        node.nodeRect.position = e.mousePosition - dragOffset;
                        e.Use();
                    }
                    break;
            }
        }
    }

    private void ProcessEdgeEvents(Event e)
    {
        // RootNode
        foreach (NodeInfo outNode in util.nodeList.RootNodeList.OutputNode)
        {
            DrawLine(
                GetOutputPosition(util.nodeList.RootNodeList.nodeRect),
                GetInputPosition(outNode.nodeRect),
                Color.white,
                2
            );
        }

        foreach (KeyValuePair<string, NodeInfo> node in util.nodeList.NodeLists)
        {
            foreach (NodeInfo outNode in node.Value.OutputNode)
            {
                DrawLine(
                    GetOutputPosition(node.Value.nodeRect),
                    GetInputPosition(outNode.nodeRect),
                    Color.white,
                    2
                );
            }
        }

        // Sequence
        foreach (SequenceInfo node in util.nodeList.SequenceLists)
        {
            foreach (NodeInfo outNode in node.OutputNode)
            {
                DrawLine(
                    GetOutputPosition(node.nodeRect),
                    GetInputPosition(outNode.nodeRect),
                    Color.white,
                    2
                );
            }
        }

        // Selector
        foreach (SelectorInfo node in util.nodeList.SelectorLists)
        {
            foreach (NodeInfo outNode in node.OutputNode)
            {
                DrawLine(
                    GetOutputPosition(node.nodeRect),
                    GetInputPosition(outNode.nodeRect),
                    Color.white,
                    2
                );
            }
        }

        // Condition
        foreach (ConditionInfo node in util.nodeList.ConditionLists)
        {
            foreach (NodeInfo outNode in node.OutputNode)
            {
                DrawLine(
                    GetOutputPosition(node.nodeRect),
                    GetInputPosition(outNode.nodeRect),
                    Color.white,
                    2
                );
            }
        }
    }

    //// 화면 줌과 팬 적용
    //GUI.matrix = Matrix4x4.TRS(
    //        new Vector3(panOffset.x, panOffset.y, 0),
    //        Quaternion.identity,
    //        new Vector3(zoomScale, zoomScale, 1)
    //    );

    private Vector2 GetInputPosition(Rect nodeRect)
    {
        Vector2 Center = new Vector2(5 + panOffset.x, 5 + panOffset.y);
        return new Vector2(nodeRect.x - 10 + Center.x, nodeRect.y + 20 + Center.y);
    }

    private Vector2 GetOutputPosition(Rect nodeRect)
    {
        Vector2 Center = new Vector2(5 + panOffset.x, 5 + panOffset.y);
        return new Vector2(nodeRect.xMax - 10 + Center.x, nodeRect.y + 20 + Center.y);
    }

    private void SetGuiStyle(Color pannelColor, Color textColor, out GUIStyle style)
    {
        style = new GUIStyle(GUI.skin.box);

        // 배경색 설정
        Texture2D backgroundTexture = new Texture2D(1, 1);
        backgroundTexture.SetPixel(0, 0, pannelColor); // 원하는 색으로 설정 (예: 파란색)
        backgroundTexture.Apply();

        style.normal.background = backgroundTexture;

        // 텍스트 색상 설정
        style.normal.textColor = textColor;
    }

    private void DrawNodePins(Rect nodeRect, string nodeName, Color mainNodeColor, Color mainNodeTextColor, Color pinColor)
    {
        GUIStyle mainNodeBoxStyle;
        GUIStyle pinBoxStyle;

        SetGuiStyle(mainNodeColor, mainNodeTextColor, out mainNodeBoxStyle);
        SetGuiStyle(pinColor, Color.white, out pinBoxStyle);

        GUI.Box(nodeRect, nodeName, mainNodeBoxStyle);

        if (nodeName != "Root")
        {
            // 입력 핀
            Rect inputPin = new Rect(nodeRect.x - 5, nodeRect.y + 20, 10, 10);
            GUI.Box(inputPin, "", pinBoxStyle);
        }

        // 출력 핀
        Rect outputPin = new Rect(nodeRect.xMax - 5, nodeRect.y + 20, 10, 10);
        GUI.Box(outputPin, "", pinBoxStyle);
    }

    private void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
    {
        // 라인 텍스쳐 초기화
        if (lineTexture == null)
        {
            lineTexture = new Texture2D(1, 1);
            lineTexture.SetPixel(0, 0, color);
            lineTexture.Apply();
        }

        Vector2 delta = pointB - pointA;
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

        GUIUtility.RotateAroundPivot(angle, pointA);

        GUI.DrawTexture(new Rect(pointA.x, pointA.y, delta.magnitude, width), lineTexture);

        GUIUtility.RotateAroundPivot(-angle, pointA);
    }
}
