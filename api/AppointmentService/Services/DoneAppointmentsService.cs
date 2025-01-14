using AppointmentService.Enums;
using AppointmentService.Utils;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.Services;
public class DoneAppointmentsServiceImpl(ApplicationDbContext dbContext)
    : DoneAppointmentsService.DoneAppointmentsServiceBase
{
    private readonly ApplicationDbContext dbContext = dbContext;

    public override async Task<AppointmentResponse> GetDoneAppointments(AppointmentRequest request, ServerCallContext context)
    {
        var appointments = await this.dbContext.Appointment
            .Where(app => app.Status == AppointmentStatus.DONE)
            .Select(app => new Appointment {Id = app.Id})
            .ToListAsync();

        var response = new AppointmentResponse();
        response.Appointments.AddRange(appointments);
        
        return response;
    }
}