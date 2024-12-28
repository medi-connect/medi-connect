import 'package:flutter/material.dart';
import 'package:frontend/models/appointment_model.dart';
import 'package:frontend/services/appointment_api.dart';
import 'dart:developer';
import '../../widgets/appointment_card.dart';

class HomePage extends StatefulWidget {
  const HomePage({Key? key}) : super(key: key);

  @override
  State<HomePage> createState() => _HomePageState();
}

class _HomePageState extends State<HomePage> {
  List<AppointmentModel> appointments = [];
  //patient, doctor / user
  bool isLoading = true;

  @override
  void initState() {
    super.initState();
    fetchAppointments();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text("My Appointments")),
      body: isLoading
          ? const Center(child: CircularProgressIndicator())
          : appointments.isEmpty
          ? const Center(child: Text("No appointments found"))
          : ListView.builder(
        itemCount: appointments.length,
        itemBuilder: (context, index) {
          final appointment = appointments[index];
          return AppointmentCard(
            patientName: 'Patient ID: ${appointment.patientId}',
            doctorName: 'Doctor ID: ${appointment.doctorId}',
            appointmentDate: appointment.startTime,
            status: appointment.status.toShortString(),
            // onCancel: () => handleCancel(appointment),
            // onConfirm: () => handleConfirm(appointment),
          );
        },
      ),
    );
  }

  Future<void> fetchAppointments() async {
    try {
      var response = await AppointmentApi().fetchAppointmentsForDoctor("1008");// hardcoded just for now, todo:change
      print(response["appointments"].toString());
      log(response.toString());

      if (response["status"] == 200) {
        final List<dynamic> jsonAppointments = response["appointments"];
        setState(() {
          appointments = jsonAppointments
              .map((json) => AppointmentModel.fromJson(json))
              .toList();
          isLoading = false;
        });
      } else {
        setState(() => isLoading = false);
        print("Error fetching appointments: ${response["message"]}");
      }
    } catch (error) {
      setState(() => isLoading = false);
      print("Error fetching appointments: $error");
    }
  }
}
