import 'dart:convert';

import 'package:flutter/material.dart';

import '../services/api_service.dart';
import 'order_details_page.dart';

class OrdersContent extends StatefulWidget {
  const OrdersContent({super.key});

  @override
  State<OrdersContent> createState() => _OrdersContentState();
}

class _OrdersContentState extends State<OrdersContent> {
  final ApiService _apiService = ApiService();
  late Future<List<dynamic>> _futureOrders;

  @override
  void initState() {
    super.initState();
    _futureOrders = _apiService.fetchOrdersForCustomer();
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<List<dynamic>>(
      future: _futureOrders,
      builder: (context, snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(child: CircularProgressIndicator());
        } else if (snapshot.hasError) {
          return Center(
            child: Text(
              'Hata: ${snapshot.error}',
              style: const TextStyle(color: Colors.red, fontSize: 16),
            ),
          );
        } else if (snapshot.hasData) {
          final orders = snapshot.data!;
          if (orders.isEmpty) {
            return Center(
              child: Container(
                padding: const EdgeInsets.all(20),
                margin: const EdgeInsets.symmetric(horizontal: 24),
                decoration: BoxDecoration(
                  color: Colors.black.withOpacity(0.5),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Icon(
                      Icons.shopping_cart_outlined,
                      size: 64,
                      color: Colors.white.withOpacity(0.9),
                    ),
                    const SizedBox(height: 12),
                    const Text(
                      'Henüz siparişiniz yok',
                      style: TextStyle(
                        fontSize: 20,
                        fontWeight: FontWeight.bold,
                        color: Colors.white,
                      ),
                    ),
                    const SizedBox(height: 8),
                    const Text(
                      'Alışverişe başlayarak ilk siparişinizi oluşturabilirsiniz!',
                      style: TextStyle(fontSize: 16, color: Colors.white),
                      textAlign: TextAlign.center,
                    ),
                    const SizedBox(height: 16),
                    ElevatedButton.icon(
                      onPressed: () {
                        Navigator.pushNamed(
                          context,
                          '/products',
                        ); // kendi rotanı yaz
                      },
                      icon: const Icon(Icons.shopping_bag),
                      label: const Text('Alışverişe Başla'),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.blueAccent,
                        foregroundColor: Colors.white,
                        padding: const EdgeInsets.symmetric(
                          horizontal: 24,
                          vertical: 12,
                        ),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(8),
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            );
          }

          return ListView.builder(
            padding: const EdgeInsets.all(12),
            itemCount: orders.length,
            itemBuilder: (context, index) {
              final order = orders[index];
              final bool isCompleted = order['isCompleted'] ?? false;

              final String? orderDateStr =
                  order['orderDate'] ?? order['OrderDate'];
              String formattedDate = 'Bilinmiyor';

              if (orderDateStr != null) {
                try {
                  final dateTime = DateTime.parse(orderDateStr);
                  formattedDate =
                      '${dateTime.day}.${dateTime.month}.${dateTime.year} - '
                      '${dateTime.hour}:${dateTime.minute.toString().padLeft(2, '0')}';
                } catch (e) {
                  formattedDate = 'Geçersiz Tarih';
                }
              }

              //final orderDetails = order['orderDetails']?['\$values'];
              final orderDetails = order['orderDetails'];

              String? imageData;
              if (orderDetails != null &&
                  orderDetails is List &&
                  orderDetails.isNotEmpty) {
                final firstDetail = orderDetails[0];
                final product = firstDetail['product'];
                imageData = product?['imageData'];
              }

              return Card(
                elevation: 4,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                margin: const EdgeInsets.symmetric(vertical: 8),
                child: ListTile(
                  contentPadding: const EdgeInsets.all(16),
                  leading: CircleAvatar(
                    backgroundColor: isCompleted ? Colors.green : Colors.orange,
                    child: Icon(
                      isCompleted ? Icons.check : Icons.access_time,
                      color: Colors.white,
                    ),
                  ),
                  title: Text(
                    isCompleted ? 'Sipariş Tamamlandı' : 'Sipariş Beklemede',
                    style: const TextStyle(fontWeight: FontWeight.bold),
                  ),
                  subtitle: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text('Tarih: $formattedDate'),
                      const SizedBox(height: 8),
                      ElevatedButton(
                        onPressed: () {
                          Navigator.push(
                            context,
                            MaterialPageRoute(
                              builder: (_) => OrderDetailsPage(order: order),
                            ),
                          );
                        },
                        style: ElevatedButton.styleFrom(
                          padding: const EdgeInsets.symmetric(
                            horizontal: 12,
                            vertical: 8,
                          ),
                        ),
                        child: const Text(
                          'Detayları Gör',
                          style: TextStyle(color: Colors.blueAccent),
                        ),
                      ),
                    ],
                  ),
                  trailing: (imageData != null && imageData.isNotEmpty)
                      ? ClipRRect(
                          borderRadius: BorderRadius.circular(8),
                          child: Image.memory(
                            base64Decode(imageData),
                            width: 50,
                            height: 50,
                            fit: BoxFit.cover,
                          ),
                        )
                      : Container(
                          width: 50,
                          height: 50,
                          decoration: BoxDecoration(
                            color: Colors.grey.shade300,
                            borderRadius: BorderRadius.circular(8),
                          ),
                          child: const Icon(
                            Icons.image_not_supported,
                            color: Colors.grey,
                            size: 32,
                          ),
                        ),
                ),
              );
            },
          );
        } else {
          return const Center(child: Text('Bir şeyler ters gitti.'));
        }
      },
    );
  }
}
