import 'package:flutter/material.dart';
import 'package:form_field_validator/form_field_validator.dart';

// TODO: this is basic registration example, change code aligning with app
class RegistrationPage extends StatefulWidget {
  const RegistrationPage({Key? key}) : super(key: key);

  @override
  State<RegistrationPage> createState() => _RegistrationPageState();
}

class _RegistrationPageState extends State<RegistrationPage> {
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
        title: const Text('Register'),
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              const SizedBox(height: 20),
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
                  PatternValidator(r'^\d{10}$', errorText: 'Enter a valid 10-digit mobile number'),
                ]),
                onSaved: (value) => _userData['mobile'] = value ?? '',
              ),
              const SizedBox(height: 20),
              ElevatedButton(
                onPressed: _submitForm,
                child: const Text(
                  'Register',
                  style: TextStyle(fontSize: 18),
                ),
                style: ElevatedButton.styleFrom(
                  padding: const EdgeInsets.symmetric(vertical: 15),
                  shape: RoundedRectangleBorder(
                    borderRadius: BorderRadius.circular(12.0),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
