using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using Archipelago.MultiClient.Net.MessageLog.Parts;
using Archipelago.MultiClient.Net.Packets;
using Archipelago.MultiClient.Net.Models;

public class GameManager : MonoBehaviour
{
    public static GameManager I;
    AudioSource audioSource;
    private void Awake()
    {
        if (I != null) Destroy(this);
        I = this;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        ProcessItems();

        textClient.color = new UnityEngine.Color(0,0,Random.value*0.01f); //bad fix for text not updating
    }

    //---shapes---

    public GameObject shapePref;
    public Transform shapeParent;
    public Transform holeParent;
    public Transform shapeSpawnPoint;
    public void SpawnShape(string name, Shape.Type type, long itemId)
    {
        var shape = Instantiate(shapePref, shapeSpawnPoint.transform.position, Quaternion.identity, shapeParent);

        shape.transform.position += new Vector3(Random.Range(-25f, 25f), Random.Range(-25f, 25f),0);
        shape.GetComponent<Shape>().type = type;
        shape.GetComponent<Shape>().itemId = itemId;
        shape.name = name;
    }

    public AudioClip[] goodSounds;
    public AudioClip[] badSounds;
    public void PlayGoodSound()
    {
        audioSource.PlayOneShot(goodSounds[Random.Range(0, goodSounds.Length)]);
    }
    public void PlayBadSound()
    {
        audioSource.PlayOneShot(badSounds[Random.Range(0, badSounds.Length)]);
    }

    //---holes---

    public Hole squareHole;
    public Hole circleHole;
    public Hole triangleHole;
    public Hole pentagonHole;

    void ResetGame()
    {
        LockHole(Shape.Type.square);
        LockHole(Shape.Type.circle);
        LockHole(Shape.Type.triangle);
        LockHole(Shape.Type.pentagon);
    }

    public bool UnlockedType(Shape.Type type)
    {
        if (type == Shape.Type.square) return squareHole.unlocked;
        if (type == Shape.Type.circle) return circleHole.unlocked;
        if (type == Shape.Type.triangle) return triangleHole.unlocked;
        if (type == Shape.Type.pentagon) return pentagonHole.unlocked;
        return false;
    }

    void UnlockHole(Shape.Type type)
    {
        if (type == Shape.Type.square) squareHole.UnlockHole();
        if (type == Shape.Type.circle) circleHole.UnlockHole();
        if (type == Shape.Type.triangle) triangleHole.UnlockHole();
        if (type == Shape.Type.pentagon) pentagonHole.UnlockHole();

        //add hole unlock vfx here?
    }

    void LockHole(Shape.Type type)
    {
        if (type == Shape.Type.square) squareHole.LockHole();
        if (type == Shape.Type.circle) circleHole.LockHole();
        if (type == Shape.Type.triangle) triangleHole.LockHole();
        if (type == Shape.Type.pentagon) pentagonHole.LockHole();
    }

    //---ap stuff---

    public TMP_InputField server;
    public TMP_InputField port;
    public TMP_InputField slotName;
    public TMP_InputField pass;

    public TextMeshProUGUI textClient;

    ArchipelagoSession session;
    LoginSuccessful loginSuccess;
    bool connected;
    string GAME_NAME = "Shapesanity 1.5: Final Mix";

    public void Connect()
    {
        ResetGame();

        session = ArchipelagoSessionFactory.CreateSession("localhost", 38281);
       // session = ArchipelagoSessionFactory.CreateSession(server.text, int.Parse(port.text));

        LoginResult result = session.TryConnectAndLogin(GAME_NAME, "Player1", ItemsHandlingFlags.AllItems);
        //LoginResult result = session.TryConnectAndLogin(GAME_NAME, slotName.text, ItemsHandlingFlags.AllItems);

        session.Items.ItemReceived += OnItemReceived;
        session.Socket.SocketClosed += OnSocketClosed;
        session.MessageLog.OnMessageReceived += OnMessageReceived;

        if (!result.Successful)
        {
            LoginFailure failure = (LoginFailure)result;
            string errorMessage = $"Failed to Connect to {server.text}:{port.text} as {slotName.text}:";

            foreach (string error in failure.Errors)
            {
                errorMessage += $"\n    {error}";
            }
            foreach (ConnectionRefusedError error in failure.ErrorCodes)
            {
                errorMessage += $"\n    {error}";
            }

            AddMessage("<color=red>" + errorMessage + "</color>");

            return;
        }

        loginSuccess = (LoginSuccessful)result;
        connected = true;
    }

    private void OnMessageReceived(LogMessage message)
    {
        string msg = "";
        foreach (var part in message.Parts)
        {
            switch (part)
            {
                case ItemMessagePart itemMessagePart:
                    var itemId = itemMessagePart.ItemId;
                    var flags = itemMessagePart.Flags;
                    break;
                case LocationMessagePart locationMessagePart:
                    var locationId = locationMessagePart.LocationId;
                    break;
                case PlayerMessagePart playerMessagePart:
                    var slotId = playerMessagePart.SlotId;
                    var isCurrentPlayer = playerMessagePart.IsActivePlayer;
                    break;
            }
            string red = part.Color.R.ToString("X").PadLeft(2, '0');
            string green = part.Color.G.ToString("X").PadLeft(2, '0');
            string blue = part.Color.B.ToString("X").PadLeft(2, '0');
            string hexColor = red + green + blue;
            msg += $"<color=#{hexColor}>{part.Text}</color>";
        }
        //textClient.text +="\n"+msg;
        AddMessage(msg);
        
    }

    public void AddMessage(string msg)
    {
        textClient.SetText(textClient.text + "\n"+ msg);
    }

    void OnSocketClosed(string reason)
    {
        connected = false;
    }

    List<long> itemQueue = new List<long>();

    void OnItemReceived(ReceivedItemsHelper helper)
    {
        ItemInfo itemInfo = helper.PeekItem();
        itemQueue.Add(itemInfo.ItemId);
        helper.DequeueItem();
    }

    void ProcessItems()
    {
        if (itemQueue.Count>0)
        {
            ReceiveItem(itemQueue[0]);
            itemQueue.RemoveAt(0);
        }
    }

    void ReceiveItem(long id)
    {
        if (id.ToString()[0] == '5') //not a shape
        {
            if (id == 500001) UnlockHole(Shape.Type.square);
            else if (id == 500002) UnlockHole(Shape.Type.circle);
            else if (id == 500003) UnlockHole(Shape.Type.triangle);
            else if (id == 500004) UnlockHole(Shape.Type.pentagon);
            else if (id == 500067) { } //filler item
        }
        else //is a shape
        {
            Shape.Type shapeType = Shape.Type.square;
            if (id.ToString()[0] == '1') shapeType = Shape.Type.square;
            if (id.ToString()[0] == '2') shapeType = Shape.Type.circle;
            if (id.ToString()[0] == '3') shapeType = Shape.Type.triangle;
            if (id.ToString()[0] == '4') shapeType = Shape.Type.pentagon;

            SpawnShape(session.Items.GetItemName(id), shapeType, id);
        }
    }

    public void SendCheck(long itemId)
    {
        if (!connected) return;

        long locationID = itemId; //convert item id to location id, for now they are equal

        session.Locations.CompleteLocationChecks(locationID);
    }

}
