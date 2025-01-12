import 'dart:convert';
import 'package:http/http.dart' as http;
import 'dart:developer';
class FeedbackAPI {
  final String _baseUrl = "localhost:8002";

  static final FeedbackAPI _instance = FeedbackAPI._internal();

  factory FeedbackAPI() {
    return _instance;
  }

  FeedbackAPI._internal();

  Future<Map<String, dynamic>> getFeedbackForAppointment(int appointmentId) async {
    try{
      var url = Uri.http(_baseUrl, 'api/v1/feedback/getFeedback/$appointmentId');
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

  Future<Map<String, dynamic>> create(
      String review,
      int rate,
      int appointmentId,
      ) async {
    try {
      var url = Uri.http(_baseUrl, 'api/v1/feedback/addFeedback');

      var response = await http.post(
        url,
        headers: <String, String>{
          'Content-Type': 'application/json; charset=UTF-8',
        },
        body: jsonEncode({
        "rate": rate,
        "appointmentId": appointmentId,
        "review": review,
          "sysTimestamp": DateTime.now().toIso8601String().toString(),
          "sysCreated": DateTime.now().toIso8601String().toString(),
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
}