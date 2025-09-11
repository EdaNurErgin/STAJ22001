
import 'package:flutter/material.dart';

import '../app_wrapper.dart';
import 'AccountContent.dart';
import 'CartPage.dart';
import 'ChatPage.dart'; // ✅ ChatPage import
import 'home_content.dart';
import 'orders_content.dart';
import 'product_list_content.dart';

class MainAppPage extends StatefulWidget {
  const MainAppPage({super.key});
  static void changeTab(BuildContext context, int index) {
    final state = context.findAncestorStateOfType<_MainAppPageState>();
    state?.changeTab(index);
  }

  @override
  State<MainAppPage> createState() => _MainAppPageState();
}

class _MainAppPageState extends State<MainAppPage> {
  int _currentIndex = 0;

  final List<Widget> _pages = [
    const HomeContent(),
    const ProductListContent(),
    const OrdersContent(),
    const ChatPage(), // ✅ 3 → ChatPage
    const AccountContent(), // 4
    const CartPageContent(), // 5
  ];

  void changeTab(int index) {
    setState(() {
      _currentIndex = index;
    });
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return AppWrapper(
      title: 'Yazılım Satış Sistemi',
      showBackButton: _currentIndex == 5,
      onBack: () {
        setState(() {
          _currentIndex = 0;
        });
      },
      actions: _currentIndex == 5
          ? null
          : [
              IconButton(
                icon: Icon(
                  Icons.shopping_cart_outlined,
                  color: theme.primaryColor,
                  size: 26,
                ),
                tooltip: 'Sepetim',
                onPressed: () {
                  setState(() {
                    _currentIndex = 5;
                  });
                },
              ),
              IconButton(
                icon: Icon(
                  Icons.account_circle_outlined,
                  color: theme.primaryColor,
                  size: 28,
                ),
                tooltip: 'Hesabım',
                onPressed: () {
                  setState(() {
                    _currentIndex = 4;
                  });
                },
              ),
            ],
      bottomNavigationBar: BottomNavigationBar(
        currentIndex: _currentIndex > 3 ? 0 : _currentIndex,
        onTap: (index) {
          setState(() {
            _currentIndex = index;
          });
        },
        selectedItemColor: Colors.blueAccent,
        unselectedItemColor: Colors.grey.shade600,
        showUnselectedLabels: true,
        type: BottomNavigationBarType.fixed,
        items: const [
          BottomNavigationBarItem(
            icon: Icon(Icons.home_outlined),
            activeIcon: Icon(Icons.home),
            label: 'Ana Sayfa',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.storefront_outlined),
            activeIcon: Icon(Icons.storefront),
            label: 'Ürünler',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.shopping_bag_outlined),
            activeIcon: Icon(Icons.shopping_bag),
            label: 'Siparişlerim',
          ),
          BottomNavigationBarItem(
            icon: Icon(Icons.support_agent_outlined), // 🎧 Destek ikonu
            activeIcon: Icon(Icons.support_agent),
            label: 'Destek',
          ),
        ],
      ),
      child: _pages[_currentIndex],
    );
  }
}
