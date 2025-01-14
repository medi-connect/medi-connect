import 'package:flutter/material.dart';

import '../user_account.dart';

class NavigationBarWidget extends StatefulWidget {
  const NavigationBarWidget(
      {Key? key,
      required this.toLogin,
      required this.toRegister,
      required this.refreshHome})
      : super(key: key);

  final VoidCallback toLogin;
  final VoidCallback toRegister;
  final VoidCallback refreshHome;

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
                onPressed: () => widget.toRegister(),
                icon: Icon(Icons.app_registration),
                tooltip: "Register",
              ),
              IconButton(
                onPressed: () => widget.toLogin(),
                icon: Icon(Icons.login),
                tooltip: "Login",
              ),
              IconButton(
                onPressed: () => {
                  UserAccount().logout(),
                  widget.refreshHome(),
                },
                icon: Icon(Icons.logout),
                tooltip: "Log out",
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
