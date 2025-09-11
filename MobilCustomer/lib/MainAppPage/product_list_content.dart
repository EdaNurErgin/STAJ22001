import 'dart:convert'; //ase64 string’leri görsel haline çevirmek için (base64Decode)

import 'package:flutter/material.dart';

import '../services/api_service.dart';
import 'order_page.dart';

class ProductListContent extends StatefulWidget {
  const ProductListContent({super.key});

  @override
  State<ProductListContent> createState() => _ProductListContentState();
}

class _ProductListContentState extends State<ProductListContent> {
  final ApiService _apiService = ApiService();
  late Future<List<dynamic>>
  _futureProducts; //PI'den gelen ürün verileri Future şeklinde tutuluyor

  @override
  void initState() {
    //Sayfa ilk yüklendiğinde yapılacaklar
    super.initState();
    _futureProducts = _apiService.getProducts();
  }

  @override
  Widget build(BuildContext context) {
    //UI
    return Padding(
      padding: const EdgeInsets.all(12.0),
      child: FutureBuilder<List<dynamic>>(
        //API'den veri gelene kadar loading/hata/datalı durumu yöneten widget.
        future: _futureProducts,
        builder: (context, snapshot) {
          //snapshot, API çağrısının durumunu içerir (hasData, hasError, vs.)
          if (snapshot.connectionState == ConnectionState.waiting) {
            return const Center(child: CircularProgressIndicator());
          } else if (snapshot.hasError) {
            return Center(
              child: Text(
                'Hata: ${snapshot.error}',
                style: const TextStyle(color: Colors.red),
              ),
            );
          } else if (!snapshot.hasData || snapshot.data!.isEmpty) {
            return const Center(
              child: Text('Ürün bulunamadı', style: TextStyle(fontSize: 18)),
            );
          }

          final products = snapshot.data!;
          return ListView.builder(
            itemCount: products.length,
            itemBuilder: (context, index) {
              final product = products[index];
              return Card(
                elevation: 5,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12),
                ),
                margin: const EdgeInsets.symmetric(vertical: 8),
                child: Padding(
                  padding: const EdgeInsets.all(12.0),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        children: [
                          product['imageData'] != null
                              ? ClipRRect(
                                  borderRadius: BorderRadius.circular(8),
                                  child: Image.memory(
                                    base64Decode(product['imageData']),
                                    width: 60,
                                    height: 60,
                                    fit: BoxFit.cover,
                                  ),
                                )
                              : const Icon(
                                  Icons.image_not_supported,
                                  color: Colors.grey,
                                  size: 60,
                                ),
                          const SizedBox(width: 12),
                          Expanded(
                            child: Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  product['name'] ?? 'Ürün Adı Yok',
                                  style: const TextStyle(
                                    fontSize: 18,
                                    fontWeight: FontWeight.bold,
                                  ),
                                ),
                                const SizedBox(height: 4),
                                Text(
                                  'Fiyat: ${product['price'] ?? '-'} ₺',
                                  style: const TextStyle(fontSize: 14),
                                ),
                                Text(
                                  'Stok: ${product['stock'] ?? '-'}',
                                  style: const TextStyle(fontSize: 14),
                                ),
                              ],
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 12),
                      SizedBox(
                        width: double.infinity,
                        child: ElevatedButton.icon(
                          onPressed: () {
                            Navigator.push(
                              context,
                              MaterialPageRoute(
                                builder: (context) =>
                                    OrderPage(product: product),
                              ),
                            );
                          },
                          style: ElevatedButton.styleFrom(
                            backgroundColor: Colors.blueAccent,
                            padding: const EdgeInsets.symmetric(vertical: 12),
                            shape: RoundedRectangleBorder(
                              borderRadius: BorderRadius.circular(8),
                            ),
                          ),
                          icon: const Icon(
                            Icons.shopping_cart_checkout,
                            color: Colors.blue,
                          ),
                          label: const Text(
                            'Sipariş Ver',
                            style: TextStyle(fontSize: 16, color: Colors.white),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              );
            },
          );
        },
      ),
    );
  }
}
