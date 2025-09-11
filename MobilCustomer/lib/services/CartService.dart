import 'package:flutter/material.dart';

class CartService extends ChangeNotifier {
  final List<Map<String, dynamic>> _cartItems = [];

  List<Map<String, dynamic>> get cartItems => _cartItems;

  void addToCart(Map<String, dynamic> product, int quantity) {
    final existing = _cartItems.indexWhere(
      (item) => item['id'] == product['id'],
    );
    if (existing != -1) {
      _cartItems[existing]['quantity'] += quantity;
    } else {
      _cartItems.add({
        'id': product['id'],
        'title': product['name'],
        'price': product['price'],
        'quantity': quantity,
        'icon': Icons.shopping_bag,
      });
    }

    notifyListeners();
  }

  double get totalPrice => _cartItems.fold(
    0.0,
    (sum, item) => sum + (item['price'] * item['quantity']),
  );

  void clearCart() {
    _cartItems.clear();
    notifyListeners();
  }

  void removeItem(int productId) {
    _cartItems.removeWhere((item) => item['id'] == productId);
    notifyListeners();
  }

  void increaseQuantity(int productId) {
    final index = _cartItems.indexWhere((item) => item['id'] == productId);
    if (index != -1) {
      _cartItems[index]['quantity'] += 1;
      notifyListeners();
    }
  }

  void decreaseQuantity(int productId) {
    final index = _cartItems.indexWhere((item) => item['id'] == productId);
    if (index != -1 && _cartItems[index]['quantity'] > 1) {
      _cartItems[index]['quantity'] -= 1;
      notifyListeners();
    }
  }
}
