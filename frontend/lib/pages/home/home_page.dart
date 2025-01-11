import 'package:flutter/material.dart';
import 'package:frontend/models/appointment_model.dart';
import 'package:frontend/models/patient_model.dart';
import 'package:frontend/services/user_api.dart';
import 'package:frontend/user_account.dart';
import 'package:frontend/widgets/navigation_bar.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../models/doctor_model.dart';

class HomePage extends StatefulWidget {
  const HomePage({Key? key}) : super(key: key);

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> with TickerProviderStateMixin {
  List<AppointmentModel> appointments = [];
  bool isLoading = true;
  bool _loggedIn = false;


  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_){
      _checkSession();
    });

  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Column(
        mainAxisAlignment: MainAxisAlignment.start,
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          NavigationBarWidget(
            toLogin: _toLogin,
            toRegister: _toRegister,
          ),
          if (_loggedIn)
            Center(
              child: Text(
                "Successfully logged in",
              ),
            )
          else
            Center(
              child: Text(
                "Not logged in",
              ),
            )
        ],
      ),
    );
  }

  Future<void> _toLogin() async {
    final result = await Navigator.of(context).pushNamed('/login');

    if (result == null) {
      print("Something wrong. Result not retrieved");
      return;
    }

    if (result is PatientModel) {
      final patient = result;
      final token = await _saveToken(patient.token, patient.tokenExpiration);
      setState(() {
        _loggedIn = token;
        UserAccount().patient = patient;
      });
    } else if (result is DoctorModel) {
      final doctor = result;
      final token = await _saveToken(doctor.token, doctor.tokenExpiration);
      setState(() {
        _loggedIn = token;
        UserAccount().doctor = doctor;
      });
    }

    print(result.toString());
  }

  Future<void> _toRegister() async {
    final result = await Navigator.of(context).pushNamed('/register');
    print(result.toString());
  }

  Future<bool> _saveToken(String token, String expiration) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString('jwt_token', token);
    await prefs.setString('token_expiration', expiration);
    return true;
  }

  void _checkSession() async {
    final validToken = await UserAPI().isTokenValid();
    if (!validToken) {
      await UserAccount().logout();
    }
    setState(() async {
      _loggedIn = validToken;
    });
  }
}
