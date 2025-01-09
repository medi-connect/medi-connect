import 'package:flutter/material.dart';

class NavigationBarWidget extends StatefulWidget {
  const NavigationBarWidget({Key? key, required this.toLogin}) : super(key: key);

  final VoidCallback toLogin;
  @override
  State<NavigationBarWidget> createState() => _NavigationBarWidgetState();
}

class _NavigationBarWidgetState extends State<NavigationBarWidget>
    with TickerProviderStateMixin {

  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      color: Colors.yellowAccent.shade100.withOpacity(0.5),
      child: Column(
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Image.asset(
                "assets/images/mc_logo.png",
                width: 150,
                height: 120,
              ),
              IconButton(
                icon: Icon(Icons.home),
                onPressed: () {},
                tooltip: "Home",
              ),
              IconButton(
                onPressed: () {},
                icon: Icon(Icons.info),
                tooltip: "About us",
              ),
              IconButton(
                onPressed: () {},
                icon: Icon(Icons.app_registration),
                tooltip: "Register",
              ),
              IconButton(
                onPressed: () => widget.toLogin(),
                icon: Icon(Icons.login),
                tooltip: "Login",
              ),
              SizedBox(
                width: 8,
              )
            ],
          ),
        ],
      ),
    );
  }
}
