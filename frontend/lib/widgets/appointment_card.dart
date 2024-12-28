import 'package:flutter/material.dart';

class AppointmentCard extends StatelessWidget {
  final String patientName;
  final String doctorName;
  final DateTime appointmentDate;
  final String status;
  final Function()? onCancel;
  final Function()? onConfirm;

  const AppointmentCard({
    Key? key,
    required this.patientName,
    required this.doctorName,
    required this.appointmentDate,
    required this.status,
    this.onCancel,
    this.onConfirm,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.symmetric(vertical: 8, horizontal: 12),
      elevation: 4,
      child: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text("Patient: $patientName", style: const TextStyle(fontSize: 16)),
            Text("Doctor: $doctorName", style: const TextStyle(fontSize: 16)),
            Text(
              "Date: ${appointmentDate.toLocal()}",
              style: const TextStyle(fontSize: 14, color: Colors.grey),
            ),
            Text(
              "Status: $status",
              style: TextStyle(
                fontSize: 14,
                color: status == "Pending" ? Colors.orange : Colors.green,
              ),
            ),
            if (onCancel != null || onConfirm != null)
              Row(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  if (onCancel != null)
                    TextButton(
                      onPressed: onCancel,
                      child: const Text("Cancel", style: TextStyle(color: Colors.red)),
                    ),
                  if (onConfirm != null)
                    TextButton(
                      onPressed: onConfirm,
                      child: const Text("Confirm", style: TextStyle(color: Colors.green)),
                    ),
                ],
              ),
          ],
        ),
      ),
    );
  }
}
