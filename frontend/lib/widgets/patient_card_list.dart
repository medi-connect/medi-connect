import 'package:flutter/material.dart';
import 'package:frontend/models/patient_model.dart';

class PatientCardList extends StatefulWidget {
  final List<PatientModel> patients;
  final ValueChanged<int> onPatientSelected;

  const PatientCardList({
    Key? key,
    required this.patients,
    required this.onPatientSelected,
  }) : super(key: key);

  @override
  State<PatientCardList> createState() => _PatientCardListState();
}

class _PatientCardListState extends State<PatientCardList> {
  int? _selectedPatient;

  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return ListView.builder(
        shrinkWrap: true,
        physics: const NeverScrollableScrollPhysics(),
        itemCount: widget.patients.length,
        itemBuilder: (context, index) {
          final patient = widget.patients[index];
          return _getCard(patient);
        });
  }

  Widget _getCard(PatientModel patient) {
    return GestureDetector(
      onTap: () {
        setState(() {
          _selectedPatient = patient.id;
        });
        widget.onPatientSelected(patient.id);
      },
      child: Card(
        color: _selectedPatient != null && patient.id == _selectedPatient
            ? Colors.blue.shade100
            : Colors.white,
        elevation: 4,
        margin: const EdgeInsets.symmetric(vertical: 8, horizontal: 16),
        child: Padding(
          padding: const EdgeInsets.all(16.0),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                patient.name ?? "Name: N/A",
                style:
                const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 4),
              Text(
                patient.surname ?? "Surname: N/A",
                style: const TextStyle(fontSize: 16),
              ),
              const SizedBox(height: 4),
              // Text(
              //   "Speciality: ${doctor.speciality ?? "N/A"}",
              //   style: const TextStyle(fontSize: 16),
              // ),
              const SizedBox(height: 4),
              Text(
                "User ID: ${patient.id}",
                style: const TextStyle(fontSize: 14, color: Colors.grey),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
