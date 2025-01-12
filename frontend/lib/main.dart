import 'package:flutter/material.dart';
import 'package:frontend/models/enums/UserType.dart';
import 'package:frontend/pages/appointments/appointment_create_page.dart';
import 'package:frontend/pages/appointments/appointments_page.dart';
import 'package:frontend/pages/home/home_page.dart';
import 'package:frontend/pages/home/login_page.dart';
import 'package:frontend/pages/home/registration_page.dart';

void main() {
  runApp(MediConnect());
}

class MediConnect extends StatelessWidget {
  const MediConnect({super.key});

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Medi-Connect',
      theme: ThemeData(
        colorScheme: ColorScheme.fromSeed(seedColor: Colors.deepPurple),
        useMaterial3: true,
      ),
      debugShowCheckedModeBanner: false,
      initialRoute: '/',
      onGenerateRoute: (settings) {
        // Define routes and pass arguments dynamically
        if (settings.name == '/appointment_create') {
          final userType = settings.arguments as UserType;
          return MaterialPageRoute(
            builder: (context) => AppointmentCreatePage(userType: userType),
          );
        } else if (settings.name == '/appointments_page') {
          final arguments = settings.arguments as Map<String, dynamic>;
          final userType = arguments["userType"] as UserType;
          final userId = arguments["userId"] as int;
          return MaterialPageRoute(
            builder: (context) => AppointmentsPage(userType: userType, userId: userId)
          );
        }

        // Default static routes
        switch (settings.name) {
          case '/':
            return MaterialPageRoute(builder: (context) => HomePage());
          case '/register':
            return MaterialPageRoute(builder: (context) => RegistrationPage());
          case '/login':
            return MaterialPageRoute(builder: (context) => LoginPage());
          default:
            return null;
        }
      },
    );
  }
}
