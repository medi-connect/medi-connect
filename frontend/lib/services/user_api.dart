import 'dart:convert';
import 'package:http/http.dart' as http;
import 'dart:developer';

class UserAPI {
  final String _baseUrl = "localhost:8001";

  static final UserAPI _instance = UserAPI._internal();

  factory UserAPI() {
    return _instance;
  }

  UserAPI._internal();

  //todo change
  // Future<Map<String, dynamic>> register(String patientId) async {
  //   try {
  //     var url = Uri.https(
  //         _baseUrl, 'api/v1/appointment/getAppointmentsForPatient/$patientId');
  //
  //     var response = await http.get(url);
  //
  //     final decodedBody = jsonDecode(response.body);
  //     return {
  //       "status": response.statusCode,
  //       "appointments": decodedBody,
  //     };
  //   } catch (e) {
  //     print("EXCEPTION CAUGHT: $e");
  //     return {
  //       "status": 400,
  //       "message": "An error occurred",
  //     };
  //   }
  // }

  Future<Map<String, dynamic>> login(String email, String password) async {
    try {
      var url = Uri.http(_baseUrl, 'api/v1/user/login');

      var response = await http.post(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
        body: jsonEncode(<String, dynamic>{
          "email": email,
          "password": password,
        }),
      );

      final decodedBody = jsonDecode(response.body);
      print(decodedBody.toString());
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
