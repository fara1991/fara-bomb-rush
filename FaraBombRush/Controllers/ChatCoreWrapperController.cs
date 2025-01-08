using ChatCore;
using ChatCore.Interfaces;
using FaraBombRush.Configs;
using UnityEngine;
using Zenject;
using FaraBombRush.Controllers.GameModes;

namespace FaraBombRush.Controllers;

public class ChatCoreWrapperController : MonoBehaviour
{
    private readonly FaraBombInteractiveModeController _bombInteractiveModeController;
    private PluginConfig _config;

    [Inject]
    private void Construct(PluginConfig config)
    {
        _config = config;
    }

    public ChatCoreWrapperController()
    {
        var chatCoreInstance = ChatCoreInstance.Create();
        chatCoreInstance.RunAllServices().GetTwitchService().OnTextMessageReceived += ChatCoreOnTextMessageReceived;
        _bombInteractiveModeController = new FaraBombInteractiveModeController();
    }

    private void ChatCoreOnTextMessageReceived(IChatService service, IChatMessage message)
    {
        if (service.DisplayName == "Twitch" && _bombInteractiveModeController.CheckCommand(message.Message))
            _bombInteractiveModeController.BombPush(message.Message, _config);
    }
}