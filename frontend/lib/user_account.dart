import 'package:frontend/models/enums/UserType.dart';
import 'package:frontend/services/doctor_api.dart';
import 'package:frontend/services/patient_api.dart';

import 'models/doctor_model.dart';
import 'models/patient_model.dart';
import 'package:shared_preferences/shared_preferences.dart';

class UserAccount {
  static final UserAccount _instance = UserAccount._internal();
  factory UserAccount() => _instance;

  UserAccount._internal();

  // Properties to hold user state
  DoctorModel? doctor;
  PatientModel? patient;

  // Method to check if a user is logged in
  bool get isLoggedIn => doctor != null || patient != null;

  // Method to log out the user
  Future<void> logout() async {
    await clearUserState();
    doctor = null;
    patient = null;
  }

  Future<void> clearUserState() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.clear();
  }

  bool loggedOut() {
    return doctor == null && patient == null;
  }

  Future<void> getPatient() async {
    final prefs = await SharedPreferences.getInstance();
    final userId = prefs.getInt('user_id');
    var getPatient = await PatientAPI().get(userId.toString());
    switch (getPatient["status"]){
      case 200:
        DateTime birthDate = DateTime.parse(getPatient["response"]["birthDate"].toString());

        final email = prefs.getString('email');
        final token = prefs.getString('jwt_token');
        final expiration = prefs.getString('token_expiration');

        final patient = PatientModel(
          birthDate,
          id: getPatient["response"]["userId"] ?? getPatient["response"]["userId"] ?? "none",
          email: email!,
          name: getPatient["response"]["name"] ?? getPatient["response"]["name"] ?? "none",
          surname: getPatient["response"]["surname"] ?? getPatient["response"]["surname"] ?? "none",
          token: token!,
          tokenExpiration: expiration!,
        );

        this.patient = patient;
        break;
      default:
        print(getPatient["message"]);
        break;
    }
  }

  Future<void> getDoctor() async {
    final prefs = await SharedPreferences.getInstance();
    final userId = prefs.getInt('user_id');
    var getDoctor = await DoctorAPI().get(userId.toString());
    switch (getDoctor["status"]){
      case 200:
        final email = prefs.getString('email');
        final token = prefs.getString('jwt_token');
        final expiration = prefs.getString('token_expiration');

        final doctor = DoctorModel(
          getDoctor["response"]["speciality"] ?? getDoctor["response"]["speciality"] ?? "none",
          id: getDoctor["response"]["userId"] ?? getDoctor["response"]["userId"] ?? "none",
          email: email!,
          name: getDoctor["response"]["name"] ?? getDoctor["response"]["name"] ?? "none",
          surname: getDoctor["response"]["surname"] ?? getDoctor["response"]["surname"] ?? "none",
          token: token!,
          tokenExpiration: expiration!,
        );

        this.doctor = doctor;
        break;
      default:
        print(getDoctor["message"]);
        break;
    }
  }
}
