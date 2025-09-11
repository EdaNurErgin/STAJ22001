
import 'package:flutter/material.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../services/api_service.dart';

class HomeContent extends StatefulWidget {
  const HomeContent({super.key});

  @override
  State<HomeContent> createState() => _HomeContentState();
}

class _HomeContentState extends State<HomeContent> {
  final ApiService _apiService = ApiService();
  String? customerName;
  bool isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadCustomerName();
  }

  Future<void> _loadCustomerName() async {
    final prefs = await SharedPreferences.getInstance();
    final customerId = prefs.getInt('customerId');

    if (customerId == null) {
      _showErrorAndSetDefault("Müşteri bilgisi bulunamadı, tekrar giriş yapın");
      return;
    }

    try {
      final customer = await _apiService.getCustomerById(customerId);

      setState(() {
        customerName = customer['fullName'] ?? 'Müşteri';
        isLoading = false;
      });
    } catch (e) {
      _showErrorAndSetDefault("Müşteri bilgisi alınamadı: ${e.toString()}");
    }
  }

  void _showErrorAndSetDefault(String message) {
    ScaffoldMessenger.of(context).showSnackBar(
      SnackBar(backgroundColor: Colors.redAccent, content: Text(message)),
    );
    setState(() {
      customerName = 'Müşteri';
      isLoading = false;
    });
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Center(
      child: Card(
        elevation: 4,
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
        margin: const EdgeInsets.symmetric(horizontal: 24),
        child: Padding(
          padding: const EdgeInsets.all(24.0),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Icon(Icons.home_outlined, size: 48, color: theme.primaryColor),
              const SizedBox(height: 16),
              Text(
                isLoading
                    ? 'Hoş Geldiniz...'
                    : 'Hoş Geldiniz ${customerName ?? ''}!',
                style: theme.textTheme.titleLarge?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
                textAlign: TextAlign.center,
              ),
              const SizedBox(height: 8),
              Text(
                'Siparişlerinizi Oluşturabilirsiniz',
                style: theme.textTheme.bodyMedium?.copyWith(
                  color: theme.hintColor,
                ),
                textAlign: TextAlign.center,
              ),
            ],
          ),
        ),
      ),
    );
  }
}
