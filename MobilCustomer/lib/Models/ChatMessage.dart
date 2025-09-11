class ChatMessage {
  final int? senderId;
  final String? senderRole;
  final int? receiverId;
  final String? receiverRole;
  final String? message;
  final String? senderName;
  final String? receiverName;
  final DateTime? timestamp;

  ChatMessage({
    this.senderId,
    this.senderRole,
    this.receiverId,
    this.receiverRole,
    this.message,
    this.senderName,
    this.receiverName,
    this.timestamp,
  });

  factory ChatMessage.fromJson(Map<String, dynamic> json) {
    return ChatMessage(
      senderId: json['senderId'],
      senderRole: json['senderRole'],
      receiverId: json['receiverId'],
      receiverRole: json['receiverRole'],
      message: json['message'],
      senderName: json['senderName'],
      receiverName: json['receiverName'],
      timestamp: DateTime.tryParse(json['timestamp'] ?? ""),
    );
  }
}
