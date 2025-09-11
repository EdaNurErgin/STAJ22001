
import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

import '../models/customer.dart';

class AccountContent extends StatefulWidget {
  const AccountContent({super.key});

  @override
  State<AccountContent> createState() => _AccountContentState();
}

class _AccountContentState extends State<AccountContent> {
  Customer?
  _customer; //, API'den gelen veriyi saklayacağın tekil bir model nesnesi.
  bool _isLoading = true;

  @override
  void initState() {
    super.initState();
    _loadCustomer();
  }

  Future<void> _loadCustomer() async {
    final prefs = await SharedPreferences.getInstance();
    final customerId = prefs.getInt('customerId');
    final token = prefs.getString('jwtToken');
    //kullanıcı giris yaptımı
    if (customerId == null || token == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Yetkilendirme hatası, tekrar giriş yapın'),
        ),
      );
      setState(() {
        _isLoading = false;
      });
      return;
    }
    //kullanıcı tokeni halen gecerli mi
    try {
      final response = await http.get(
        Uri.parse("http://10.0.2.2:5255/api/CustomerApi/$customerId"),
        // http://10.0.2.2:5255/api EMULATOR
        // http://192.168.20.110:5255 APK İCİN
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
      );

      if (response.statusCode == 200) {
        final jsonData = json.decode(response.body);
        setState(() {
          _customer = Customer.fromJson(jsonData);
          _isLoading = false;
        });
      } else if (response.statusCode == 401) {
        //token kontrolu
        throw Exception("Yetkilendirme başarısız, tekrar giriş yapın");
      } else {
        throw Exception(
          "Müşteri bilgisi getirilemedi (${response.statusCode})",
        );
      }
    } catch (e) {
      setState(() {
        _isLoading = false;
      });
      ScaffoldMessenger.of(
        context,
      ).showSnackBar(SnackBar(content: Text('Hata: ${e.toString()}')));
    }
  }

  Future<void> _logout() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.clear();

    ScaffoldMessenger.of(context).showSnackBar(
      const SnackBar(
        backgroundColor: Colors.green,
        content: Text('Çıkış yapıldı'),
      ),
    );

    await Future.delayed(const Duration(seconds: 1));
    Navigator.of(context).pushNamedAndRemoveUntil('/login', (route) => false);
  }

  @override
  Widget build(BuildContext context) {
    return _isLoading
        ? const Center(child: CircularProgressIndicator())
        : Padding(
            padding: const EdgeInsets.all(16.0),
            child: Center(
              child: Card(
                elevation: 8,
                shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(16),
                ),
                child: Padding(
                  padding: const EdgeInsets.all(24.0),
                  child: ConstrainedBox(
                    constraints: const BoxConstraints(maxWidth: 500),
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        const Text(
                          'Hesap Bilgilerim',
                          style: TextStyle(
                            fontSize: 28,
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                        const SizedBox(height: 20),
                        if (_customer != null) ...[
                          _buildInfoRow(
                            Icons.person,
                            'Ad Soyad',
                            _customer!.fullName,
                          ),
                          _buildInfoRow(
                            Icons.phone,
                            'Telefon',
                            _customer!.phoneNumber,
                          ),
                          _buildInfoRow(
                            Icons.local_shipping,
                            'Teslimat Adresi',
                            _customer!.shippingAddress,
                          ),
                          _buildInfoRow(
                            Icons.receipt,
                            'Fatura Adresi',
                            _customer!.billingAddress,
                          ),
                          const SizedBox(height: 30),
                          SizedBox(
                            width: double.infinity,
                            child: ElevatedButton.icon(
                              icon: const Icon(Icons.logout),
                              label: const Text('Çıkış Yap'),
                              style: ElevatedButton.styleFrom(
                                backgroundColor: Colors.redAccent,
                                padding: const EdgeInsets.symmetric(
                                  vertical: 16,
                                ),
                                shape: RoundedRectangleBorder(
                                  borderRadius: BorderRadius.circular(8),
                                ),
                              ),
                              onPressed: _logout,
                            ),
                          ),
                        ] else
                          const Text(
                            "Müşteri bilgileri bulunamadı",
                            style: TextStyle(color: Colors.redAccent),
                          ),
                      ],
                    ),
                  ),
                ),
              ),
            ),
          );
  }

  Widget _buildInfoRow(IconData icon, String label, String? value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8.0),
      child: Row(
        children: [
          Icon(icon, color: Colors.blueAccent),
          const SizedBox(width: 12),
          Expanded(
            child: Text(
              "$label: ${value ?? "-"}",
              style: const TextStyle(fontSize: 16),
            ),
          ),
        ],
      ),
    );
  }
}
