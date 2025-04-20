using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO; 
using Newtonsoft.Json; 
using TMPro;
using UnityEngine.UI;
using JetBrains.Annotations;

public class QLearning : MonoBehaviour
{
    public TextMeshProUGUI qTableDisplay;
    public TextMeshProUGUI responseDisplay;
    public TextMeshProUGUI qTableText; // overall table
    private GretaController gretaController;
    private ExpressionClient userClassification;
    private Nuzzle nuzzleAction;
    private ExcitedBehaviour excitedScript;
    private Dictionary<(int, int), float> QTable = new Dictionary<(int, int), float>();
    private System.Random random = new System.Random();

    public bool interactionRunning = false;

    public int state { get; private set; }
    public int userID = 1;
    public TMP_InputField idInput;
    private int lastAction = -1; // store last action performed
    private int[] actionsState1 = {0, 1, 2}; // just follow, follow high gait, jump and follow
    private int[] actionsState2 = {0, 1, 2}; // nuzzle, headbutt, jump up
    string[] stringActions1 = { "Normal Gait Walk", "High Gait Walk", "Excited Behaviour" };
string[] stringActions2 = { "Nuzzle", "Headbutt", "Jump Up" };

    private float? reward1 = null;
    private float? reward2 = null;

    private float alpha = 0.1f;  // Learning rate
    private float gamma = 0.9f;  // Discount factor
    private float epsilon = 0.9f; // Exploration rate

    //private string SavePath => $"C:/Users/arian/My project (1)/Assets/qtable_user{userID}.json";
    private string SavePath;
    public class QTableEntry
    {
        public int State;
        public int Action;
        public float QValue;
    }
    void Start()
    {
        gretaController = GetComponent<GretaController>();
        userClassification = GetComponent<ExpressionClient>();
        nuzzleAction = GetComponent<Nuzzle>();
        excitedScript = GetComponent<ExcitedBehaviour>();
        state = 1; // initial state (person detected)

        if (idInput != null) 
        {
            idInput.onEndEdit.AddListener(EnterId);
        }       
    }
    private void EnterId(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        userID = int.Parse(id);
        SavePath = $"C:/Users/arian/My project (1)/Assets/qtable_user{id}.json";
        LoadQTable();
    }
    public int ChooseAction() // choose based on Q-learning
    {
        int[] availableActions = state == 1 ? actionsState1 : actionsState2; // gets actions for state 1 or state 2
        if (random.NextDouble() < epsilon) // exploration
        {
            return availableActions[random.Next(availableActions.Length)];
        }
        else // exploitation
        {
            int bestAction = availableActions[0]; // initialise with first action
            float bestValue = float.MinValue; // initialise with minimum value
            
            foreach (int action in availableActions) // iterate through actions to find best value
            {
                float value = QTable.GetValueOrDefault((state, action), 0); // get value from Q table or initialise with 0
                if (value > bestValue)
                {
                    bestValue = value;
                    bestAction = action;
                }
            }
            return bestAction;
        }
    }
    public void UpdateQTableDisplay()
    {
        // int[] availableActions = state == 1 ? actionsState1 : actionsState2; // gets actions for state 1 or state 2
        // string displayText = $"State: {state}\n";
        float value = QTable.GetValueOrDefault((state, lastAction), 0);
        string actionText = "";

        if (state == 1)
        {
            if (lastAction == 0) actionText = $"State 1 - Normal Gait Walk:\n New Q-Value: {value}\n";
            else if (lastAction == 1) actionText = $"State 1 - High Gait Walk:\n New Q-Value: {value}\n";
            else if (lastAction == 2) actionText = $"State 1 - Excited Behaviour:\n New Q-Value: {value}\n";
        } 
        else if (state == 2)
        {
            if (lastAction == 0) actionText = $"State 2 - Nuzzle:\n New Q-Value: {value}\n";
            else if (lastAction == 1) actionText = $"State 2 - Headbutt:\n New Q-Value: {value}\n";
            else if (lastAction == 2) actionText = $"State 2 - Jump Up:\n New Q-Value: {value}\n";
        }
        qTableDisplay.text += actionText;
    }
    public void ExecuteAction(int action)
    {
        interactionRunning = true;
        lastAction = action;
        switch (action)
        {
            case 0:
                if (state == 1) {
                    // Debug.Log("Follow Behaviour: 0");
                    } else {
                        nuzzleAction.StartNuzzle(3f, 30f, 60f, -20f, 20f);
                        // Debug.Log("Nuzzle: 0");
                    }
                break;
            case 1:
                if (state == 1) {
                    // Debug.Log("Follow High Gait Behaviour: 1");
                } else {
                    nuzzleAction.StartNuzzle(2f, 0f, 60f, 0f, 0f);
                    // add leg movement
                    // Debug.Log("Headbutt: 1");
                }
                break;
            case 2:
                if (state == 1) {
                    excitedScript.StartExcitement();
                    // Debug.Log("Jump and Follow Behaviour: 2");
                } else {
                    excitedScript.StartExcitement();
                    // Debug.Log("Jump Up: 2");
                }
                break;
            default:
                Debug.Log("Invalid action: " + action);
                break;
        }
    }
    // update Q-table
    public void UpdateQTable(float reward)
    {
        if (lastAction == -1) return; // no action performed

        int nextState = GetNextState();
        int bestNextAction = GetBestNextAction(nextState);

        // update Q table: 
        (int, int) currentSA = (state, lastAction); // current state-action pair
        if (!QTable.ContainsKey(currentSA))
        {
            QTable[currentSA] = 0;
        }
        float oldQValue = QTable[currentSA];
        float futureQ = QTable.GetValueOrDefault((nextState, bestNextAction), 0);
        
        // Q(s, a) = Q(s, a) + α * [r + γ * max Q(s', a') - Q(s, a)]
        QTable[currentSA] = oldQValue + alpha * (reward + gamma * futureQ - oldQValue);

        Debug.Log($"Updated Q-table: State {state}, Action {lastAction}, New Q-Value: {QTable[currentSA]}");

        UpdateQTableDisplay(); // update ui when action is executed

        if (state == 1){
            reward1 = reward;
        }
        if (state == 2){
            reward2 = reward;
        }
        state = nextState;

        if (state == 0)
        {
            Debug.Log("Saving Q-table...");
            SaveQTable();
        }
        // print users overall response: +ve, -ve or mixed
        OverallResponse();
        // once both rewards have been collected, display Q-table
        DisplayQTable();
        // if (reward1.HasValue && reward2.HasValue)
        // {
        //     DisplayQTable();
        // }
    }    
    public void OverallResponse()
    {
        string overallResponse = "";
        if (reward1.HasValue && reward2.HasValue)
        {
            if (reward1 > 0 && reward2 > 0)
            {
                overallResponse = "Positive";
            }
            else if (reward1 < 0 && reward2 < 0)
            {
                overallResponse = "Negative";
            }
            else
            {
                overallResponse = "Mixed";
            }
        }
        // display overall response
        responseDisplay.text = overallResponse;
    }
    private int GetNextState()
    {
        // move to state 2 when robot reaches person i.e is user within x distance of user
        if (state == 1)
        {
            return 2;
        }
        if (state == 2 && !gretaController.IsPersonDetected())
        {
            return 1;
        }
        return 0; // if state == 0 terminate learning
    }
    int GetBestNextAction(int nextState)
    {
        int[] availableActions = nextState == 1 ? actionsState1 : actionsState2; // gets actions for state 1 or state 2
        int bestAction = availableActions[0]; // initialise with first action
        float bestValue = float.MinValue; // initialise with minimum value
        
        foreach (int action in availableActions) // iterate through actions to find best value
        {
            float value = QTable.GetValueOrDefault((nextState, action), 0); // get value from Q table or initialise with 0
            if (value > bestValue)
            {
                bestValue = value;
                bestAction = action;
            }
        }
        return bestAction;
    }
    public
     bool GoalStateIsReached(int currentState)
    {
        return currentState == 0;
    }

    private void SaveQTable() // save Q-table to file
    {
        List<QTableEntry> serialiseQTable = new List<QTableEntry>();

        foreach (var entry in QTable)
        {
            serialiseQTable.Add(new QTableEntry
            {
                State = entry.Key.Item1,
                Action = entry.Key.Item2,
                QValue = entry.Value
            });
        }

        string json = JsonConvert.SerializeObject(serialiseQTable, Formatting.Indented);
        File.WriteAllText(SavePath, json);
        }
    private void LoadQTable()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            var serialiseQTable = JsonConvert.DeserializeObject<List<QTableEntry>>(json);

            QTable.Clear();

            foreach (var entry in serialiseQTable)
            {
                QTable[(entry.State, entry.Action)] = entry.QValue;
            }
            Debug.Log("Q-table loaded successfully.");
        }
        else
        {
            Debug.Log("No previous Q-table found, starting fresh.");
        }
    }
    private void DisplayQTable()
    {
        string result = "";
        for (int s = 1; s<=2; s++)
        {
            result += $"State {s}:\n";
            for (int a = 0; a<=2; a++)
            {
                string actionName = (s == 1) ? stringActions1[a] : stringActions2[a];
                float value = QTable.GetValueOrDefault((s, a), 0);
                result += $"{actionName}: {value:F2}\n";
            }
            result += "\n";
        }
        qTableText.text = result;
    }
}
