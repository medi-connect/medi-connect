import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import '../models/enums/UserType.dart';
import '../user_account.dart';

class DoctorDashboard extends StatefulWidget {
  const DoctorDashboard({Key? key}) : super(key: key);

  @override
  State<DoctorDashboard> createState() => _DoctorDashboardState();
}

class _DoctorDashboardState extends State<DoctorDashboard> {

  @override
  void initState() {
    super.initState();

  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        Text(
          "Welcome, ${UserAccount().doctor!.name}!",
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
            Navigator.pushNamed(context, "/appointment_create", arguments: UserType.DOCTOR);
          },
          child: Text("Create"),
        ),
        SizedBox(
          height: 16,
        ),
        ElevatedButton(
          onPressed: () {},
          child: Text("View all"),
        ),
      ],
    );
  }


}
