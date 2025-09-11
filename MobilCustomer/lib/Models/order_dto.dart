import 'OrderDetailDto.dart';

class OrderDto {
  final int id;
  final DateTime orderDate;
  final bool isCompleted;
  final String customerName;
  final String userName;
  final List<OrderDetailDto> orderDetails;

  OrderDto({
    required this.id,
    required this.orderDate,
    required this.isCompleted,
    required this.customerName,
    required this.userName,
    required this.orderDetails,
  });

  factory OrderDto.fromJson(Map<String, dynamic> json) {
    return OrderDto(
      id: json['id'],
      orderDate: DateTime.parse(json['orderDate']),
      isCompleted: json['isCompleted'],
      customerName: json['customerName'] ?? '',
      userName: json['userName'] ?? '',
      orderDetails: (json['orderDetails'] as List)
          .map((e) => OrderDetailDto.fromJson(e))
          .toList(),
    );
  }
}
