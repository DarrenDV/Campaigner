using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class PlayerListItem : MonoBehaviour
{
    /*
     *  This class is used for the player list items in the lobby scene
     *  It will be used to display the player's name and avatar
     *
     *  I am not sure if I want to keep the avatar part incorporated, removing it would make the script a lot simpler
     * 
     */
    
    
    public string PlayerName;
    public int ConnectionID;
    public ulong PlayerSteamID;
    private bool AvatarReceived;

    public TextMeshProUGUI PlayerNameText;
    public RawImage PlayerAvatar;
    
    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

    private void Start()
    {
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if(callback.m_steamID.m_SteamID == PlayerSteamID)
        {
            PlayerAvatar.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
    }
    
    private void GetPlayerIcon()
    {
        int avatar = SteamFriends.GetLargeFriendAvatar(new CSteamID(PlayerSteamID));
        if (avatar == -1)
        {
            return;
        }
        
        PlayerAvatar.texture = GetSteamImageAsTexture(avatar);
    }

    public void SetPlayerValues()
    {
        PlayerNameText.text = PlayerName;
        if (!AvatarReceived)
        {
            GetPlayerIcon();
        }
    }
    
    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        AvatarReceived = true;
        return texture;
    }
}
