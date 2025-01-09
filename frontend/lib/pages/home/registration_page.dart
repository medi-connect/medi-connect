import 'package:flutter/material.dart';
import 'package:form_field_validator/form_field_validator.dart';

import '../../models/enums/UserType.dart';

// TODO: this is basic registration example, change code aligning with app
class RegistrationPage extends StatefulWidget {
  const RegistrationPage({Key? key}) : super(key: key);

  @override
  State<RegistrationPage> createState() => _RegistrationPageState();
}

class _RegistrationPageState extends State<RegistrationPage> {
  final _formKey = GlobalKey<FormState>();
  UserType _userType = UserType.DOCTOR;
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

  void _submitForm() {
    if (_formKey.currentState!.validate()) {
      _formKey.currentState!.save();
      print('User Data: $_userData');
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Login'),
      ),
      body: Form(
        key: _formKey,
        child: Center(
          child: SingleChildScrollView(
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 8.0),
              constraints: const BoxConstraints(
                maxWidth: 1000, // Constrain the width of the form
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.stretch,
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
                    label: "First Name",
                    hint: "Enter your first name",
                    icon: Icons.person,
                    validator: MultiValidator([
                      RequiredValidator(errorText: 'First name is required'),
                      MinLengthValidator(3, errorText: 'Minimum 3 characters'),
                    ]),
                    onSaved: (value) => _userData['firstName'] = value ?? '',
                  ),
                  _buildTextField(
                    label: "Last Name",
                    hint: "Enter your last name",
                    icon: Icons.person_outline,
                    validator: MultiValidator([
                      RequiredValidator(errorText: 'Last name is required'),
                      MinLengthValidator(3, errorText: 'Minimum 3 characters'),
                    ]),
                    onSaved: (value) => _userData['lastName'] = value ?? '',
                  ),
                  _buildTextField(
                    label: "Email",
                    hint: "Enter your email address",
                    icon: Icons.email,
                    inputType: TextInputType.emailAddress,
                    validator: MultiValidator([
                      RequiredValidator(errorText: 'Email is required'),
                      EmailValidator(errorText: 'Enter a valid email address'),
                    ]),
                    onSaved: (value) => _userData['email'] = value ?? '',
                  ),
                  _buildTextField(
                    label: "Mobile Number",
                    hint: "Enter your mobile number",
                    icon: Icons.phone,
                    inputType: TextInputType.phone,
                    validator: MultiValidator([
                      RequiredValidator(errorText: 'Mobile number is required'),
                      PatternValidator(r'^\d{10}$',
                          errorText: 'Enter a valid 10-digit mobile number'),
                    ]),
                    onSaved: (value) => _userData['mobile'] = value ?? '',
                  ),
                  Container(
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.start,
                      children: <Widget>[
                        Container(
                          width: double.infinity,
                          child: Padding(
                            padding: const EdgeInsets.symmetric(
                                vertical: 16.0, horizontal: 32),
                            child: Text(
                              "I am a:",
                              style: TextStyle(
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                          ),
                        ),
                        ListTile(
                          title: const Text('Doctor'),
                          leading: Radio<UserType>(
                            value: UserType.DOCTOR,
                            groupValue: _userType,
                            onChanged: (value) {
                              setState(() {
                                _userType = value!;
                                print("Button value: $value");
                              });
                            },
                          ),
                        ),
                        ListTile(
                          title: const Text('Patient'),
                          leading: Radio<UserType>(
                            value: UserType.PATIENT,
                            groupValue: _userType,
                            onChanged: (value) {
                              setState(() {
                                _userType = value!;
                                print("Button value: $value");
                              });
                            },
                          ),
                        ),
                      ],
                    ),
                  ),
                  const SizedBox(height: 20),
                  Row(children: [
                    const SizedBox(width: 20),
                    ElevatedButton(
                      onPressed: _submitForm,
                      child: const Text(
                        'Login',
                        style: TextStyle(fontSize: 18, color: Colors.white),
                      ),
                      style: ElevatedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(
                            vertical: 15, horizontal: 20),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12.0),
                        ),
                        backgroundColor: Colors.blueAccent,
                      ),
                    ),
                  ]),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}
