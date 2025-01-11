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
}
