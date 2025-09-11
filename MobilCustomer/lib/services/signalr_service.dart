import 'package:shared_preferences/shared_preferences.dart';
import 'package:signalr_netcore/signalr_client.dart';

import '../Models/ChatMessage.dart';

class SignalRService {
  static final SignalRService _instance = SignalRService._internal();
  factory SignalRService() => _instance;
  SignalRService._internal();

  HubConnection? _hubConnection;

  Function(ChatMessage)? onMessageReceived;

  /*Future<void> connect(int customerId) async {
    final serverUrl = "http://localhost:5255/messagehub"; // API adresin

    _hubConnection = HubConnectionBuilder()
        .withUrl(serverUrl)
        .withAutomaticReconnect()
        .build();

    _hubConnection!.onclose(({error}) => print("Connection Closed"));

    _hubConnection!.on("ReceiveMessage", _handleIncomingMessage);

    await _hubConnection!.start();
    print("✅ SignalR bağlantısı kuruldu (Customer: $customerId)");
  }*/

  Future<void> connect(int customerId) async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('jwtToken');

    final serverUrl = "http://10.0.2.2:5255/messagehub";
    //http://10.0.2.2:5255/api EMULATOR   BU DA APK http://192.168.20.110:5255/messagehub

    final httpOptions = HttpConnectionOptions();
    httpOptions.accessTokenFactory = () async => token ?? "";

    _hubConnection = HubConnectionBuilder()
        .withUrl(serverUrl, options: httpOptions)
        .withAutomaticReconnect()
        .build();

    _hubConnection!.onclose(
      ({error}) => print("🔌 SignalR bağlantısı kapandı"),
    );

    _hubConnection!.on("ReceiveMessage", _handleIncomingMessage);

    await _hubConnection!.start();
    print("✅ SignalR bağlantısı kuruldu (Customer: $customerId)");
  }

  void _handleIncomingMessage(List<Object?>? args) {
    if (args != null && args.isNotEmpty) {
      final data = args[0] as Map<String, dynamic>;
      final msg = ChatMessage.fromJson(data);
      onMessageReceived?.call(msg);
    }
  }

  Future<void> disconnect() async {
    await _hubConnection?.stop();
    print("🔌 SignalR bağlantısı kapatıldı");
  }
}
