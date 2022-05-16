using Photon.Pun;
using TMPro;
using WebSocketSharp;
using UnityEngine.SceneManagement;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public TMP_InputField createInput;
    public TMP_InputField joinInput;

    public void CreateRoom()
    {
        if (!createInput.text.IsNullOrEmpty())
        {
            PhotonNetwork.CreateRoom(createInput.text);
        }
    }

    public void JoinRoom()
    {
        if (!joinInput.text.IsNullOrEmpty())
        {
            PhotonNetwork.JoinRoom(joinInput.text);
        }
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
