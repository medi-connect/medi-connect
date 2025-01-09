import 'package:flutter/material.dart';
import 'package:frontend/models/appointment_model.dart';
import 'package:frontend/widgets/navigation_bar.dart';

class HomePage extends StatefulWidget {
  const HomePage({Key? key}) : super(key: key);

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> with TickerProviderStateMixin {
  List<AppointmentModel> appointments = [];
  bool isLoading = true;


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
          ),
        ],
      ),
    );
  }

  Future<void> _toLogin() async {
      final result = await Navigator.of(context).pushNamed('/login');
      print(result.toString());
  }
}
