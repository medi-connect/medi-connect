
class UserModel {
  final int id;
  final String email;
  final String name;
  final String surname;
  final String token;
  final String tokenExpiration;
  // final bool status;

  UserModel({
    required this.id,
    required this.email,
    required this.name,
    required this.surname,
    required this.token,
    required this.tokenExpiration,
    // required this.status,
});
}
