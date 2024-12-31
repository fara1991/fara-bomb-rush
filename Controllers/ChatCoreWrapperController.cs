using ChatCore;
using ChatCore.Interfaces;

namespace FaraBombRush.Controllers;

public class ChatCoreWrapperController {
	private readonly FaraBombCommandController _bombCommandController;

	public ChatCoreWrapperController() {
		var chatCoreInstance = ChatCoreInstance.Create();
		chatCoreInstance.RunAllServices().GetTwitchService().OnTextMessageReceived += ChatCoreOnTextMessageReceived;
		_bombCommandController = new FaraBombCommandController();
	}

	private void ChatCoreOnTextMessageReceived(IChatService service, IChatMessage message) {
		if (service.DisplayName == "Twitch" && _bombCommandController.CheckCommand(message.Message)) {
			_bombCommandController.BombPush(message.Message);
		}
	}
}