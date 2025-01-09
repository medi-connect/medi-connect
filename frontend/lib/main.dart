import 'package:flutter/material.dart';
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
      routes: {
        '/': (context) => HomePage(),
        "/register": (context) => RegistrationPage(),
        "/login": (context) => LoginPage(),
      },
    );
  }
}
