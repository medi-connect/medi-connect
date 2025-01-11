import 'package:frontend/models/user_model.dart';

class DoctorModel extends UserModel{
  final String speciality;

  DoctorModel(this.speciality,
      {required super.id,
        required super.email,
        required super.name,
        required super.surname,
        required super.token,
        required super.tokenExpiration,
      });
}