import 'dart:math';

import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:frontend/models/appointment_model.dart';
import 'package:frontend/models/enums/UserType.dart';
import 'package:frontend/models/patient_model.dart';
import 'package:frontend/services/user_api.dart';
import 'package:frontend/user_account.dart';
import 'package:frontend/widgets/navigation_bar.dart';
import 'package:frontend/widgets/patient_dashboard.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../../models/doctor_model.dart';
import '../../services/patient_api.dart';
import '../../widgets/doctor_dashboard.dart';

class HomePage extends StatefulWidget {
  const HomePage({Key? key}) : super(key: key);

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> with TickerProviderStateMixin {
  List<AppointmentModel> appointments = [];
  bool _loggedIn = false;

  @override
  void initState() {
    super.initState();
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
            refreshHome: _refreshHome,
          ),
          Container(
            child: FutureBuilder<void>(
              future: _checkSession(),
              builder: (BuildContext context, AsyncSnapshot<void> snapshot) {
                if (snapshot.connectionState == ConnectionState.done) {
                  print(_loggedIn);
                  if (_loggedIn && UserAccount().patient != null)
                    return Column(
                      children: [
                        PatientDashboard(),
                      ],
                    );
                  else if (_loggedIn && UserAccount().doctor != null) {
                    return Column(
                      children: [
                        DoctorDashboard()
                      ],
                    );
                  }
                  else
                    return Center(
                      child: Text(
                        "Not logged in",
                      ),
                    );
                } else {
                  return const CupertinoActivityIndicator();
                }
                return Center();
              },
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
      final token = await _saveToken(patient.token, patient.tokenExpiration, patient.id, false, patient.email);
      setState(() {
        _loggedIn = token;
        UserAccount().patient = patient;
      });
    } else if (result is DoctorModel) {
      final doctor = result;
      final token = await _saveToken(doctor.token, doctor.tokenExpiration, doctor.id, true, doctor.email);
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

  Future<bool> _saveToken(String token, String expiration, int userId, bool isDoctor, String email) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString('jwt_token', token);
    await prefs.setString('token_expiration', expiration);
    await prefs.setInt('user_id', userId);
    await prefs.setBool('is_doctor', isDoctor);
    await prefs.setString('email', email);
    return true;
  }

  Future<void> _checkSession() async {
    final validToken = await UserAPI().isTokenValid();
    if (validToken) {
      final prefs = await SharedPreferences.getInstance();
      final isDoctor = prefs.getBool('is_doctor');
      if (!isDoctor!) {
        await UserAccount().getPatient();
      } else {
        await UserAccount().getDoctor();
      }
    }
    else {
      await UserAccount().logout();
    }
    _loggedIn = validToken;
  }

  void _refreshHome() {
    if (mounted) setState(() {});
  }
}
