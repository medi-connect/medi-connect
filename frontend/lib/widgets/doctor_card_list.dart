import 'package:flutter/material.dart';

import '../models/doctor_model.dart';

class DoctorCardList extends StatefulWidget {
  final List<DoctorModel> doctors;
  final ValueChanged<int> onDoctorSelected;

  const DoctorCardList({
    Key? key,
    required this.doctors,
    required this.onDoctorSelected,
  }) : super(key: key);

  @override
  State<DoctorCardList> createState() => _DoctorCardListState();
}

class _DoctorCardListState extends State<DoctorCardList> {
  int? _selectedDoctor;

  @override
  void initState() {
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return ListView.builder(
        shrinkWrap: true,
        physics: const NeverScrollableScrollPhysics(),
        itemCount: widget.doctors.length,
        itemBuilder: (context, index) {
          final doctor = widget.doctors[index];
          return _getCard(doctor);
        });
  }

  Widget _getCard(DoctorModel doctor) {
    return GestureDetector(
      onTap: () {
        setState(() {
          _selectedDoctor = doctor.id;
        });
        widget.onDoctorSelected(doctor.id);
      },
      child: Card(
        color: _selectedDoctor != null && doctor.id == _selectedDoctor
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
                doctor.name ?? "Name: N/A",
                style:
                    const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
              ),
              const SizedBox(height: 4),
              Text(
                doctor.surname ?? "Surname: N/A",
                style: const TextStyle(fontSize: 16),
              ),
              const SizedBox(height: 4),
              Text(
                "Speciality: ${doctor.speciality ?? "N/A"}",
                style: const TextStyle(fontSize: 16),
              ),
              const SizedBox(height: 4),
              Text(
                "User ID: ${doctor.id}",
                style: const TextStyle(fontSize: 14, color: Colors.grey),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
