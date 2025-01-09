import 'dart:convert';
import 'package:http/http.dart' as http;
import 'dart:developer';

class DoctorAPI {
  final String _baseUrl = "localhost:8003";

  static final DoctorAPI _instance = DoctorAPI._internal();

  factory DoctorAPI() {
    return _instance;
  }

  DoctorAPI._internal();

  Future<Map<String, dynamic>> get(String id) async {
    try {
      var url = Uri.http(_baseUrl, 'api/v1/doctor/getDoctor/$id');

      var response = await http.get(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
      );

      print(url);

      final decodedBody = jsonDecode(response.body);

      if (response.statusCode == 200) {
        return {
          "status": response.statusCode,
          "response": decodedBody,
        };
      }
      return {
        "status": response.statusCode,
        "message": "Something went wrong, status code: ${response.statusCode}."
      };
    } catch (e) {
      return {
        "status": 400,
        "message": "Exception occurred: $e",
      };
    }
  }
}
