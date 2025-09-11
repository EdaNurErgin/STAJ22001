class Customer {
  final int id;
  final String? fullName;
  final String? phoneNumber;
  final String? shippingAddress;
  final String? billingAddress;

  Customer({
    required this.id,
    this.fullName,
    this.phoneNumber,
    this.shippingAddress,
    this.billingAddress,
  });

  factory Customer.fromJson(Map<String, dynamic> json) {
    return Customer(
      id: json['id'],
      fullName: json['fullName'],
      phoneNumber: json['phoneNumber'],
      shippingAddress: json['shippingAddress'],
      billingAddress: json['billingAddress'],
    );
  }
}
