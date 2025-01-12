
class FeedbackModel {
  final int id;
  final int rate;
  final String review;
  final int appointmentId;

  FeedbackModel({
    required this.id,
    required this.rate,
    required this.review,
    required this.appointmentId,
  });
}
