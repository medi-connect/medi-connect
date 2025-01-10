import 'dart:convert';
import 'package:http/http.dart' as http;
import 'dart:developer';

class PatientAPI {
  final String _baseUrl = "localhost:8001";

  static final PatientAPI _instance = PatientAPI._internal();

  factory PatientAPI() {
    return _instance;
  }

  PatientAPI._internal();

  Future<Map<String, dynamic>> register(
      String email,
      String birthDate,
      String password,
      String name,
      String surname,
      ) async {
    try {
      var url = Uri.http(_baseUrl, 'api/v1/patient/register');

      var response = await http.post(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
        body: jsonEncode({
          "email": email,
          "birthDate": birthDate,
          "password": password,
          "name": name,
          "surname": surname,
        }),
      );

      print(url);

      if (response.statusCode == 200) {
        return {
          "status": response.statusCode,
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

  Future<Map<String, dynamic>> get(String id) async {
    try {
      var url = Uri.http(_baseUrl, 'api/v1/patient/getPatient/$id');

      var response = await http.get(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
      );
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
