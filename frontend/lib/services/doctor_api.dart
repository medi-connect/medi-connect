import 'dart:convert';
import 'package:http/http.dart' as http;
import 'dart:developer';

class DoctorAPI {
  final String _baseUrl = "72.144.116.77";

  static final DoctorAPI _instance = DoctorAPI._internal();

  factory DoctorAPI() {
    return _instance;
  }

  DoctorAPI._internal();

  Future<Map<String, dynamic>> register(
    String email,
    String speciality,
    String password,
    String name,
    String surname,
  ) async {
    try {
      var url = Uri.http(_baseUrl, 'doctor-service/api/v1/doctor/register');

      var response = await http.post(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
        body: jsonEncode(<String, dynamic>{
          "email": email,
          "speciality": speciality,
          "password": password,
          "name": name,
          "surname": surname,
        }),
      );

      print(url);
      print(response);

      // final decodedBody = jsonDecode(response.body);
      // print(response.body);
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
      var url = Uri.http(_baseUrl, 'doctor-service/api/v1/doctor/getDoctor/$id');

      print(url);
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

  Future<Map<String, dynamic>> getAll() async {
    try {
      var url = Uri.http(_baseUrl, 'doctor-service/api/v1/doctor/getAllDoctors');

      var response = await http.get(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
      );

      final decodedBody = jsonDecode(response.body);

      print(url);
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
