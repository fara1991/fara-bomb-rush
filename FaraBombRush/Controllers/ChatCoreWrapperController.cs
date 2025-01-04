using ChatCore;
using ChatCore.Interfaces;
using FaraBombRush.Configs;
using UnityEngine;
using Zenject;

namespace FaraBombRush.Controllers;

public class ChatCoreWrapperController : MonoBehaviour
{
    private readonly FaraBombCommandController _bombCommandController;
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
        _bombCommandController = new FaraBombCommandController();
    }

    private void ChatCoreOnTextMessageReceived(IChatService service, IChatMessage message)
    {
        if (service.DisplayName == "Twitch" && _bombCommandController.CheckCommand(message.Message))
            _bombCommandController.BombPush(message.Message, _config);
    }
}