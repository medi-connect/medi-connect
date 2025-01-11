import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:form_field_validator/form_field_validator.dart';
import 'package:fluttertoast/fluttertoast.dart';
import 'package:frontend/models/appointment_model.dart';
import 'package:frontend/services/appointment_api.dart';
import 'package:frontend/services/doctor_api.dart';
import 'package:frontend/services/patient_api.dart';
import '../../models/doctor_model.dart';
import '../../models/enums/UserType.dart';
import '../../models/patient_model.dart';
import '../../user_account.dart';
import '../../widgets/doctor_card_list.dart';
import 'package:intl/intl.dart';
import '../../widgets/patient_card_list.dart';

class AppointmentCreatePage extends StatefulWidget {
  const AppointmentCreatePage({Key? key, required this.userType})
      : super(key: key);

  final UserType userType;

  @override
  State<AppointmentCreatePage> createState() => _AppointmentCreatePageState();
}

class _AppointmentCreatePageState extends State<AppointmentCreatePage> {
  final _formKey = GlobalKey<FormState>();
  final Map<String, dynamic> _appointmentData = {
    "StartTime": null,
    "EndTime": null,
    "Title": "",
    "Description": "",
    "Status": 0,
    "DoctorId": null,
    "PatientId": null,
    "CreatedBy": false,
    "SysTimestamp": null,
    "SysCreated": null,
  };
  final List<DoctorModel> _doctors = [];
  final List<PatientModel> _patients = [];
  final DateFormat _formatter = DateFormat("yyyy-MM-dd'T'HH:mm:ss");
  String title = "";
  String description = "";
  int? selectedPatient;
  final TextEditingController _titleController = TextEditingController();
  final TextEditingController _descriptionController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _titleController.text = _appointmentData["Title"] ?? "";
    _descriptionController.text = _appointmentData["Description"] ?? "";
  }

  @override
  void dispose() {
    _titleController.dispose();
    _descriptionController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Create Appointment'),
      ),
      body: FutureBuilder<void>(
        future: widget.userType == UserType.PATIENT
            ? _getAllDoctors()
            : _getAllPatients(),
        builder: (BuildContext context, AsyncSnapshot<void> snapshot) {
          if (snapshot.connectionState == ConnectionState.done) {
            return Form(
              key: _formKey,
              child: SingleChildScrollView(
                child: Container(
                  padding: const EdgeInsets.symmetric(
                      horizontal: 16.0, vertical: 20.0),
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
                        label: "Title",
                        hint: "Enter appointment title",
                        icon: Icons.title,
                        validator: MultiValidator([
                          RequiredValidator(errorText: "Title is required"),
                        ]),
                        controller: _titleController,
                        onSaved: (value) =>
                            _appointmentData["Title"] = value ?? "",
                      ),
                      _buildTextField(
                        label: "Description",
                        hint: "Enter appointment description",
                        icon: Icons.description,
                        validator: MultiValidator([
                          RequiredValidator(
                              errorText: "Description is required"),
                        ]),
                        controller: _descriptionController,
                        onSaved: (value) =>
                            _appointmentData["Description"] = value ?? "",
                      ),
                      _buildDateTimePicker(
                        context: context,
                        label: "Start Time",
                        key: "StartTime",
                      ),
                      _buildDateTimePicker(
                        context: context,
                        label: "End Time",
                        key: "EndTime",
                      ),
                      const SizedBox(height: 20),
                      const Text(
                        "Select a Doctor:",
                        style: TextStyle(
                            fontSize: 16, fontWeight: FontWeight.bold),
                      ),
                      const SizedBox(height: 10),
                      if (widget.userType == UserType.PATIENT)
                        DoctorCardList(
                          doctors: _doctors,
                          onDoctorSelected: (selectedDoctorId) {
                            _appointmentData["DoctorId"] = selectedDoctorId;
                          },
                        )
                      else
                        PatientCardList(
                          patients: _patients,
                          onPatientSelected: (selectedPatientId) {
                            _appointmentData["PatientId"] = selectedPatientId;
                          },
                        ),
                      const SizedBox(height: 20),
                      Center(
                        child: ElevatedButton(
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
                            'Create Appointment',
                            style: TextStyle(fontSize: 18, color: Colors.white),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            );
          } else {
            return const Center(child: CupertinoActivityIndicator());
          }
        },
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
    TextEditingController? controller,
    void Function(String?)? onSaved,
  }) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8.0),
      child: TextFormField(
        controller: controller,
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

  Future<void> _getAllDoctors() async {
    var getDoctors = await DoctorAPI().getAll();
    switch (getDoctors["status"]) {
      case 200:
        final List<DoctorModel> doctors = [];
        for (final doctor in getDoctors["response"]) {
          DoctorModel currentDoctor = DoctorModel(
            doctor["speciality"] ?? "none",
            id: doctor["userId"] ?? 0,
            email: doctor["email"] ?? "none",
            name: doctor["name"] ?? "none",
            surname: doctor["surname"] ?? "none",
            token: "none",
            tokenExpiration: "none",
          );
          doctors.add(currentDoctor);
        }
        _doctors.clear();
        _doctors.addAll(doctors);

        break;
      default:
        Fluttertoast.showToast(msg: getDoctors["message"]);
        break;
    }
  }

  Future<void> _getAllPatients() async {
    var getPatients = await PatientAPI().getAll();
    switch (getPatients["status"]) {
      case 200:
        final List<PatientModel> patients = [];
        for (final patient in getPatients["response"]) {
          DateTime birthDate = DateTime.parse(patient["birthDate"].toString());
          PatientModel currentPatient = PatientModel(
            birthDate,
            id: patient["userId"] ?? 0,
            email: patient["email"] ?? "none",
            name: patient["name"] ?? "none",
            surname: patient["surname"] ?? "none",
            token: "none",
            tokenExpiration: "none",
          );
          patients.add(currentPatient);
        }
        _patients.clear();
        _patients.addAll(patients);

        break;
      default:
        Fluttertoast.showToast(msg: getPatients["message"]);
        break;
    }
  }

  Future<void> _submitForm() async {
    if (_formKey.currentState!.validate()) {
      _formKey.currentState!.save();

      print("Appointment Created: ${_appointmentData.toString()}");
      Fluttertoast.showToast(
          msg: "Appointment Created: ${_appointmentData.toString()}");

      final appointmentCreate = await AppointmentAPI().create(
        _appointmentData["StartTime"],
        _appointmentData["EndTime"],
        _appointmentData["Title"],
        _appointmentData["Description"],
        AppointmentStatus.pending.index,
        widget.userType == UserType.PATIENT
            ? _appointmentData["DoctorId"]
            : UserAccount().doctor!.id,
        widget.userType == UserType.PATIENT
            ? UserAccount().patient!.id
            : _appointmentData["PatientId"],
        widget.userType == UserType.PATIENT ? false : true,
      );

      if (appointmentCreate["status"] == 200) {
        Fluttertoast.showToast(msg: "Success");
        if (mounted) Navigator.pop(context);
      } else {
        Fluttertoast.showToast(msg: appointmentCreate["message"]);
      }
    } else {
      Fluttertoast.showToast(msg: "Please fill out all required fields.");
    }
  }

  Future<void> _selectDateTime(
      {required BuildContext context, required String key}) async {
    DateTime? pickedDate = await showDatePicker(
      context: context,
      initialDate: DateTime.now(),
      firstDate: DateTime(2000),
      lastDate: DateTime(2100),
    );

    if (pickedDate == null) return; // User canceled

    TimeOfDay? pickedTime = await showTimePicker(
      context: context,
      initialTime: TimeOfDay.now(),
    );

    if (pickedTime == null) return; // User canceled

    // Combine date and time
    DateTime selectedDateTime = DateTime(
      pickedDate.year,
      pickedDate.month,
      pickedDate.day,
      pickedTime.hour,
      pickedTime.minute,
    );

    _appointmentData[key] = _formatter.format(selectedDateTime);
  }

  Widget _buildDateTimePicker(
      {required BuildContext context,
      required String label,
      required String key}) {
    return ListTile(
      leading: Icon(Icons.access_time),
      title: Text(
        _appointmentData[key] ?? "Select $label",
        style: TextStyle(
          color: _appointmentData[key] != null
              ? Colors.black
              : Colors.grey, // Grey text if not selected
        ),
      ),
      trailing: Icon(Icons.calendar_today),
      onTap: () async =>
          {await _selectDateTime(context: context, key: key), _refresh()},
    );
  }

  void _refresh() {
    if (mounted) setState(() {});
  }
}
