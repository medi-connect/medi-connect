import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:frontend/models/appointment_model.dart';
import 'package:frontend/services/appointment_api.dart';
import '../../models/enums/UserType.dart';
import '../../widgets/appointment_card.dart';
import 'package:fluttertoast/fluttertoast.dart';

class AppointmentsPage extends StatefulWidget {
  const AppointmentsPage({Key? key, required this.userType, required this.userId}) : super(key: key);

  final UserType userType;
  final int userId;
  @override
  State<AppointmentsPage> createState() => _AppointmentsPageState();
}

class _AppointmentsPageState extends State<AppointmentsPage> {
  List<AppointmentModel> _appointments = [];
  bool isLoading = true;

  @override
  void initState() {
    super.initState();
    // fetchAppointments();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
        appBar: AppBar(title: const Text("My Appointments")),
        body: FutureBuilder<void>(
          future: widget.userType == UserType.PATIENT
              ? _fetchPatientAppointments()
              : _fetchDoctorAppointments(),
          builder: (BuildContext context, AsyncSnapshot<void> snapshot) {
            if (snapshot.connectionState == ConnectionState.done) {
              return Container(
                child: ListView.builder(
                  itemCount: _appointments.length,
                  itemBuilder: (context, index) {
                    final appointment = _appointments[index];
                    return AppointmentCard(
                      appointment: appointment,
                      userType: widget.userType,
                      // onCancel: () => handleCancel(appointment),
                      // onConfirm: () => handleConfirm(appointment),
                    );
                  },
                ),
              );
            } else {
              return CupertinoActivityIndicator();
            }
          },
        )
    );
  }

  Future<void> _fetchPatientAppointments() async {
    _appointments.clear();

    final response = await AppointmentAPI().fetchAppointmentsForPatient(widget.userId.toString());

    if (response["status"] as int == 200) {
      for (final appointment in response["response"]) {
        DateTime startTime = DateTime.parse(appointment["startTime"].toString());
        DateTime endTime = DateTime.parse(appointment["endTime"].toString());
        DateTime sysTimestamp = DateTime.parse(appointment["sysTimestamp"].toString());
        DateTime sysCreated = DateTime.parse(appointment["sysCreated"].toString());

        late AppointmentStatus status = AppointmentStatusExtension.fromString(appointment["status"].toString());

        final appointmentModel = AppointmentModel(
          id: appointment["id"],
          startTime: startTime,
          endTime: endTime,
          title: appointment["title"] ?? "none",
          description: appointment["title"] ?? "none",
          status: status,
          doctorId: appointment["doctorId"] ?? "none",
          patientId: appointment["patientId"] ?? "none",
          createdBy: appointment["createdBy"] as bool,
          sysTimestamp: sysTimestamp,
          sysCreated: sysCreated,
        );
        _appointments.add(appointmentModel);
      }
    } else {
      print(response["message"]);
      Fluttertoast.showToast(msg: response["message"]);
    }
  }

  Future<void> _fetchDoctorAppointments() async {
    _appointments.clear();

    final response = await AppointmentAPI().fetchAppointmentsForDoctor(widget.userId.toString());

    if (response["status"] as int == 200) {

      for (final appointment in response["response"]) {
        DateTime startTime = DateTime.parse(appointment["startTime"].toString());
        DateTime endTime = DateTime.parse(appointment["endTime"].toString());
        DateTime sysTimestamp = DateTime.parse(appointment["sysTimestamp"].toString());
        DateTime sysCreated = DateTime.parse(appointment["sysCreated"].toString());

        late AppointmentStatus status = AppointmentStatusExtension.fromString(appointment["status"].toString());

        final appointmentModel = AppointmentModel(
          id: appointment["id"],
          startTime: startTime,
          endTime: endTime,
          title: appointment["title"] ?? "none",
          description: appointment["description"] ?? "none",
          status: status,
          doctorId: appointment["doctorId"] ?? "none",
          patientId: appointment["patientId"] ?? "none",
          createdBy: appointment["createdBy"] as bool,
          sysTimestamp: sysTimestamp,
          sysCreated: sysCreated,
        );
        _appointments.add(appointmentModel);
      }
    } else {
      print(response["message"]);
      Fluttertoast.showToast(msg: response["message"]);
    }
  }
}
