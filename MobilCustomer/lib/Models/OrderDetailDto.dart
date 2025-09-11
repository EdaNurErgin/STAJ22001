class OrderDetailDto {
  final int productId;
  final String productName;
  final String? imageData;
  final int quantity;

  OrderDetailDto({
    required this.productId,
    required this.productName,
    required this.imageData,
    required this.quantity,
  });

  factory OrderDetailDto.fromJson(Map<String, dynamic> json) {
    return OrderDetailDto(
      productId: json['productId'],
      productName: json['productName'],
      imageData: json['imageData'], // Base64 string veya null
      quantity: json['quantity'],
    );
  }
}
