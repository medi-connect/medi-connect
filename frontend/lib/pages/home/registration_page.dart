import 'package:flutter/material.dart';
import 'package:form_field_validator/form_field_validator.dart';
import 'package:frontend/services/doctor_api.dart';
import 'package:frontend/services/patient_api.dart';
import '../../models/enums/UserType.dart';
import 'package:fluttertoast/fluttertoast.dart';

class RegistrationPage extends StatefulWidget {
  const RegistrationPage({Key? key}) : super(key: key);

  @override
  State<RegistrationPage> createState() => _RegistrationPageState();
}

class _RegistrationPageState extends State<RegistrationPage> {
  final _formKey = GlobalKey<FormState>();
  UserType _userType = UserType.DOCTOR;
  bool _passwordMatch = true;
  final Map<String, String> _userData = {};

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Register'),
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
                    onSaved: (value) => _userData['name'] = value ?? '',
                  ),
                  _buildTextField(
                    label: "Last Name",
                    hint: "Enter your last name",
                    icon: Icons.person_outline,
                    validator: MultiValidator([
                      RequiredValidator(errorText: 'Last name is required'),
                      MinLengthValidator(3, errorText: 'Minimum 3 characters'),
                    ]),
                    onSaved: (value) => _userData['surname'] = value ?? '',
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
                    label: "Password",
                    hint: "Enter your password",
                    icon: Icons.key_sharp,
                    validator: MultiValidator([
                      RequiredValidator(errorText: 'Password is required!'),
                      MinLengthValidator(6, errorText: "At least 6 characters required!"),
                    ]),
                    onSaved: (value) => _userData['password'] = value ?? '',
                    isObscure: true,
                  ),
                  _buildTextField(
                    label: "Repeat password",
                    hint: "Enter your password again",
                    icon: Icons.key_sharp,
                    validator: MultiValidator([
                      RequiredValidator(errorText: 'Password repeat is required!'),
                      MinLengthValidator(6, errorText: "At least 6 characters required!"),
                    ]),
                    onSaved: (value) => _userData['passwordRepeat'] = value ?? '',
                    isObscure: true,
                  ),
                  if (_userType == UserType.DOCTOR)
                    _buildTextField(
                    label: "Speciality",
                    hint: "Enter your speciality",
                    icon: Icons.medical_services_outlined,
                    validator: MultiValidator([
                      RequiredValidator(errorText: 'Speciality is required'),
                      // PatternValidator(r'^\d{10}$',
                      //     errorText: 'Enter a valid 10-digit mobile number'),
                    ]),
                    onSaved: (value) => _userData['speciality'] = value ?? '',
                  ),
                  if (_userType == UserType.PATIENT)
                    _buildTextField(
                      label: "Date of birth",
                      hint: "Enter your date of birth in format YYYY-MM-DD",
                      icon: Icons.date_range,
                      inputType: TextInputType.datetime,
                      validator: MultiValidator([
                        RequiredValidator(errorText: 'Date of birth is required'),
                        PatternValidator(r'^\d{4}\-(0[1-9]|1[012])\-(0[1-9]|[12][0-9]|3[01])$',
                            errorText: 'Bad input, check your format!'),
                        // DateValidator('YYYY-MM-DD', errorText: "Bad input, check your input!")
                      ]),
                      onSaved: (value) => _userData['birthDate'] = value ?? '',
                    ),
                  Container(
                    child: Column(
                      mainAxisAlignment: MainAxisAlignment.start,
                      children: <Widget>[
                        Container(
                          width: double.infinity,
                          child: const Padding(
                            padding: EdgeInsets.symmetric(
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
                              });
                            },
                          ),
                        ),
                      ],
                    ),
                  ),
                  if (!_passwordMatch)
                    const Text(
                      "Passwords don't match!",
                      style: TextStyle(
                        fontWeight: FontWeight.bold,
                        color: Colors.red,
                      ),
                    ),
                  const SizedBox(height: 20),
                  Row(children: [
                    const SizedBox(width: 20),
                    ElevatedButton(
                      onPressed: _submitForm,
                      style: ElevatedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(
                            vertical: 15, horizontal: 20),
                        shape: RoundedRectangleBorder(
                          borderRadius: BorderRadius.circular(12.0),
                        ),
                        backgroundColor: Colors.blueAccent,
                      ),
                      child: const Text(
                        'Register',
                        style: TextStyle(fontSize: 18, color: Colors.white),
                      ),
                    ),
                  ]),
                  const SizedBox(height: 20),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }

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

  Future<void> _submitForm() async {
    if (_formKey.currentState!.validate()) {
      _formKey.currentState!.save();
      print('User Data: $_userData');
    }

    if(_userData["password"].toString() != _userData["passwordRepeat"].toString() && mounted) {
      setState(() {
        _passwordMatch = false;
      });
      return;
    }

    if (_userType == UserType.DOCTOR) {
      await _registerDoctor();
    } else{
      await _registerPatient();
    }
  }

  Future<void> _registerDoctor() async {
    final register = await DoctorAPI().register(
      _userData["email"].toString(),
      _userData["speciality"].toString(),
      _userData["password"].toString(),
      _userData["name"].toString(),
      _userData["surname"].toString(),
    );

    if (register["status"] == 200) {
      if (mounted) Navigator.pop(context, "success");
    } else {
      print(register["message"]);
      Fluttertoast.showToast(msg: register["message"]);
    }
  }

  Future<void> _registerPatient() async {
    final register = await PatientAPI().register(
      _userData["email"].toString(),
      _userData["birthDate"].toString(),
      _userData["password"].toString(),
      _userData["name"].toString(),
      _userData["surname"].toString(),
    );

    if (register["status"] == 200) {
      if (mounted) Navigator.pop(context, "success");
    } else {
      Fluttertoast.showToast(msg: register["message"]);
    }
  }
}
