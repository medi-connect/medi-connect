import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import '../models/enums/UserType.dart';
import '../user_account.dart';

class PatientDashboard extends StatefulWidget {
  const PatientDashboard({Key? key}) : super(key: key);

  @override
  State<PatientDashboard> createState() => _PatientDashboardState();
}

class _PatientDashboardState extends State<PatientDashboard> {

  @override
  void initState() {
    super.initState();

  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text(
            "Welcome, ${UserAccount().patient!.name}!",
          style: TextStyle(
            fontWeight: FontWeight.bold,
            fontSize: 30,
          ),
        ),
        SizedBox(
          height: 32,
        ),
        Text(
          "Appointments",
          style: TextStyle(
            fontWeight: FontWeight.normal,
            fontSize: 22,
          ),
        ),
        SizedBox(
          height: 32,
        ),
        ElevatedButton(
          onPressed: () {
            Navigator.pushNamed(context, "/appointment_create", arguments: UserType.PATIENT);
          },
          child: Text("Create"),
        ),
        SizedBox(
          height: 16,
        ),
        ElevatedButton(
          onPressed: () {
            Navigator.pushNamed(context, "/appointments_page", arguments: {
              "userType": UserType.PATIENT,
              "userId": UserAccount().patient!.id,
            });
          },
          child: Text("View all"),
        ),
      ],
    );
  }
}
