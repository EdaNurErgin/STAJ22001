

import 'package:flutter/material.dart';
import 'package:jwt_decoder/jwt_decoder.dart';
import 'package:provider/provider.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../services/CartService.dart';
import '../services/api_service.dart';
import 'MainAppPage/AccountContent.dart';
import 'MainAppPage/main_app_page.dart';
import 'MainAppPage/product_list_content.dart';
import 'background_wrapper.dart';
import 'login/login_page.dart';

void main() async {
  WidgetsFlutterBinding.ensureInitialized();

  final prefs = await SharedPreferences.getInstance();
  final token = prefs.getString('jwtToken');
  final isLoggedIn = prefs.getBool('isLoggedIn') ?? false;
  //token süresi dolnca oturum kapatma
  bool validSession = false;
  if (isLoggedIn &&
      token != null &&
      token.isNotEmpty &&
      !JwtDecoder.isExpired(token)) {
    validSession = true;
  } else {
    await prefs.clear(); // geçersizse temizle
  }

  //runApp(MyApp(isLoggedIn: validSession));
  runApp(
    ChangeNotifierProvider(
      create: (_) => CartService(),
      child: MyApp(isLoggedIn: validSession),
    ),
  );
}

class MyApp extends StatelessWidget {
  final bool isLoggedIn;

  const MyApp({super.key, required this.isLoggedIn});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      initialRoute: '/',
      routes: {
        '/': (context) =>
            isLoggedIn ? const MainAppPage() : MyHomePage(title: ''),
        '/login': (context) => LoginPage(),
        '/main': (context) => const MainAppPage(),
        '/account': (context) => const AccountContent(),
        '/products': (context) => const ProductListContent(),
      },
    );
  }
}

class MyHomePage extends StatelessWidget {
  final String title;
  final ApiService apiService = ApiService();
  MyHomePage({super.key, required this.title});
  void _callApi() async {
    await apiService.testApiCall();
  }

  @override
  Widget build(BuildContext context) {
    return BackgroundWrapper(
      child: Scaffold(
        backgroundColor: Colors.transparent,
        body: Center(
          child: Card(
            elevation: 8,
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(16),
            ),
            color: Colors.white.withOpacity(0.85),
            child: Padding(
              padding: const EdgeInsets.all(32.0),
              child: ConstrainedBox(
                constraints: const BoxConstraints(maxWidth: 300, minWidth: 300),
                child: Column(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    const Icon(Icons.login, size: 64, color: Colors.blueAccent),
                    const SizedBox(height: 16),
                    const Text(
                      'Yazılım Satış Sistemine Hoş Geldiniz',
                      textAlign: TextAlign.center,
                      style: TextStyle(
                        fontSize: 22,
                        fontWeight: FontWeight.bold,
                        color: Colors.black87,
                      ),
                    ),
                    const SizedBox(height: 32),
                    SizedBox(
                      width: double.infinity,
                      child: ElevatedButton.icon(
                        onPressed: () {
                          Navigator.pushNamed(context, '/login');
                        },
                        icon: const Icon(Icons.lock_open, color: Colors.white),
                        label: const Text('Giriş Yap'),
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.blueAccent,
                          padding: const EdgeInsets.symmetric(vertical: 16),
                          textStyle: const TextStyle(fontSize: 18),
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(8),
                          ),
                        ),
                      ),
                    ),
                    const SizedBox(height: 16),
                    SizedBox(
                      width: double.infinity,
                      child: ElevatedButton.icon(
                        onPressed: () {
                          ScaffoldMessenger.of(context).showSnackBar(
                            const SnackBar(
                              content: Text('Bize Ulaşın tıklandı'),
                            ),
                          );
                        },
                        icon: const Icon(
                          Icons.support_agent,
                          color: Colors.white,
                        ),
                        label: const Text('Bize Ulaşın'),
                        style: ElevatedButton.styleFrom(
                          backgroundColor: Colors.green,
                          padding: const EdgeInsets.symmetric(vertical: 16),
                          textStyle: const TextStyle(fontSize: 18),
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(8),
                          ),
                        ),
                      ),
                    ),
                    /* SizedBox(height: 16),
                    ElevatedButton(
                      onPressed: _callApi,
                      child: const Text("API'yi Test Et"),
                    ),*/
                  ],
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
