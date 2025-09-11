/*import 'dart:convert';

import 'package:http/http.dart' as http;
import 'package:shared_preferences/shared_preferences.dart';

class ApiService {
  final String baseUrl = "http://10.0.2.2:5255/api"; // emülatör
  // final String baseUrl = "http://192.168.1.10:5255/api"; // gerçek cihaz

  /* Future<List<dynamic>> getProducts() async {
    final response = await http.get(Uri.parse("$baseUrl/ProductApi"));

    if (response.statusCode == 200) {
      return json.decode(response.body);
    } else {
      throw Exception("Ürünler yüklenemedi");
    }
  }
*/

  Future<List<dynamic>> getProducts() async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('jwtToken');

    final response = await http.get(
      Uri.parse("$baseUrl/ProductApi"),
      headers: {
        "Content-Type": "application/json",
        "Authorization": "Bearer $token",
      },
    );

    if (response.statusCode == 200) {
      final decoded = json.decode(response.body);
      if (decoded is Map<String, dynamic> && decoded.containsKey(r'$values')) {
        return decoded[r'$values'];
      } else {
        throw Exception("Beklenen formatta veri gelmedi");
      }
    } else {
      throw Exception("Ürünler yüklenemedi");
    }
  }

  /* Future<List<dynamic>> getProducts() async {
    final response = await http.get(Uri.parse("$baseUrl/ProductApi"));

    if (response.statusCode == 200) {
      final decoded = json.decode(response.body);
      if (decoded is Map<String, dynamic> && decoded.containsKey(r'$values')) {
        return decoded[r'$values'];
      } else {
        throw Exception("Beklenen formatta veri gelmedi");
      }
    } else {
      throw Exception("Ürünler yüklenemedi");
    }
  }
*/
  Future<Map<String, dynamic>> getProductById(int id) async {
    final response = await http.get(Uri.parse("$baseUrl/ProductApi/$id"));

    if (response.statusCode == 200) {
      return json.decode(response.body);
    } else {
      throw Exception("Ürün detayı yüklenemedi");
    }
  }

  Future<Map<String, dynamic>> login(
    String phoneNumber,
    String password,
  ) async {
    final url = Uri.parse("$baseUrl/AuthApi/customer-login");
    final response = await http.post(
      url,
      headers: {'Content-Type': 'application/json'},
      body: json.encode({'phoneNumber': phoneNumber, 'password': password}),
    );

    if (response.statusCode == 200) {
      final jsonData = json.decode(response.body);

      // TOKEN'I KAYDET
      final prefs = await SharedPreferences.getInstance();
      await prefs.setString('jwtToken', jsonData['token']);

      return jsonData;
    } else if (response.statusCode == 401) {
      throw Exception("Telefon numarası veya şifre hatalı");
    } else {
      throw Exception("Giriş yapılamadı");
    }
  }

  /* Future<Map<String, dynamic>> login(
    String phoneNumber,
    String password,
  ) async {
    final url = Uri.parse("$baseUrl/AuthApi/customer-login");
    final response = await http.post(
      url,
      headers: {'Content-Type': 'application/json'},
      body: json.encode({'phoneNumber': phoneNumber, 'password': password}),
    );

    print("Status code: ${response.statusCode}");
    print("Response body: ${response.body}");
    if (response.statusCode == 200) {
      return json.decode(response.body);
    } else if (response.statusCode == 401) {
      throw Exception("Telefon numarası veya şifre hatalı");
    } else {
      throw Exception("Giriş yapılamadı");
    }
  }

  Future<Map<String, dynamic>> getCustomerById(int id) async {
    final response = await http.get(Uri.parse("$baseUrl/CustomerApi/$id"));

    print("GetCustomer status: ${response.statusCode}");
    print("GetCustomer body: ${response.body}");

    if (response.statusCode == 200) {
      return json.decode(response.body);
    } else if (response.statusCode == 404) {
      throw Exception("Müşteri bulunamadı");
    } else {
      throw Exception("Müşteri bilgisi alınamadı");
    }
  }
 */
  /*Future<void> submitOrder(Map<String, dynamic> product, int quantity) async {
    final response = await http.post(
      Uri.parse(
        "$baseUrl/OrderApi",
      ), // ✅ Artık baseUrl kullanıyor, sabit port hatası yok
      headers: {"Content-Type": "application/json"},
      body: jsonEncode({
        "userId": 1, // Test için sabit, gerçek uygulamada oturumdan gelir
        "customerId": 1,
        "isCompleted": false,
        "orderDetails": [
          {"productId": product['id'], "quantity": quantity},
        ],
      }),
    );

    print("SubmitOrder status: ${response.statusCode}");
    print("SubmitOrder body: ${response.body}");

    if (response.statusCode == 201) {
      print("✅ Sipariş başarıyla oluşturuldu");
    } else {
      throw Exception(
        "Sipariş oluşturulamadı: ${response.statusCode} - ${response.body}",
      );
    }
  }*/

  Future<void> submitOrder(Map<String, dynamic> product, int quantity) async {
    final prefs = await SharedPreferences.getInstance();
    final customerId = prefs.getInt('customerId');

    if (customerId == null) {
      throw Exception("Müşteri bilgisi bulunamadı, lütfen tekrar giriş yapın.");
    }

    final response = await http.post(
      Uri.parse("$baseUrl/OrderApi"),
      headers: {"Content-Type": "application/json"},
      body: jsonEncode({
        "userId": 1, // Bu sabit kalabilir ya da aynı şekilde oturumdan al
        "customerId": customerId, // ✅ Artık giriş yapan kişinin ID'si
        "isCompleted": false,
        "orderDetails": [
          {"productId": product['id'], "quantity": quantity},
        ],
      }),
    );
    final payload = {
      "userId": 1,
      "customerId": customerId,
      "isCompleted": false,
      "orderDetails": [
        {"productId": product['id'], "quantity": quantity},
      ],
    };
    print("Gönderilen JSON: ${jsonEncode(payload)}");

    print("SubmitOrder status: ${response.statusCode}");
    print("SubmitOrder body: ${response.body}");

    if (response.statusCode == 201) {
      print("✅ Sipariş başarıyla oluşturuldu");
    } else {
      throw Exception(
        "Sipariş oluşturulamadı: ${response.statusCode} - ${response.body}",
      );
    }
  }

  Future<List<dynamic>> fetchOrdersForCustomer() async {
    final prefs = await SharedPreferences.getInstance();
    final customerId = prefs.getInt('customerId');

    if (customerId == null) {
      throw Exception("Müşteri bilgisi bulunamadı");
    }

    final url = "$baseUrl/OrderApi/customer/$customerId";
    print("Çağırılan URL: $url");

    final response = await http.get(Uri.parse(url));

    if (response.statusCode == 200) {
      final decoded = json.decode(response.body);

      if (decoded is Map<String, dynamic> && decoded.containsKey(r'$values')) {
        return decoded[r'$values'];
      } else if (decoded is List) {
        return decoded;
      } else {
        throw Exception("Beklenen formatta veri gelmedi");
      }
    } else {
      throw Exception("Siparişler yüklenemedi");
    }
  }

  /*Future<List<dynamic>> fetchOrders() async {
    final response = await http.get(Uri.parse("$baseUrl/OrderApi"));

    if (response.statusCode == 200) {
      final decoded = json.decode(response.body);
      print("Gelen sipariş JSON: ${json.encode(decoded)}"); // 🔍 Konsolda gör
      if (decoded is Map<String, dynamic> && decoded.containsKey(r'$values')) {
        return decoded[r'$values'];
      } else if (decoded is List) {
        return decoded;
      } else {
        throw Exception("Beklenen formatta veri gelmedi");
      }
    } else {
      throw Exception("Siparişler yüklenemedi");
    }
  }*/
}


*/

import 'dart:convert';

import 'package:http/http.dart' as http;
import 'package:jwt_decoder/jwt_decoder.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../Models/ChatMessage.dart';

class ApiService {
  final String baseUrl = "http://10.0.2.2:5255/api"; // emülatör için
  // http://192.168.20.110/api   192.168.20.110     http://192.168.20.110:5255/api  emülator icin : http://10.0.2.2:5255/api

  Future<void> testApiCall() async {
    final url = Uri.parse(
      "$baseUrl/AuthApi",
    ); // veya AllowAnonymous olan başka endpoint

    try {
      final response = await http.get(url);

      if (response.statusCode == 200) {
        print("✅ Test başarılı: ${response.body}");
      } else {
        print("❌ API Hatası: ${response.statusCode}");
      }
    } catch (e) {
      print("🔥 Bağlantı hatası: $e");
    }
  }

  Future<Map<String, dynamic>> login(
    String phoneNumber,
    String password,
  ) async {
    final url = Uri.parse("$baseUrl/AuthApi/customer-login");
    final response = await http.post(
      url,
      headers: {'Content-Type': 'application/json'},

      body: json.encode({'phoneNumber': phoneNumber, 'password': password}),
    );

    if (response.statusCode == 200) {
      final jsonData = json.decode(response.body);
      final prefs = await SharedPreferences.getInstance();

      final token = jsonData['token'];
      await prefs.setString('jwtToken', token);

      final decodedToken = JwtDecoder.decode(token);
      print("Decoded Token: ${jsonEncode(decodedToken)}");

      final idStr = decodedToken['Id']?.toString();
      final id = int.tryParse(idStr ?? '');
      print("Çekilen Id değeri: $idStr");

      if (id == null) {
        throw Exception("Token içinde müşteri ID yok");
      }

      await prefs.setInt('customerId', id);
      await prefs.setBool('isLoggedIn', true);

      return jsonData;
    } else if (response.statusCode == 401) {
      throw Exception("Telefon numarası veya şifre hatalı");
    } else {
      throw Exception("Giriş yapılamadı: ${response.statusCode}");
    }
  }

  Future<List<dynamic>> getProducts() async {
    final response = await _authorizedGet("$baseUrl/ProductApi");
    if (response.statusCode == 200) {
      final decoded = json.decode(response.body);
      if (decoded is Map<String, dynamic> && decoded.containsKey(r'$values')) {
        return decoded[r'$values'];
      } else if (decoded is List) {
        return decoded;
      } else {
        throw Exception("Beklenen formatta veri gelmedi");
      }
    } else {
      throw Exception("Ürünler yüklenemedi: ${response.statusCode}");
    }
  }

  /*Future<Map<String, dynamic>> getCustomerById(int id) async {
    final response = await _authorizedGet("$baseUrl/CustomerApi/$id");
    if (response.statusCode == 200) {
      return json.decode(response.body);
    } else if (response.statusCode == 401) {
      throw Exception("Yetkilendirme hatası: tekrar giriş yapın");
    } else if (response.statusCode == 404) {
      throw Exception("Müşteri bulunamadı");
    } else {
      throw Exception("Müşteri bilgisi alınamadı: ${response.statusCode}");
    }
  } */

  Future<Map<String, dynamic>> getCustomerById(int id) async {
    final url = "$baseUrl/CustomerApi/$id";
    print("getCustomerById URL: $url");

    final response = await _authorizedGet(url);

    print("GetCustomer status: ${response.statusCode}");
    print("GetCustomer body: ${response.body}");

    if (response.statusCode == 200) {
      return json.decode(response.body);
    } else if (response.statusCode == 401) {
      throw Exception("Yetkilendirme hatası: tekrar giriş yapın");
    } else if (response.statusCode == 404) {
      throw Exception("Müşteri bulunamadı");
    } else {
      throw Exception("Müşteri bilgisi alınamadı: ${response.statusCode}");
    }
  }

  Future<void> submitOrder(Map<String, dynamic> product, int quantity) async {
    final prefs = await SharedPreferences.getInstance();
    final customerId = prefs.getInt('customerId');
    if (customerId == null) {
      throw Exception("Müşteri bilgisi bulunamadı, tekrar giriş yapın.");
    }

    final payload = {
      "userId": 1,
      "customerId": customerId,
      "isCompleted": false,
      "orderDetails": [
        {"productId": product['id'], "quantity": quantity},
      ],
    };

    final response = await _authorizedPost(
      "$baseUrl/OrderApi",
      body: jsonEncode(payload),
    );

    if (response.statusCode == 201) {
      print("✅ Sipariş başarıyla oluşturuldu");
    } else if (response.statusCode == 401) {
      throw Exception("Yetkilendirme hatası: tekrar giriş yapın");
    } else {
      throw Exception(
        "Sipariş oluşturulamadı: ${response.statusCode} - ${response.body}",
      );
    }
  }

  Future<List<dynamic>> fetchOrdersForCustomer() async {
    final prefs = await SharedPreferences.getInstance();
    final customerId = prefs.getInt('customerId');
    if (customerId == null) {
      throw Exception("Müşteri bilgisi bulunamadı");
    }

    final response = await _authorizedGet(
      "$baseUrl/OrderApi/customer/$customerId",
    );

    if (response.statusCode == 200) {
      final decoded = json.decode(response.body);
      if (decoded is Map<String, dynamic> && decoded.containsKey(r'$values')) {
        return decoded[r'$values'];
      } else if (decoded is List) {
        return decoded;
      } else {
        throw Exception("Beklenen formatta veri gelmedi");
      }
    } else if (response.statusCode == 401) {
      throw Exception("Yetkilendirme hatası: tekrar giriş yapın");
    } else {
      throw Exception("Siparişler yüklenemedi: ${response.statusCode}");
    }
  }

  // ---------------------------
  // YARDIMCI METOTLAR
  // ---------------------------
  /*Future<http.Response> _authorizedGet(String url) async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('jwtToken');
    print("👉 GET isteği URL: $url");
    print("👉 GET isteği Token: $token");

    return await http.get(
      Uri.parse(url),
      headers: {
        "Content-Type": "application/json",
        "Authorization": "Bearer $token",
      },
    );
  }*/
  Future<void> submitFullOrder(List<Map<String, dynamic>> cartItems) async {
    final prefs = await SharedPreferences.getInstance();
    final customerId = prefs.getInt('customerId');
    if (customerId == null) {
      throw Exception("Müşteri bilgisi bulunamadı, tekrar giriş yapın.");
    }

    //backenddeki karsılgıı orderDetails modeli
    final List<Map<String, dynamic>> orderDetails = cartItems
        .map((item) => {"productId": item['id'], "quantity": item['quantity']})
        .toList();

    final payload = {
      "userId": 1, // sabit olabilir ya da oturumdan alınır
      "customerId": customerId,
      "isCompleted": false,
      "orderDetails": orderDetails,
    };

    final response = await _authorizedPost(
      "$baseUrl/OrderApi",
      body: jsonEncode(payload),
    );

    print("Gönderilen Sipariş: ${jsonEncode(payload)}");

    if (response.statusCode == 201) {
      print("✅ Sipariş başarıyla oluşturuldu");
    } else {
      throw Exception(
        "Sipariş oluşturulamadı: ${response.statusCode} - ${response.body}",
      );
    }
  }

  Future<void> sendMessage(String text) async {
    final prefs = await SharedPreferences.getInstance();
    final customerId = prefs.getInt('customerId');
    final token = prefs.getString('jwtToken');

    if (customerId == null || token == null) {
      throw Exception("Kimlik bilgileri bulunamadı, tekrar giriş yapın.");
    }

    final message = {
      "senderId": customerId,
      "senderRole": "Customer",
      "receiverId": 1, // Web tarafındaki kullanıcı (admin) ID'si
      "receiverRole": "User",
      "message": text,
    };

    final response = await http.post(
      Uri.parse("$baseUrl/MessageApi"),
      headers: {
        "Content-Type": "application/json",
        "Authorization": "Bearer $token",
      },
      body: jsonEncode(message),
    );

    if (response.statusCode == 200) {
      print("✅ Mesaj gönderildi");
    } else {
      print("❌ Mesaj gönderilemedi: ${response.statusCode} - ${response.body}");
      throw Exception("Mesaj gönderilemedi");
    }
  }

  Future<List<ChatMessage>> loadConversation(int receiverId) async {
    final prefs = await SharedPreferences.getInstance();
    final customerId = prefs.getInt('customerId');

    if (customerId == null) {
      throw Exception("Müşteri bilgisi bulunamadı");
    }

    final response = await _authorizedGet(
      "$baseUrl/MessageApi/conversation?senderId=$customerId&receiverId=$receiverId",
    );

    if (response.statusCode == 200) {
      final decoded = json.decode(response.body);
      final List<ChatMessage> messages = (decoded as List)
          .map((e) => ChatMessage.fromJson(e))
          .toList();
      return messages;
    } else {
      throw Exception("Mesajlar yüklenemedi: ${response.statusCode}");
    }
  }

  Future<http.Response> _authorizedGet(String url) async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('jwtToken');

    if (token == null || JwtDecoder.isExpired(token)) {
      throw Exception("Oturum süresi dolmuş, tekrar giriş yapın.");
    }
    print("GET isteği: $url");
    print("GET token: Bearer $token");

    final response = await http.get(
      Uri.parse(url),
      headers: {
        "Content-Type": "application/json",
        "Authorization": "Bearer $token",
      },
    );

    print("GET status: ${response.statusCode}");
    print("GET body: ${response.body}");

    return response;
  }

  Future<http.Response> _authorizedPost(
    String url, {
    required String body,
  }) async {
    final prefs = await SharedPreferences.getInstance();
    final token = prefs.getString('jwtToken');
    print("👉 POST isteği URL: $url");
    print("👉 POST isteği Token: $token");
    print("👉 POST Body: $body");

    if (token == null || JwtDecoder.isExpired(token)) {
      throw Exception("Oturum süresi dolmuş, tekrar giriş yapın.");
    }
    return await http.post(
      Uri.parse(url),
      headers: {
        "Content-Type": "application/json",
        "Authorization": "Bearer $token",
      },
      body: body,
    );
  }
}
