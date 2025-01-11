import 'package:frontend/models/user_model.dart';

class PatientModel extends UserModel {
  final DateTime birthDate;

  PatientModel(
    this.birthDate, {
    required super.id,
    required super.email,
    required super.name,
    required super.surname,
    required super.token,
    required super.tokenExpiration,
  });
}