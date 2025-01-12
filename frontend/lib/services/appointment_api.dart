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
      var url = Uri.http(_baseUrl, 'api/v1/appointment/getAppointmentsForPatient/$patientId');
      var response = await http.get(url);

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
        "message": "Exception occurred",
      };
    }
  }

  Future<Map<String, dynamic>> create(
      String startTime,
      String endTime,
      String title,
      String description,
      int status,
      int doctorId,
      int patientId,
      bool createdBy,
      ) async {
    try {
      var url = Uri.http(_baseUrl, 'api/v1/appointment/createAppointment');

      var response = await http.post(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
        body: jsonEncode({
          "StartTime": startTime,
          "EndTime": endTime,
          "Title": title,
          "Description": description,
          "Status": status,
          "DoctorId": doctorId,
          "PatientId": patientId,
          "CreatedBy": false,
          "SysTimestamp": DateTime.now().toIso8601String().toString(),
          "SysCreated": DateTime.now().toIso8601String().toString(),
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

  Future<Map<String, dynamic>> modifyStatus(int id, String status) async {
    try{
      var url = Uri.http(_baseUrl, 'api/v1/appointment/modifyStatus/$id');
      var response = await http.put(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
        body: jsonEncode(<String, dynamic>{
          "status": status,
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
        "message": "Exception occurred",
      };
    }
  }

  Future<Map<String, dynamic>> modifyDescription(int id, String description) async {
    try{
      var url = Uri.http(_baseUrl, 'api/v1/appointment/modifyDescription/$id');
      var response = await http.put(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
        body: jsonEncode(<String, dynamic>{
          "description": description,
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
        "message": "Exception occurred",
      };
    }
  }
}