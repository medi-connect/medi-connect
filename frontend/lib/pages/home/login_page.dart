import 'package:flutter/material.dart';
import 'package:form_field_validator/form_field_validator.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'package:frontend/models/patient_model.dart';
import 'package:frontend/services/doctor_api.dart';
import '../../services/patient_api.dart';
import '../../services/user_api.dart';

class LoginPage extends StatefulWidget {
  const LoginPage({Key? key}) : super(key: key);

  @override
  State<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends State<LoginPage> {
  final _formKey = GlobalKey<FormState>();
  final Map<String, String> _userData = {};

  Widget _buildTextField({
    required String label,
    required String hint,
    required IconData icon,
    required MultiValidator validator,
    TextInputType inputType = TextInputType.text,
    bool isObscure = false,
    void Function(String?)? onSaved,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8.0),
      child: TextFormField(
        keyboardType: inputType,
        obscureText: isObscure,
        validator: validator,
        onSaved: onSaved,
        decoration: InputDecoration(
          labelText: label,
          hintText: hint,
          prefixIcon: Icon(icon),
          errorStyle: const TextStyle(fontSize: 14.0),
          border: OutlineInputBorder(
            borderRadius: BorderRadius.circular(12.0),
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Login'),
      ),
      body: Form(
        key: _formKey,
        child: Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            SingleChildScrollView(
              child: Container(
                padding: const EdgeInsets.symmetric(horizontal: 8.0),
                constraints: const BoxConstraints(
                  maxWidth: 1000, // Constrain the width of the form
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Center(
                      child: Image.asset(
                        "assets/images/mc_logo.png",
                        width: 150,
                        height: 120,
                      ),
                    ),
                    const SizedBox(height: 20),
                    _buildTextField(
                      label: "Email",
                      hint: "Enter your email address",
                      icon: Icons.email,
                      inputType: TextInputType.emailAddress,
                      validator: MultiValidator([
                        RequiredValidator(errorText: "Email is required"),
                        EmailValidator(
                            errorText: "Enter a valid email address"),
                      ]),
                      onSaved: (value) => _userData["email"] = value ?? "",
                    ),
                    _buildTextField(
                      label: "Password",
                      hint: "Enter your password",
                      icon: Icons.key_sharp,
                      validator: MultiValidator([
                        RequiredValidator(errorText: "Last name is required"),
                        MinLengthValidator(3,
                            errorText: "Minimum 3 characters"),
                      ]),
                      onSaved: (value) => _userData["password"] = value ?? "",
                    ),
                    const SizedBox(height: 20),
                    Row(children: [
                      const SizedBox(width: 8),
                      ElevatedButton(
                        onPressed: _login,
                        style: ElevatedButton.styleFrom(
                          padding: const EdgeInsets.symmetric(
                              vertical: 15, horizontal: 20),
                          shape: RoundedRectangleBorder(
                            borderRadius: BorderRadius.circular(12.0),
                          ),
                          backgroundColor: Colors.blueAccent,
                        ),
                        child: const Text(
                          'Login',
                          style: TextStyle(fontSize: 18, color: Colors.white),
                        ),
                      ),
                    ]),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _login() async {
    if (_formKey.currentState!.validate()) {
      _formKey.currentState!.save();
      print('User Data: $_userData');
    }

    if (_userData["email"] == null || _userData["password"] == null) {
      return;
    }

    var userLogin = await UserAPI().login(_userData["email"]!, _userData["password"]!);

    if (userLogin["status"] == 200) {
      if (userLogin["response"]["token"] != null && userLogin["response"]["token"].toString().isNotEmpty) {

        if (userLogin["response"]["isDoctor"] as bool) {
          //todo see issue with doctorservice api, GET method specifically
          await _getDoctor(userLogin["response"]["userId"].toString());
        } else {
          await _getPatient(userLogin["response"]["userId"].toString());
        }

      }
    } else {
      Fluttertoast.showToast(msg: userLogin["message"]);
    }
  }

  Future<void> _getDoctor(String id) async {
    var getDoctor = await DoctorAPI().get(id);
    switch (getDoctor["status"]){
      case 200:
        DateTime birthDate = DateTime.parse(getDoctor["response"]["birthDate"].toString());

        final patient = PatientModel(
          birthDate,
          id: getDoctor["response"]["userId"] ?? getDoctor["response"]["userId"] ?? "none",
          email: _userData["email"] != null ? _userData["email"]! : "none",
          name: getDoctor["response"]["name"] ?? getDoctor["response"]["name"] ?? "none",
          surname: getDoctor["response"]["surname"] ?? getDoctor["response"]["surname"] ?? "none",
        );
        if (context.mounted) {
          Navigator.pop(context, patient);
        }
        break;
      default:
        Fluttertoast.showToast(msg: getDoctor["message"]);
        break;
    }
  }

  Future<void> _getPatient(String id) async {
    var getPatient = await PatientAPI().get(id);
    switch (getPatient["status"]){
      case 200:
        DateTime birthDate = DateTime.parse(getPatient["response"]["birthDate"].toString());

        final patient = PatientModel(
          birthDate,
          id: getPatient["response"]["userId"] ?? getPatient["response"]["userId"] ?? "none",
          email: _userData["email"] != null ? _userData["email"]! : "none",
          name: getPatient["response"]["name"] ?? getPatient["response"]["name"] ?? "none",
          surname: getPatient["response"]["surname"] ?? getPatient["response"]["surname"] ?? "none",
        );
        if (context.mounted) {
          Navigator.pop(context, patient);
        }
        break;
      default:
        Fluttertoast.showToast(msg: getPatient["message"]);
        break;
    }
  }
}
