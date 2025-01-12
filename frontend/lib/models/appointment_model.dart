import 'package:flutter/foundation.dart';

enum AppointmentStatus {
  pending,
  confirmed,
  declined,
  canceled,
  done,
}

extension AppointmentStatusExtension on AppointmentStatus {
  String toShortString() {
    return describeEnum(this).toUpperCase();
  }

  static AppointmentStatus fromString(String status) {
    switch (status.toUpperCase()) {
      case 'CONFIRMED':
        return AppointmentStatus.confirmed;
      case 'DECLINED':
        return AppointmentStatus.declined;
      case 'CANCELED':
        return AppointmentStatus.canceled;
      case 'DONE':
        return AppointmentStatus.done;
      default:
        return AppointmentStatus.pending;
    }
  }
}

class AppointmentModel {
  final int id;
  final DateTime startTime;
  final DateTime? endTime;
  final String title;
  final String? description;
  final AppointmentStatus status;
  final int doctorId;
  final int patientId;
  final bool createdBy; // False for Patient, True for Doctor
  final DateTime sysTimestamp;
  final DateTime sysCreated;

  AppointmentModel({
    required this.id,
    required this.startTime,
    this.endTime,
    required this.title,
    this.description,
    this.status = AppointmentStatus.pending,
    required this.doctorId,
    required this.patientId,
    required this.createdBy,
    required this.sysTimestamp,
    required this.sysCreated,
  });

  factory AppointmentModel.fromJson(Map<String, dynamic> json) {
    return AppointmentModel(
      id: json['id'],
      startTime: DateTime.parse(json['startTime']),
      endTime: json['endTime'] != null ? DateTime.parse(json['endTime']) : null,
      title: json['title'],
      description: json['description'],
      status: AppointmentStatusExtension.fromString(json['status']),
      doctorId: json['doctorId'],
      patientId: json['patientId'],
      createdBy: json['createdBy'],
      sysTimestamp: DateTime.parse(json['sysTimestamp']),
      sysCreated: DateTime.parse(json['sysCreated']),
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'id': id,
      'startTime': startTime.toIso8601String(),
      'endTime': endTime?.toIso8601String(),
      'title': title,
      'description': description,
      'status': status.toShortString(),
      'doctorId': doctorId,
      'patientId': patientId,
      'createdBy': createdBy,
      'sysTimestamp': sysTimestamp.toIso8601String(),
      'sysCreated': sysCreated.toIso8601String(),
    };
  }

  @override
  String toString() {
    return 'AppointmentModel(id: $id, title: $title, status: $status, startTime: $startTime, endTime: $endTime, doctorId: $doctorId, patientId: $patientId)';
  }

  bool isOverlapping(DateTime otherStartTime, DateTime otherEndTime) {
    return startTime.isBefore(otherEndTime) && otherStartTime.isBefore(endTime ?? otherStartTime);
  }
}
