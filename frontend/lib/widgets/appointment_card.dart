import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:frontend/models/doctor_model.dart';
import 'package:frontend/models/patient_model.dart';
import 'package:frontend/services/appointment_api.dart';
import 'package:frontend/user_account.dart';

import '../models/appointment_model.dart';
import '../models/enums/UserType.dart';
import 'package:fluttertoast/fluttertoast.dart';

class AppointmentCard extends StatefulWidget {
  final AppointmentModel appointment;
  final Function()? onCancel;
  final Function()? onConfirm;
  final UserType userType;

  const AppointmentCard({
    Key? key,
    required this.appointment,
    this.onCancel,
    this.onConfirm,
    required this.userType,
  }) : super(key: key);

  @override
  State<AppointmentCard> createState() => _HomePageState();
}

class _HomePageState extends State<AppointmentCard>
    with TickerProviderStateMixin {
  late PatientModel _patient;
  late DoctorModel _doctor;
  late AppointmentStatus _status;
  late TextEditingController _descriptionController;

  @override
  void initState() {
    super.initState();
    print(widget.appointment.id);
    _status = widget.appointment.status;
    print(widget.appointment.description);
    _descriptionController = TextEditingController(text: widget.appointment.description);
  }

  @override
  void dispose() {
    _descriptionController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return FutureBuilder<void>(
      future:
          widget.userType == UserType.PATIENT ? _getDoctor() : _getPatient(),
      builder: (BuildContext context, AsyncSnapshot<void> snapshot) {
        if (snapshot.connectionState == ConnectionState.done) {
          return Card(
            margin: const EdgeInsets.symmetric(vertical: 8, horizontal: 12),
            elevation: 4,
            child: Padding(
              padding: const EdgeInsets.all(16.0),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(widget.appointment.title, style: TextStyle(fontWeight: FontWeight.bold, fontSize: 22),),
                  SizedBox(height: 16,),
                  if (widget.userType == UserType.DOCTOR)
                    Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text(
                          "Edit Description:",
                          style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
                        ),
                        TextFormField(
                          controller: _descriptionController,
                          maxLines: 3,
                          decoration: const InputDecoration(
                            border: OutlineInputBorder(),
                            hintText: "Enter appointment description",
                          ),
                        ),
                        const SizedBox(height: 16),
                        ElevatedButton(
                          onPressed: () {
                            _editDescription(_descriptionController.text);
                          },
                          child: const Text("Update Description"),
                        ),
                      ],
                    )
                  else
                    Text(
                      widget.appointment.description!,
                      style: const TextStyle(
                          fontWeight: FontWeight.w400, fontSize: 15),
                    ),
                  Text("Patient: ${_patient.name} ${_patient.surname}",
                      style: const TextStyle(fontSize: 16)),
                  Text("Doctor: ${_doctor.name} ${_doctor.surname}",
                      style: const TextStyle(fontSize: 16)),
                  Text(
                    "Start: ${widget.appointment.startTime.toLocal()}",
                    style: const TextStyle(fontSize: 14, color: Colors.grey),
                  ),
                  Text(
                    "End: ${widget.appointment.endTime?.toLocal()}",
                    style: const TextStyle(fontSize: 14, color: Colors.grey),
                  ),
                  if (widget.userType == UserType.PATIENT)
                    Text(
                    "Status: ${widget.appointment.status.toShortString()}",
                    style: TextStyle(
                      fontSize: 14,
                      color:
                          widget.appointment.status.toShortString() == "Pending"
                              ? Colors.orange
                              : Colors.green,
                    ),
                  ) else
                    Row(
                      children: [
                        const Text(
                          "Status:",
                          style: TextStyle(fontSize: 14),
                        ),
                        const SizedBox(width: 8),
                        DropdownButton<AppointmentStatus>(
                          value: _status,
                          items: [AppointmentStatus.pending, AppointmentStatus.canceled, AppointmentStatus.confirmed, AppointmentStatus.declined]
                              .map((status) => DropdownMenuItem<AppointmentStatus>(
                            value: status,
                            child: Text(status.toShortString()),
                          ))
                              .toList(),
                          onChanged: (newStatus) {
                            setState(() {
                              _status = newStatus!;
                              // widget.appointment.status =
                              //     AppointmentStatus.values.firstWhere(
                              //             (e) =>
                              //         e.toShortString() == newStatus);
                            });
                            // _updateStatus(); // Call the backend/state update
                          },
                        ),
                        SizedBox(
                          width: 16,
                        ),
                        ElevatedButton(
                            onPressed: () {
                              _editStatus(_status.toShortString());
                            }, child: Text("Confirm change")),
                      ],
                    )
                  // if (onCancel != null || onConfirm != null)
                  //   Row(
                  //     mainAxisAlignment: MainAxisAlignment.end,
                  //     children: [
                  //       if (onCancel != null)
                  //         TextButton(
                  //           onPressed: onCancel,
                  //           child: const Text("Cancel", style: TextStyle(color: Colors.red)),
                  //         ),
                  //       if (onConfirm != null)
                  //         TextButton(
                  //           onPressed: onConfirm,
                  //           child: const Text("Confirm", style: TextStyle(color: Colors.green)),
                  //         ),
                  //     ],
                  //   ),
                ],
              ),
            ),
          );
        } else {
          return CupertinoActivityIndicator();
        }
      },
    );
  }

  Future<void> _getPatient() async {
    final patient =
        await UserAccount().getPatient(widget.appointment.patientId.toString());
    if (patient != null) {
      _patient = patient;
      _doctor = UserAccount().doctor!;
    }
  }

  Future<void> _getDoctor() async {
    final doctor =
        await UserAccount().getDoctor(widget.appointment.doctorId.toString());
    if (doctor != null) {
      _doctor = doctor;
      _patient = UserAccount().patient!;
    }
  }

  Future<void> _editStatus(String status) async {
    final response = await AppointmentAPI().modifyStatus(widget.appointment.id, status.toUpperCase());

    if (response["status"] == 200) {
      Fluttertoast.showToast(msg: "Successfull status change");
    } else {
      print(response["message"]);
      Fluttertoast.showToast(msg: response["message"]);
    }
  }

  Future<void> _editDescription(String description) async {
    final response = await AppointmentAPI().modifyDescription(widget.appointment.id, description);

    if (response["status"] == 200) {
      Fluttertoast.showToast(msg: "Description updated successfully");
    } else {
      print(response["message"]);
      Fluttertoast.showToast(msg: response["message"]);
    }
  }
}
