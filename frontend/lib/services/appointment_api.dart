import 'dart:convert';
import 'package:http/http.dart' as http;
import 'dart:developer';
class AppointmentAPI {
  final String _baseUrl = "localhost:8004";

  static final AppointmentAPI _instance = AppointmentAPI._internal();

  factory AppointmentAPI() {
    return _instance;
  }

  AppointmentAPI._internal();

  Future<Map<String, dynamic>> fetchAppointmentsForPatient(String patientId) async {
    try{
      var url = Uri.https(_baseUrl, 'api/v1/appointment/getAppointmentsForPatient/$patientId');

      var response = await http.get(url);

      final decodedBody = jsonDecode(response.body);
      return {
        "status": response.statusCode,
        "appointments": decodedBody,
      };
    } catch (e) {
      print("EXCEPTION CAUGHT: $e");
      return {
        "status": 400,
        "message": "An error occurred",
      };
    }
  }

  Future<Map<String, dynamic>> fetchAppointmentsForDoctor(String doctorId) async {
    try{
      var url = Uri.http(_baseUrl, 'api/v1/appointment/getAppointmentsForDoctor/$doctorId');
      var response = await http.get(url);
      final decodedBody = jsonDecode(response.body);
      if (response.statusCode == 200) {
        return {
          "status": response.statusCode,
          "appointments": decodedBody,
        };
      }
      return {
        "status": response.statusCode,
        "message": "Something went wrong, status code: ${response.statusCode}."
      };
    } catch (e) {
      return {
        "status": 400,
        "message": "Exception occurred",
      };
    }
  }
}